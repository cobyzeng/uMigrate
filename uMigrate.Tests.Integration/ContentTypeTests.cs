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
        public void SortProperties_ReordersProperties_WhenCurrentSortOrderDoesNotMatchComparison() {
            RunMigration(m => {
                var dataType = m.DataTypes.Add("Test", Constants.PropertyEditors.TextboxAlias, null).Object;
                m.ContentTypes
                    .Add("Test")
                    .AddProperty("Property2", dataType)
                    .AddProperty("Property3", dataType)
                    .AddProperty("Property1", dataType);
            });

            // ReSharper disable once StringCompareIsCultureSpecific.1
            RunMigration(m => m.ContentType("Test").SortProperties(null, (a, b) => string.Compare(a.Name, b.Name)));

            var contentType = Services.ContentTypeService.GetContentType("Test");
            CollectionAssert.AreEqual(
                new[] { "Property1", "Property2", "Property3" },
                contentType.PropertyTypes.OrderBy(p => p.SortOrder).Select(p => p.Alias).ToArray()
            );
        }

        [Test]
        public void SortProperties_ReordersProperties_WhenCurrentSortOrderDoesNotMatchProvided() {
            RunMigration(m => {
                var dataType = m.DataTypes.Add("Test", Constants.PropertyEditors.TextboxAlias, null).Object;
                m.ContentTypes
                    .Add("Test")
                    .AddProperty("Property2", dataType)
                    .AddProperty("Property3", dataType)
                    .AddProperty("Property1", dataType);
            });

            // ReSharper disable once StringCompareIsCultureSpecific.1
            RunMigration(m => m.ContentType("Test").SortProperties(null, "Property1", "Property2", "Property3"));

            var contentType = Services.ContentTypeService.GetContentType("Test");
            CollectionAssert.AreEqual(
                new[] { "Property1", "Property2", "Property3" },
                contentType.PropertyTypes.OrderBy(p => p.SortOrder).Select(p => p.Alias).ToArray()
            );
        }

        [Test]
        public void Delete_RemovesByAlias_WhenCalledOnRootSetWithAlias() {
            RunMigration(m => m.ContentTypes.Add("ToDelete"));
            RunMigration(m => m.ContentTypes.Delete("ToDelete"));

            var deleted = Services.ContentTypeService.GetContentType("ToDelete");
            Assert.IsNull(deleted);
        }

        [Test]
        public void Delete_RemovesAllFiltered_WhenCalledOnFilteredSet() {
            RunMigration(m => m.ContentTypes.Add("ToDelete"));
            RunMigration(m => m.ContentTypes.Where(t => t.Alias == "ToDelete").Delete());

            var deleted = Services.ContentTypeService.GetContentType("ToDelete");
            Assert.IsNull(deleted);
        }
    }
}
