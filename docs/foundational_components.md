# Foundational Components

The **MobCAT** project includes building blocks to simplify the implementation of common application functionality and cross-platform architecture using recommended patterns and practices. This includes: 

- [IoC (Inversion of Control) using Service Container](#ioc-with-service-container)
- [Robust http services using BaseHttpService](#robust-http-services-using-basehttpservice)
- [Simplification of validation and logging with Guard and Logger](#simplification-of-validation-and-logging-with-guard-and-logger)

## IoC (Inversation of Control) using Service Container
There are many techniques and supporting libraries available for implementing **IoC (Inversion of Control)**. Common implementations of this pattern include **[Dependency Injection](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/dependency-injection)** and **[Service Locator](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff648968(v=pandp.10))**.   

The **MobCAT** library includes a **ServiceContainer** class as a light-weight and simple mechanism for implementing the **[Service Locator Pattern](https://docs.microsoft.com/en-us/previous-versions/msp-n-p/ff648968(v=pandp.10))** since it is quick to get started with and is more than sufficient in many cases. 

The process of initializing and registering the respective services with **ServiceContainer** can often be centralized within a common **Bootstrap** class. Both common and platform-specific startup actions can also be invoked using the optional **Action** parameters supported by the **Begin** method. The **Begin** method is called from the **AppDelegate** for iOS and **MainActivity** for Android respectively.

### Indicative Usage

**Bootstrap**
```cs
public static class Bootstrap
{
   public static void Begin(
      Action commonBegin = null, 
      Action platformSpecificBegin = null)
   {
      commonBegin?.Invoke();
      platformSpecificBegin?.Invoke();
   }
}
```

**App.xaml.cs**
```
public static void CommonBootstrap()
{
   // e.g. Registering MobCAT.Forms INavigationService implementation
   var navigationService = new NavigationService(); 
   navigationService.RegisterViewModels(typeof(MainPage).GetTypeInfo().Assembly);
   
   ServiceContainer.Register<INavigationService>(navigationService);
   ServiceContainer.Register<IMyHttpService>(
      () => new MyHttpService("<base_address>"));
}
```
**NOTE:** This could be defined within the core app project in the case of Xamarin native solutions.

**AppDelegate.cs**
```cs
public override bool FinishedLaunching(
   UIApplication uiApplication, 
   NSDictionary launchOptions)
{
   global::Xamarin.Forms.Forms.Init();

   Bootstrap.Begin(App.CommonBootstrap, () =>
   {
      ServiceContainer.Register<IMyPlatformService>(
         () => new MyiOSService());
   });

   LoadApplication(new App());

   return base.FinishedLaunching(uiApplication, launchOptions);
}
```

**MainActivity.cs**
```cs
protected override void OnCreate(Bundle savedInstanceState)
{
   TabLayoutResource = Resource.Layout.Tabbar;
   ToolbarResource = Resource.Layout.Toolbar;

   base.OnCreate(savedInstanceState);

   global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

   Bootstrap.Begin(App.CommonBootstrap, () =>
   {
      ServiceContainer.Register<IMyPlatformService>(
         () => new MyAndroidService());
   });

   LoadApplication(new App());
}
```

This enables both platform-specific and platform-agnostic components to be registered and subsequently resolved via a common interface within shared code. It also enables mock services to be registered for UI test or unit test purposes.

## Robust http services using BaseHttpService
The **BaseHttpService** class can be used to simplify working with REST APIs in a reliable manner by encapsulating the [correct usage of HttpClient](https://aspnetmonsters.com/2016/08/2016-08-27-httpclientwrong/), handling common tasks such as serialization, and implementing [recommended cloud design patterns](https://docs.microsoft.com/en-us/azure/architecture/patterns/) such as [retry](https://docs.microsoft.com/en-us/azure/architecture/patterns/retry).

### Indicative Usage

```cs
public class MyHttpService : BaseHttpService, IMyHttpService
{
   const string MyItemsEndpoint = "api/items";

   public MyHttpService(string baseAddress) : 
      base(baseAddress){}

   public Task<IEnumerable<MyItem>> GetItemsAsync(CancellationToken cancellationToken = default(CancellationToken))
      => GetAsync<IEnumerable<MyItem>>(MyItemsEndpoint, cancellationToken);
}
```

### Request Permutations
For those high-level request methods that expect return values from the response, the default behavior is to obtain that value through deserializing the content based on the specified type. In some cases, deserialization is not required and so this can be changed to return the raw response instead.

```cs
_myHttpService.GetAsync<bool>(MyOtherGetEndpoint, cancellationToken, null, false);
```

The **DeleteAsync** and **GetAsync** by their nature do not have the option to send content as part of their requests. While **GetAsync** will always expect to return a value, this is optional for the **DeleteAsync** method which can be used to simply ensure the response has a success status code where there is no return value expected.  

The **PostAsync** and **PutAsync** methods can be used in the following additional ways:

```cs
// 1. Send payload of type **T** ensuring successful status code with no return type
_myHttpService.PutAsync<MyItem>(MyItemsEndpoint, updatedItem);

// 2. Send payload of type K returning a value of type T if successful
var response = await _myHttpService.PutAsync<ResponseModel, MyItem>(MyItemsEndpoint, updatedItem);
```

### Commonly Used Capabilities  

#### Setting Default Request Headers
Defines the headers that persist across **HttpClient** requests. Typically performed as a one-off action, but could also be used to change values to reflect changes. Specifying *true* in the first **shouldClear** parameter will clear all previous default headers.
```cs
_myHttpService.SetDefaultRequestHeaders(false, new KeyValuePair<string, string>("api_key", "<api_key_value>"));
```

#### Modifying Http Client
Enables use of the underling **HttpClient** object directly.

```cs
_myHttpService.ModifyHttpClient((client) => client.Timeout = TimeSpan.FromMilliseconds(5000));
```

#### Modifying the HttpRequest
It is possible to modify the underlying **HttpRequest** before it is sent. For example, adding headers specific to a subset of operations or ad-hoc manipulation of the request uri.

```cs
Task<IEnumerable<MyItem>> GetItemsAsync()
   => _myHttpService.GetAsync<IEnumerable<MyItem>>(
      MyItemsEndpoint, 
      cancellationToken, 
      ModifyGetAsyncRequest);

void ModifyGetAsyncRequest(HttpRequestMessage request)
   => request.Headers.Add("header_name", "header_value");
```

### Debugging
If you run into issues with the Http messages and responses being sent and received, the **BaseHttpService** provides a couple of callbacks that can be especially useful.

1. **HttpRequestMessageSent** allows inspection of the **HttpRequestMessage** that is being sent from the service.
2. **HttpResponseMessageReceived** allows inspection of the **HttpResponseMessage** received by the service. Note: this is the raw response, there is no deserialization done at this point.

Example:
```cs
var service = new MyHttpService();

#if DEBUG
service.HttpRequestMessageSent += request => Debug.WriteLine(request);
service.HttpResponseMessageReceived += response => Debug.WriteLine(response);
#endif
```

## Simplification of validation and logging with Guard and Logger

### Guard
The **Guard** singleton can aid in reducing duplicate code related to validating method parameters. For example:  

```cs
public Task<MyItem> GetItemAsync(string id, CancellationToken cancellationToken = default)
{
   if (string.IsNullOrWhiteSpace(id))
      throw new ArgumentException($"Parameter {nameof(id)} cannot be null or whitespace");
}
```

OR

```cs
public Task<MyItem> GetItemAsync(string id, CancellationToken cancellationToken = default)
{
   Guard.NullOrWhitespace(id);
}
```

Both result in throwing an **ArgumentException** with the message *'Parameter id cannot be null or whitespace'*. This becomes more beneficial when there are a greater number of paramters to validate.

### Logger  
The **Logger** singleton can be used to simplify and encourage consistent logging thoughout the app. It enables the registration of any implementation of **ILoggingService** using the **ConsoleLoggingService** implementation by default. This approach allows implementations to be switched out in a single place without impacting the broader codebase.  

#### Indicative Usage

```cs
Logger.RegisterService(new AppCenterLoggingService());
```

OR

```cs
ServiceContainer.Register<ILoggingService>(() => new AppCenterLoggingService()());
```

The **Logger** singleton uses the **ServiceContainer** under the hood but provides a convenience method for registering the underlying **ILoggingService** implementation. Registering the **ILogginerService** implementation directly with the **ServiceContainer** will achieve the same result.