using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IDataTypeFilteredSetSyntax : IFilteredSetSyntax<IDataTypeDefinition, IDataTypeFilteredSetSyntax> {
        [PublicAPI, NotNull] IDataTypeFilteredSetSyntax WhereEditorAliasIs([NotNull] string alias);
        [PublicAPI, NotNull] IDataTypeSetSyntax SetEditorAlias([NotNull] string alias);

        [PublicAPI, NotNull] IDataTypeSetSyntax SetPreValue([NotNull] string name, [NotNull] string value);
        [PublicAPI, NotNull] IDataTypeSetSyntax SetPreValues([NotNull] object preValues, bool overwrite = false);

        [PublicAPI, NotNull] IDataTypeSetSyntax ChangePreValues([NotNull] Action<PreValueCollection> change);
        [PublicAPI, NotNull] IDataTypeSetSyntax ChangePreValues([NotNull] Action<PreValueCollection, IDataTypeDefinition> change);
        [PublicAPI, NotNull] IDataTypeSetSyntax ChangePreValue([NotNull] string key, [NotNull] Action<PreValue> change);

        [PublicAPI, NotNull] IDataTypeSetSyntax ChangeAllPropertyValues<TFrom, TTo>([NotNull] Func<TFrom, TTo> change);
        [PublicAPI, NotNull] IDataTypeSetSyntax ChangeAllPropertyValues<TFrom, TTo>([NotNull] Func<TFrom, IContent, TTo> change);
        [PublicAPI, NotNull] IDataTypeSetSyntax ChangeAllPropertyValues<TFrom, TTo>([NotNull] Func<TFrom, IContent, PropertyType, TTo> change);

        [PublicAPI] void Delete();
    }
}
