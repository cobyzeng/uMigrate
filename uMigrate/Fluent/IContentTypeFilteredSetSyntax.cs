using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using uMigrate.Internal;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IContentTypeFilteredSetSyntax : IFilteredSetSyntax<IContentType, IContentTypeFilteredSetSyntax> {
        [Obsolete(ObsoleteMessages.UseOverloadThatTakesName)]
        [PublicAPI, NotNull] IContentTypeSetSyntax AddChild([NotNull] string alias, params Action<IContentType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax AddChild([NotNull] string alias, [NotNull] string name, params Action<IContentType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax AllowUnder([NotNull] IContentTypeFilteredSetSyntax otherContentTypes);
        [PublicAPI, NotNull] IContentTypeSetSyntax AllowUnder([NotNull] string otherContentTypeAlias);

        [PublicAPI, NotNull] IContentTypeSetSyntax SetParent([CanBeNull] string parentContentTypeAlias, bool removeOldParentFromCompositions = false);
        [PublicAPI, NotNull] IContentTypeSetSyntax SetParent([CanBeNull] IContentType parentContentType, bool removeOldParentFromCompositions = false);

        [PublicAPI, NotNull] IContentTypeSetSyntax AddPropertyGroup([NotNull] string name);
        [PublicAPI, NotNull] IContentTypeSetSyntax RenamePropertyGroup([NotNull] string oldName, [NotNull] string newName);
        [PublicAPI, NotNull] IContentTypeSetSyntax SortPropertyGroups([NotNull] Func<PropertyGroup, PropertyGroup, int> compare);
        [PublicAPI, NotNull] IContentTypeSetSyntax SortPropertyGroups([NotNull] params string[] sortedPropertyGroupNames);
        [PublicAPI, NotNull] IContentTypeSetSyntax RemovePropertyGroup([NotNull] string name);

        [Obsolete(ObsoleteMessages.UseOverloadThatTakesName)]
        [PublicAPI, NotNull] IContentTypeSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] string dataTypeName, [CanBeNull] string propertyGroupName = null, [NotNull] params Action<PropertyType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] string propertyName, [NotNull] string dataTypeName, [CanBeNull] string propertyGroupName, [NotNull] params Action<PropertyType>[] setups);
        [Obsolete(ObsoleteMessages.UseOverloadThatTakesName)]
        [PublicAPI, NotNull] IContentTypeSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] IDataTypeDefinition dataType, [CanBeNull] string propertyGroupName = null, [NotNull] params Action<PropertyType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] string propertyName, [NotNull] IDataTypeDefinition dataType, [CanBeNull] string propertyGroupName = null, [NotNull] params Action<PropertyType>[] setups);
        [PublicAPI, NotNull] IContentTypeSetSyntax RemoveProperty([NotNull] string propertyAlias);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperties([NotNull] Func<PropertyType, IContentType, bool> filter, [NotNull] Action<PropertyType, IContentType> change);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperty([NotNull] string propertyAlias, [NotNull] Action<PropertyType> change);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperty([NotNull] string propertyAlias, [NotNull] Action<PropertyType, IContentType> change);
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeProperty<TFrom, TTo>([NotNull] string propertyAlias, [NotNull] Action<PropertyType> change, [NotNull] Func<TFrom, TTo> convert);
        [PublicAPI, NotNull] IContentTypeSetSyntax MoveProperty([NotNull] string propertyAlias, [CanBeNull] string newPropertyGroupName);
        [PublicAPI, NotNull] IContentTypeSetSyntax SortProperties([CanBeNull] string propertyGroupName, [NotNull] Func<PropertyType, PropertyType, int> compare);
        [PublicAPI, NotNull] IContentTypeSetSyntax SortProperties([CanBeNull] string propertyGroupName, [NotNull] params string[] sortedPropertyAliases);

        [PublicAPI, NotNull] IContentTypeSetSyntax Change([NotNull] Action<IContentType> change);

        [PublicAPI, NotNull] IContentTypeSetSyntax AllowTemplate([NotNull] ITemplate template);
        [PublicAPI, NotNull] IContentTypeSetSyntax DefaultTemplate([CanBeNull] ITemplate template);
        
        [PublicAPI, NotNull] IContentTypeFilteredSetSyntax Parent();
        [PublicAPI, NotNull] IContentTypeFilteredSetSyntax DescendantsAndSelf();

        [PublicAPI, NotNull] IEnumerable<IContent> GetAllContents();
        [PublicAPI, NotNull] IContentTypeSetSyntax ChangeContents([NotNull] Func<IContent, bool> filter, [NotNull] Action<IContent> change);

        [PublicAPI] void Delete();
    }
}
