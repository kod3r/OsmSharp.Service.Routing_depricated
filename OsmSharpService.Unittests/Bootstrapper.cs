using NUnit.Framework;
using OsmSharp.Osm.PBF.Streams;
using OsmSharpService.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OsmSharpService.Unittests
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
                "OsmSharpService.Unittests.testdata.osm.pbf"));
            OperationProcessor.Settings["source_stream"] = source;

            OsmSharpService.SelfHost.Program.Start("http://127.0.0.1:666/");
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
