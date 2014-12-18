using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Umbraco.Core.Models;
using uMigrate.Fluent;

namespace uMigrate.Internal.SyntaxImplementations {
    public class ContentSetSyntax : SetSyntaxBase<IContent, IContentSetSyntax, IContentFilteredSetSyntax>, IContentSetSyntax {
        public ContentSetSyntax([NotNull] IMigrationContext context, [CanBeNull] IReadOnlyList<IContent> contents = null)
            : base(context, () => contents ?? context.Services.ContentService.GetDescendants(-1).AsReadOnlyList()) {
        }

        public IContentSetSyntax Add(string name, string contentTypeAlias) {
            Argument.NotNullOrEmpty("name", name);
            Argument.NotNullOrEmpty("contentTypeAlias", contentTypeAlias);

            var contentType = Services.ContentTypeService.GetContentType(contentTypeAlias);
            Ensure.That(contentType != null, "Could not find content type '{0}'.", contentTypeAlias);
            return Add(name, contentType);
        }

        public IContentSetSyntax Add(string name, IContentType contentType) {
            Argument.NotNullOrEmpty("name", name);
            Argument.NotNull("contentType", contentType);

            var content = new Content(name, -1, contentType);
            Services.ContentService.SaveAndPublishWithStatus(content);

            Logger.Log("Content: added '{0}'.", content.Name);
            return NewSet(new[] { content });
        }

        protected override IContentSetSyntax NewSet(IEnumerable<IContent> items) {
            return new ContentSetSyntax(Context, items.AsReadOnlyList());
        }

        protected override string GetName(IContent item) {
            return item.Name;
        }
    }
}
