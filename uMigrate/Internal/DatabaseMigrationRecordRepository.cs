using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;

namespace uMigrate.Internal {
    public class DatabaseMigrationRecordRepository : IMigrationRecordRepository {
        private readonly UmbracoDatabase _database;

        public DatabaseMigrationRecordRepository(UmbracoDatabase database) {
            _database = database;
        }

        public IReadOnlyList<MigrationRecord> GetAll() {
            if (!_database.TableExist(MigrationRecord.DefaultTableName))
                return new MigrationRecord[0];

            return _database.Fetch<MigrationRecord>("where 1=1");
        }

        public void SaveNew(MigrationRecord record) {
            EnsureTable();
            _database.Insert(record);
        }

        public void Save(IReadOnlyList<MigrationRecord> migrations) {
            EnsureTable();
            foreach (var migration in migrations) {
                var exists = _database.Exists<MigrationRecord>(migration.Version);
                if (exists) {
                    _database.Update(migration);
                }
                else {
                    _database.Insert(migration);
                }
            }
        }

        private void EnsureTable() {
            if (_database.TableExist(MigrationRecord.DefaultTableName))
                return;

            _database.CreateTable<MigrationRecord>();
        }
    }
}
