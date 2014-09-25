using System;
using System.Collections.Generic;
using System.Linq;

namespace uMigrate.Fluent {
    public interface ISetSyntax<TItem, TSelf, TFilteredSelf> : IFilteredSetSyntax<TItem, TFilteredSelf>
        where TFilteredSelf : IFilteredSetSyntax<TItem, TFilteredSelf>
        where TSelf: TFilteredSelf
    {
    }
}