namespace Microsoft.MobCAT.Repository.SQLiteNet.Test.Models
{
    public class SQLiteNetSampleModel : BaseSQLiteNetModel
    {
        public string SampleString { get; set; }
        public int SampleInt { get; set; }
        public long TimestampTicks { get; set; }
    }
}