﻿// OsmSharp - OpenStreetMap (OSM) SDK
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
using OsmSharp.Service.Routing.Routing.Primitives;
using ServiceStack;

namespace OsmSharp.Service.Routing.Routing
{
    /// <summary>
    /// Resolving service; request to resolve a point.
    /// </summary>
    [Route("/resolving", "GET,POST,PUT,OPTIONS")]
    public class ResolveOperation
    {
        /// <summary>
        /// The vehicle type.
        /// </summary>
        public Vehicle Vehicle { get; set; }

        /// <summary>
        /// The hooks to resolve.
        /// </summary>
        public RoutingHook[] Hooks { get; set; }
    }
}