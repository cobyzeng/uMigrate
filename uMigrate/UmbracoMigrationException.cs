using System;
using System.Runtime.Serialization;

namespace uMigrate {
    [Serializable]
    public class UmbracoMigrationException : Exception {
        public UmbracoMigrationException() {}
        public UmbracoMigrationException(string message) : base(message) {}
        public UmbracoMigrationException(string message, Exception inner) : base(message, inner) {}

        protected UmbracoMigrationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context) {}
    }
}