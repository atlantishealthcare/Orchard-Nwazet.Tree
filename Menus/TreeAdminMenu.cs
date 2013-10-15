using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Localization;
using Orchard.UI.Navigation;

namespace Nwazet.Tree.Menus {
    [OrchardFeature("Nwazet.Tree")]
    public class ContentAdminMenu : INavigationProvider {

        public ContentAdminMenu() {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public string MenuName {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder) {
            builder
                .AddImageSet("nwazet-tree")
                .Add(T("The Tree"), "1.4.1",
                     item => item.Action("Index", "Tree",
                                         new RouteValueDictionary {
                                             {"area", "Nwazet.Tree"}
                                         }));
        }
    }
}