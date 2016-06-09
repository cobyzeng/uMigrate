using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;

namespace uMigrate.Internal {
    public class AppDomainAssemblyMigrationTypeProvider : IMigrationTypeProvider {
        private readonly ILog _logger;

        public AppDomainAssemblyMigrationTypeProvider(ILog logger) {
            _logger = logger;
        }

        public IEnumerable<Type> GetAllMigrationTypes() {
            var baseType = typeof(IUmbracoMigration);
            return AppDomain.CurrentDomain.GetAssemblies()
                            .SelectMany(GetTypesSafe)
                            .Where(baseType.IsAssignableFrom)
                            .Where(t => !t.IsInterface && !t.IsAbstract && !t.IsNestedPrivate);
        }

        private IEnumerable<Type> GetTypesSafe(Assembly assembly) {
            Type[] types;
            try {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex) {
                _logger.Error($"Failed to load one or more types from assembly {assembly.FullName}:{Environment.NewLine}{string.Join(Environment.NewLine, ex.LoaderExceptions.AsEnumerable())}", ex);
                types = ex.Types;
            }
            return types;
        }
    }
}