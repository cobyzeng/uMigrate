using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Hosting;
using log4net;
using log4net.Config;
using ReflectionMagic;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.IO;

namespace uMigrate.Tests.Integration.Internal {
    public class TestUmbracoApplication : UmbracoApplicationBase {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TestUmbracoApplication));

        protected override IBootManager GetBootManager() {
            return new CoreBootManager(this);
        }

        public void Start() {
            XmlConfigurator.Configure();
            UmbracoConfig.For.AsDynamic().SetUmbracoSettings(new UmbracoSettingsSection());

            ReleaseMainDomLock();

            var database = new TestDatabaseHelper();
            database.Drop();
            database.Create();

            Directory.CreateDirectory(IOHelper.MapPath("~/App_Plugins"));

            Application_Start(this, EventArgs.Empty);
        }

        private void ReleaseMainDomLock() {
            // Umbraco uses a named semaphore that can survive a test restart.
            // So it needs to be released explictly.
            var name = $"UMBRACO-{HostingEnvironment.ApplicationID}-MAINDOM-LCK";
            using (var semaphore = new Semaphore(0, 1, name)) {
                try {
                    while (semaphore.Release() > 0) {
                        Logger.InfoFormat("Explicitly released MainDom semaphore {0}.", name);
                    }
                }
                catch (SemaphoreFullException) {}
            }
        }

        public void Stop() {
            Application_End(this, EventArgs.Empty);
        }

        public ApplicationContext ApplicationContext => ApplicationContext.Current;
    }
}