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
            TruncateLogIfRequired(record);
            _database.Insert(record);
        }

        public void Save(IReadOnlyList<MigrationRecord> records) {
            EnsureTable();
            foreach (var record in records) {
                TruncateLogIfRequired(record);

                var exists = _database.Exists<MigrationRecord>(record.Version);
                if (exists) {
                    _database.Update(record);
                }
                else {
                    _database.Insert(record);
                }
            }
        }

        private static void TruncateLogIfRequired(MigrationRecord record) {
            if (record.Log == null || record.Log.Length <= MigrationRecord.DefaultMaxLogLength)
                return;

            var suffix = Environment.NewLine + "… Log limit reached …";
            record.Log = record.Log.Substring(0, MigrationRecord.DefaultMaxLogLength - suffix.Length) + suffix;
        }

        private void EnsureTable() {
            if (_database.TableExist(MigrationRecord.DefaultTableName))
                return;

            _database.CreateTable<MigrationRecord>();
        }
    }
}
