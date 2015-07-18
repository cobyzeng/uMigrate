using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Umbraco.Core;

namespace uMigrate.Tests.Integration {
    public class DataTypeTests : IntegrationTestsBase {
        [Test]
        public void SetEditorAlias_SetsEditorAlias() {
            var id = Guid.NewGuid();
            RunMigration(m => m.DataTypes.Add("Test", Constants.PropertyEditors.NoEditAlias, id));
            RunMigration(m => m.DataType("Test").SetEditorAlias(Constants.PropertyEditors.TextboxAlias));

            var dataType = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.AreEqual(Constants.PropertyEditors.TextboxAlias, dataType.PropertyEditorAlias);
        }

        [Test]
        public void Delete_RemovesByAlias_WhenCalledOnRootSetWithAlias() {
            var id = Guid.NewGuid();
            RunMigration(m => m.DataTypes.Add("ToDelete", Constants.PropertyEditors.TextboxAlias, id));
            RunMigration(m => m.DataTypes.Delete("ToDelete"));

            var deleted = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.IsNull(deleted);
        }

        [Test]
        public void Delete_RemovesAllFiltered_WhenCalledOnFilteredSet() {
            var id = Guid.NewGuid();
            RunMigration(m => m.DataTypes.Add("ToDelete", Constants.PropertyEditors.TextboxAlias, id));
            RunMigration(m => m.DataTypes.Where(t => t.Name == "ToDelete").Delete());

            var deleted = Services.DataTypeService.GetDataTypeDefinitionById(id);
            Assert.IsNull(deleted);
        }
    }
}
