using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace uMigrate.Fluent {
    public interface IDataTypeSetSyntax : IDataTypeFilteredSetSyntax {
        [PublicAPI, NotNull] IDataTypeSetSyntax Add([NotNull] string name, [NotNull] string editorAlias, [CanBeNull] Guid? key);
        [PublicAPI, NotNull] IDataTypeSetSyntax Delete();
    }
}
