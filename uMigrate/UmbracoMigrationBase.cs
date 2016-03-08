using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
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

        // ReSharper disable once NotNullMemberIsNotInitialized
        [PublicAPI, NotNull] protected IMigrationContext Context { get; private set; }

        public virtual string Version => GetVersionFromTypeName();

        public virtual string Name => GetType().Name;

        protected abstract void Run();

        [PublicAPI, NotNull] public IContentSetSyntax Contents => new ContentSetSyntax(Context);

        [PublicAPI, NotNull]
        public IContentTypeFilteredSetSyntax ContentType(string alias) {
            var contentType = Context.Services.ContentTypeService.GetContentType(alias);
            Ensure.That(contentType != null, "Content type '{0}' was not found.", alias);
            return new ContentTypeSetSyntax(Context, new[] { contentType });
        }

        [PublicAPI, NotNull] public IContentTypeSetSyntax ContentTypes => new ContentTypeSetSyntax(Context);

        [PublicAPI, NotNull]
        public IDataTypeFilteredSetSyntax DataType(string name) {
            var dataTypes = DataTypes.Where(d => d.Name == name);
            Ensure.That(dataTypes.Objects.Count > 0, "Data type '{0}' was not found.", name);
            return dataTypes;
        }

        [PublicAPI, NotNull] public IDataTypeSetSyntax DataTypes => new DataTypeSetSyntax(Context);

        [PublicAPI, NotNull]
        public ITemplateFilteredSetSyntax Template(string alias) {
            var template = Templates.Where(t => t.Alias == alias);
            Ensure.That(template.Objects.Count > 0, "Template '{0}' was not found.", alias);
            return template;
        }

        [PublicAPI, NotNull] public IMacroSetSyntax Macros => new MacroSetSyntax(Context);

        [PublicAPI, NotNull]
        public IMacroFilteredSetSyntax Macro(string alias) {
            var macro = Macros.Where(t => t.Alias == alias);
            Ensure.That(macro.Objects.Count > 0, "Macro '{0}' was not found.", alias);
            return macro;
        }

        [PublicAPI, NotNull] public ITemplateSetSyntax Templates => new TemplateSetSyntax(Context);

        [PublicAPI, NotNull] public IPackageSetSyntax Packages => new PackageSetSyntax(Context);

        [PublicAPI, NotNull] public IDatabaseSyntax Database => new DatabaseSyntax(Context.Database, new EmbeddedResourceHelper(this));

        [PublicAPI] public IMigrationLogger Logger => Context.Logger;

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