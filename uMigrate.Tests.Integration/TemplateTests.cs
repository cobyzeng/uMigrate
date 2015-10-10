using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using umbraco.IO;
using Umbraco.Core;

namespace uMigrate.Tests.Integration {
    public class TemplateTests : IntegrationTestsBase {
        private static void EnsureTemplateFile(string path) {
            path = IOHelper.MapPath(path);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.Create(path).Close();
        }
    }
}
