using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReflectionMagic;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace uMigrate.Tests.Integration.Internal {
    partial class TestDatabaseHelper {
        private void UmbracoVersionSpecificCreate() {
            new UmbracoDatabase(ConnectionStringName).CreateDatabaseSchema(false);
        }
    }
}
