// OsmSharp - OpenStreetMap (OSM) SDK
// Copyright (C) 2013 Abelshausen Ben
//
// This file is part of OsmSharp.
//
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
//
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.

using System.Linq;
using OsmSharp.Routing;

namespace OsmSharp.Service.Routing.Core.Primitives.GeoJSON
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