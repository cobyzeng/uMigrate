using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using uMigrate.Internal;

namespace uMigrate {
    public class UmbracoMigrator {
        private readonly IMigrationResolver _migrationResolver;
        private readonly IMigrationContext _context;

        public UmbracoMigrator(IMigrationResolver migrationResolver, IMigrationContext context) {
            _migrationResolver = migrationResolver;
            _context = context;
        }

        public void Run() {
            var alreadyMigratedVersions = _context.MigrationRecords.GetAll().Select(x => x.Version);
            var migrations = _migrationResolver.GetAllMigrations()
                .Where(x => !alreadyMigratedVersions.Contains(x.Version));

            migrations.MigrateEach(migration => {
                var logWriter = new StringWriter();
                migration.Migrate(_context.WithLogger(new MigrationLogger(logWriter)));

                _context.MigrationRecords.SaveNew(new MigrationRecord {
                    Version = migration.Version,
                    DateExecuted = DateTime.UtcNow,
                    Name = migration.Name,
                    Log = logWriter.ToString()
                });
            }, migration => migration.GetType().Name);

            _context.ContentService.RePublishAll();
            _context.RefreshContent();
        }
    }
}