using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IContentTypeFilteredSetSyntax : IFilteredSetSyntax<IContentType, IContentTypeFilteredSetSyntax> {
        [PublicAPI, NotNull] IContentTypeSetSyntax AddChild([NotNull] string alias, params Action<IContentType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax AllowUnder([NotNull] IContentTypeFilteredSetSyntax otherContentTypes);
        [PublicAPI, NotNull] IContentTypeSetSyntax AllowUnder([NotNull] string otherContentTypeAlias);

        [PublicAPI, NotNull] IContentTypeSetSyntax AddPropertyGroup([NotNull] string name);
        [PublicAPI, NotNull] IContentTypeSetSyntax RenamePropertyGroup([NotNull] string oldName, [NotNull] string newName);
        [PublicAPI, NotNull] IContentTypeSetSyntax SortPropertyGroups([NotNull] Func<PropertyGroup, PropertyGroup, int> compare);
        [PublicAPI, NotNull] IContentTypeSetSyntax RemovePropertyGroup([NotNull] string name);

        [PublicAPI, NotNull] IContentTypeSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] string dataTypeName, [CanBeNull] string propertyGroupName = null, [NotNull] params Action<PropertyType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] IDataTypeDefinition dataType, [CanBeNull] string propertyGroupName = null, [NotNull] params Action<PropertyType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax RemoveProperty([NotNull] string propertyAlias);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperties([NotNull] Func<PropertyType, IContentType, bool> filter, [NotNull] Action<PropertyType, IContentType> change);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperty([NotNull] string propertyAlias, [NotNull] Action<PropertyType> change);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperty([NotNull] string propertyAlias, [NotNull] Action<PropertyType, IContentType> change);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperty<TFrom, TTo>([NotNull] string propertyAlias, [NotNull] Action<PropertyType> change, [NotNull] Func<TFrom, TTo> convert);
        [PublicAPI, NotNull] IContentTypeSetSyntax MoveProperty([NotNull] string propertyAlias, [NotNull] string newPropertyGroupName);
        [PublicAPI, NotNull] IContentTypeSetSyntax SortProperties([NotNull] string propertyGroupName, [NotNull] Func<PropertyType, PropertyType, int> compare);

        [PublicAPI, NotNull] IContentTypeSetSyntax Change([NotNull] Action<IContentType> change);

        [PublicAPI, NotNull] IContentTypeSetSyntax AllowTemplate([NotNull] ITemplate template);
        [PublicAPI, NotNull] IContentTypeSetSyntax DefaultTemplate([CanBeNull] ITemplate template);

        [PublicAPI, NotNull] IContentTypeFilteredSetSyntax DescendantsAndSelf();

        [PublicAPI, NotNull] IEnumerable<IContent> GetAllContents();
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeContents([NotNull] Func<IContent, bool> filter, [NotNull] Action<IContent> change);
    }
}
