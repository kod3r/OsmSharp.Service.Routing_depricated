using ServiceStack;
using System;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Routing service implementation.
    /// </summary>
    public class RoutingRestService : Service
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
            long beforeTicks = DateTime.Now.Ticks;
            object result = OperationProcessor.GetInstance().ProcessRoutingOperation(request);
            long afterTicks = DateTime.Now.Ticks;

            OsmSharp.Logging.Log.TraceEvent("RoutingRestService", OsmSharp.Logging.TraceEventType.Information,
                string.Format("Routing request finished after {0}ms",
                    new TimeSpan(afterTicks - beforeTicks).TotalMilliseconds));

            return result;
        }
    }
}