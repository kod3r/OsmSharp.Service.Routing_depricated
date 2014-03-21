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
using OsmSharpService.Core.Routing.Primitives;
using ServiceStack;

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