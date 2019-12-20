using System;
using System.Collections.Generic;
using Microsoft.MobCAT.Repository.Test.Models;

namespace Microsoft.MobCAT.Repository.Test
{
    internal static class TestData
    {
		internal static IList<SampleModel> GenerateBulkOperationsTestData(int collectioncount)
        {
            var dataItems = new List<SampleModel>();

            for (int i = 0; i < collectioncount; i++)
                dataItems.Add(GenerateSingleOperationsTestData(i + 1));

            return dataItems;
        }

		internal static SampleModel GenerateSingleOperationsTestData(int index)
        {
            return new SampleModel
            {
                Id = (index).ToString(),
                SampleStringProperty = $"Test item {index}",
                SampleIntProperty = index,
                Timestamp = DateTimeOffset.UtcNow
            };
        }
    }
}