using SQLite;

namespace Microsoft.MobCAT.Repository.SQLiteNet
{
    public abstract class BaseSQLiteNetModel
    {
        [PrimaryKey]
        public string Id { get; set; }
    }
}