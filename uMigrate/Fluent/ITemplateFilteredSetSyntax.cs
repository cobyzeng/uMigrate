using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface ITemplateFilteredSetSyntax : IFilteredSetSyntax<ITemplate, ITemplateFilteredSetSyntax> {
        [PublicAPI, NotNull] ITemplateSetSyntax Change([NotNull] Action<ITemplate> action);
    }
}
