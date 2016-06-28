using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IContentTypePropertyGroupSetSyntax : IContentTypeSetSyntax {
        [PublicAPI, NotNull] IContentTypePropertyGroupSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] string propertyName, [NotNull] IDataTypeDefinition dataType, [CanBeNull] Action<PropertyType> setup = null);
        [PublicAPI, NotNull] IContentTypePropertyGroupSetSyntax AddProperty([NotNull] string propertyAlias, [NotNull] string propertyName, [NotNull] string dataTypeName, [CanBeNull] Action<PropertyType> setup = null);
    }
}
