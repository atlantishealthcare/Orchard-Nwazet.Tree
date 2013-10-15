using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Environment.Extensions;
using Orchard.Mvc.Routes;

namespace Nwazet.Tree.Routes {
    [OrchardFeature("Nwazet.Tree")]
    public class Routes : IRouteProvider {
        public void GetRoutes(ICollection<RouteDescriptor> routes) {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes() {
            return new[] {
                new RouteDescriptor {
                    Route = new Route(
                        "the-tree",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Tree"},
                            {"controller", "Tree"},
                            {"action", "Index"},
                            {"type", null},
                            {"id", null}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Tree"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "the-tree/getchildren/{parentType}/{parentId}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Tree"},
                            {"controller", "Tree"},
                            {"action", "GetChildren"},
                            {"parentType", null},
                            {"parentId", null}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Tree"}
                        },
                        new MvcRouteHandler())
                },
                new RouteDescriptor {
                    Route = new Route(
                        "the-tree/{type}/{id}",
                        new RouteValueDictionary {
                            {"area", "Nwazet.Tree"},
                            {"controller", "Tree"},
                            {"action", "Index"},
                            {"type", null},
                            {"id", null}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary {
                            {"area", "Nwazet.Tree"}
                        },
                        new MvcRouteHandler()),
                }
            };
        }
    }
}