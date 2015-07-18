using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using umbraco.interfaces;

namespace uMigrate.Tests.Integration.Internal {
    [UsedImplicitly]
    public class StubCacheRefresher : ICacheRefresher {
        private static readonly Guid PageRefresherGuid = Guid.ParseExact("27AB3022-3DFA-47b6-9119-5945BC88FD66", "D");

        public string Name {
            get { return "StubCacheRefresher"; }
        }

        public void Refresh(Guid Id) {
        }

        public void Refresh(int Id) {
        }

        public void RefreshAll() {
        }

        public void Remove(int Id) {
        }

        public Guid UniqueIdentifier {
            get { return PageRefresherGuid; }
        }
    }
}
