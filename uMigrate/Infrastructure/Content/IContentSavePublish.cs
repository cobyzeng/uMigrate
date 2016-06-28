using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core.Models;

namespace uMigrate.Infrastructure.Content {
    public interface IContentSavePublish {
        IEnumerable<IContent> GetVersionsToChange(IContent content, IMigrationContext context);
        void SaveAndOrPublish(IContent content, IMigrationContext context);
    }
}
