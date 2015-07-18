using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using uMigrate.Internal;

namespace uMigrate.Tests.Unit {
    [TestFixture]
    public class MigrationResolverTests {
        [Test]
        public void GetAllMigrations_Throws_IfDuplicateMigrationsAreFound() {
            var typeProviderMock = MockMigrationTypes(typeof(TestMigrationDuplicate1), typeof(TestMigrationDuplicate2));

            Assert.Throws<UmbracoMigrationException>(
                () => new MigrationResolver(typeProviderMock.Object).GetAllMigrations()
            );
        }
        
        private static Mock<IMigrationTypeProvider> MockMigrationTypes(params Type[] args) {
            var mock = new Mock<IMigrationTypeProvider>();
            mock.Setup(x => x.GetAllMigrationTypes()).Returns(args);
            return mock;
        }

        private class TestMigrationDuplicate1 : UmbracoMigrationBase {
            public override string Version { get { return "Duplicate"; } }
            protected override void Run() { }
        }

        private class TestMigrationDuplicate2 : UmbracoMigrationBase {
            public override string Version { get { return "Duplicate"; } }
            protected override void Run() { }
        }
    }
}
