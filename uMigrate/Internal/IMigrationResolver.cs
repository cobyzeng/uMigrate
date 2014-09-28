using System.Collections.Generic;

namespace uMigrate.Internal {
    public interface IMigrationResolver {
        IEnumerable<IUmbracoMigration> GetAllMigrations();
    }
}