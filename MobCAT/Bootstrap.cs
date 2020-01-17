using System;

namespace Microsoft.MobCAT
{
    /// <summary>
    /// Supports registration and initialization of application dependencies in a centralized manner.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Register and initialize commaon and platform-specific dependencies.
        /// </summary>
        /// <param name="commonBegin">Action used to handle registration and initialization of common application dependencies.</param>
        /// <param name="platformSpecificBegin">Action used to handle registration and initialization of platform-specific application dependencies.</param>
        public static void Begin(
           Action commonBegin = null,
           Action platformSpecificBegin = null)
        {
            commonBegin?.Invoke();
            platformSpecificBegin?.Invoke();
        }
    }
}