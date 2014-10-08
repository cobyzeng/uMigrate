using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using uMigrate.Internal;

namespace uMigrate.Infrastructure {
    public class UmbracoMigrator {
        [PublicAPI] public static event EventHandler<MigrationRunStartingEventArgs> RunStarting = delegate {};
        [PublicAPI] public static event EventHandler<MigrationRunEventArgs> RunCompleted = delegate {};

        [PublicAPI] public static event EventHandler<MigrationStartingEventArgs> MigrationStarting = delegate {};
        [PublicAPI] public static event EventHandler<MigrationEventArgs> MigrationCompleted = delegate {};

        private readonly IMigrationResolver _migrationResolver;
        private readonly IMigrationContext _context;

        public UmbracoMigrator(IMigrationResolver migrationResolver, IMigrationContext context) {
            _migrationResolver = migrationResolver;
            _context = context;
        }

        public void Run() {
            var runStartingArgs = new MigrationRunStartingEventArgs(_context);
            RunStarting(this, runStartingArgs);
            if (runStartingArgs.Cancel)
                return;

            var alreadyMigratedVersions = _context.MigrationRecords.GetAll().Select(x => x.Version);
            var migrations = _migrationResolver.GetAllMigrations()
                .Where(x => !alreadyMigratedVersions.Contains(x.Version));

            migrations.MigrateEach(migration => {
                var startingArgs = new MigrationStartingEventArgs(migration, _context);
                MigrationStarting(this, startingArgs);
                if (startingArgs.Cancel)
                    return;

                var logWriter = new StringWriter();
                migration.Migrate(_context.WithLogger(new MigrationLogger(logWriter)));

                _context.MigrationRecords.SaveNew(new MigrationRecord {
                    Version = migration.Version,
                    DateExecuted = DateTime.UtcNow,
                    Name = migration.Name,
                    Log = logWriter.ToString()
                });
                MigrationCompleted(this, new MigrationEventArgs(migration, _context));
            }, migration => migration.GetType().Name);

            _context.ContentService.RePublishAll();
            _context.RefreshContent();

            RunCompleted(this, new MigrationRunEventArgs(_context));
        }
    }
}