using System;
using System.Collections.Generic;
using System.IO;

namespace uMigrate.Internal {
    public class MigrationLogger : IMigrationLogger {
        private readonly TextWriter _writer;

        public MigrationLogger(TextWriter writer) {
            _writer = writer;
        }

        public void Log(string message) {
            _writer.WriteLine(message);
        }

        public void Log(string format, params object[] args) {
            _writer.WriteLine(format, args);
        }
    }
}
