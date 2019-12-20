# MobCAT.Forms Components

The **MobCAT.Forms** project includes building blocks, for those applications built with **Xamarin.Forms** and the **MobCAT** library, to simplify the implementation of common application functionality and cross-platform architecture using recommended patterns and practices. This includes:

- [Base Pages in support of MVVM](#base-pages-in-support-of-mvvm)  
- [BaseBehavior](#basebehavior)
- [INavigationService implementation for Xamarin.Forms](#inavigationservice-implementation-for-xamarin.forms)
- [Localization using resx resources](#localization-using-resx-resources)
- [Right to Left Text support](#right-to-left-text-support)


## Base Pages in support of MVVM
Primary base class for **Xamarin.Forms** **Pages** implementing the **MVVM Pattern** using the **MobCAT** library. Simplifies the initialization of a given **ViewModel** and setting it as the **ContentPage** **BindingContext**. Enables **Pages** to be initialized and navigated to from **ViewModel** classes while remaining decoupled from **Xamarin.Forms**. Derives from the standard Xamarin.Forms **ContentPage**.

#### Indicative Usage

**XAML MARKUP:**
```xml
?xml version="1.0" encoding="utf-8"?>
<base:BaseContentPage
    xmlns="http://xamarin.com/schemas/2014/forms"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:base="clr-namespace:Microsoft.MobCAT.Forms.Pages;assembly=MobCAT.Forms"
    xmlns:vm="clr-namespace:SampleApp.ViewModels;assembly=SampleApp"
    Title="Sample Page"
    x:DataType="vm:SampleViewModel"
    x:TypeArguments="vm:SampleViewModel"
    x:Class="SampleApp.Forms.Pages.SamplePage">

    ...

</base:BaseContentPage>
```
**C# CODEBEHIND:**
```cs
[DesignTimeVisible(true)]
[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class SamplePage : BaseContentPage<SampleViewModel>
{
   public SamplePage()
   {
      InitializeComponent();
   }
}
```

#### Other Xamarin.Forms Page base classes
There are other specialized **Page** base classes. These include:

**BaseNavigationPage**  
Derives from the standard Xamarin.Forms **NavigationPage** adding support for traditional Xamarin.Forms navigation.  

--> **REVIEW:** There seems to be little point in this page?? <--

**BaseTabbedPage**  
Derives from the standard Xamarin.Forms **TabbedPage** simplifying the initialization of a given **ViewModel** and setting it as the **ContentPage** **BindingContext** in a similar manner to the **BaseContentPage** class but extended to support this for each page hosted by the **TabbedPage**.

## BaseBehavior
Base class simplifying the implementation of custom **[Behaviors](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/behaviors/)**. Encapsulates common functionality required by a **Behavior** reducing code duplication to enable greater focus on handling those aspects that are specific to that specific **Behavior**.

#### Indicative Usage
```cs
public class ClearListViewSelectionBehavior : Behavior<ListView>
{
   protected override void OnAttachedTo(ListView bindable)
   {
      base.OnAttachedTo(bindable);
      bindable.ItemSelected += ListView_ItemSelected;
   }

   protected override void OnDetachingFrom(ListView bindable)
   {
      base.OnDetachingFrom(bindable);
      bindable.ItemSelected -= ListView_ItemSelected;
   }

   void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
      => AssociatedObject.SelectedItem = null;
}
```

## INavigationService implementation for Xamarin.Forms
Provides an implementation of the **INavigationService** to enable the navigation between **Xamarin.Forms** **Pages** from within **ViewModels**. 

#### Indicative Usage

```cs
var navigationService = new NavigationService();
navigationService.RegisterViewModels(typeof(MainPage).GetTypeInfo().Assembly);

ServiceContainer.Register<INavigationService>(navigationService);
```

This setup is typically performed in **App.xaml.cs** or as part of dependency registration when performed in a centralized manner. The **Type** used to resolve the **Assembly**, as part of the call to **NavigationService.RegisterViewModels**, can be any **Type** from within the **Assembly** containing the **Xamarin.Forms** **Pages** looking to support **ViewModel** driven navigation using the Xamarin.Forms **INavigationService** implementation.

## Localization using resx resources
Localization is implemented using .resx resources based on the [official Xamarin sample](https://github.com/xamarin/xamarin-forms-samples/tree/master/UsingResxLocalization) as **LocalizationService** and **BaseLocalizationService**.

To support additional languages, create a .resx file language using the [CultureInfo](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo?view=netframework-4.7.2) **TwoLetterISOLanguageName**. For example, **AppResources.ar.resx** for Arabic. Supported CultureInfos can be queried using [CultureInfo.GetCultures(types)](https://docs.microsoft.com/en-us/dotnet/api/system.globalization.cultureinfo.getcultures?view=netstandard-2.0) on each device.

The localized strings are then populated as XML data:

```xml
<data
    name="heavythunderstorm"
    xml:space="preserve">
    <value>Heavy thunderstorm</value>
    <comment>weather condition</comment>
</data>
```

Localized strings can then be fetched using **LocalizationService.Translate(resourceName)**.

## Right to Left Text support
RTL (Right-to-Left) language support is enabled using the [Xamarin Forms Device.FlowDirection Property](https://docs.microsoft.com/en-us/dotnet/api/xamarin.forms.device.flowdirection?view=xamarin-forms).

```cs
public static Xamarin.Forms.FlowDirection FlowDirection { get; }
```

It can be set on the FlowDirection property of the ContentPage like so

```cs
FlowDirection="{x:Static Device.FlowDirection}"
```