using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IContentSetSyntax : IContentFilteredSetSyntax, ISetSyntax<IContent, IContentSetSyntax, IContentFilteredSetSyntax> {
        [PublicAPI, NotNull] IContentSetSyntax Add([NotNull] string name, [NotNull] string contentTypeAlias, [CanBeNull] Action<IContent> setup = null);
        [PublicAPI, NotNull] IContentSetSyntax Add([NotNull] string name, [NotNull] IContentType contentType, [CanBeNull] Action<IContent> setup = null);
    }
}
