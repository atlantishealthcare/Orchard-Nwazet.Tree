using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nwazet.Tree.Services;
using Nwazet.Tree.ViewModels;
using Orchard;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using Orchard.UI.Admin;

namespace Nwazet.Tree.Controllers {
    [Admin]
    [OrchardFeature("Nwazet.Tree")]
    public class TreeController : Controller {
        private readonly IEnumerable<ITreeNodeProvider> _treeNodeProviders;

        public IOrchardServices Services { get; set; }

        public TreeController(IEnumerable<ITreeNodeProvider> treeNodeProviders, IOrchardServices services) {
            _treeNodeProviders = treeNodeProviders;
            Services = services;

            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [HttpPost]
        [Themed(false)]
        public ActionResult GetChildren(string parentId, string parentType) {
            var notAuthorized = T("Not authorized to access the admin panel.");
            if (!Services.Authorizer.Authorize(StandardPermissions.AccessAdminPanel, notAuthorized)) {
                return Json(new {Success = false, Message = notAuthorized.Text});
            }

            try {
                var children = _treeNodeProviders
                    .SelectMany(p => p.GetChildren(parentType, parentId));
                return Json(children);
            }
            catch (Exception exception) {
                return
                    Json(new {Success = false, Message = T("Tree expansion failed: {0}", exception.Message).ToString()});
            }
        }

        public ActionResult Index(string id = "/", string type = "root") {
            if (!Services.Authorizer.Authorize(StandardPermissions.AccessAdminPanel,
                                               T("Not allowed to access admin panel")))
                return new HttpUnauthorizedResult();

            var node = GetTreeNode(id, type) ??
                       new TreeNode {Id = "/", Type = "root", Title = T("Root").Text};
            var children = _treeNodeProviders
                .SelectMany(p => p.GetChildren(type, id));
            var model = new TreeExplorerViewModel {
                Node = node,
                Children = children
            };
            if (Request.IsAjaxRequest()) {
                return new JsonResult {Data = model};
            }
            return View(model);
        }

        private TreeNode GetTreeNode(string id, string type) {
            return _treeNodeProviders
                .Select(p => p.Get(type, id))
                .FirstOrDefault(p => p != null);
        }
    }
}