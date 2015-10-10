using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using uMigrate.Internal;

namespace uMigrate.Infrastructure {
    public class UmbracoMigrator {
        [PublicAPI] public static event EventHandler<MigrationRunStartingEventArgs> RunStarting = delegate {};
        [PublicAPI] public static event EventHandler<MigrationRunEventArgs> RunCompleted = delegate {};

        [PublicAPI] public static event EventHandler<MigrationStartingEventArgs> MigrationStarting = delegate {};
        [PublicAPI] public static event EventHandler<MigrationEventArgs> MigrationCompleted = delegate {};

        private readonly IMigrationResolver _migrationResolver;
        private readonly IMigrationContext _context;
        private readonly ILog _logger;

        public UmbracoMigrator(IMigrationResolver migrationResolver, IMigrationContext context, ILog logger) {
            _migrationResolver = migrationResolver;
            _context = context;
            _logger = logger;
        }

        public void Run() {
            _logger.Info("Migration run starting.");

            var runStartingArgs = new MigrationRunStartingEventArgs(_context);
            RunStarting(this, runStartingArgs);
            if (runStartingArgs.Cancel) {
                _logger.Info("Migration run cancelled by MigrationRunStarting event handler.");
                return;
            }

            var alreadyMigratedVersions = new HashSet<string>(_context.MigrationRecords.GetAll().Select(x => x.Version));
            var migrations = _migrationResolver.GetAllMigrations();
            migrations.MigrateEach(migration => {
                var migrationLogName = migration.GetType().FullName;
                if (alreadyMigratedVersions.Contains(migration.Version)) {
                    _logger.DebugFormat("Migration '{0}' with version '{1}' was run before, skipping.", migrationLogName, migration.Version);
                    return;
                }

                _logger.InfoFormat("Migration '{0}' starting.", migrationLogName);
                var startingArgs = new MigrationStartingEventArgs(migration, _context);
                MigrationStarting(this, startingArgs);
                if (startingArgs.Cancel) {
                    _logger.InfoFormat("Migration '{0}' cancelled by MigrationStarting event handler.", migrationLogName);
                    return;
                }

                var logWriter = new StringWriter();
                migration.Migrate(_context.WithLogger(new MigrationLogger(logWriter, _logger)));
                _logger.DebugFormat("Migration '{0}' changes applied.", migrationLogName);

                _context.MigrationRecords.SaveNew(new MigrationRecord {
                    Version = migration.Version,
                    DateExecuted = DateTime.UtcNow,
                    Name = migration.Name,
                    Log = logWriter.ToString()
                });
                MigrationCompleted(this, new MigrationEventArgs(migration, _context));
                _logger.InfoFormat("Migration '{0}' completed.", migrationLogName);
            }, migration => migration.GetType().Name);

            _logger.DebugFormat("Refreshing XML cache.");
            _context.RefreshContent();

            RunCompleted(this, new MigrationRunEventArgs(_context));
            _logger.Info("Migration run completed.");
        }
    }
}