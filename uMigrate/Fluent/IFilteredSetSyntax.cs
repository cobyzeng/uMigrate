using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace uMigrate.Fluent {
    // ReSharper disable TypeParameterCanBeVariant
    public interface IFilteredSetSyntax<TItem, TSelf>
    // ReSharper restore TypeParameterCanBeVariant
        where TSelf : IFilteredSetSyntax<TItem, TSelf>
    {
        [PublicAPI, NotNull] TSelf Where([NotNull] Func<TItem, bool> predicate);

        [PublicAPI, NotNull] IReadOnlyList<TItem> Objects { get; }
        [PublicAPI, NotNull] TItem Object { get; }
    }
}
