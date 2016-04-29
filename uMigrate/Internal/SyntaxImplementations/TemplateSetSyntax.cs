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
            : base(context, () => templates ?? context.Services.FileService.GetTemplates().ToArray())
        {
        }

        public ITemplateSetSyntax Add(string alias, string name, RenderingEngine engine) {
            Argument.NotNullOrEmpty("alias", alias);
            Argument.NotNullOrEmpty("name", name);

            var fileSystem = GetFileSystem(engine);
            var fileName = GetFileName(alias, engine);
            var filePath = fileSystem.GetFullPath(fileName);
            if (!fileSystem.FileExists(fileName))
                throw new NotSupportedException($"Cannot add a template: file '{filePath}' does not exist. Migration can only create templates from existing files.");

            var content = fileSystem.ReadAllText(fileName);

            var template = Services.FileService.GetTemplate(alias);
            if (template == null) {
                template = new Template(filePath, name, alias);
            }
            else if (template.Name != name) {
                template = new Template(filePath, name, alias) {
                    Id = template.Id,
                    CreateDate = template.CreateDate
                };
            }

            template.Content = content;
            Services.FileService.SaveTemplate(template);
            Logger.Log($"Template: added '{name}' (alias: '{alias}', {engine}).");
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

            var template = Services.FileService.GetTemplate(alias);
            if (template == null) {
                Logger.Log($"Template: '{alias}' does not exist, ignored delete request.");
                return this;
            }

            RemoveAllReferencesToTemplate(template, canDeleteContentVersions);
            Services.FileService.DeleteTemplate(alias);
            Logger.Log($"Template: deleted '{alias}'.");
            return this;
        }

        // probably fixed by http://issues.umbraco.org/issue/U4-2356
        private void RemoveAllReferencesToTemplate(ITemplate template, bool canDeleteContentVersions) {
            var contentTypes = Services.ContentTypeService.GetAllContentTypes()
                .Where(c => c.AllowedTemplates.Any(t => t.Alias == template.Alias))
                .ToArray();

            contentTypes.MigrateEach(contentType => {
                contentType.RemoveTemplate(template);
                Services.ContentTypeService.Save(contentType);
                Logger.Log($"ContentType: '{contentType.Name}', removed template '{template.Alias}'.");

                var contents = Services.ContentService.GetContentOfContentType(contentType.Id).ToArray();
                contents.MigrateEach(c => RemoveAllReferencesToTemplate(c, template, canDeleteContentVersions));
            });
        }

        private void RemoveAllReferencesToTemplate(IContent content, ITemplate template, bool canDeleteContentVersions) {
            if (content.Template != null && content.Template.Alias == template.Alias) {
                content.Template = null;
                Services.ContentService.SaveThenPublishIfPublished(content);
                Logger.Log($"Content: '{content.Name}' (id {content.Id}), set default template to none.");
            }

            var reloaded = Services.ContentService.GetById(content.Id);
            var versions = Services.ContentService.GetVersions(reloaded.Id).ToArray();
            versions.MigrateEach(version => {
                if (version.Version == reloaded.Version)
                    return;

                if (version.Template == null || version.Template.Alias != template.Alias)
                    return;

                if (!canDeleteContentVersions) {
                    throw new UmbracoMigrationException(
                        $"Template '{template.Alias}' is referenced by '{content.Name}' (id: {content.Id}), version '{version.Version}'. Set canDeleteContentVersions to true to delete it."
                    );
                }

                Services.ContentService.DeleteVersion(version.Id, version.Version, false);
                Logger.Log($"Content: '{content.Name}' (id {content.Id}), deleted version {version.Version}.");
            }, v => $"version '{v.Version}' of content '{content.Name}' (id {content.Id})");
        }
        
        public ITemplateSetSyntax Change(Action<ITemplate> action) {
            Argument.NotNull("action", action);

            ChangeWithManualSave(action);
            Services.FileService.SaveTemplate(Objects);
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
