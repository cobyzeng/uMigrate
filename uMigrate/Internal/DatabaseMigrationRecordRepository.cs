using System;
using System.Collections.Generic;
using Umbraco.Core.Persistence;

namespace uMigrate.Internal {
    public class DatabaseMigrationRecordRepository : IMigrationRecordRepository {
        private const string TableName = "migrationRecord";
        private const string TableFullName = "dbo." + TableName;
        private const string PrimaryKeyName = "Version";
        private readonly UmbracoDatabase _database;

        public DatabaseMigrationRecordRepository(UmbracoDatabase database) {
            _database = database;
        }

        public IReadOnlyList<MigrationRecord> GetAll() {
            if (!_database.TableExist(TableName))
                return new MigrationRecord[0];

            return _database.Fetch<MigrationRecord>("SELECT * FROM " + TableFullName);
        }

        public void SaveNew(MigrationRecord record) {
            EnsureTable();
            _database.Insert(TableName, PrimaryKeyName, false, record);
        }

        private void EnsureTable() {
            if (_database.TableExist(TableName))
                return;

            _database.Execute(@"
                CREATE TABLE " + TableFullName + @" (
                    [Version] nvarchar(50) NOT NULL CONSTRAINT PK_" + TableName + @"_Version PRIMARY KEY,
                    [Name] nvarchar(200) NOT NULL,
                    DateExecuted datetime NOT NULL,
                    [Log] nvarchar(max) NULL
                )
            ");
        }
    }
}
