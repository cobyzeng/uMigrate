using System.Collections.Generic;
using JetBrains.Annotations;

namespace uMigrate {
    public interface IUmbracoMigration {
        [NotNull] string Version { get; }
        [NotNull] string Name { get; }
        void Migrate([NotNull] IMigrationContext context);
    }
}