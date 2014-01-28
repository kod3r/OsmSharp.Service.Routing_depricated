using OsmSharp.Routing;
using OsmSharpService.Core.Routing.Primitives.GeoJSON;

namespace OsmSharpService.Core.Routing.Primitives
{
    /// <summary>
    /// A generic response for a routing request.
    /// </summary>
    public class RoutingResponse
    {
        /// <summary>
        /// Holds the status.
        /// </summary>
        public OsmSharpServiceResponseStatusEnum Status { get; set; }

        /// <summary>
        /// Holds a message with more details about the status.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Holds the calculated weights.
        /// </summary>
        public double[][] Weights { get; set; }

        /// <summary>
        /// The resulting route.
        /// </summary>
        public Route Route { get; set; }

        /// <summary>
        /// The resulting route as an array for coordinates.
        /// </summary>
        public double[][] RouteArray { get; set; }

        /// <summary>
        /// The resulting route as a GeoJSON line string.
        /// </summary>
        public Feature RouteLineString { get; set; }

        /// <summary>
        /// Returns all hooks that have not been routed.
        /// </summary>
        public RoutingHook[] UnroutableHooks { get; set; }

        /// <summary>
        /// Returns a description for this response.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Response: {0}:{1}", this.Status.ToString(), this.StatusMessage);
        }
    }
}