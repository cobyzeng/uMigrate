﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace uMigrate.Tests.Integration {
    public class DataTypeTests : IntegrationTestsBase {
        [Test]
        public void ChangeAllPropertyValues_ChangesAllPropertiesOfSpecifiedType() {
            var id = Guid.NewGuid();
            IContent content = null;
            Prepare(m => {
                var dataType = m.DataTypes.Add("Test", Constants.PropertyEditors.NoEditAlias, id);
                var contentType = m.ContentTypes
                    .Add("contentType", "Content Type")
                    .PropertyGroup(null)
                        .AddProperty("property1", "Property 1", dataType.Object)
                        .AddProperty("property2", "Property 2", dataType.Object);

                content = Services.ContentService.CreateContent("Content", -1, contentType.Object.Alias);
                content.SetValue("property1", "A");
                content.SetValue("property2", "B");
                Services.ContentService.Save(content);
            });
            Migrate(m => m.DataType("Test").ChangeAllPropertyValues<string, string>(s => s + s));

            var reloaded = Services.ContentService.GetById(content.Id);
            Assert.AreEqual("AA", reloaded.GetValue("property1"));
            Assert.AreEqual("BB", reloaded.GetValue("property2"));
        }

        [Test]
        public void ChangeAllPropertyValues_DoesNotLogChangeWhenValuesAreSame() {
            var id = Guid.NewGuid();
            IContent content;
            Prepare(m => {
                var dataType = m.DataTypes.Add("Test", Constants.PropertyEditors.NoEditAlias, id);
                var contentType = m.ContentTypes
                    .Add("contentType", "Content Type")
                    .PropertyGroup(null).AddProperty("property", "Property", dataType.Object);

                content = Services.ContentService.CreateContent("Content", -1, contentType.Object.Alias);
                content.SetValue("property", "TestPropertyValue");
                Services.ContentService.Save(content);
            });
            Migrate(m => m.DataType("Test").ChangeAllPropertyValues<string, string>(s => s));

            var record = MigrationRecords.GetAll().Last();
            StringAssert.DoesNotContain("TestPropertyValue", record.Log);
        }

        [Test]
        public void ChangeAllPropertyValues_ChangesInheritedProperties() {
            var id = Guid.NewGuid();
            IContent child = null;
            Prepare(m => {
                var dataType = m.DataTypes.Add("Test", Constants.PropertyEditors.NoEditAlias, id);
                var parentType = m.ContentTypes
                    .Add("parent", "Parent")
                    .PropertyGroup(null).AddProperty("property", "Property", dataType.Object);
                var childType = m.ContentTypes.Add("child", "Child").SetParent(parentType.Object);

                child = Services.ContentService.CreateContent("Content", -1, childType.Object.Alias);
                child.SetValue("property", "old");
                Services.ContentService.Save(child);
            });
            Migrate(m => m.DataType("Test").ChangeAllPropertyValues<string, string>(s => "new"));

            var reloaded = Services.ContentService.GetById(child.Id);
            Assert.AreEqual("new", reloaded.GetValue("property"));
        }

        [Test]
        public void SetEditorAlias_SetsEditorAlias() {
            var id = Guid.NewGuid();
            Prepare(m => m.DataTypes.Add("Test", Constants.PropertyEditors.NoEditAlias, id));
            Migrate(m => m.DataType("Test").SetEditorAlias(Constants.PropertyEditors.TextboxAlias));

            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.AreEqual(Constants.PropertyEditors.TextboxAlias, dataType.PropertyEditorAlias);
        }

        [Test]
        public void SetPreValue_DoesNotLosePreviousPreValues_WhenDoneQuicklyInSequence() {
            var dataTypeId = -1;
            Prepare(m => dataTypeId = m.DataTypes.Add("Test", Constants.PropertyEditors.NoEditAlias, Guid.NewGuid()).Object.Id);
            Migrate(m => m.DataType("Test").SetPreValue("A", "ValueOfA").SetPreValue("B", "ValueOfB"));

            var preValues = Services.DataTypeService.GetPreValuesCollectionByDataTypeId(dataTypeId);

            Assert.AreEqual("ValueOfA", GetValue(preValues, "A"));
            Assert.AreEqual("ValueOfB", GetValue(preValues, "B"));
        }

        [Test]
        public void Change_SavesChanges() {
            var id = Guid.NewGuid();
            Prepare(m => m.DataTypes.Add("OldName", Constants.PropertyEditors.NoEditAlias, id));

            Migrate(m => m.DataType("OldName").Change(d => d.Name = "NewName"));

            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.AreEqual("NewName", dataType.Name);
        }

        [Test]
        public void Delete_RemovesByAlias_WhenCalledOnRootSetWithAlias() {
            var id = Guid.NewGuid();
            Prepare(m => m.DataTypes.Add("ToDelete", Constants.PropertyEditors.TextboxAlias, id));
            Migrate(m => m.DataTypes.Delete("ToDelete"));

            var deleted = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.IsNull(deleted);
        }

        [Test]
        public void Delete_RemovesAllFiltered_WhenCalledOnFilteredSet() {
            var id = Guid.NewGuid();
            Prepare(m => m.DataTypes.Add("ToDelete", Constants.PropertyEditors.TextboxAlias, id));
            Migrate(m => m.DataTypes.Where(t => t.Name == "ToDelete").Delete());

            var deleted = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.IsNull(deleted);
        }

        [CanBeNull]
        private string GetValue(PreValueCollection preValues, string key) {
            PreValue preValue;
            if (!preValues.PreValuesAsDictionary.TryGetValue(key, out preValue))
                return null;

            return preValue.Value;
        }
    }
}
