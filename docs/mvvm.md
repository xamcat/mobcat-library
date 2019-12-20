# MVVM

 The [Model-View-ViewModel Pattern](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm) aids in decoupling the business and presentation logic of an application from its user interface definition. It encourages re-usable and extensible code that is easier to maintain, test, and evolve.  

## Pattern Overview
There are three core components in the [MVVM Pattern](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm):

![MVVM Illustration](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm-images/mvvm.png)

**Model:**  
Encapsulates the data used by the application. It represents the domain model which can be data along with business and validation logic. These can include DTOs (data transfer objects), POCOs (plain old CLR objects), as well as generated entity or proxy objects. 

**View:**  
Represents the appearance of what the user sees on the screen. A **View** is often defined separately to the code that underpins it. For example, AXML on Android, Storyboards on iOS, or XAML for UWP or Xamarin.Forms apps. A code-first approach can also be used and may be preferred. Both are valid and have their pros and cons. It is recommended that **View** related code contains only logic that implements visual behavior and does not contain any business logic.

**ViewModel:**  
Defines the functionality and data to be represented by the **View** and coordinates the **View** interactions and changes to **Model** classes that underpin it. The **ViewModel** typically exposes **Properties** and **Commands** that both informs the state of the **View** and can be used to notify of state changes or user input. 


 The relevant supporting components can be found in [MobCAT/MVVM](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM). Some of these are described in further detail below.  

 ## MobCAT Components
The **MobCAT** library provides a basic implementation of this pattern through a set of base classes, interfaces, and concrete implementations.  

- [AsyncCommand and Command](#asynccommand-and-command)
- [BaseNotifyPropertyChanged](#basenotifypropertychanged)
- [BaseViewModel](#baseviewmodel)
- [IValueConverter](#ivalueconverter)
- [VirtualCollection](#virtualcollection)

 ### [AsyncCommand](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/AsyncCommand.cs) and [Command](https://github.com/xamcat/mobcat-library/tree/master/MobCAT/MVVM/Command.cs)  
Implementations for asynchronous and synchronous bindable commands using the **ICommand** interface. 

#### Indicative Usage
```cs
AsyncCommand _myCommand;
Task _doSomethingTask;

public ICommand MyCommand => 
   _myCommand ??
   (_myCommand = new AsyncCommand(ExecuteMyCommandAsync, MyCommandCanExecute));

Task ExecuteMyCommandAsync(object arg = null)
   => DoSomethingAsync()

Task DoSomethingAsync()
{
   if (_doSomethingTask == null || _doSomethingTask.IsCompleted)
         _doSomethingTask = DoSomethingTask();

   return _doSomethingTask;
}

async Task DoSomethingTask() { ... }

bool MyCommandCanExecute(object arg = null) 
   => _doSomethingTask == null || _doSomethingTask.IsCompleted;
```

### [BaseNotifyPropertyChanged](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/BaseNotifyPropertyChanged.cs)  
Implementation for INotifyPropertyChanged with support for watching properties. This supports the use of data binding in native Android and iOS projects as well as those leveraging the out-of-the-box binding support provided by Xamarin.Forms. Simplifies updates to backing properties and change notification via the **INotifyPropertyChanged** **PropertyChanged** method.

#### Indicative Usage
```cs
public string MyText
{
   get => _myText;
   set => RaiseAndUpdate(ref _myText, value);
}
```

OR

```cs
public string MyText
{
   get { return _model.Text; }
   set { RaiseAndUpdate(() => _model.Text != value, () => _model.Text = value); }
}
```

It is sometimes necessary to raise change notification on behalf of another **Property** in addition to the current one. For example, when the value of a **Property** is determined by processing the value of another **Property**.  

```cs
public string MyText
{
   get => _myText;

   set 
   { 
      RaiseAndUpdate(ref _myText, value);
      Raise(nameof(MyUppercaseText));
   }
}

public string MyUppercaseText => _myText?.ToUpper();
```

For those apps built in Xamarin native, where there is no out-of-the-box support for **Data Binding**, it is possible to use the simple property watcher mechanism instead. The appropriate handlers are typically registered (and cleared down) in the corresponding **Activity** or **ViewController** on **Android** and **iOS** respectively.

```cs
// Register method to handle UI updates when a change notification is raised for that property
_myViewModel.WatchProperty(nameof(MyViewModel.Temperature), UpdateTemperatureIndicator);

...

// Clear the property watchers when the Activity/ViewController no longer needs them
_myViewModel.ClearWatchers();
```

### [BaseViewModel](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/BaseViewModel.cs)
Primary base class providing the foundations for **ViewModels**. Provides **Properties** for IsBusy, IsNotBusy, overridable methods for InitAsync() and Dispose(). Derived from **BaseNotifyPropertyChanged**. 

#### Indicative Usage
```cs
public class MyViewModel : BaseViewModel
{
   public override async Task InitAsync()
   {
      await DoInitializationWorkAsync();
      // Other synchronous initialization work 
      // e.g. subscribe to event handlers etc.
   }

   protected override void Dispose(bool disposing)
   {
      // Clean-up work 
      // e.g. unsubscribe from event handlers etc.
      base.Dispose(disposing);
   }
}
```

#### Other ViewModel base classes
There are other specialized **ViewModel** base classes deriving from [BaseViewModel](https://github.com/xamarin/mobcat/blob/master/mobcat_shared/MobCAT/MVVM/BaseViewModel.cs).  

**[BaseCollectionViewModel](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/BaseCollectionViewModel.cs)**  
Base **ViewModel** for **Views** whose primary function is to present a collection or list. In addition to the **Properties** provided by the [BaseViewModel](https://github.com/xamarin/mobcat/blob/master/mobcat_shared/MobCAT/MVVM/BaseViewModel.cs), this provides **Properties** for Items (**ObservableCollection&lt;T>**) and SelectedItem along with an abstract LoadItems **Task** and optional **Actions** for ItemSelected, Reload, Failed.  

**[BaseNavigationViewModel](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/BaseCNavigationViewModel.cs)**  
Base ViewModel that exposes the **INavigationService** through the **Navigation** property.  
### [INavigationService](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/Abstractions/INavigationService.cs)  
Defines the contract for **NativigationService** implementations that enable the navigation between **Views** from within **ViewModels**. This supports typical push and pop operations that can attach existing **ViewModel** instances or create a new one for a given **View** along with a number of options such as discarding current views, popping to root etc.

#### Indicative Usage

**Navigate to an existing ViewModel instance**
```cs
Task ShowModalSettingsAsync()
   => Navigation.PushModalAsync(_settingsViewModel);

Task ShowDetailsAsync()
   => Navigation.PushAsync(_detailViewModel);
```

**Navigate by ViewModel type**
```cs
Task ShowModalSettingsAsync()
      => Navigation.PushModalAsync<SettingsViewModel>();

Task ShowDetailsAsync()
   => Navigation.PushAsync<DetailViewModel>();
```

**NOTE:** The example above assumes that the **INavigationService** is accessed from a **ViewModel** that derives from **BaseNavigationViewModel** with a convenience **Navigation** **Property**. 

### [IValueConverter](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/Abstractions/IValueConverter.cs)  
Standalone **IValueConverter** interface enabling the decoupling of components from the Xamarin.Forms framework allowing for greater portability and re-use.

### [VirtualCollection](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/VirtualCollection.cs)
An **ObservableCollection** implementation for virtual collections which handles updating the items, VirtualCount, and VirtualPageSize through the method AddPage(IEnumerable&lt;TItem> collection, int? virtualCount = null, int? pageNumber = null, int? pageSize = null)