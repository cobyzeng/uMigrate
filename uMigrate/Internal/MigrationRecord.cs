using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace uMigrate.Internal {
    public class MigrationRecord {
        [NotNull] public string Name { get; set; }
        [NotNull] public string Version { get; set; }
        public DateTime DateExecuted { get; set; }
        public string Log { get; set; }
    }
}