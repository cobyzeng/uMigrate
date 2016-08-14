using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using umbraco;
using umbraco.cms.businesslogic.propertytype;
using uMigrate.Infrastructure;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Repositories;

namespace uMigrate.Internal {
    public class MigrationContext : IMigrationContext {
        private static readonly bool UmbracoHasIsolatedCache = typeof(CacheHelper).GetProperty("IsolatedRuntimeCache") != null;

        // I think this is for early versions only (e.g. 7.1.4)
        private static readonly Action<Type> ClearLegacyRepositoryCache = BuildClearLegacyRepositoryCache();

        private readonly CacheHelper _cacheHelper;

        public MigrationContext(
            [NotNull] IServiceContext services,
            [NotNull] UmbracoDatabase database,
            [NotNull] CacheHelper cacheHelper,
            [NotNull] IMigrationRecordRepository migrationRecords,
            [NotNull] MigrationConfiguration configuration,
            [CanBeNull] IMigrationLogger logger = null
        ) {
            Services = Argument.NotNull(nameof(services), services);
            Database = Argument.NotNull(nameof(database), database);
            MigrationRecords = Argument.NotNull(nameof(migrationRecords), migrationRecords);
            Configuration = Argument.NotNull(nameof(configuration), configuration);
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

            // This is some private APIs that don't seem to be relevant since at least 7.3.5,
            // but we'll keep them until we drop 7.1.4
            ClearLegacyRepositoryCache?.Invoke(entityType);
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
        public MigrationConfiguration Configuration { get; }

        public IMigrationContext WithLogger(IMigrationLogger logger) {
            return new MigrationContext(Services, Database, _cacheHelper, MigrationRecords, Configuration, logger);
        }

        private static Action<Type> BuildClearLegacyRepositoryCache() {
            var assembly = typeof(IRepository).Assembly;
            var currentProviderProperty = assembly.GetType("Umbraco.Core.Persistence.Caching.RuntimeCacheProvider")?.GetProperty("Current");
            if (currentProviderProperty == null)
                return null;

            var clearMethod = assembly.GetType("Umbraco.Core.Persistence.Caching.IRepositoryCacheProvider")?.GetMethod("Clear");
            if (clearMethod == null)
                return null;

            var typeParameter = Expression.Parameter(typeof(Type));
            return Expression.Lambda<Action<Type>>(
                Expression.Call(Expression.Property(null, currentProviderProperty), clearMethod, typeParameter),
                typeParameter
            ).Compile();
        }
    }
}