using Foundation;
using Microsoft.MobCAT.Repository.InMemory.Test;
using NUnit.Runner.Services;
using UIKit;

namespace Microsoft.MobCAT.Repositories.InMemory.Test.iOS 
{
    [Register("AppDelegate")]
    public class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            global::Xamarin.Forms.Forms.Init();

            var nunit = new NUnit.Runner.App
            {
                Options = new TestOptions
                {
                    AutoRun = true,
                    CreateXmlResultFile = true
                }
            };

            Microsoft.MobCAT.Repository.Test.Bootstrap.Begin(() => new InMemorySampleRepositoryContext());

            LoadApplication(nunit);

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}