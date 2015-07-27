using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using uMigrate.Internal;

namespace uMigrate.Tests.Integration {
    public class MigrationRecordTests : IntegrationTestsBase {
        [Test]
        public void Log_DoesNotFail_IfLengthExceedsLimits() {
            RunMigration(m => m.Logger.Log(new string('#', 9000)));

            var log = MigrationRecords.GetAll().Last().Log;
            Assert.AreEqual(log.Length, MigrationRecord.DefaultMaxLogLength);
        }
    }
}
