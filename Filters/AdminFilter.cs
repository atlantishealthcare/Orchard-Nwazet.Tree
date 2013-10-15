using System.Web.Mvc;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Filters;
using Orchard.UI.Resources;

namespace Nwazet.Tree.Filters {
    [OrchardFeature("Nwazet.Tree")]
    public class AdminFilter : FilterProvider, IResultFilter {
        private readonly IResourceManager _resourceManager;
        private readonly UrlHelper _url;

        public AdminFilter(IResourceManager resourceManager, UrlHelper urlHelper) {
            _resourceManager = resourceManager;
            _url = urlHelper;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void OnResultExecuting(ResultExecutingContext filterContext) {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult) ||
                !Orchard.UI.Admin.AdminFilter.IsApplied(filterContext.RequestContext))
                return;
            _resourceManager.Require("script", "jQuery");
            _resourceManager.Include("script", "~/Modules/Nwazet.Tree/Scripts/the-tree.js", null);
            _resourceManager.Include("stylesheet", "~/Modules/Nwazet.Tree/Styles/the-tree.css", null);
            _resourceManager.RegisterFootScript(@"
<script type=""text/javascript"">
//<![CDATA[
    var the_nwazet_tree = {
        res: {
            openTagLabel: """ + T("Tree") + @""",
            loadChildError: """ + T("Error loading child nodes") + @"""
        },
        serviceUrl: """ + _url.Action("GetChildren", "Tree", new {parentId = "__id__", parentType = "__type__", area = "Nwazet.Tree"}) + @""",
        explorerUrl: """ + _url.Action("Index", "Tree", new { id = "__id__", type = "__type__", area = "Nwazet.Tree" }) + @"""
    };
//]]>
</script>");
        }

        public void OnResultExecuted(ResultExecutedContext filterContext) {
        }
    }
}