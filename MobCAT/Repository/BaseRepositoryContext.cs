using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.MobCAT.Repository.Abstractions;

namespace Microsoft.MobCAT.Repository
{
    /// <summary>
    /// Base class for implementing a repository store with one or more repositories sharing a single connection.
    /// </summary>
    /// <typeparam name="T">The datastore connection <see cref="Type"/> used by the repository store.</typeparam>
    public class BaseRepositoryContext<T> : IBaseRepositoryContext<T>, IDisposable
    {
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseRepositoryContext{T}"/> class.
        /// </summary>
        /// <param name="folderPath">Filepath representing the storage location for the datastore.</param>
        /// <param name="datastoreName">Name of the datastore without extension.</param>
        public BaseRepositoryContext(string folderPath, string datastoreName)
        {
            Guard.NullOrWhitespace(folderPath);
            Guard.NullOrWhitespace(datastoreName);
            DatastoreName = $"{datastoreName}.db3";
            DatastoreFilepath = Path.Combine(folderPath, datastoreName);
            SetupAsync().ConfigureAwait(false); 
        }

        /// <summary>
        /// Gets the name of the datastore.
        /// </summary>
        public string DatastoreName { get; private set; }

        /// <summary>
        /// Gets the filepath for the datastore.
        /// </summary>
        public string DatastoreFilepath { get; private set; }

        /// <summary>
        /// Gets the <see cref="T"/> connection for the datastore.
        /// </summary>
        public T Connection
        {
            get;
            protected set;
        }

        /// <summary>
        /// Derived classes should use this to create the connection and register the repositories associated with that store.
        /// </summary>
        /// <remarks>Platform-specific actions are required</remarks>
        /// <returns>The Task.</returns>
        protected virtual Task<T> OnOpenConnectionAsync(string datastoreFilepath) => throw new NotImplementedException();

        /// <summary>
        /// Derived classes should use this to close the connection and handle clean-up of repositories associated with that store.
        /// </summary>
        /// <returns>The Task.</returns>
        protected virtual Task OnCloseConnectionAsync() => throw new NotImplementedException();

        protected virtual void OnResetRepositories() { }

        /// <summary>
        /// Protected Dispose implementation.
        /// </summary>
        /// <param name="disposing">Flag indicating whether the call to <see cref="Dispose(bool)"/> comes from a <see cref="Dispose()"/> method or a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    try
                    {
                        ResetAsync().GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                    }
                }

                _disposed = true;
            }
        }

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Resets the store connection and clears associated repositories.
        /// </summary>
        /// <returns>The Task.</returns>
        public async Task ResetAsync()
        {
            await OnCloseConnectionAsync();
            ResetRepositories();

            Connection = default(T);
        }

        /// <summary>
        /// Establishes a connection to the datastore and registers the repositories associated with it.
        /// </summary>
        /// <returns>The Task.</returns>
        public async Task SetupAsync()
        {
            if (!EqualityComparer<T>.Default.Equals(Connection, default(T)))
                await ResetAsync();

            Connection = await OnOpenConnectionAsync(DatastoreFilepath);
        }

        /// <summary>
        /// Deletes the file underpinning the local datastore.
        /// </summary>
        /// <returns>The Task.</returns>
        public async Task DeleteAsync()
        {
            if (!EqualityComparer<T>.Default.Equals(Connection, default(T)))
                await ResetAsync();

            if (File.Exists(DatastoreFilepath))
                File.Delete(DatastoreFilepath);

            await Task.Delay(500); // TODO: Review - this is required on Android to prevent transient test failures!
        }

        /// <summary>
        /// Resets the repositories managed by the <see cref="BaseRepositoryContext{T}"/> store.
        /// </summary>
        public void ResetRepositories()
            => OnResetRepositories();
    }
}