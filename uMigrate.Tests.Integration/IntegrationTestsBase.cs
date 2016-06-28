using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using Moq;
using NUnit.Framework;
using uMigrate.Infrastructure;
using uMigrate.Internal;
using uMigrate.Tests.Integration.Internal;
using Umbraco.Core.Services;

namespace uMigrate.Tests.Integration {
    public abstract class IntegrationTestsBase {
        private readonly DelegateMigration _migration = new DelegateMigration();
        private UmbracoMigrator _migrator;
        private TestUmbracoApplication _application;

        [SetUp]
        protected virtual void BeforeEachTest() {
            _application = new TestUmbracoApplication();
            _application.Start();

            MigrationRecords = new DatabaseMigrationRecordRepository(_application.ApplicationContext.DatabaseContext.Database);
            MigrationConfiguration = new MigrationConfiguration();
            _migrator = CreateMigrator();
        }

        [TearDown]
        protected virtual void AfterEachTest() {
            if (_application != null) {
                _application.Stop();
                ((IDisposable) _application.ApplicationContext)?.Dispose();
            }
            _application = null;
            _migrator = null;
        }

        protected IMigrationRecordRepository MigrationRecords { get; private set; }
        protected MigrationConfiguration MigrationConfiguration { get; private set; }

        protected ServiceContext Services => _application?.ApplicationContext.Services;

        private UmbracoMigrator CreateMigrator() {
            var applicationContext = _application.ApplicationContext;
            return new UmbracoMigrator(
                Mock.Of<IMigrationResolver>(m => m.GetAllMigrations() == new[] { _migration }),
                new MigrationContext(
                    new ServiceContextWrapper(applicationContext.Services),
                    applicationContext.DatabaseContext.Database,
                    applicationContext.ApplicationCache,
                    MigrationRecords,
                    MigrationConfiguration,
                    new MigrationLogger(TextWriter.Null, LogManager.GetLogger(typeof(MigrationLogger)))
                ),
                LogManager.GetLogger(typeof(UmbracoMigrator))
            );
        }

        protected void Prepare([InstantHandle] Action<UmbracoMigrationBase> run) {
            // the difference is only in sematics (Prepare = Arrange, Migrate = Act)
            Migrate(run);
        }

        protected void Migrate([InstantHandle] Action<UmbracoMigrationBase> run) {
            _migration.NextRun = run;
            _migrator.Run();
        }

        protected T GetFluent<T>([InstantHandle] Func<UmbracoMigrationBase, T> get) {
            var result = default(T);
            _migration.NextRun = m => result = get(m);
            _migrator.Run();

            return result;
        }

        private class DelegateMigration : UmbracoMigrationBase {
            public Action<UmbracoMigrationBase> NextRun { get; set; }
            protected override void Run() {
                NextRun(this);
            }

            public override string Version => $"Test-{Guid.NewGuid():N}";
        }
    }
}
