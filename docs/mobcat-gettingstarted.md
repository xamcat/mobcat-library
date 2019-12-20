# Getting Started with MobCAT
There are two main approaches to using the **MobCAT** libraries in your application.
1. [NuGet package](#nuget-package)
2. [Project reference](#project-reference)


## NuGet Package
Referencing the **MobCAT** libraries via your own NuGet feed is the preferred and best approach in most cases. 

The prebuilt packages can be downloaded from the **MobCAT** repository **[Packages](https://github.com/xamcat/mobcat-library/packages)** tab or directly using the links below.

- [MobCAT](https://github.com/xamcat/mobcat-library/packages/14497)
- [MobCAT.Forms](https://github.com/xamcat/mobcat-library/packages/14499)
- [MobCAT.Repositories.EntityFrameworkCore]()
- [MobCAT.Repositories.SqliteNet]()

These packages can then be added / published to your local / private **NuGet** feed then subsequently consumed by your projects like any other **NuGet** package.

### Configuring a NuGet Feed
To create and [add](https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds#initializing-and-maintaining-hierarchical-folders) / [publish](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/publish?view=azure-devops) packages to a [local](https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds) / [private](https://docs.microsoft.com/en-gb/azure/devops/artifacts/get-started-nuget?view=azure-devops&tabs=new-nav#create-a-feed) **NuGet** package feed, follow the steps in the relevant documentation below:

#### Local NuGet Package Feeds
- [Creating a local NuGet package feed](https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds)  

#### Private NuGet Package Feeds on Azure Dev Ops
- [Creating a private NuGet package feed on Azure Dev Ops](https://docs.microsoft.com/en-gb/azure/devops/artifacts/get-started-nuget?view=azure-devops&tabs=new-nav#create-a-feed)
- [Publishing to a NuGet package feed on Azure Dev Ops](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/publish?view=azure-devops)


### Consuming NuGet Packages in Visual Studio
Add the feed's endpoint as a package source in **Visual Studio** using either the [Package Manager UI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio#package-sources) or the [nuget sources](https://docs.microsoft.com/en-us/nuget/reference/cli-reference/cli-ref-sources) command in order to consume **NuGet** packages from that feed. 

**Local Package Endpoint:**  
For local package feeds, add its pathname e.g. *\\\\myfolder\\packages* to the list of sources.

**Azure Dev Ops Package Endpoint:**  
For **NuGet** feeds hosted in **Azure Dev Ops**, [get your feed's NuGet package source information](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/consume?view=azure-devops#get-your-feeds-nuget-package-source-information) using the **Connect to Feed** link on the respective **Artefacts** page.

One you have the appropriate endpoint, you can follow the steps in the [Visual Studio NuGet Quickstart document](https://docs.microsoft.com/en-gb/azure/devops/artifacts/get-started-nuget?view=azure-devops&tabs=new-nav#consume-your-package-in-visual-studio) to add it as a package source in **Visual Studio**. 

## Project Reference
You can incorporate the **MobCAT** libraries into your solutions via project reference. However, this is not recommended in most cases unless you intend to maintain the source code as part of that solution moving forward. 

To add the **MobCAT** libraries into your solution:

1. Download the [source code](https://github.com/xamcat/mobcat-library/archive/master.zip)
2. Unzip **master.zip** and copy to your preferred working directory
3. In **Visual Studio**, add the respective project(s) to your solution
4. Add the appropriate project reference(s)