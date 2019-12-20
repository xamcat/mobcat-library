# Repository Pattern with Entity Framework Core
This walkthrough demonstrates how to implement the **Repository Pattern** with **Entity Framework Core (EF Core)** using the base classes provided by the **MobCAT** and **MobCAT.Repositories.EntityFrameworkCore** libraries. 

This is arranged into the following high-level steps:
1. [Solution Setup](#solution-setup)
2. [Define Common Repository Types](#define-common-repository-types)
3. [Build the **EF Core** implementation](#build-the-ef-core-implementation)
4. [Try it out](#try-it-out)

## Prerequisites
This walkthrough assumes that you have the **MobCAT** library source code or **NuGet** package that can be referenced as a project or a package. See [Getting started with MobCAT](mobcat-gettingstarted.md).

## Solution Setup

1. Create a new **Xamarin.Forms** project called **SampleApp** including **Android** and **iOS** platform targets  
2. Update all **NuGet** packages across the solution
3. Create a new **netstandard** project to contain the app data concerns e.g. **SampleApp.Data**
4. Create a new **netstandard** project for the **EF Core** implementation of the data concerns e.g. **SampleApp.Data.EntityFrameworkCore**
5. Reference the **MobCAT** library and **SampleApp.Data** in all projects
6. Add a reference to **SampleApp.Data.EntityFrameworkCore** in the platform targets 
7. Add the **Microsoft.EntityFrameworkCore** and **Microsoft.EntityFrameworkCore.Sqlite** **NuGet** packages to **SampleApp.Data.EntityFrameworkCore** and both platform targets
8. Add the **Newtonsoft.Json** and **Polly** **NuGet** packages to the platform targets

## Define Common Repository Types

1. In **SampleApp.Data**, define the business model object (you can optionally derive from **BaseModel** for this as below)

   ```cs
   public class SampleModel : BaseModel
   {
      public string Text { get; set; }
      public DateTimeOffset Timestamp { get; set; }
   }
   ```

2. In the same project, define the **Repository** and optionally the **RepositoryContext** abstractions

   ```cs
   public interface ISampleRepository : IBaseRepository<SampleModel> 
   {
      // Other application-specific operations here
   }

   public interface ISampleRepositoryContext : IRepositoryContext
   {
      ISampleRepository SampleRepository { get; }  
      // Other repositories and high-level operations here
   }
   ```

## Build the **EF Core** implementation
1. In **SampleApp.Data.EntityFrameworkCore**, add a new class called **EFCoreSampleModel** deriving from **BaseEFCoreNetModel** with the following implementation  

   ```cs
   public class EFCoreSampleModel : BaseEFCoreModel
   {
      public string SampleString { get; set; }
      public int SampleInt { get; set; }
      public long TimestampTicks { get; set; }
   }
   ```

   **NOTE:** The types and names of the **Properties** are purposefully different to the **SampleModel** to make the data mapping exercise more involved  

2. In **SampleApp.Data.EntityFrameworkCore**, add a new class called **EFCoreSampleRepository** deriving from **BaseEFCoreRepository** and implementing the **ISampleRepository** interface. The constructor should take a **SqliteConnection** parameter so it can be passed into the **BaseEFCoreRepository** base class

    ```cs
    public class EFCoreSampleRepository : BaseEFCoreRepository<SampleModel, EFCoreSampleModel>, ISampleRepository  
    {
       public EFCoreSampleRepository(SqliteConnection connection)
          : base(connection) {}
    }
   ```
3. In the same file, override the **ToModelType** and **ToRepositoryType** in order to implement the data mapping logic for the repository

   ```cs
   protected override SampleModel ToModelType(EFCoreSampleModel repositoryType)
      => repositoryType == null ? null : new SampleModel
      {
            Id = repositoryType.Id,
            Text = repositoryType.Text,
            Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero) 
      };

   protected override EFCoreSampleModel ToRepositoryType(SampleModel modelType)
      => modelType == null ? null : new EFCoreSampleModel
      {
            Id = modelType.Id,
            Text = modelType.Text,
            TimestampTicks = modelType.Timestamp.UtcTicks
      };
   ```

   **NOTE:** There are no additional operations defined by the **ISampleRepository**.

4. In **SampleApp.Data.EntityFrameworkCore**, add a new class called **EFCoreSampleRepositoryContext** deriving from **BaseEFCoreRepositoryContext** and implementing the **ISampleRepositoryContext** interface. The constructor should take two string parameters, **folderPath** and **datastoreName**, so they can be passed into the **BaseEFCoreRepositoryContext** base class

   ```cs
   public class EFCoreSampleRepositoryContext : BaseEFCoreRepositoryContext, ISampleRepositoryContext
   {
      public EFCoreSampleRepositoryContext(string folderPath, string datastoreName)
         : base (folderPath, datastoreName) {}

      public ISampleRepository SampleRepository { get; }
   }
      ```

5. Update **EFCoreSampleRepositoryContext** so it instantiates the **ISampleRepository** implementation in a lazy loading manner passing in the shared **Connection** from the base class

      ```cs
      ISampleRepository _sampleRepository;

      public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new EFCoreSampleRepository(Connection));
      ```

6. In the same file, override the **OnResetRepositories** method setting the **_sampleRepository** backing field to null

      ```cs
      protected override void OnResetRepositories()
         => _sampleRepository = null;
      ```

7. In the **SampleApp** project, add a new class called **Bootstrap** with the following definition

      ```cs
      public static class Bootstrap
      {
         const string DatastoreName = "sqlitenet_datastore";

         public static void Begin(Func<string, ISampleRepositoryContext> sampleRepositoryContext)
            => ServiceContainer.Register(sampleRepositoryContext(DatastoreName));
      }
      ```

8. In **MainActivity**, within the **Android** target, update the **OnCreate** method to call **Bootstrap.Begin** just before the call to **LoadApplication**

      ```cs
      var storageFilepath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            
      Bootstrap.Begin((datastoreName) => new EFCoreSampleRepositoryContext(storageFilepath, datastoreName));
      ```

      **NOTE:** We are passing in a **Func** to resolve a new instance of **EFCoreSampleRepositoryContext** as the **ISampleRepository** implementation at the appropriate juncture. The **storageFilepath** represents the location where the **SQLite** database will be created

9. In **AppDelegate**, within the **iOS** target, update the **FinishedLaunching** method in the same manner resolving the **storageFilepath** to the **Library** directory instead

      ```cs
      var storageFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            
      Bootstrap.Begin((datastoreName) => new EFCoreSampleRepositoryContext(storageFilepath, datastoreName));
      ```

## Try it out
1. In **MainPage.xaml.cs**, add a method called **TestRepositoryAsync**, with the following throw-away code to quickly validate a subset of the basic repository operations

   ```cs
   public async Task TestRepositoryAsync()
   {
      var sampleRepositoryContext = ServiceContainer.Resolve<ISampleRepositoryContext>();

      var sampleModel = new SampleModel 
      {
         Id = Guid.NewGuid().ToString(),
         Text = $"Test Model",
         Timestamp = DateTimeOffset.UtcNow
      };

      // Create (insert) operation
      await sampleRepositoryContext.SampleRepository.InsertItemAsync(sampleModel).ConfigureAwait(false);

      // Read (get) operation
      var item = await sampleRepositoryContext.SampleRepository.GetItemAsync(sampleModel.Id).ConfigureAwait(false);
   }
   ```

2. In the same file, override the **OnAppearing** method to call the **TestRepositoryAsync** method

      ```cs
      protected override void OnAppearing()
      {
         base.OnAppearing();
         _ = Task.Run(() => TestRepositoryAsync());
      }
      ```