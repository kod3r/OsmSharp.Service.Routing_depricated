using ServiceStack;

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
            return OperationProcessor.GetInstance().ProcessRoutingOperation(request);
        }

        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [EnableCors]
        public object Post(RoutingOperation request)
        {
            return OperationProcessor.GetInstance().ProcessRoutingOperation(request);
        }
    }
}