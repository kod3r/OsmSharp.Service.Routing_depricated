﻿// OsmSharp - OpenStreetMap (OSM) SDK
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

using System.Reflection;
using NUnit.Framework;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Service.Routing;

namespace OsmSharp.Service.Routing.Unittests
{
    /// <summary>
    /// The bootstrapper to start the service to test.
    /// </summary>
    [SetUpFixture]
    public class Bootstrapper
    {
        /// <summary>
        /// Executed before any tests run.
        /// </summary>
        [SetUp]
        public void RunBeforeAnyTests()
        {
            // create the source and start the service for testing.
            var source = new PBFOsmStreamSource(Assembly.GetExecutingAssembly().GetManifestResourceStream(
                "OsmSharp.Service.Routing.Unittests.testdata.osm.pbf"));
            OperationProcessor.Settings["source_stream"] = source;

            OsmSharp.Service.Routing.SelfHost.Program.Start("http://127.0.0.1:666/");
        }

        /// <summary>
        /// Executed after testing has finished.
        /// </summary>
        [TearDown]
        public void RunAfterAnyTests()
        {

        }
    }
}