using System;
using System.Collections.Generic;
using ClientDependency.Core.Logging;
using log4net;
using Moq;
using NUnit.Framework;
using uMigrate.Infrastructure;
using uMigrate.Internal;

namespace uMigrate.Tests.Unit {
    [TestFixture]
    public class UmbracoMigratorTests {
        [Test]
        public void Run_CallsMigrateOnMigration() {
            var context = MockMigrationContext();
            var mockMigrationResolver = new Mock<IMigrationResolver>();

            var mockMigration = new Mock<IUmbracoMigration>();
            mockMigration.Setup(x => x.Version).Returns("TestVersion");
            mockMigration.Setup(x => x.Migrate(It.IsAny<IMigrationContext>()));

            mockMigrationResolver.Setup(x => x.GetAllMigrations())
                .Returns(new[] { mockMigration.Object });

            new UmbracoMigrator(mockMigrationResolver.Object, context, MockLogger()).Run();

            mockMigration.Verify(x => x.Migrate(It.IsAny<IMigrationContext>()), Times.Once());
        }

        private static IMigrationContext MockMigrationContext() {
            var mock = new Mock<IMigrationContext> { DefaultValue = DefaultValue.Mock };
            return mock.Object;
        }

        private static ILog MockLogger() {
            return new Mock<ILog>().Object;
        }
    }
}