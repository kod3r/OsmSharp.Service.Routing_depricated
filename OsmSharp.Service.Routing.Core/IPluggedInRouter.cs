using OsmSharp.Collections.Tags;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmSharp.Service.Routing.Core
{
    /// <summary>
    /// Represents a router created from the plugin.
    /// </summary>
    public interface IPluggedInRouter
    {
        /// <summary>
        /// Resolves a location.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="location"></param>
        /// <param name="matcher"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        RouterPoint Resolve(Vehicle vehicle, GeoCoordinate location, IEdgeMatcher matcher, TagsCollectionBase tags);

        /// <summary>
        /// Checks connectivity.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="next"></param>
        /// <param name="weight"></param>
        /// <returns></returns>
        bool CheckConnectivity(Vehicle vehicle, RouterPoint next, float weight);

        /// <summary>
        /// Calculates a route.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="previous"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        Route Calculate(Vehicle vehicle, RouterPoint previous, RouterPoint next);

        /// <summary>
        /// Calculates many to many weights.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="routerPoint1"></param>
        /// <param name="routerPoint2"></param>
        /// <returns></returns>
        double[][] CalculateManyToManyWeight(Vehicle vehicle, RouterPoint[] routerPoint1, RouterPoint[] routerPoint2);

        /// <summary>
        /// Calculates a point to closest.
        /// </summary>
        /// <param name="vehicle"></param>
        /// <param name="first"></param>
        /// <param name="routerPoint"></param>
        /// <returns></returns>
        Route CalculateToClosest(Vehicle vehicle, RouterPoint first, RouterPoint[] routerPoint);
    }
}