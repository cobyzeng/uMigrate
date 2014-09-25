using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using Umbraco.Core.Persistence;
using uMigrate.Fluent;

namespace uMigrate.Internal.SyntaxImplementations {
    public class DatabaseSyntax : IDatabaseSyntax {
        private readonly UmbracoDatabase _database;
        private readonly IEmbeddedResourceHelper _resourceHelper;

        public DatabaseSyntax(UmbracoDatabase database, IEmbeddedResourceHelper resourceHelper) {
            _database = database;
            _resourceHelper = resourceHelper;
        }

        public IDatabaseSyntax ExecuteEmbeddedScripts(params string[] resourceNames) {
            foreach (var resourceName in resourceNames) {
                ExecuteEmbeddedScript(resourceName);
            }
            return this;
        }

        public IDatabaseSyntax ExecuteEmbeddedScript(string resourceName) {
            var script = _resourceHelper.GetText(resourceName);
            _database.BeginTransaction();
            try {
                foreach (var part in Regex.Split(script, @"[\r\n]+\s*GO\s*[\r\n]+", RegexOptions.IgnoreCase)) {
                    try {
                        using (var command = _database.CreateCommand(_database.Connection, "")) {
                            command.CommandText = part;
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex) {
                        throw new UmbracoMigrationException("Failed to execute script (" + ex.Message + "):\r\n" + part, ex);
                    }
                }
            }
            catch (Exception) {
                _database.AbortTransaction();
                throw;
            }
            finally {
                _database.CompleteTransaction();
            }

            return this;
        }
    }
}
