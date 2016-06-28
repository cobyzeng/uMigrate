using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using uMigrate.Infrastructure.Content;

namespace uMigrate.Infrastructure {
    public class MigrationConfiguration {
        [NotNull] private IContentSavePublish _contentSavePublish;

        public MigrationConfiguration() {
            _contentSavePublish = new SaveLatestAndPublishIfPublished();
        }

        [NotNull]
        public IContentSavePublish ContentSavePublish {
            get { return _contentSavePublish; }
            set { _contentSavePublish = Argument.NotNull(nameof(value), value); }
        }
    }
}
