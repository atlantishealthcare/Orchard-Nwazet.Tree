using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Contrib.Taxonomies.Models;
using Contrib.Taxonomies.Services;
using Orchard.ContentManagement;
using Orchard.Core.Containers.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.Mvc.Html;

namespace Nwazet.Tree.Services {
    [OrchardFeature("Nwazet.Tree.TaxonomyBranches")]
    public class TaxonomyTreeNodeProvider : ITreeNodeProvider {
        private readonly ITaxonomyService _taxonomyService;
        private readonly IContentManager _contentManager;
        private readonly UrlHelper _url;

        public TaxonomyTreeNodeProvider(
            ITaxonomyService taxonomyService,
            IContentManager contentManager,
            UrlHelper urlHelper) {

            _taxonomyService = taxonomyService;
            _contentManager = contentManager;
            _url = urlHelper;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public IEnumerable<TreeNode> GetChildren(string nodeType, string nodeId) {
            switch (nodeType) {
                case "root":
                    return new[] {
                        GetTaxonomiesNode()
                    };
                case "taxonomies":
                    return _taxonomyService.GetTaxonomies()
                        .Select(GetTaxonomyNode)
                        .OrderBy(n => n.Title)
                        .ToList();
                case "taxonomy":
                    var taxonomyId = int.Parse(nodeId);
                    return _taxonomyService.GetTerms(taxonomyId)
                        .Where(t => t.Path.Trim('/') == "")
                        .Select(
                            t => new TreeNode {
                                Title = t.Name,
                                Type = "taxonomy-term",
                                Id = t.Id.ToString(CultureInfo.InvariantCulture),
                                Url = _url.ItemEditUrl(t)
                            }
                        )
                        .OrderBy(n => n.Title)
                        .ToList();
                case "taxonomy-term":
                    var termId = int.Parse(nodeId);
                    var term = _taxonomyService.GetTerm(termId);
                    var childTermNodes = _taxonomyService
                        .GetChildren(term)
                        .Select(GetTermNode)
                        .OrderBy(n => n.Title);
                    var childItems = _taxonomyService
                        .GetContentItemsQuery(term)
                        .Where(i => i.Terms.Any(tr => tr.TermRecord.Id == term.Id))
                        .List()
                        .Select(GetContentItemNode)
                        .OrderBy(n => n.Title);
                    return childTermNodes
                        .Union(childItems)
                        
                        .ToList();
            }
            return new TreeNode[0];
        }

        private TreeNode GetContentItemNode(IContent item) {
            return new TreeNode {
                Title = _contentManager.GetItemMetadata(item).DisplayText,
                Type = "content-item-" + item.Id,
                Id = item.Id.ToString(CultureInfo.InvariantCulture),
                Url = _url.ItemEditUrl(item),
                IsLeaf = !item.Has<ContainerPart>()
            };
        }

        private TreeNode GetTermNode(TermPart term) {
            return new TreeNode {
                Title = term.Name,
                Type = "taxonomy-term",
                Id = term.Id.ToString(CultureInfo.InvariantCulture),
                Url = _url.ItemEditUrl(term)
            };
        }

        private TreeNode GetTaxonomyNode(TaxonomyPart taxonomy) {
            return new TreeNode {
                Title = taxonomy.Name,
                Type = "taxonomy",
                Id = taxonomy.Id.ToString(CultureInfo.InvariantCulture),
                Url = _url.Action(
                    "Index", "TermAdmin",
                    new RouteValueDictionary {
                        {"area", "Contrib.Taxonomies"},
                        {"taxonomyId", taxonomy.Id}
                    })
            };
        }

        private TreeNode GetTaxonomiesNode() {
            return new TreeNode {
                Title = T("Taxonomies").Text,
                Type = "taxonomies",
                Id = "taxonomies"
            };
        }

        public TreeNode Get(string nodeType, string nodeId) {
            switch (nodeType) {
                case "taxonomies":
                    return GetTaxonomiesNode();
                case "taxonomy":
                    var taxonomyId = int.Parse(nodeId);
                    var taxonomy = _taxonomyService.GetTaxonomy(taxonomyId);
                    return GetTaxonomyNode(taxonomy);
                case "taxonomy-term":
                    var itemId = int.Parse(nodeId);
                    var term = _taxonomyService.GetTerm(itemId);
                    if (term != null) {
                        return GetTermNode(term);
                    }
                    break;
            }
            return null;
        }
    }
}