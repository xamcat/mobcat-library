using System;
using System.IO;
using Foundation;
using Microsoft.MobCAT.Repository.Test;
using NUnit.Runner.Services;
using UIKit;

namespace Microsoft.MobCAT.Repository.EntityFrameworkCore.Test.iOS
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

            var storageFilepath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library");
            Bootstrap.Begin((datastoreName) => new EFCoreSampleRepositoryContext(Guard.NullOrWhitespace(storageFilepath), datastoreName));

            LoadApplication(nunit);

            return base.FinishedLaunching(uiApplication, launchOptions);
        }
    }
}