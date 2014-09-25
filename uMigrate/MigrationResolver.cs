using System;
using System.Collections.Generic;
using System.Linq;

namespace uMigrate {
    public class MigrationResolver : IMigrationResolver {
        public IEnumerable<IUmbracoMigration> GetAllMigrations() {
            var type = typeof(IUmbracoMigration);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .ToList()
                .SelectMany(a => a.GetTypes())
                .Where(x => type.IsAssignableFrom(x) && x.GetConstructor(Type.EmptyTypes) != null);

            var migrations = types.Select(Activator.CreateInstance)
                .Cast<IUmbracoMigration>()
                .OrderBy(x => x.Version)
                .ToList();

            return migrations;
        }
    }
}