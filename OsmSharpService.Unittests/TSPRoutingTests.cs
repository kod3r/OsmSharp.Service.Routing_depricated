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
using NUnit.Framework;
using OsmSharp.Math.Geo;
using OsmSharp.Routing;
using OsmSharpService.Core.Routing;
using OsmSharpService.Core.Routing.Primitives;
using ServiceStack;

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

            // instantiate the routinghook.
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
            var client = new JsonServiceClient("http://127.0.0.1:666/");
            client.Timeout = new TimeSpan(0, 5, 0);

            // set the request.
            var routingResponse = client.Get(
                        new RoutingOperation()
                        {
                            Vehicle = Vehicle.Car.UniqueName,
                            Hooks = hooks,
                            Type = RoutingOperationType.TSP
                        });
        }
    }
}