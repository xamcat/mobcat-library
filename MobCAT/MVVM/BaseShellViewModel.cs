using System;
using System.Threading.Tasks;
using Microsoft.MobCAT.MVVM.Abstractions;

namespace Microsoft.MobCAT.MVVM
{
    public class BaseShellViewModel : BaseNavigationViewModel
    {
        private IRouteNavigationService _registeredService;

        protected new IRouteNavigationService Navigation
        {
            get
            {
                if (_registeredService == null)
                {
                    _registeredService = ServiceContainer.Resolve<IRouteNavigationService>(true);

                    if (_registeredService == null)
                        Logger.Warn($"No {nameof(IRouteNavigationService)} implementation has been registered.");
                }
                return _registeredService;
            }
        }
    }
}