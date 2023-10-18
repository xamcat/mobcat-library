using System;
using System.Threading.Tasks;
using Microsoft.MobCAT.MVVM.Abstractions;
using Microsoft.Maui;

namespace Microsoft.MobCAT.Forms.Services
{
    public class ShellNavigationService : NavigationService, IRouteNavigationService
    {
        public Task GoToRouteAsync(string route)
            => Shell.Current.GoToAsync(new ShellNavigationState(route));

        public Task GoToRouteAsync(Uri route)
            => Shell.Current.GoToAsync(new ShellNavigationState(route));
    }
}