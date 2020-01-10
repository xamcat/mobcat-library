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
There are various ways in which you can make these packages available [using your own private NuGet feeds](https://docs.microsoft.com/en-us/nuget/hosting-packages/overview). Documentation for some of the popular **NuGet** feed hosting options have been listed below for convenience: 

#### Local NuGet Package Feeds
- [Creating a local NuGet package feed](https://docs.microsoft.com/en-us/nuget/hosting-packages/local-feeds)  

#### Private NuGet Package Feeds
- [Azure DevOps: Creating a private NuGet package feed](https://docs.microsoft.com/en-gb/azure/devops/artifacts/get-started-nuget?view=azure-devops&tabs=new-nav#create-a-feed)
- [Azure DevOps: Publishing to a NuGet package feed](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/publish?view=azure-devops)
- [GitHub: Configuring dotnet CLI for use with packages](https://help.github.com/en/github/managing-packages-with-github-packages/configuring-dotnet-cli-for-use-with-github-packages)

### Consuming NuGet Packages in Visual Studio
Add the feed's endpoint as a package source in **Visual Studio** using either the [Package Manager UI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-visual-studio#package-sources) or the [nuget sources](https://docs.microsoft.com/en-us/nuget/reference/cli-reference/cli-ref-sources) command in order to consume **NuGet** packages from that feed. 

**Local Package Endpoint:**  
For local package feeds, add its pathname e.g. *\\\\myfolder\\packages* to the list of sources.

**Azure Dev Ops Package Endpoint:**  
For **NuGet** feeds hosted in **Azure Dev Ops**, [get your feed's NuGet package source information](https://docs.microsoft.com/en-us/azure/devops/artifacts/nuget/consume?view=azure-devops#get-your-feeds-nuget-package-source-information) using the **Connect to Feed** link on the respective **Artefacts** page. The resulting endpoint should indicatively be: *https://pkgs.dev.azure/OWNER/_packaging/PROJECT/nuget/v3/index.json* where *OWNER* is the user or organization account that owns the project.

**GitHub Packages Endpoint:**  
For **NuGet** feeds hosted in **GitHub Packages**, you should follow the steps provided in the [authenticating to GitHub Packages](https://help.github.com/en/github/managing-packages-with-github-packages/configuring-dotnet-cli-for-use-with-github-packages#authenticating-to-github-packages) documentation. The resulting endpoint should indicatively be: *https://nuget.pkg.github.com/OWNER/index.json* where *OWNER* is the name of the user or organization account that owns the project.

One you have the appropriate endpoint, and completed any steps that are specific to the **NuGet package source**, you can follow the steps in the [Visual Studio NuGet Quickstart document](https://docs.microsoft.com/en-gb/azure/devops/artifacts/get-started-nuget?view=azure-devops&tabs=new-nav#consume-your-package-in-visual-studio) to add it as a package source in **Visual Studio**. 

## Project Reference
You can incorporate the **MobCAT** libraries into your solutions via project reference. However, this is not recommended in most cases unless you intend to maintain the source code as part of that solution moving forward. 

To add the **MobCAT** libraries into your solution:

1. Acquire the source code by:

    a) Downloading and extract/unzip the source code from the [master](https://github.com/xamcat/mobcat-library/archive/master.zip) or [dev](https://github.com/xamcat/mobcat-library/archive/dev.zip) branch as appropriate  
    b) Cloning the [repo](https://github.com/xamcat/mobcat-library) via *SSH* or *HTTPS* 

2. Copy to your preferred working directory as appropriate
3. In **Visual Studio**, add the respective project(s) to your solution
4. Add the appropriate project reference(s)