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

namespace OsmSharp.Service.Routing.Routing.Primitives
{
    /// <summary>
    /// A generic response for a resolve request.
    /// </summary>
    public class ResolveResponse
    {
        /// <summary>
        /// Holds the resolve status.
        /// </summary>
        public OsmSharpServiceResponseStatusEnum Status { get; set; }

        /// <summary>
        /// Holds a message with more details about the status.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// The resolved version of the hook(s).
        /// </summary>
        public RoutingHookResolved[] ResolvedHooks { get; set; }

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