# Repository Pattern with sqlite-net
This walkthrough demonstrates how to implement the **Repository Pattern** with **sqlite-net** using the base classes provided by the **MobCAT** and **MobCAT.Repositories.SQLiteNet** libraries. 

This is arranged into the following high-level steps:
1. [Solution Setup](#solution-setup)
2. [Define Common Repository Types](#define-common-repository-types)
3. [Build the **sqlite-net** implementation](#build-the-sqlite-net-implementation)
4. [Try it out](#try-it-out)

## Prerequisites
This walkthrough assumes that you have the **MobCAT** and **MobCAT.Repository.SqliteNet** source code that can be referenced as a project or you have made these available as **NuGet** packages. See [Getting started with MobCAT](mobcat-gettingstarted.md).

## Solution Setup

1. Create a new **Xamarin.Forms** project called **SampleApp** including **Android** and **iOS** platform targets  
2. Update all **NuGet** packages across the solution
3. Create a new **netstandard** project to contain the app data concerns e.g. **SampleApp.Data**
4. Create a new **netstandard** project for the **sqlite-net** implementation of the data concerns e.g. **SampleApp.Data.SqliteNet**
5. Reference the **MobCAT** library and **SampleApp.Data** in all projects
6. Add a reference to **SampleApp.Data.SqliteNet** in the platform targets 
7. Add the **sqlite-net-pcl** **NuGet** and **MobCAT.Repository.SQLiteNet** packages to **SampleApp.Data.SqliteNet** and both platform targets
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

## Build the **sqlite-net** implementation
1. In **SampleApp.Data.SqliteNet**, add a new class called **SQLiteNetSampleModel** deriving from **BaseSQLiteNetModel** with the following implementation  

    ```cs
    public class SQLiteNetSampleModel : BaseSQLiteNetModel
    {
       public string Text { get; set; }
       public long TimestampTicks { get; set; }
    }
    ```

      **NOTE:** The types and names of the **Properties** are purposefully different to the **SampleModel** to make the data mapping exercise more involved  

2. In **SampleApp.Data.SqliteNet**, add a new class called **SQLiteNetSampleRepository** deriving from **BaseSQLiteNetRepository** and implementing the **ISampleRepository** interface. The constructor should take a **SQLiteAsyncConnection** parameter so it can be passed into the **BaseSQLiteNetRepository** base class

    ```cs
    public class SQLiteNetSampleRepository : BaseSQLiteNetRepository<SampleModel, SQLiteNetSampleModel>, ISampleRepository  
    {
       public SQLiteNetSampleRepository(SQLiteAsyncConnection connection)
          : base(connection) {}
    }
   ```
3. In the same file, override the **ToModelType** and **ToRepositoryType** in order to implement the data mapping logic for the repository

      ```cs
      protected override SampleModel ToModelType(SQLiteNetSampleModel repositoryType)
         => repositoryType == null ? null : new SampleModel
         {
               Id = repositoryType.Id,
               Text = repositoryType.Text,
               Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero) 
         };

      protected override SQLiteNetSampleModel ToRepositoryType(SampleModel modelType)
         => modelType == null ? null : new SQLiteNetSampleModel
         {
               Id = modelType.Id,
               Text = modelType.Text,
               TimestampTicks = modelType.Timestamp.UtcTicks
         };
      ```

      **NOTE:** There are no additional operations defined by the **ISampleRepository**.

4. In **SampleApp.Data.SqliteNet**, add a new class called **SQLiteNetSampleRepositoryContext** deriving from **BaseSQLiteNetRepositoryContext** and implementing the **ISampleRepositoryContext** interface. The constructor should take two string parameters, **folderPath** and **datastoreName**, so they can be passed into the **BaseSQLiteNetRepository** base class

      ```cs
      public class SQLiteNetSampleRepositoryContext : BaseSQLiteNetRepositoryContext, ISampleRepositoryContext
      {
         public SQLiteNetSampleRepositoryContext(string folderPath, string datastoreName)
            : base (folderPath, datastoreName) {}

         public ISampleRepository SampleRepository { get; }
      }
      ```

5. Update **SQLiteNetSampleRepositoryContext** so it instantiates the **ISampleRepositoryContext** implementation in a lazy loading manner passing in the shared **Connection** from the base class

      ```cs
      ISampleRepository _sampleRepository;

      public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new SQLiteNetSampleRepository(Connection));
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
            
      Bootstrap.Begin((datastoreName) => new SQLiteNetSampleRepositoryContext(storageFilepath, datastoreName));
      ```

      **NOTE:** We are passing in a **Func** to resolve a new instance of **SQLiteNetSampleRepositoryContext** as the **ISampleRepositoryContext** implementation at the appropriate juncture. The **storageFilepath** represents the location where the **SQLite** database will be created

9. In **AppDelegate**, within the **iOS** target, update the **FinishedLaunching** method in the same manner resolving the **storageFilepath** to the **Library** directory instead

      ```cs
      var storageFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            
      Bootstrap.Begin((datastoreName) => new SQLiteNetSampleRepositoryContext(storageFilepath, datastoreName));
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