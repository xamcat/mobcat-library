using System;

namespace Microsoft.MobCAT
{
    [AttributeUsage(AttributeTargets.All)]
    public class PreserveAttribute : Attribute
    {
        public bool Conditional { get; set; }
    }
}