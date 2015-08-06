using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using NUnit.Framework;
using ReflectionMagic;
using umbraco.cms.businesslogic.macro;
using uMigrate.Fluent;
using Umbraco.Core.IO;

namespace uMigrate.Tests.Integration {
    public class PackageTests : IntegrationTestsBase {
        private static class TestPackageNames {
            public const string Chosen = "Chosen_Multi-Select_Menu_0.1.zip";
        }

        [TestCase(TestPackageNames.Chosen, "~/App_Plugins/ChosenSelectMenu/chosen/chosen-sprite.png")]
        [TestCase(TestPackageNames.Chosen, "~/Views/MacroPartials/ChosenSelectMenuView.cshtml")]
        public void Install_CreatesExpectedFiles(string packageFileName, string deployedFileName) {           
            InstallPackage(packageFileName);

            deployedFileName = IOHelper.MapPath(deployedFileName);
            Assert.IsTrue(File.Exists(deployedFileName), "File '{0}' was not found.", deployedFileName);
        }

        [TestCase(TestPackageNames.Chosen, "Chosen Multi-Select Menu")]
        public void Install_CreatesExpectedDataTypes(string packageFileName, string expectedDataTypeName) {
            InstallPackage(packageFileName);
            var dataType = Services.DataTypeService.GetAllDataTypeDefinitions().FirstOrDefault(d => d.Name == expectedDataTypeName);

            Assert.IsNotNull(dataType, "Data Type '{0}' was not found.", expectedDataTypeName);
        }

        [TestCase(TestPackageNames.Chosen, "ChosenSelectMenuOutput")]
        public void Install_CreatesExpectedMacros(string packageFileName, string expectedMacroAlias) {
            InstallPackage(packageFileName);
            var macro = Services.MacroService.GetByAlias(expectedMacroAlias);

            Assert.IsNotNull(macro, "Macro '{0}' was not found.", expectedMacroAlias);
        }

        [Test]
        public void Install_OverwritesExistingMacros_IfOverwriteMacrosIsSetAndMacroWithTheSameAliasAlreadyExists() {
            var macroAlias = "ChosenSelectMenuOutput";

            var existing = Macro.MakeNew(macroAlias);
            existing.Alias = macroAlias;
            existing.ScriptingFile = "NotOverwritten";
            existing.Save();

            InstallPackage(TestPackageNames.Chosen, new PackageInstallSettings {
                OverwriteMacros = true
            });

            var macro = Services.MacroService.GetByAlias(macroAlias);
            Assert.AreEqual("~/Views/MacroPartials/ChosenSelectMenuView.cshtml", macro.ScriptPath);
        }

        private void InstallPackage(string packageFileName, PackageInstallSettings settings = null) {
            RunMigration(m => m.Packages.Install("~/TestPackages/" + packageFileName, settings));
        }

        protected override void BeforeEachTest() {
            base.BeforeEachTest();
            SetupMockWebEnvironment();
        }

        protected override void AfterEachTest() {
            ResetMockWebEnvironment();
            base.AfterEachTest();
        }

        private void SetupMockWebEnvironment() {
            // Used by Package Installer
            var rootPath = IOHelper.MapPath("~");
            var virtualPathString = "/";
            var virtualPath = (object)typeof(HttpRuntime).Assembly
                .GetType("System.Web.VirtualPath", true)
                .AsDynamicType()
                .CreateNonRelativeTrailingSlash(virtualPathString);

            AppDomain.CurrentDomain.SetData(".appPath", rootPath);
            AppDomain.CurrentDomain.SetData(".appVPath", virtualPathString);

            var httpRuntime = GetHttpRuntime().AsDynamic();
            httpRuntime._appDomainAppPath = rootPath;
            httpRuntime._appDomainAppVPath = virtualPath;

            var hostingEnvironment = (new HostingEnvironment()).AsDynamic();
            var configMapPathFactory = (IConfigMapPathFactory)Activator.CreateInstance(
                typeof(IConfigMapPathFactory).Assembly.GetType("System.Web.Hosting.SimpleConfigMapPathFactory", true)
            );
            hostingEnvironment._appPhysicalPath = rootPath;
            hostingEnvironment._appVirtualPath = virtualPath;
            hostingEnvironment._configMapPath = configMapPathFactory.Create(virtualPathString, rootPath);

            HttpContext.Current = new HttpContext(new SimpleWorkerRequest("Stub", "", new StringWriter()));
        }

        private void ResetMockWebEnvironment() {
            var httpRuntime = GetHttpRuntime().AsDynamic();
            httpRuntime._appDomainAppPath = null;
            httpRuntime._appDomainAppVPath = null;

            typeof(HostingEnvironment)
                .AsDynamicType()
                ._theHostingEnvironment = null;

            HttpContext.Current = null;
        }

        private static HttpRuntime GetHttpRuntime() {
            return typeof(HttpRuntime).AsDynamicType()._theRuntime;
        }
    }
}
