namespace Microsoft.MobCAT.Repository.EntityFrameworkCore.Test.Models
{
    public class EFCoreSampleModel : BaseEFCoreModel
    {
		public string SampleString { get; set; }
		public int SampleInt { get; set; }
		public long TimestampTicks { get; set; }
	}
}