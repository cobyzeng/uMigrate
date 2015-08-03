using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace uMigrate.Fluent {
    public interface IPackageSetSyntax {
        void Install([NotNull] string packagePath, [CanBeNull] PackageInstallSettings settings = null);
    }
}
