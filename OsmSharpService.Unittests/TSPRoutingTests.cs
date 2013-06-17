using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OsmSharp.Routing;
using OsmSharpService.Core.Routing;
using OsmSharpService.Core.Routing.Primitives;
using OsmSharp.Tools.Math.Geo;
using ServiceStack.ServiceClient.Web;

namespace OsmSharpService.Unittests
{
    /// <summary>
    /// Contains some simple TSP routing tests.
    /// </summary>
    [TestFixture]
    public class TSPRoutingTests
    {
        /// <summary>
        /// Holds a simple TSP routing test.
        /// </summary>
        [Test]
        public void SimpleTSPRoutingTest()
        {
            // creates the array of the routing hook.
            var hooks = new RoutingHook[4];

            // create the array of geocoordinates.
            var coordinates = new GeoCoordinate[]
                                  {
                                      new GeoCoordinate(51.09030, 3.44391),
                                      new GeoCoordinate(51.09002, 3.44380),
                                      new GeoCoordinate(51.089900970459, 3.44386267662048),
                                      new GeoCoordinate(51.0862655639648, 3.44465517997742)
                                  };

            // instantiate the routinghooK
            for (int idx = 0; idx < 4; idx++)
            {
                hooks[idx] = new RoutingHook()
                                 {
                                     Id = idx,
                                     Latitude = (float) coordinates[idx].Latitude,
                                     Longitude = (float) coordinates[idx].Longitude,
                                     Tags = new RoutingHookTag[0]
                                 };
            }

            // create Json client.
            var client = new JsonServiceClient("http://localhost:666/");
            client.Timeout = new TimeSpan(0, 5, 0);

            // set the request.
            var routingResponse = client.Send<RoutingResponse>(
                        new RoutingOperation()
                        {
                            Vehicle = VehicleEnum.Car,
                            Hooks = hooks,
                            Type = RoutingOperationType.TSP
                        });

        }
    }
}
