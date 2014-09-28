using System;
using System.Collections.Generic;
using System.Linq;

namespace uMigrate.Internal {
    public class AppDomainAssemblyMigrationTypeProvider : IMigrationTypeProvider {
        public IEnumerable<Type> GetAllMigrationTypes() {
            var baseType = typeof(IUmbracoMigration);
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(a => a.GetTypes())
                            .Where(baseType.IsAssignableFrom)
                            .Where(t => !t.IsInterface && !t.IsAbstract);
        }
    }
}