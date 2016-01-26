using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using Moq;
using Umbraco.Core;
using Umbraco.Core.Configuration.UmbracoSettings;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;
using Umbraco.Core.Persistence.UnitOfWork;
using Umbraco.Core.Profiling;
using Umbraco.Core.Services;

namespace uMigrate.Tests.Integration.Internal {
    partial class TestDatabaseHelper {
        private void UmbracoVersionSpecificCreate() {
            var logger = Logger.CreateWithDefaultLog4NetConfiguration();
            var syntaxProvider = new SqlCeSyntaxProvider();
            var cacheHelper = new CacheHelper();
            var database = new UmbracoDatabase(ConnectionString, new SqlCeProviderFactory(), logger);
            var databaseFactory = Mock.Of<IDatabaseFactory>(f => f.CreateDatabase() == database);
            var databaseContext = new DatabaseContext(
                databaseFactory, logger,
                new SqlSyntaxProviders(new [] { syntaxProvider })
            );
            var migrationEntryService = new MigrationEntryService(
                new PetaPocoUnitOfWorkProvider(databaseFactory),
                new RepositoryFactory(cacheHelper, logger, syntaxProvider, new UmbracoSettingsSection()),
                logger,
                Mock.Of<IEventMessagesFactory>()
            );
            var serviceContext = new ServiceContext(migrationEntryService: migrationEntryService);
            var applicationContext = new ApplicationContext(
                databaseContext, serviceContext, cacheHelper,
                new ProfilingLogger(logger, Mock.Of<IProfiler>())
            );

            var helper = new DatabaseSchemaHelper(databaseContext.Database, logger, syntaxProvider);
            helper.CreateDatabaseSchema(false, applicationContext);
        }
    }
}
