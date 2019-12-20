using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MobCAT.Repository.Test.Abstractions;
using Microsoft.MobCAT.Repository.Test.Models;
using NUnit.Framework;

namespace Microsoft.MobCAT.Repository.Test
{
    [Parallelizable(ParallelScope.None)]
    public class BasicOperationTests
    {
        ISampleRepositoryContext _repositoryContext;
        SampleModel _singleOperationsTestData;
        IEnumerable<SampleModel> _bulkOperationsTestData;

        ISampleRepositoryContext RepositoryContext => _repositoryContext ?? (_repositoryContext = ServiceContainer.Resolve<ISampleRepositoryContext>());

        readonly Action<SampleModel, SampleModel, string> ValidateSampleModelOperation = (item, comparisonItem, operation) =>
        {
            Assert.True(item != null, $"Item was not inserted, but the {nameof(operation)} operation returned successfully");
            Assert.True(item.Id == comparisonItem.Id, $"{nameof(item.Id)} was not inserted and/or parsed correctly");
            Assert.True(item.SampleIntProperty == comparisonItem.SampleIntProperty, $"{nameof(item.SampleIntProperty)} was not inserted and/or parsed correctly");
            Assert.True(item.SampleStringProperty == comparisonItem.SampleStringProperty, $"{nameof(item.SampleStringProperty)} was not inserted and/or parsed correctly");
            Assert.True(item.Timestamp.UtcTicks == comparisonItem.Timestamp.UtcTicks, $"{nameof(item.Timestamp)} not inserted and/or parsed correctly");
        };

        [SetUp]
        public void Arrange()
        {
            RepositoryContext.SetupAsync().GetAwaiter().GetResult();
            _singleOperationsTestData = TestData.GenerateSingleOperationsTestData(0);
            _bulkOperationsTestData = TestData.GenerateBulkOperationsTestData(5);
        }

        [TearDown]
        public void Teardown()
        {
            RepositoryContext.DeleteAsync().GetAwaiter().GetResult();
            _singleOperationsTestData = null;
            _bulkOperationsTestData = null;
        }

        [Test, Order(0), TestCase(TestName = "Drop Table")]
        public void DropTableTest()
        {
            // Verifies it is possible to perform operations following drop table
            ISampleRepository sampleRepository = RepositoryContext.SampleRepository;

            Assert.NotNull(sampleRepository);

            var comparisonItem = _singleOperationsTestData;

            // Create (insert) operation (before drop table)
            sampleRepository.InsertItemAsync(_singleOperationsTestData).GetAwaiter().GetResult();

            // Read (get) operation
            var item = sampleRepository.GetItemAsync(_singleOperationsTestData.Id).Result;
            ValidateSampleModelOperation(item, comparisonItem, nameof(sampleRepository.InsertItemAsync));

            // Perform Drop Table operation
            sampleRepository.DropTableAsync().GetAwaiter().GetResult();

            // Create (insert) operation (after drop table)
            sampleRepository.InsertItemAsync(_singleOperationsTestData).GetAwaiter().GetResult();

            // Read (get) operation
            item = sampleRepository.GetItemAsync(_singleOperationsTestData.Id).Result;
            ValidateSampleModelOperation(item, comparisonItem, nameof(sampleRepository.InsertItemAsync));
        }

