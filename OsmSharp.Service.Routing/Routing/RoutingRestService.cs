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

using System;
using ServiceStack;

namespace OsmSharp.Service.Routing.Routing
{
    /// <summary>
    /// Routing service implementation.
    /// </summary>
    public class RoutingRestService : ServiceStack.Service
    {
        /// <summary>
        /// Makes sure to respond with the proper CORS headers.
        /// </summary>
        /// <param name="request"></param>
        [EnableCors]
        public void Options(RoutingOperation request)
        {
            OsmSharp.Logging.Log.TraceEvent("", OsmSharp.Logging.TraceEventType.Information, "logging");
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EnableCors]
        public object Get(RoutingOperation request)
        {
            return this.ProcessRoutingOperation(request);
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EnableCors]
        public object Post(RoutingOperation request)
        {
            return this.ProcessRoutingOperation(request);
        }

        /// <summary>
        /// Process the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private object ProcessRoutingOperation(RoutingOperation request)
        {
            var beforeTicks = DateTime.Now.Ticks;
            var result = OperationProcessor.GetInstance().ProcessRoutingOperation(request);
            var afterTicks = DateTime.Now.Ticks;

            OsmSharp.Logging.Log.TraceEvent("RoutingRestService", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Routing request finished after {0}ms",
                    new TimeSpan(afterTicks - beforeTicks).TotalMilliseconds));

            return result;
        }
    }
}