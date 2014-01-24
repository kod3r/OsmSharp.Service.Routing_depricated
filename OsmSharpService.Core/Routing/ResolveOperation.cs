﻿using OsmSharp.Routing;
using ServiceStack;
using OsmSharpService.Core.Routing.Primitives;

namespace OsmSharpService.Core.Routing
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
