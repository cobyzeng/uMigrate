using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Formatting;
using System.Web.Http;
using JetBrains.Annotations;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;
using uMigrate.Internal;
using Constants = Umbraco.Core.Constants;

namespace uMigrate.UI {
    [PluginController("uMigrate")]
    [Tree(Constants.Applications.Developer, "umigrate", "Migrations", sortOrder: 8)]
    public class MigrationTreeController : TreeController {
        private readonly IMigrationRecordRepository _repository;

        [UsedImplicitly]
        public MigrationTreeController() {
            // can we use DI here?
            _repository = new DatabaseMigrationRecordRepository(DatabaseContext.Database);
        }

        public MigrationTreeController(IMigrationRecordRepository repository) {
            _repository = repository;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings) {
            return new MenuItemCollection();
        }

        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings) {
            if (id != Constants.System.Root.ToInvariantString())
                throw new NotSupportedException();

            var nodes = new TreeNodeCollection();
            var index = 0;
            foreach (var log in _repository.GetAll().OrderBy(m => m.Version)) {
                var node = CreateTreeNode(
                    log.Version, id, queryStrings,
                    (index + 1) + ". " + log.Name,
                    "icon-settings"
                );
                nodes.Add(node);
                index += 1;
            }

            return nodes;
        }

        public object GetTreeNodeData(string id) {
            var migration = _repository.GetAll().FirstOrDefault(m => m.Version == id);
            if (migration == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            return new {
                name = migration.Name,
                date = migration.DateExecuted,
                log = migration.Log
            };
        }
    }
}
