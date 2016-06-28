using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using JetBrains.Annotations;
using log4net;
using Umbraco.Core;
using uMigrate.Internal;

namespace uMigrate.Infrastructure {
    [UsedImplicitly, PublicAPI]
    public class MigrationApplicationEventHandler : ApplicationEventHandler {
        private const string EnabledAppSettingKey = "uMigrate:Enabled";
        
        // that's just for some performance optimizations, e.g. you may skip reacting to changes during migration.
        [PublicAPI] public static bool MigrationRunning { get; private set; }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
            var logger = LogManager.GetLogger(typeof(MigrationApplicationEventHandler));

            MigrationRunning = true;
            try {
                var enabled = GetEnabled();
                if (!enabled) {
                    logger.WarnFormat("uMigrate is disabled (app setting {0} set to false), skipping.", EnabledAppSettingKey);
                    return;
                }

                RunMigrations(applicationContext);
            }
            catch (Exception ex) {
                logger.Fatal("uMigrate failed.", ex);
                throw;
            }
            finally {
                MigrationRunning = false;
            }
        }

        private static bool GetEnabled() {
            var enabledString = ConfigurationManager.AppSettings[EnabledAppSettingKey];
            if (string.IsNullOrEmpty(enabledString))
                return true;

            try {
                return bool.Parse(enabledString);
            }
            catch (FormatException ex) {
                throw new Exception($"Failed to parse app setting {EnabledAppSettingKey} as boolean: {ex.Message}.", ex);
            }
        }

        private void RunMigrations(ApplicationContext applicationContext) {
            if (HttpContext.Current != null && HttpContext.Current.User == null)
                HttpContext.Current.User = new GenericPrincipal(new GenericIdentity("0"), new string[0]);

            var recordRepository = new DatabaseMigrationRecordRepository(applicationContext.DatabaseContext.Database);
            var migrationResolver = new MigrationResolver(
                new AppDomainAssemblyMigrationTypeProvider(LogManager.GetLogger(typeof(AppDomainAssemblyMigrationTypeProvider)))
            );
            var context = new MigrationContext(
                new ServiceContextWrapper(applicationContext.Services),
                applicationContext.DatabaseContext.Database,
                applicationContext.ApplicationCache,
                recordRepository,
                new MigrationConfiguration()
            );
            var logger = LogManager.GetLogger(typeof(UmbracoMigrator));
            var migrator = new UmbracoMigrator(migrationResolver, context, logger);

            migrator.Run();
        }
    }
}