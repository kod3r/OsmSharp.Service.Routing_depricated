using OsmSharp.Routing;
using ServiceStack;
using OsmSharpService.Core.Routing.Primitives;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Routing resource; tells the services what to route.
    /// </summary>
    [Route("/routing", "GET,POST,PUT,OPTIONS")]
    public class RoutingOperation : IReturn<RoutingResponse>
    {
        /// <summary>
        /// Holds the routing resource type.
        /// </summary>
        public RoutingOperationType Type { get; set; }

        /// <summary>
        /// Holds the routing return type.
        /// </summary>
        public RouteReturnType ReturnType { get; set; }

        /// <summary>
        /// The vehicle type.
        /// </summary>
        public string Vehicle { get; set; }

        /// <summary>
        /// The hooks for the router to route on.
        /// </summary>
        public RoutingHook[] Hooks { get; set; }
    }

    /// <summary>
    /// Represents the type of resource requested.
    /// </summary>
    public enum RoutingOperationType
    {
        /// <summary>
        /// Returns a route along all the points in the given order.
        /// </summary>
        Regular,
        /// <summary>
        /// Returns a shortest route along all the points using the first point as start point and end point.
        /// </summary>
        TSP,
        /// <summary>
        /// Returns a shortest route along all the points using the first point as start point but the end point is flexible.
        /// </summary>
        OpenTSP,
        /// <summary>
        /// Returns all the weights between the given points indexed the same way as the points given.
        /// </summary>
        ManyToMany,
        /// <summary>
        /// Returns a route between the first given point to the closest point in the rest of the points given.
        /// </summary>
        ToClosest
    }

    /// <summary>
    /// Represents the type of return requested.
    /// </summary>
    public enum RouteReturnType
    {
        /// <summary>
        /// Returns an entire serialized route.
        /// </summary>
        Route,
        /// <summary>
        /// Returns an array for coordinates.
        /// </summary>
        Array,
        /// <summary>
        /// Returns a linestring geometry.
        /// </summary>
        LineString
    }
}
