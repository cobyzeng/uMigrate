using System;
using JetBrains.Annotations;

namespace uMigrate.Infrastructure {
    public class MigrationRunEventArgs : EventArgs {
        public MigrationRunEventArgs([NotNull] IMigrationContext context) {
            Context = Argument.NotNull("context", context);
        }

        [NotNull] public IMigrationContext Context { get; private set; }
    }
}