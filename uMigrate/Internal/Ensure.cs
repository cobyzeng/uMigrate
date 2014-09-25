using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace uMigrate.Internal {
    public static class Ensure {
        [ContractAnnotation("condition:false => halt")]
        [StringFormatMethod("errorMessage")]
        public static void That(bool condition, string errorMessage, params object[] args) {
            if (condition)
                return;

            throw new UmbracoMigrationException(string.Format(errorMessage, args));
        }
    }
}
