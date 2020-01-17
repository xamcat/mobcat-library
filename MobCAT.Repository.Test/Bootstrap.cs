using System;
using Microsoft.MobCAT.Repository.Test.Abstractions;

namespace Microsoft.MobCAT.Repository.Test
{
    public static class Bootstrap
    {
        const string DatastoreName = "test_db";

        public static void Begin(Func<ISampleRepositoryContext> sampleRepositoryContext)
            => ServiceContainer.Register(sampleRepositoryContext);

        public static void BeginWithDatastore(Func<string, ISampleRepositoryContext> initializeRepositoryContext)
            => ServiceContainer.Register(initializeRepositoryContext(DatastoreName));
    }
}