using System;
using System.IO;
using System.Reflection;
using log4net.Config;
using Umbraco.Core;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.IO;

namespace uMigrate.Tests.Integration.Internal {
    public class TestUmbracoApplication : UmbracoApplicationBase {
        protected override IBootManager GetBootManager() {
            return new CoreBootManager(this);
        }

        public void Start() {
            XmlConfigurator.Configure();
            typeof(UmbracoConfig).GetMethod(
                "SetUmbracoSettings",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
            ).Invoke(UmbracoConfig.For, new[] { new UmbracoSettingsSection() });

            var database = new TestDatabaseHelper();
            database.Drop();
            database.Create();

            Directory.CreateDirectory(IOHelper.MapPath("~/App_Plugins"));
            Application_Start(this, EventArgs.Empty);
        }

        public void Stop() {
            Application_End(this, EventArgs.Empty);
        }

        public ApplicationContext ApplicationContext {
            get { return ApplicationContext.Current; }
        }
    }
}