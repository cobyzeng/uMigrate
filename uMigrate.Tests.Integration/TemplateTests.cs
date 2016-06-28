using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using File = System.IO.File;

namespace uMigrate.Tests.Integration {
    public class TemplateTests : IntegrationTestsBase {
        [Test]
        public void Add_UpdatesTemplateName_IfTemplateMatchesByAlias() {
            var path = @"Views\Test.cshtml";

            EnsureTemplateFile(path);
            var template = new Template(path, "Old Name", "Test");
            Services.FileService.SaveTemplate(template);

            Migrate(m => m.Templates.Add(template.Alias, "New Name", RenderingEngine.Mvc));

            var reloaded = Services.FileService.GetTemplate(template.Id);
            Assert.AreEqual("New Name", reloaded.Name);
        }

        private static void EnsureTemplateFile(string path) {
            path = IOHelper.MapPath(path);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Create(path).Close();
        }
    }
}
