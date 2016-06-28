using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uMigrate.Internal {
    internal static class ObsoleteMessages {
        public const string UseOverloadThatTakesName = "Please use overload that takes name instead. This method will be removed in a future version.";
        public const string UseAddPropertyFromPropertyGroup = "Please use PropertyGroup(\"Group Name\").AddProperty(…) or AddPropertyGroup(\"Group Name\").AddProperty(…) instead. This method will be removed in a future version.";
    }
}
