using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using log4net.Core;
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

            var database = _application.ApplicationContext.DatabaseContext.Database;
            MigrationRecords = new DatabaseMigrationRecordRepository(database);
            _migrator = new UmbracoMigrator(
                Mock.Of<IMigrationResolver>(m => m.GetAllMigrations() == new[] {_migration}),
                new MigrationContext(
                    new ServiceContextWrapper(_application.ApplicationContext.Services),
                    database,
                    MigrationRecords,
                    new MigrationLogger(TextWriter.Null, LogManager.GetLogger(typeof(MigrationLogger)))
                ),
                LogManager.GetLogger(typeof(UmbracoMigrator))
            );
        }

        [TearDown]
        protected virtual void AfterEachTest() {
            if (_application != null) {
                _application.Stop();
                ((IDisposable) _application.ApplicationContext).Dispose();
            }
            _application = null;
            _migrator = null;
        }

        public IMigrationRecordRepository MigrationRecords { get; private set; }
        
        protected ServiceContext Services {
            get {
                if (_application == null)
                    return null;

                return _application.ApplicationContext.Services;
            }
        }

        protected void RunMigration(Action<UmbracoMigrationBase> run) {
            _migration.NextRun = run;
            _migrator.Run();
        }

        private class DelegateMigration : UmbracoMigrationBase {
            public Action<UmbracoMigrationBase> NextRun { get; set; }
            protected override void Run() {
                NextRun(this);
            }

            public override string Version {
                get { return "Test-" + Guid.NewGuid().ToString("N"); }
            }
        }
    }
}
