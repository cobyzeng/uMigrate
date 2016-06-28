using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using uMigrate.Internal;

namespace uMigrate.Tests.Integration {
    public class MigrationRecordTests : IntegrationTestsBase {
        [Test]
        public void Log_DoesNotFail_IfLengthExceedsLimits() {
            Migrate(m => m.Logger.Log(new string('#', 9000)));

            var log = MigrationRecords.GetAll().Last().Log;
            Assert.AreEqual(log.Length, MigrationRecord.DefaultMaxLogLength);
        }

        [Test]
        public void Save_InsertsNewRecord_IfItDidNotExist() {
            var record = new MigrationRecord {
                Version = Guid.NewGuid().ToString("N"),
                Name = "Test",
                DateExecuted = DateTime.Now
            };
            MigrationRecords.Save(new[] { record });

            var found = MigrationRecords.GetAll().SingleOrDefault(r => r.Version == record.Version);
            Assert.IsNotNull(found);
        }

        [Test]
        public void Save_UpdatesExistingRecord() {
            var record = new MigrationRecord {
                Version = Guid.NewGuid().ToString("N"),
                Name = "Old",
                DateExecuted = DateTime.Now
            };
            MigrationRecords.SaveNew(record);

            record.Name = "New";
            MigrationRecords.Save(new[] { record });

            var reloaded = MigrationRecords.GetAll().Single(r => r.Version == record.Version);
            Assert.AreEqual("New", reloaded.Name);
        }
    }
}
