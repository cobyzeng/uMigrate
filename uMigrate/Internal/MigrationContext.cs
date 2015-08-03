using System.Collections;
using System.Collections.Generic;
using System.Web;
using JetBrains.Annotations;
using umbraco;
using umbraco.cms.businesslogic.propertytype;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;

namespace uMigrate.Internal {
    public class MigrationContext : IMigrationContext {
        public MigrationContext(
            [NotNull] IServiceContext services,
            [NotNull] UmbracoDatabase database,
            [NotNull] IMigrationRecordRepository migrationRecords,
            [CanBeNull] IMigrationLogger logger = null
        ) {
            Services = Argument.NotNull("services", services);
            Database = Argument.NotNull("database", database);
            MigrationRecords = Argument.NotNull("migrationRecords", migrationRecords);
            Logger = logger;
        }

        public IFileSystem GetFileSystem(string rootPath) {
            return new PhysicalFileSystem(rootPath);
        }

        public void ClearCaches() {
            // kind of hacky, but Umbraco RuntimeCache is internal
            if (HttpContext.Current == null)
                return;

            var cache = HttpContext.Current.Cache;
            foreach (DictionaryEntry entry in cache){
                cache.Remove((string)entry.Key);
            }        
        }

        public void RefreshContent() {
            library.RefreshContent();
        }

        public void WorkaroundToRenamePropertyGroupAndSave(PropertyGroup propertyGroup, string newName) {
            var oldApiGroup = PropertyTypeGroup.GetPropertyTypeGroup(propertyGroup.Id);
            oldApiGroup.Name = newName;
            oldApiGroup.Save();
            propertyGroup.Name = newName; // makes sure any later saves would be correct
        }

        public UmbracoDatabase Database { get; private set; }
        public IMigrationRecordRepository MigrationRecords { get; private set; }
        public IMigrationLogger Logger { get; private set; }
        public IServiceContext Services { get; private set; }

        public IMigrationContext WithLogger(IMigrationLogger logger) {
            return new MigrationContext(Services, Database, MigrationRecords, logger);
        }
    }
}