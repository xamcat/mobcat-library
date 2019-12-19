using System;
using Microsoft.MobCAT.Repository.Test.Abstractions;

namespace Microsoft.MobCAT.Repository.Test
{
    public static class Bootstrap
    {
        const string DatastoreName = "sqlitenet_testdb";

        public static void Begin(Func<string, ISampleRepositoryContext> sampleRepositoryContext)
            => ServiceContainer.Register(sampleRepositoryContext(DatastoreName));
    }
}