using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IMacroSetSyntax : IMacroFilteredSetSyntax, ISetSyntax<IMacro, IMacroSetSyntax, IMacroFilteredSetSyntax> {
        [PublicAPI, NotNull] IMacroSetSyntax Add([NotNull] string alias, [NotNull] params Action<IMacro>[] setups);
        [PublicAPI, NotNull] IMacroSetSyntax Delete([NotNull] string alias);
    }
}
