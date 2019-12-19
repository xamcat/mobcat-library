using System.ComponentModel.DataAnnotations;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore
{
    public abstract class BaseEFCoreModel
    {
        [Key, Required]
        public string Id { get; set; }
    }
}