using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface ITemplateSetSyntax : ITemplateFilteredSetSyntax, ISetSyntax<ITemplate, ITemplateSetSyntax, ITemplateFilteredSetSyntax> {
        [PublicAPI, NotNull] ITemplateSetSyntax Add([NotNull] string alias, [NotNull] string name, RenderingEngine engine);
        [PublicAPI, NotNull] ITemplateSetSyntax Delete([NotNull] string alias, bool canDeleteContentVersions = false);
    }
}
