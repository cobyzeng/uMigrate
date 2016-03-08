using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Umbraco.Core.Models;

namespace uMigrate.Fluent {
    public interface IMacroFilteredSetSyntax : IFilteredSetSyntax<IMacro, IMacroFilteredSetSyntax> {
        [PublicAPI, NotNull] IMacroFilteredSetSyntax Change([NotNull] Action<IMacro> change);

        [PublicAPI, NotNull] IMacroFilteredSetSyntax Delete();
    }
}
