using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core;

namespace uMigrate.Tests.Integration {
    public class ContentTypeTests : IntegrationTestsBase {
        [Test]
        public void AddChild_CreatesChildWithInheritedProperty() {
            RunMigration(m => {
                m.ContentTypes
                 .Add("Parent")
                 .AddProperty("ParentProperty", m.DataTypes.Add("Test", Constants.PropertyEditors.TextboxAlias, null).Object)
                 .AddChild("Child");
            });

            var childType = Services.ContentTypeService.GetContentType("Child");
            Assert.IsTrue(childType.PropertyTypeExists("ParentProperty"));
        }

        [Test]
        public void Delete_RemovesContentByAlias_WhenCalledOnRootSetWithAlias() {
            RunMigration(m => m.ContentTypes.Add("ToDelete"));
            RunMigration(m => m.ContentTypes.Delete("ToDelete"));

            var deleted = Services.ContentTypeService.GetContentType("ToDelete");
            Assert.IsNull(deleted);
        }

        [Test]
        public void Delete_RemovesAllMatchingContents_WhenCalledOnFileredSet() {
            RunMigration(m => m.ContentTypes.Add("ToDelete"));
            RunMigration(m => m.ContentTypes.Where(t => t.Alias == "ToDelete").Delete());

            var deleted = Services.ContentTypeService.GetContentType("ToDelete");
            Assert.IsNull(deleted);
        }
    }
}
