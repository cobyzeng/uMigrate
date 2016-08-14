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

        public IContentSetSyntax Add(string name, string contentTypeAlias, Action<IContent> setup = null) {
            Argument.NotNullOrEmpty(nameof(name), name);
            Argument.NotNullOrEmpty(nameof(contentTypeAlias), contentTypeAlias);

            var contentType = Services.ContentTypeService.GetContentType(contentTypeAlias);
            Ensure.That(contentType != null, "Could not find content type '{0}'.", contentTypeAlias);
            return Add(name, contentType, setup);
        }

        public IContentSetSyntax Add(string name, IContentType contentType, Action<IContent> setup = null) {
            Argument.NotNullOrEmpty(nameof(name), name);
            Argument.NotNull(nameof(contentType), contentType);

            var content = new Content(name, -1, contentType);
            setup?.Invoke(content);
            Services.ContentService.SaveAndPublishWithStatus(content);

            Logger.Log("Content: added '{0}'.", content.Name);
            return NewSet(content);
        }

        protected override IContentSetSyntax NewSet(IEnumerable<IContent> items) {
            return new ContentSetSyntax(Context, items.AsReadOnlyList());
        }

        protected override string GetName(IContent item) => item.Name;
    }
}
