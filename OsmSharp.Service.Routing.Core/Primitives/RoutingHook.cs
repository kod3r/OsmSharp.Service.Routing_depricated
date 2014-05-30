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

namespace OsmSharp.Service.Routing.Core.Primitives
{
    /// <summary>
    /// A routing hook; a point route from/to.
    /// </summary>
    public class RoutingHook
    {
        /// <summary>
        /// The id of the hook.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The latitude.
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// The longitude
        /// </summary>
        public float Longitude { get; set; }

        /// <summary>
        /// The routing hook tags.
        /// </summary>
        public RoutingHookTag[] Tags { get; set; }
    }
}