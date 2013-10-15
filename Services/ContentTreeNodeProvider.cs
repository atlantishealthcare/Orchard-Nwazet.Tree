using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Common.Models;
using Orchard.Core.Containers.Models;
using Orchard.Core.Contents.Settings;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Html;

namespace Nwazet.Tree.Services {
    [OrchardFeature("Nwazet.Tree.ContentBranches")]
    public class ContentTreeNodeProvider : ITreeNodeProvider {
        private readonly IContentManager _contentManager;
        private readonly UrlHelper _url;

        public ContentTreeNodeProvider(IContentManager contentManager, UrlHelper urlHelper) {
            _contentManager = contentManager;
            _url = urlHelper;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<TreeNode> GetChildren(string nodeType, string nodeId) {
            switch (nodeType) {
                case "root":
                    return new[] {
                        GetContentTypesNode()
                    };
                case "content-types":
                    return _contentManager.GetContentTypeDefinitions()
                        .Where(d => d.Settings.GetModel<ContentTypeSettings>().Creatable)
                        .OrderBy(d => d.DisplayName)
                        .Select(GetContentTypeNode);
                case "content-type":
                    return _contentManager
                        .Query(VersionOptions.Latest, nodeId)
                        .List()
                        .Where(i => !i.Has<CommonPart>() || i.As<CommonPart>().Container == null)
                        .Select(GetContentItemNode)
                        .OrderBy(i => i.Title);
            }
            if (nodeType.StartsWith("content-item-")) {
                var containerId = int.Parse(nodeId);
                return _contentManager
                    .Query<CommonPart, CommonPartRecord>(VersionOptions.Latest)
                    .Where(i => i.Container.Id == containerId)
                    .List()
                    .Select(i => GetContentItemNode(i.ContentItem))
                    .OrderBy(i => i.Title);
            }
            return new TreeNode[0];
        }

        private TreeNode GetContentItemNode(ContentItem item) {
            return new TreeNode {
                Title = _contentManager.GetItemMetadata(item).DisplayText,
                Type = "content-item-" + item.Id,
                Id = item.Id.ToString(CultureInfo.InvariantCulture),
                Url = _url.ItemEditUrl(item),
                IsLeaf = !item.Has<ContainerPart>()
            };
        }

        private TreeNode GetContentTypeNode(ContentTypeDefinition definition) {
            return new TreeNode {
                Title = definition.DisplayName,
                Type = "content-type",
                Id = definition.Name,
                Url = _url.Action(
                    "List", "Admin",
                    new RouteValueDictionary {
                        {"area", "Contents"},
                        {"model.Id", definition.Name}
                    })
            };
        }

        private TreeNode GetContentTypesNode() {
            return new TreeNode {
                Title = T("Content Types").Text,
                Type = "content-types",
                Id = "content-types"
            };
        }

        public TreeNode Get(string nodeType, string nodeId) {
            switch (nodeType)
            {
                case "content-types":
                    return GetContentTypesNode();
                case "content-type":
                    return _contentManager.GetContentTypeDefinitions()
                        .Where(d => d.Name.Equals(nodeId, StringComparison.OrdinalIgnoreCase))
                        .Select(GetContentTypeNode)
                        .FirstOrDefault();
            }
            if (nodeType.StartsWith("content-item-"))
            {
                var nodeIdInt = int.Parse(nodeId);
                var item = _contentManager.Get(nodeIdInt);
                return GetContentItemNode(item);
            }
            return null;
        }
    }
}