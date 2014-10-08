using JetBrains.Annotations;

namespace uMigrate.Infrastructure {
    public class MigrationEventArgs : MigrationRunEventArgs {
        public MigrationEventArgs([NotNull] IUmbracoMigration migration, [NotNull] IMigrationContext context) : base(context) {
            Migration = Argument.NotNull("migration", migration);
        }

        [NotNull] public IUmbracoMigration Migration { get; private set; }
    }
}