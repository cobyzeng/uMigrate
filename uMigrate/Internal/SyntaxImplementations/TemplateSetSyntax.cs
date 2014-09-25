using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using uMigrate.Fluent;

namespace uMigrate.Internal.SyntaxImplementations {
    public class TemplateSetSyntax : SetSyntaxBase<ITemplate, ITemplateSetSyntax, ITemplateFilteredSetSyntax>, ITemplateSetSyntax {
        public TemplateSetSyntax(IMigrationContext context, [CanBeNull] IReadOnlyList<ITemplate> templates = null)
            : base(context, () => templates ?? context.FileService.GetTemplates().ToArray())
        {
        }

        public ITemplateSetSyntax Add(string alias, string name, RenderingEngine engine) {
            Argument.NotNullOrEmpty("alias", alias);
            Argument.NotNullOrEmpty("name", name);

            var fileSystem = GetFileSystem(engine);
            var fileName = GetFileName(alias, engine);
            var filePath = fileSystem.GetFullPath(fileName);
            if (!fileSystem.FileExists(fileName))
                throw new NotSupportedException("Cannot add a template: file '" + filePath + "' does not exist. Migration can only create templates from existing files.");

            var content = fileSystem.ReadAllText(fileName);

            var template = Context.FileService.GetTemplate(alias);
            if (template == null) {
                template = new Template(filePath, name, alias);
            }
            else if (template.Name != name) {
                // temporary, can be implemented later if needed
                var message = string.Format(
                    "Cannot add a template with alias '{0}' name '{1}'. There is an existing template with that alias, but a different name ('{2}'), which is not currently supported.",
                    alias, name, template.Name
                );
                throw new NotSupportedException(message);
            }

            template.Content = content;
            Context.FileService.SaveTemplate(template);
            Logger.Log("Template: added '{0}' (alias: '{1}', {2}).", name, alias, engine);
            return NewSet(template);
        }

        private IFileSystem GetFileSystem(RenderingEngine engine) {
            return Context.GetFileSystem(engine == RenderingEngine.Mvc ? SystemDirectories.MvcViews : SystemDirectories.Masterpages);
        }

        private string GetFileName(string alias, RenderingEngine engine) {
            return alias + (engine == RenderingEngine.Mvc ? ".cshtml" : ".master");
        }

        public ITemplateSetSyntax Delete(string alias, bool canDeleteContentVersions = false) {
            Argument.NotNullOrEmpty("alias", alias);

            var template = Context.FileService.GetTemplate(alias);
            if (template == null) {
                Logger.Log("Template: '{0}' does not exist, ignored delete request.", alias);
                return this;
            }

            RemoveAllReferencesToTemplate(template, canDeleteContentVersions);
            Context.FileService.DeleteTemplate(alias);
            Logger.Log("Template: deleted '{0}'.", alias);
            return this;
        }

        // probably fixed by http://issues.umbraco.org/issue/U4-2356
        private void RemoveAllReferencesToTemplate(ITemplate template, bool canDeleteContentVersions) {
            var contentTypes = Context.ContentTypeService.GetAllContentTypes()
                .Where(c => c.AllowedTemplates.Any(t => t.Alias == template.Alias))
                .ToArray();

            contentTypes.MigrateEach(contentType => {
                contentType.RemoveTemplate(template);
                Context.ContentTypeService.Save(contentType);
                Logger.Log("ContentType: '{0}', removed template '{1}'.", contentType.Name, template.Alias);

                var contents = Context.ContentService.GetContentOfContentType(contentType.Id).ToArray();
                contents.MigrateEach(c => RemoveAllReferencesToTemplate(c, template, canDeleteContentVersions));
            });
        }

        private void RemoveAllReferencesToTemplate(IContent content, ITemplate template, bool canDeleteContentVersions) {
            if (content.Template != null && content.Template.Alias == template.Alias) {
                content.Template = null;
                Context.ContentService.SaveThenPublishIfPublished(content);
                Logger.Log("Content: '{0}' (id {1}), set default template to none.", content.Name, content.Id);
            }

            var reloaded = Context.ContentService.GetById(content.Id);
            var versions = Context.ContentService.GetVersions(reloaded.Id).ToArray();
            versions.MigrateEach(version => {
                if (version.Version == reloaded.Version)
                    return;

                if (version.Template == null || version.Template.Alias != template.Alias)
                    return;

                if (!canDeleteContentVersions) {
                    var message = string.Format(
                        "Template '{0}' is referenced by '{1}' (id: {2}), version '{3}'. Set canDeleteContentVersions to true to delete it.",
                        template.Alias, content.Name, content.Id, version.Version
                    );
                    throw new UmbracoMigrationException(message);
                }

                Context.ContentService.DeleteVersion(version.Id, version.Version, false);
                Logger.Log("Content: '{0}' (id {1}), deleted version {2}.", content.Name, content.Id, version.Version);
            }, v => string.Format("version '{0}' of content '{1}' (id {2})", v.Version, content.Name, content.Id));
        }
        
        public ITemplateSetSyntax Change(Action<ITemplate> action) {
            Argument.NotNull("action", action);

            ChangeWithManualSave(action);
            Context.FileService.SaveTemplate(Objects);
            return this;
        }

        protected override string GetName(ITemplate item) {
            return item.Name;
        }

        protected override ITemplateSetSyntax NewSet(IEnumerable<ITemplate> items) {
            return new TemplateSetSyntax(Context, items.AsReadOnlyList());
        }
    }
}
