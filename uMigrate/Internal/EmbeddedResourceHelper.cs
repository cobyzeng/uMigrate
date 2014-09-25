using System;
using System.Collections.Generic;
using System.IO;

namespace uMigrate.Internal {
    public class EmbeddedResourceHelper : IEmbeddedResourceHelper {
        private readonly IUmbracoMigration _migration;

        public EmbeddedResourceHelper(IUmbracoMigration migration) {
            _migration = migration;
        }

        public string GetText(string relativeName) {
            var migrationType = _migration.GetType();
            var assembly = migrationType.Assembly;
            var fullName = migrationType.Namespace + "." + relativeName;

            using (var stream = assembly.GetManifestResourceStream(fullName)) {
                if (stream == null)
                    throw new FileNotFoundException("Resource '" + fullName + "' was not found in " + assembly + ".", fullName);

                using (var reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
