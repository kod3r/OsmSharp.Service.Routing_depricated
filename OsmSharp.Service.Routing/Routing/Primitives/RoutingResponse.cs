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

using OsmSharp.Routing;
using OsmSharp.Service.Routing.Routing.Primitives.GeoJSON;

namespace OsmSharp.Service.Routing.Routing.Primitives
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