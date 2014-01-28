using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives.GeoJSON
{
    /// <summary>
    /// Represents a linestring.
    /// </summary>
    public class LineString
    {
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public GeometryType type { get; set; }

        /// <summary>
        /// Gets or sets the coordinates.
        /// </summary>
        public double[][] coordinates { get; set; }
    }
}
