using JetBrains.Annotations;

namespace uMigrate.Infrastructure {
    public class MigrationStartingEventArgs : MigrationEventArgs {
        public MigrationStartingEventArgs([NotNull] IUmbracoMigration migration, [NotNull] IMigrationContext context) : base(migration, context) {
        }

        public bool Cancel { get; set; }
    }
}