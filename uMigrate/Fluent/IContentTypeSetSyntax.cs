using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using uMigrate.Internal;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IContentTypeSetSyntax : IContentTypeFilteredSetSyntax, ISetSyntax<IContentType, IContentTypeSetSyntax, IContentTypeFilteredSetSyntax> {
        [Obsolete(ObsoleteMessages.UseOverloadThatTakesName)]
        [PublicAPI, NotNull] IContentTypeSetSyntax Add([NotNull] string alias, params Action<IContentType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax Add([NotNull] string alias, [NotNull] string name, params Action<IContentType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax Delete([NotNull] string alias);
    }
}
