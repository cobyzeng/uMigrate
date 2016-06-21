using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models;

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
        public void SetParent_ChangesParentToNewValue() {
            RunMigration(m => {
                m.ContentTypes.Add("Parent");
                m.ContentTypes.Add("Child");
            });
            RunMigration(m => m.ContentType("Child").SetParent("Parent"));

            var childType = Services.ContentTypeService.GetContentType("Child");
            var parentType = Services.ContentTypeService.GetContentType("Parent");
            Assert.AreEqual(parentType.Id, childType.ParentId);
        }

        [Test]
        public void SetParent_RemovesOldParentFromCompositions_IfRemoveOldParentIsSet() {
            RunMigration(m => m.ContentTypes.Add("OldParent").AddChild("Child"));
            RunMigration(m => m.ContentType("Child").SetParent((string)null, removeOldParentFromCompositions: true));

            var childType = Services.ContentTypeService.GetContentType("Child");
            Assert.IsFalse(childType.ContentTypeCompositionExists("OldParent"));
        }

        [Test]
        public void SetParent_ChangesParentCorrectlySoThatPropertyTypesAreInherited() {
            RunMigration(m => {
                var stubDataType = m.DataTypes.Add("Stub", Constants.PropertyEditors.IntegerAlias, null);
                m.ContentTypes.Add("Parent").AddProperty("parentProperty", stubDataType.Object);
                m.ContentTypes.Add("Child").AddProperty("childProperty", stubDataType.Object);
            });
            RunMigration(m => m.ContentType("Child").SetParent("Parent"));

            var childType = Services.ContentTypeService.GetContentType("Child");
            Assert.IsTrue(childType.PropertyTypeExists("parentProperty"));
        }

        [Test]
        public void SetParent_RemovesParent_WhenSetToNull() {
            RunMigration(m => m.ContentTypes.Add("Parent").AddChild("Child"));
            RunMigration(m => m.ContentType("Child").SetParent((string)null));

            var childType = Services.ContentTypeService.GetContentType("Child");
            Assert.AreEqual(Constants.System.Root, childType.ParentId);
        }

        [Test]
        public void SortPropertyGroups_ReordersPropertyGroups_WhenCurrentSortOrderDoesNotMatchComparison() {
            RunMigration(m => {
                m.ContentTypes
                    .Add("Test")
                    .AddPropertyGroup("PropertyGroup2")
                    .AddPropertyGroup("PropertyGroup3")
                    .AddPropertyGroup("PropertyGroup1");
            });

            // ReSharper disable once StringCompareIsCultureSpecific.1
            RunMigration(m => m.ContentType("Test").SortPropertyGroups((a, b) => string.Compare(a.Name, b.Name)));

            var contentType = Services.ContentTypeService.GetContentType("Test");
            CollectionAssert.AreEqual(
                new[] { "PropertyGroup1", "PropertyGroup2", "PropertyGroup3" },
                contentType.PropertyGroups.OrderBy(p => p.SortOrder).Select(p => p.Name).ToArray()
            );
        }

        [Test]
        public void SortPropertyGroups_ReordersPropertyGroups_WhenCurrentSortOrderDoesNotMatchProvided() {
            RunMigration(m => {
                m.ContentTypes
                    .Add("Test")
                    .AddPropertyGroup("PropertyGroup2")
                    .AddPropertyGroup("PropertyGroup3")
                    .AddPropertyGroup("PropertyGroup1");
            });

            // ReSharper disable once StringCompareIsCultureSpecific.1
            RunMigration(m => m.ContentType("Test").SortPropertyGroups("PropertyGroup1", "PropertyGroup2", "PropertyGroup3"));

            var contentType = Services.ContentTypeService.GetContentType("Test");
            CollectionAssert.AreEqual(
                new[] { "PropertyGroup1", "PropertyGroup2", "PropertyGroup3" },
                contentType.PropertyGroups.OrderBy(p => p.SortOrder).Select(p => p.Name).ToArray()
            );
        }

        [Test]
        public void MoveProperty_MovesPropertyToDefaultGroup_IfGroupNameIsNull() {
            RunMigration(m => m.ContentTypes.Add("Test").AddProperty("Property", StubDataType(m), propertyGroupName: "OldGroup"));

            RunMigration(m => m.ContentType("Test").MoveProperty("Property", null));

            var contentType = Services.ContentTypeService.GetContentType("Test");
            CollectionAssert.DoesNotContain(contentType.PropertyGroups["OldGroup"].PropertyTypes.Select(p => p.Alias), "Property");
            CollectionAssert.Contains(contentType.PropertyTypes.Select(p => p.Alias), "Property");
        }

        [Test]
        public void MoveProperty_MovesPropertyToSpecifiedGroup() {
            RunMigration(m => m.ContentTypes.Add("Test").AddProperty("Property", StubDataType(m)));

            RunMigration(m => m.ContentType("Test").MoveProperty("Property", "NewGroup"));

            var contentType = Services.ContentTypeService.GetContentType("Test");
            CollectionAssert.Contains(contentType.PropertyGroups["NewGroup"].PropertyTypes.Select(p => p.Alias), "Property");
        }

        [Test]
        public void SortProperties_ReordersProperties_WhenCurrentSortOrderDoesNotMatchComparison() {
            RunMigration(m => {
                var dataType = StubDataType(m);
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
                var dataType = StubDataType(m);
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
        public void Parent_ReturnsAllParents() {
            RunMigration(m => {
                m.ContentTypes
                    .Add("Parent1").AddChild("Child1")
                    .Add("Parent2").AddChild("Child2");
            });

            var parents = GetFluent(
                m => m.ContentTypes
                      .Where(t => t.Name.StartsWith("Child"))
                      .Parent().Objects
            );

            CollectionAssert.AreEquivalent(
                new[] { "Parent1", "Parent2" },
                parents.Select(p => p.Name)
            );
        }

        [Test]
        public void Parent_ReturnsEmpty_IfContentTypeIsOnTopLevel() {
            RunMigration(m => m.ContentTypes.Add("Top"));
            var parents = GetFluent(m => m.ContentType("Top").Parent().Objects);

            CollectionAssert.IsEmpty(parents.Select(p => p.Name));
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

        private static IDataTypeDefinition StubDataType(UmbracoMigrationBase m) {
            return m.DataTypes.Add("Test", Constants.PropertyEditors.TextboxAlias, null).Object;
        }
    }
}
