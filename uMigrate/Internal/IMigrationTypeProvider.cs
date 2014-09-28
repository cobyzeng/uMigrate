using System;
using System.Collections.Generic;
using System.Linq;

namespace uMigrate.Internal {
    public interface IMigrationTypeProvider {
        IEnumerable<Type> GetAllMigrationTypes();
    }
}
