using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace uMigrate.Fluent {
    [PublicAPI]
    public interface ISetSyntax<TItem, TSelf, TFilteredSelf> : IFilteredSetSyntax<TItem, TFilteredSelf>
        where TFilteredSelf : IFilteredSetSyntax<TItem, TFilteredSelf>
        where TSelf: TFilteredSelf 
    {
        [NotNull, PublicAPI] TFilteredSelf From([NotNull] params TItem[] items);
        [NotNull, PublicAPI] TFilteredSelf From([NotNull] IEnumerable<TItem> items);
    }
}