using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace uMigrate.Internal {
    public class MigrationResolver : IMigrationResolver {
        private readonly IMigrationTypeProvider _typeProvider;

        public MigrationResolver(IMigrationTypeProvider typeProvider) {
            _typeProvider = typeProvider;
        }

        public IEnumerable<IUmbracoMigration> GetAllMigrations() {
            var types = _typeProvider.GetAllMigrationTypes();
            var migrations = types.Select(Activator.CreateInstance)
                                  .Cast<IUmbracoMigration>()
                                  .OrderBy(x => x.Version)
                                  .ToList();

            EnsureNoDuplicates(migrations);
            return migrations;
        }

        private void EnsureNoDuplicates(IReadOnlyList<IUmbracoMigration> migrations) {
            var duplicates = migrations.GroupBy(m => m.Version)
                                       .Where(g => g.Count() > 1)
                                       .ToArray();

            if (duplicates.Length == 0)
                return;

            var message = new StringBuilder("Found multiple migrations with same versions:").AppendLine();
            foreach (var group in duplicates) {
                message.AppendFormat("  Version '{0}':", group.Key).AppendLine();
                foreach (var duplicate in group) {
                    message.Append("    Migration ").AppendLine(duplicate.GetType().AssemblyQualifiedName);
                }
            }
            throw new UmbracoMigrationException(message.ToString());
        }
    }
}