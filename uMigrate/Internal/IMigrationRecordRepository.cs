using System.Collections.Generic;
using JetBrains.Annotations;

namespace uMigrate.Internal {
    public interface IMigrationRecordRepository {
        [NotNull] IReadOnlyList<MigrationRecord> GetAll();
        void SaveNew([NotNull] MigrationRecord record);
        void Save([NotNull] IReadOnlyList<MigrationRecord> records);
    }
}