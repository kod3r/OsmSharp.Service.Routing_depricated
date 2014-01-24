using ServiceStack;

namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Routing service implementation.
    /// </summary>
    public class RoutingRestService : Service
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Any(RoutingOperation request)
        {
            return OperationProcessor.GetInstance().ProcessRoutingOperation(request);
        }
    }
}