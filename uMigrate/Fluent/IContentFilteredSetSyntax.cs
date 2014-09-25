using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IContentFilteredSetSyntax : IFilteredSetSyntax<IContent, IContentFilteredSetSyntax> {
    }
}
