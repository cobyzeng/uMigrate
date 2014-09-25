using System.Collections.Generic;

namespace uMigrate {
    public interface IMigrationResolver {
        IEnumerable<IUmbracoMigration> GetAllMigrations();
    }
}