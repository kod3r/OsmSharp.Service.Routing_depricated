using OsmSharp.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives.GeoJSON
{
    /// <summary>
    /// Represents a GeoJSON feature.
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Type type { get; set; }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        public LineString geometry { get; set; }

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public Properties properties { get; set; }

        /// <summary>
        /// Creates a LineString feature from the given route.
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        public static Feature FromRoute(Route route)
        {
            var feature = new Feature();
            feature.geometry = new LineString();
            feature.properties = new Properties();
            feature.type = Type.Feature;

            feature.properties.TotalDistance = route.TotalDistance;
            feature.properties.TotalTime = route.TotalTime;

            feature.geometry.coordinates = route.GetPoints().Select(x => new double[] { x.Longitude, x.Latitude }).ToArray();
            feature.geometry.type = GeometryType.LineString;

            return feature;
        }
    }
}
