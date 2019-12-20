# MobCAT

[![Build Status](https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/MobCAT-CI?branchName=master)](https://dotnetcst.visualstudio.com/MobCAT/_build/latest?definitionId=60&branchName=master)

MobCAT is a toolbox created by our team to highlight and support the application of best practices and good architectural patterns for [Xamarin.Forms](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/), [Xamarin.Android](https://docs.microsoft.com/en-us/xamarin/#pivot=platforms&panel=Android), and [Xamarin.iOS](https://docs.microsoft.com/en-us/xamarin/#pivot=platforms&panel=iOS) apps.  

These projects can provide a foundation or light-weight starting point which can be built upon when adopted as part of your Xamarin solutions. 

The projects themselves are .NET Standard portable projects. The intent is that the source is cloned/forked and incorporated into your solutions directly through project reference or via a private NuGet feed.   

Please take a look at the [sample projects](https://github.com/xamcat/mobcat-samples) to get a better understanding of their usage.

## MobCAT

In this project, you can find foundational components that can be leveraged in both Xamarin.Forms and native Xamarin projects. 

Core components:

- [BaseHttpService.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/Services/BaseHttpService.cs)
- [Guard.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/Guard.cs)
- [Logger.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/Logger.cs)
- [ServiceContainer.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/ServiceContainer.cs)

See [Foundational Components Overview](docs/foundational_components.md) for more information. 

In addition to the core components above there are foundational components in support of common architectural patterns. These include:

### MVVM  
The [Model-View-ViewModel Pattern](https://docs.microsoft.com/en-us/xamarin/xamarin-forms/enterprise-application-patterns/mvvm) aids in decoupling the business and presentation logic of an application from its user interface. It encourages re-usable and extensible code that is easier to maintain, test, and evolve.

Useful files:  

- [AsyncCommand.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/AsyncCommand.cs)
- [BaseNotifyPropertyChanged.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/BaseNotifyPropertyChanged.cs)
- [BaseViewModel.cs](https://github.com/xamarin/mobcat/blob/master/mobcat_shared/MobCAT/MVVM/BaseViewModel.cs)
- [BaseNavigationViewModel.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/BaseNavigationViewModel.cs)
- [Command.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/Command.cs)
- [VirtualCollection.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/MVVM/VirtualCollection.cs)

See [MVVM Overview](docs/mvvm.md) for more information. 

### Repository
The [Repository Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design#the-repository-pattern) is a well-documented way of working with a data source whereby the underlying storage mechanism is decoupled from the storage intent through an abstraction. This enables the centralization of storage related code making unit testing of business logic easier and driving greater code re-use across solutions.  

The **MobCAT** library provides a basic implementation of this pattern through a common abstraction and a set of base classes along with implementations for popular frameworks such as [sqlite-net](https://github.com/praeclarum/sqlite-net) and [Entity Framework Core](ttps://docs.microsoft.com/en-us/ef/core/) along with an in-memory implementation to aid in testing and prototyping.  

See the [Repository Pattern Overview](docs/repository.md) for more information.

Useful files:

- [BaseRepository.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/Repositories/BaseRepository.cs)
- [BaseRepositoryStore.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT/Repositories/BaseRepositoryStore.cs)

Walkthroughs: 

* [Repository Pattern with Entity Framework Core](docs/repository-efcore-gettingstarted.md) 
* [Repository Pattern with an InMemory Data Store](docs/repository-inmemory-gettingstarted.md)
* [Repository Pattern with sqlite-net](docs/repository-sqlite-net-gettingstarted.md)   

These demonstrate how to implement the **Repository Pattern** using the base classes provided by the **MobCAT** library

## MobCAT.Forms

[![Build Status](https://dotnetcst.visualstudio.com/MobCAT/_apis/build/status/MobCAT-CI?branchName=master)](https://dotnetcst.visualstudio.com/MobCAT/_build/latest?definitionId=60&branchName=master)

In this project you can find components, specific to Xamarin.Forms, to support cross-platform architecture, user inferface, and patterns such as [MVVM](#mvvm).  

Useful files:

- [BaseBehavior.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT.Forms/Behaviors/BaseBehavior.cs)
- [BaseContentPage.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT.Forms/Pages/BaseContentPage.cs)
- [BaseNavigationPage.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT.Forms/Pages/BaseNavigationPage.cs)
- [InfiniteListView.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT.Forms/Controls/InfiniteListView.cs)
- [NavigationService.cs](https://github.com/xamcat/mobcat-library/blob/master/MobCAT.Forms/Services/NavigationService.cs)

See the [MobCAT.Forms Overview](docs/forms.md) for more information.