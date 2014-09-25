using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace uMigrate.Tests.Unit {
    public class UmbracoMigrationBaseTests {
        [Test]
        public void Version_ReturnsNumbersFromTypeNameByDefault() {
            var migration = new Migration_1234567_Test();
            Assert.AreEqual("1234567", migration.Version);
        }

        // ReSharper disable once InconsistentNaming
        private class Migration_1234567_Test : UmbracoMigrationBase {
            protected override void Run() { }
        }
    }
}
