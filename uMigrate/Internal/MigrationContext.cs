using System.Collections;
using System.Collections.Generic;
using System.Web;
using JetBrains.Annotations;
using umbraco;
using umbraco.cms.businesslogic.propertytype;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;

namespace uMigrate.Internal {
    public class MigrationContext : IMigrationContext {
        [NotNull]
        private readonly ServiceContext _services;

        public MigrationContext(
            [NotNull] ServiceContext services,
            [NotNull] UmbracoDatabase database,
            [NotNull] IMigrationRecordRepository migrationRecords,
            [CanBeNull] IMigrationLogger logger = null
        ) {
            _services = services;
            Database = database;
            MigrationRecords = migrationRecords;
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

        public IContentService ContentService {
            get { return _services.ContentService; }
        }

        public IContentTypeService ContentTypeService {
            get { return _services.ContentTypeService; }
        }

        public IDataTypeService DataTypeService {
            get { return _services.DataTypeService; }
        }

        public IFileService FileService {
            get { return _services.FileService; }
        }

        public IMediaService MediaService {
            get { return _services.MediaService; }
        }

        public IUserService UserService {
            get { return _services.UserService; }
        }

        public IMigrationContext WithLogger(IMigrationLogger logger) {
            return new MigrationContext(_services, Database, MigrationRecords, logger);
        }
    }
}