        [Test, Order(1), TestCase(TestName = "Single Operations")]
        public void SingleOperationsTest()
        {
            ISampleRepository sampleRepository = RepositoryContext.SampleRepository;
            Assert.NotNull(sampleRepository);

            // Create (insert) operation
            sampleRepository.InsertItemAsync(_singleOperationsTestData).GetAwaiter().GetResult();

            // Read (get) operation
            var item = sampleRepository.GetItemAsync(_singleOperationsTestData.Id).Result;
            var comparisonItem = _singleOperationsTestData;

            ValidateSampleModelOperation(item, comparisonItem, nameof(sampleRepository.InsertItemAsync));

            // Update operation
            item.SampleIntProperty = ++item.SampleIntProperty;
            item.SampleStringProperty = $"{item.SampleStringProperty} - UPDATED";
            var currentTimestamp = DateTimeOffset.UtcNow;
            item.Timestamp = currentTimestamp;
            sampleRepository.UpdateItemAsync(item).GetAwaiter().GetResult();

            // Re-read to validate updates
            var updatedItem = sampleRepository.GetItemAsync(_singleOperationsTestData.Id).GetAwaiter().GetResult();
            ValidateSampleModelOperation(updatedItem, item, nameof(sampleRepository.UpdateItemAsync));

            // Upsert Operation (existing item)
            updatedItem.SampleStringProperty = updatedItem.SampleStringProperty.Replace("UPDATED", "UPSERTED");
            sampleRepository.UpsertItemAsync(updatedItem).GetAwaiter().GetResult();

            // Re-read to validate updates
            var upsertedExistingItem = sampleRepository.GetItemAsync(_singleOperationsTestData.Id).GetAwaiter().GetResult();
            ValidateSampleModelOperation(upsertedExistingItem, updatedItem, nameof(sampleRepository.UpdateItemAsync));

            // Upsert Operation (new item)
            var upsertNewItem = new SampleModel
            {
                Id = _singleOperationsTestData.Id + "_new_upsert",
                SampleIntProperty = 1,
                SampleStringProperty = "UPSERTED ITEM",
                Timestamp = DateTimeOffset.UtcNow
            };

            sampleRepository.UpsertItemAsync(upsertNewItem).GetAwaiter().GetResult();

            // Re-read to validate updates
            var upsertedNewItem = sampleRepository.GetItemAsync(upsertNewItem.Id).GetAwaiter().GetResult();
            ValidateSampleModelOperation(upsertedNewItem, upsertNewItem, nameof(sampleRepository.UpsertItemAsync));

            var allItems = sampleRepository.GetAsync().GetAwaiter().GetResult();
            Assert.True(allItems.Count() == 2, $"New item was not inserted as a result of the upsert operation, but the {nameof(sampleRepository.UpsertItemAsync)} operation returned successfully");

            // Delete (remove) operation
            sampleRepository.RemoveItemAsync(updatedItem).GetAwaiter().GetResult();

            // Re-read to validate updates
            var expectedNullResult = sampleRepository.GetItemAsync(_singleOperationsTestData.Id).GetAwaiter().GetResult();
            Assert.IsNull(expectedNullResult, $"Item was not deleted, but the {nameof(sampleRepository.RemoveItemAsync)} operation returned successfully");
        }

        [Test, Order(2), TestCase(TestName = "Bulk Operations")]
        public void BulkOperationsTest()
        {
            ISampleRepository sampleRepository = RepositoryContext.SampleRepository;
            Assert.NotNull(sampleRepository);

            // Bulk create (insert) operation
            sampleRepository.InsertAsync(_bulkOperationsTestData).GetAwaiter().GetResult();

            // Read all (get all) operation
            var persistedItems = sampleRepository.GetAsync().GetAwaiter().GetResult();
            Assert.True(persistedItems.Count() == _bulkOperationsTestData.Count(), $"Unexpected number of items persisted in {nameof(sampleRepository.InsertAsync)}");

            for (int i = 0; i < _bulkOperationsTestData.Count(); i++)
                ValidateSampleModelOperation(persistedItems.ElementAt(i), _bulkOperationsTestData.ElementAt(i), nameof(sampleRepository.InsertAsync));

            // Bulk update operation
            var currentTimestamp = DateTimeOffset.UtcNow;

            foreach (var item in persistedItems)
            {
                item.SampleIntProperty = ++item.SampleIntProperty;
                item.SampleStringProperty = $"{item.SampleStringProperty} - UPDATED";
                item.Timestamp = currentTimestamp;
            }

            sampleRepository.UpdateAsync(persistedItems).GetAwaiter().GetResult();

            // Re-read to validate updates
            var persistedUpdatedItems = sampleRepository.GetAsync().GetAwaiter().GetResult();

            for (int i = 0; i < _bulkOperationsTestData.Count(); i++)
                ValidateSampleModelOperation(persistedUpdatedItems.ElementAt(i), persistedItems.ElementAt(i), nameof(sampleRepository.UpdateAsync));

            // Bulk delete (remove) operation
            sampleRepository.RemoveAsync(persistedUpdatedItems).GetAwaiter().GetResult();

            // Re-read to validate updates
            var expectedNoResults = sampleRepository.GetAsync().Result;
            Assert.That(!expectedNoResults.Any(), $"Items were not deleted, but the {nameof(sampleRepository.RemoveAsync)} operation returned successfully");
        }
    }
}