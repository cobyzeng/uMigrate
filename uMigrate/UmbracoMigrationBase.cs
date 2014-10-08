using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Umbraco.Core.Models;
using uMigrate.Fluent;
using uMigrate.Internal;
using uMigrate.Internal.SyntaxImplementations;

namespace uMigrate {
    [PublicAPI]
    public abstract class UmbracoMigrationBase : IUmbracoMigration {
        public void Migrate(IMigrationContext context) {
            Context = context;
            Run();
        }

        [PublicAPI]
        protected IMigrationContext Context { get; private set; }

        public virtual string Version {
            get { return GetVersionFromTypeName(); }
        }

        public virtual string Name {
            get { return GetType().Name; }
        }

        protected abstract void Run();

        [PublicAPI]
        public IContentSetSyntax Contents {
            get { return new ContentSetSyntax(Context); }
        }

        [PublicAPI]
        public IContentTypeFilteredSetSyntax ContentType(string alias) {
            var contentType = Context.ContentTypeService.GetContentType(alias);
            Ensure.That(contentType != null, "Content type '{0}' was not found.", alias);
            return new ContentTypeSetSyntax(Context, new[] { contentType });
        }

        [PublicAPI]
        public IContentTypeSetSyntax ContentTypes {
            get { return new ContentTypeSetSyntax(Context); }
        }

        [PublicAPI]
        public IDataTypeFilteredSetSyntax DataType(string name) {
            var dataTypes = DataTypes.Where(d => d.Name == name);
            Ensure.That(dataTypes.Objects.Count > 0, "Data type '{0}' was not found.", name);
            return dataTypes;
        }

        [PublicAPI]
        public IDataTypeSetSyntax DataTypes {
            get { return new DataTypeSetSyntax(Context); }
        }

        [PublicAPI]
        public ITemplateSetSyntax Templates {
            get { return new TemplateSetSyntax(Context); }
        }

        [PublicAPI]
        public IDatabaseSyntax Database {
            get { return new DatabaseSyntax(Context.Database, new EmbeddedResourceHelper(this)); }
        }

        [PublicAPI]
        public IMigrationLogger Logger {
            get { return Context.Logger; }
        }

        private string GetVersionFromTypeName() {
            var name = GetType().Name;
            var versionMatch = Regex.Match(name, @"\d+");
            if (!versionMatch.Success) {
                throw new UmbracoMigrationException(
                    "Could not find migration version in type name of " + name + "." +
                    "Please override Version property if you want to use a custom approach."
                );
            }

            return versionMatch.Value;
        }
    }
}