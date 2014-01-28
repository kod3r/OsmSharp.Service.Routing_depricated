using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OsmSharpService.Core.Routing.Primitives.GeoJSON
{
    /// <summary>
    /// Represents GeoJSON default route properties.
    /// </summary>
    public class Properties
    {
        /// <summary>
        /// The distance in meter.
        /// </summary>
        public double TotalDistance { get; set; }

        /// <summary>
        /// The time in seconds.
        /// </summary>
        public double TotalTime { get; set; }
    }
}
