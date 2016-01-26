using System;
using JetBrains.Annotations;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Persistence;
using uMigrate.Internal;

namespace uMigrate {
    [PublicAPI]
    public interface IMigrationContext {
        [PublicAPI] [NotNull] IServiceContext Services { get; }
        [NotNull] IFileSystem GetFileSystem(string rootPath);

        // this one is not mockable at the moment, but we may not want to use it in the future anyway
        [NotNull] UmbracoDatabase Database { get; }
        
        // this should not normally be used by migrations, but just in case
        [NotNull] IMigrationRecordRepository MigrationRecords { get; }

        [CanBeNull] IMigrationLogger Logger { get; }

        void ClearRuntimeCache(Type entityType);

        // wraps umbraco.library.RefreshContent
        void RefreshContent();

        // ContentTypeService crashes if you try to do rename through it, so I am using old APIs here
        void WorkaroundToRenamePropertyGroupAndSave([NotNull] PropertyGroup propertyGroup, [NotNull] string newName);

        [NotNull] IMigrationContext WithLogger([CanBeNull] IMigrationLogger logger);
    }
}