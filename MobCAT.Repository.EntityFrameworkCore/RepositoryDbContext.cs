using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore
{
    public class RepositoryDbContext<T> : DbContext where T : BaseEFCoreModel, new()
    {
        readonly SqliteConnection _connection;

        public RepositoryDbContext(SqliteConnection connection)
        {
            _connection = connection;
        }

        public DbSet<T> Items { get; set; }

        public string TableName => GetTableName();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSqlite(_connection);

        string GetTableName()
        {
            var entityTypes = Model.GetEntityTypes();
            var entityType = entityTypes.First(t => t.ClrType == typeof(T));
            var tableNameAnnotation = entityType.GetAnnotation("Relational:TableName");

            return tableNameAnnotation.Value.ToString();
        }
    }
}