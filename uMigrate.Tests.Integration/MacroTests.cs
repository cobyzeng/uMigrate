using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using umbraco.cms.businesslogic.macro;
using Umbraco.Core;

namespace uMigrate.Tests.Integration {
    public class MacroTests : IntegrationTestsBase {
        [Test]
        public void Add_CreatesNewMacro() {
            RunMigration(m => m.Macros.Add("TestAlias", x => x.Name = "TestName"));

            var macro = Services.MacroService.GetByAlias("TestAlias");
            Assert.IsNotNull(macro);
            Assert.AreEqual("TestName", macro.Name);
        }

        [Test]
        public void Add_UpdatesExistingMacro_IfMacroWithSpecifiedAliasAlreadyExists() {
            RunMigration(m => m.Macros.Add("TestAlias", x => x.Name = "TestName1"));

            RunMigration(m => m.Macros.Add("TestAlias", x => x.Name = "TestName2"));

            var macro = Services.MacroService.GetByAlias("TestAlias");
            Assert.IsNotNull(macro);
            Assert.AreEqual("TestName2", macro.Name);
        }

        [Test]
        public void Change_UpdatesMacro() {
            RunMigration(m => m.Macros.Add("TestAlias", x => x.Name = "TestName1"));

            RunMigration(m => m.Macro("TestAlias").Change(x => x.Name = "TestName2"));

            var macro = Services.MacroService.GetByAlias("TestAlias");
            Assert.IsNotNull(macro);
            Assert.AreEqual("TestName2", macro.Name);
        }

        [Test]
        public void Delete_DeletesMacro() {
            Macro.MakeNew("TestAlias");

            RunMigration(m => m.Macro("TestAlias").Delete());

            var macro = Services.MacroService.GetByAlias("TestAlias");
            Assert.IsNull(macro);
        }

        [Test]
        public void Delete_DeletesMacro_DirectlyByAlias() {
            Macro.MakeNew("TestAlias");

            RunMigration(m => m.Macros.Delete("TestAlias"));

            var macro = Services.MacroService.GetByAlias("TestAlias");
            Assert.IsNull(macro);
        }
    }
}
