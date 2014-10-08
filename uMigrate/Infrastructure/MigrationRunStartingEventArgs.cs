using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace uMigrate.Infrastructure {
    public class MigrationRunStartingEventArgs : MigrationRunEventArgs {
        public MigrationRunStartingEventArgs([NotNull] IMigrationContext context) : base(context) {
        }

        public bool Cancel { get; set; }
    }
}
