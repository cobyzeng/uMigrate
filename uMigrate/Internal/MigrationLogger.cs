using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace uMigrate.Internal {
    public class MigrationLogger : IMigrationLogger {
        private readonly TextWriter _writer;
        private readonly ILog _logger;

        public MigrationLogger(TextWriter writer, ILog logger) {
            _writer = writer;
            _logger = logger;
        }

        public void Log(string message) {
            _writer.WriteLine(message);
            _logger.Debug(message);
        }

        public void Log(string format, params object[] args) {
            _writer.WriteLine(format, args);
            _logger.DebugFormat(format, args);
        }

        public ILog System {
            get { return _logger; }
        }
    }
}
