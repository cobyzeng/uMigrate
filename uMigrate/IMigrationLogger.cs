using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace uMigrate {
    public interface IMigrationLogger {
        void Log(string message);

        [StringFormatMethod("format")]
        void Log(string format, params object[] args);
    }
}
