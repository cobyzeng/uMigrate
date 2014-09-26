using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using JetBrains.Annotations;
using Umbraco.Core;
using Umbraco.Web;
using Umbraco.Web.UI.JavaScript;

namespace uMigrate.UI {
    [UsedImplicitly]
    public class RegisterControllerUrl : ApplicationEventHandler {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext) {
            ServerVariablesParser.Parsing += (_, variables) => {
                var urlDictionary = (IDictionary<string, object>)variables["umbracoUrls"];
                var url = new UrlHelper(new RequestContext(new HttpContextWrapper(HttpContext.Current), new RouteData()));

                urlDictionary.Add(
                    "uMigrate.MigrationTreeController",
                    url.GetUmbracoApiServiceBaseUrl<MigrationTreeController>(c => c.GetTreeNodeData(null))
                );
            };
        }
    }
}