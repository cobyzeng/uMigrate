using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using umbraco;
using umbraco.cms.businesslogic.propertytype;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;

namespace uMigrate.Internal {
    public class MigrationContext : IMigrationContext {
        private static readonly bool UmbracoHasIsolatedCache = 
            typeof (CacheHelper).GetProperty("IsolatedRuntimeCache") != null;

        private readonly CacheHelper _cacheHelper;

        public MigrationContext(
            [NotNull] IServiceContext services,
            [NotNull] UmbracoDatabase database,
            [NotNull] CacheHelper cacheHelper,
            [NotNull] IMigrationRecordRepository migrationRecords,
            [CanBeNull] IMigrationLogger logger = null
        ) {
            Services = Argument.NotNull(nameof(services), services);
            Database = Argument.NotNull(nameof(database), database);
            MigrationRecords = Argument.NotNull(nameof(migrationRecords), migrationRecords);
            Logger = logger;

            _cacheHelper = cacheHelper;
        }

        public IFileSystem GetFileSystem(string rootPath) {
            return new PhysicalFileSystem(rootPath);
        }

        public void ClearRuntimeCache(Type entityType) {
            var cache = (ICacheProvider)_cacheHelper.RuntimeCache;
            if (UmbracoHasIsolatedCache) {
                // Since uMigrate is compiled to Umbraco 7.1.4, it can't statically depend
                // on IsolatedRuntimeCache which doesn't exist there
                cache = (ICacheProvider)((dynamic)_cacheHelper).IsolatedRuntimeCache.GetOrCreateCache(entityType);
            }

            cache.ClearAllCache();
        }

        public void RefreshContent() {
            library.RefreshContent();
        }

        public void WorkaroundToRenamePropertyGroupAndSave(PropertyGroup propertyGroup, string newName) {
            Argument.NotNullOrEmpty("newName", newName);
            var legacyGroup = PropertyTypeGroup.GetPropertyTypeGroup(propertyGroup.Id);
            legacyGroup.Name = newName;
            legacyGroup.Save();
            propertyGroup.Name = newName; // makes sure any later saves would be correct
        }

        public UmbracoDatabase Database { get; private set; }
        public IMigrationRecordRepository MigrationRecords { get; private set; }
        public IMigrationLogger Logger { get; private set; }
        public IServiceContext Services { get; private set; }

        public IMigrationContext WithLogger(IMigrationLogger logger) {
            return new MigrationContext(Services, Database, _cacheHelper, MigrationRecords, logger);
        }
    }
}