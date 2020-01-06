using System;
using System.Threading.Tasks;
using Microsoft.MobCAT.Repository.Abstractions;

namespace Microsoft.MobCAT.Repository.InMemory
{
    public class BaseInMemoryRepositoryContext : IRepositoryContext
    {
        protected virtual void OnResetRepositories() { }

        public Task DeleteAsync()
            => Task.Run(new Action(OnResetRepositories));

        public Task ResetAsync()
            => Task.Run(new Action(OnResetRepositories));

        public Task SetupAsync()
            => Task.Run(new Action(OnResetRepositories));

        public void ResetRepositories()
            => OnResetRepositories();
    }
}