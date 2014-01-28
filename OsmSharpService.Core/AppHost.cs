using Funq;
using ServiceStack;

namespace OsmSharpService.Core
{
    /// <summary>
    /// Defines how ServiceStack hosts the services.
    /// </summary>
    public class AppHost : AppHostHttpListenerBase 
    {
        /// <summary>
        /// Creates a new app host.
        /// </summary>
        public AppHost()
            : base("OsmSharp REST Service!", new System.Reflection.Assembly[] { typeof(AppHost).Assembly })
        {

        }

        /// <summary>
        /// Allows for custom configuration.
        /// </summary>
        /// <param name="container"></param>
        public override void Configure(Container container)
        {

        }
    }
}