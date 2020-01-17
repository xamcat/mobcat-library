# Repository Pattern with an InMemory Data Store
This walkthrough demonstrates how to implement the **Repository Pattern** with an **in-memory** data store using the base classes provided by the **MobCAT** library. 

This is arranged into the following high-level steps:
1. [Solution Setup](#solution-setup)
2. [Define Common Repository Types](#define-common-repository-types)
3. [Build the **in-memory** implementation](#build-the-in-memory-implementation)
4. [Try it out](#try-it-out)

## Prerequisites
This walkthrough assumes that you have the **MobCAT** library source code or **NuGet** package that can be referenced as a project or a package. See [Getting started with MobCAT](mobcat-gettingstarted.md).

## Solution Setup

1. Create a new **Xamarin.Forms** project called **SampleApp** including **Android** and **iOS** platform targets  
2. Update all **NuGet** packages across the solution
3. Create a new **netstandard** project to contain the app data concerns e.g. **SampleApp.Data**
4. Create a new **netstandard** project for the **in-memory** implementation of the data concerns e.g. **SampleApp.Data.InMemory**
5. Reference the **MobCAT** library and **SampleApp.Data** in all projects
6. Add a reference to **SampleApp.Data.InMemory** in the platform targets 
7. Add the **Newtonsoft.Json** and **Polly** **NuGet** packages to the platform targets

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

## Build the **in-memory** implementation
1. In **SampleApp.Data.InMemory**, add a new class called **InMemorySampleModel** deriving from **BaseInMemoryModel** with the following implementation  

    ```cs
    public class InMemorySampleModel : BaseInMemoryModel
    {
       public string Text { get; set; }
       public long TimestampTicks { get; set; }
    }
    ```

   **NOTE:** The types and names of the **Properties** are purposefully different to the **SampleModel** to make the data mapping exercise more involved  

2. In **SampleApp.Data.InMemory**, add a new class called **InMemorySampleRepository** deriving from **BaseInMemoryRepository** and implementing the **ISampleRepository** interface. 

    ```cs
    public class InMemorySampleRepository : BaseInMemoryRepository<SampleModel, InMemorySampleModel>, ISampleRepository  
    {
    }
   ```
3. In the same file, override the **ToModelType** and **ToRepositoryType** in order to implement the data mapping logic for the repository

   ```cs
   protected override SampleModel ToModelType(InMemorySampleModel repositoryType)
      => repositoryType == null ? null : new SampleModel
      {
            Id = repositoryType.Id,
            Text = repositoryType.Text,
            Timestamp = new DateTimeOffset(repositoryType.TimestampTicks, TimeSpan.Zero) 
      };

   protected override InMemorySampleModel ToRepositoryType(SampleModel modelType)
      => modelType == null ? null : new InMemorySampleModel
      {
            Id = modelType.Id,
            Text = modelType.Text,
            TimestampTicks = modelType.Timestamp.UtcTicks
      };
      ```

      **NOTE:** There are no additional operations defined by the **ISampleRepository**.

4. In **SampleApp.Data.InMemory**, add a new class called **InMemorySampleRepositoryContext** deriving from **BaseInMemoryRepositoryContext** and implementing the **ISampleRepositoryContext** interface

   ```cs
   public class InMemorySampleRepositoryContext : BaseInMemoryRepositoryContext, ISampleRepositoryContext
   {
      public ISampleRepository SampleRepository { get; }
   }
   ```

5. Update **InMemorySampleRepositoryContext** so it instantiates the **ISampleRepository** implementation in a lazy loading manner

   ```cs
   ISampleRepository _sampleRepository;

   public ISampleRepository SampleRepository => _sampleRepository ?? (_sampleRepository = new InMemorySampleRepository());
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
      public static void Begin(
         Func<ISampleRepositoryContext> sampleRepositoryContext)
            => ServiceContainer.Register(sampleRepositoryContext());
   }
   ```

8. In **MainActivity**, within the **Android** target, update the **OnCreate** method to call **Bootstrap.Begin** just before the call to **LoadApplication**

   ```cs      
   Bootstrap.Begin(() => new InMemorySampleRepositoryContext());
   ```

   **NOTE:** We are passing in a **Func** to resolve a new instance of **InMemoryRepositoryContext** as the **ISampleRepositoryContext** implementation at the appropriate juncture

9. In **AppDelegate**, within the **iOS** target, update the **FinishedLaunching** method in the same manner

   ```cs            
   Bootstrap.Begin(() => new InMemorySampleRepositoryContext());
   ```

   **NOTE:** This is the same code as in **MainActivity**. However, this allows flexibility to support platform-specific configuration in future.

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