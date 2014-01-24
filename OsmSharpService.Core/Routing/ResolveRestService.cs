

using ServiceStack;
namespace OsmSharpService.Core.Routing
{
    /// <summary>
    /// Routing service implementation.
    /// </summary>
    public class ResolveRestService : Service
    {
        /// <summary>
        /// Executes the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public object Execute(ResolveOperation request)
        {
            return OperationProcessor.GetInstance().ProcessResolveResource(request);
        }
    }
}
