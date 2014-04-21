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

using OsmSharp.Collections.Tags;
using OsmSharp.Collections.Tags.Index;
using OsmSharp.Osm.PBF.Streams;
using OsmSharp.Osm.Streams;
using OsmSharp.Osm.Streams.Filters;
using OsmSharp.Osm.Xml.Streams;
using OsmSharp.Routing;
using OsmSharp.Routing.CH;
using OsmSharp.Routing.Graph;
using OsmSharp.Routing.Graph.Router.Dykstra;
using OsmSharp.Routing.Osm.Graphs;
using OsmSharp.Routing.Osm.Interpreter;
using OsmSharp.Routing.Osm.Streams.Graphs;
using OsmSharpService.Core;
using System;
using System.IO;

namespace OsmSharpService.Plugin.Default
{
    /// <summary>
    /// Default implementation of the router factory.
    /// </summary>
    public class RouterFactory : RouterFactoryBase
    {
        /// <summary>
        /// Defines the create router delegate.
        /// </summary>
        /// <returns></returns>
        private delegate Router CreateRouterDelegate();

        /// <summary>
        /// Holds the create router delegate.
        /// </summary>
        private CreateRouterDelegate _createRouter;

        /// <summary>
        /// Initializes this router factory.
        /// </summary>
        public override void Initialize()
        {
            // initialize the interpreters.
            var interpreter = new OsmRoutingInterpreter();

            if (OperationProcessor.Settings.ContainsKey("graph.simple.flat"))
            { // use the live edge graph flat-file to create the router.
                TagsCollectionBase metaTags;
                var serializer = new OsmSharp.Routing.Osm.Graphs.Serialization.LiveEdgeFlatfileSerializer();
                var data = serializer.Deserialize(
                    new FileInfo(OperationProcessor.Settings["graph.simple.flat"].ToString()).OpenRead(), out metaTags, false);

                this._createRouter = new CreateRouterDelegate(() =>
                {
                    return Router.CreateLiveFrom(data,
                        new DykstraRoutingLive(), interpreter);
                });
            }
            else if (OperationProcessor.Settings.ContainsKey("graph.contracted.flat"))
            { // use the contracted graph flat-file to create the router.
                TagsCollectionBase metaTags;
                var serializer = new OsmSharp.Routing.CH.Serialization.CHEdgeFlatfileSerializer();
                var data = serializer.Deserialize(
                    new FileInfo(OperationProcessor.Settings["graph.contracted.flat"].ToString()).OpenRead(), out metaTags, false);

                this._createRouter = new CreateRouterDelegate(() =>
                {
                    return Router.CreateCHFrom(data, new CHRouter(), interpreter);
                });
            }
            else if (OperationProcessor.Settings.ContainsKey("graph.contracted.mobile"))
            { // use the contracted indexed flat-file to create the datasource.
                TagsCollectionBase metaTags;
                var serializer = new OsmSharp.Routing.CH.Serialization.Sorted.CHEdgeDataDataSourceSerializer(false);
                var data = serializer.Deserialize(
                    new FileInfo(OperationProcessor.Settings["graph.contracted.mobile"].ToString()).OpenRead(), out metaTags, false);

                this._createRouter = new CreateRouterDelegate(() =>
                {
                    return Router.CreateCHFrom(data, new CHRouter(), interpreter);
                });
            }
            else
            { // check if there is already a stream.
                OsmStreamSource source = null;
                if (OperationProcessor.Settings.ContainsKey("source_stream"))
                { // the source stream already exists.
                    source = OperationProcessor.Settings["source_stream"] as OsmStreamSource;
                }
                else if (OperationProcessor.Settings.ContainsKey("pbf_file"))
                { // a pbf file is the source.
                    source = new PBFOsmStreamSource(
                        (new FileInfo(OperationProcessor.Settings["pbf_file"] as string)).OpenRead());
                }
                else if (OperationProcessor.Settings.ContainsKey("xml_file"))
                { // an xml file is the source.
                    source = new XmlOsmStreamSource(
                        (new FileInfo(OperationProcessor.Settings["xml_file"] as string)).OpenRead());
                }
                else
                { // no valid configuration was found.
                    throw new InvalidOperationException("No valid source of OSM-data found in the OperationProcessor.Settings file.");
                }

                // creates a tagged index.
                var tagsIndex = new TagsTableCollectionIndex();

                // read from the OSM-stream.
                var data = new DynamicGraphRouterDataSource<LiveEdge>(tagsIndex);
                var targetData = new LiveGraphOsmStreamTarget(data, interpreter, tagsIndex);
                var sorter = new OsmStreamFilterSort();
                sorter.RegisterSource(source);
                targetData.RegisterSource(sorter);
                targetData.Pull();

                this._createRouter = new CreateRouterDelegate(() =>
                {
                    return Router.CreateLiveFrom(data, new DykstraRoutingLive(), interpreter);
                });
            }
        }

        /// <summary>
        /// Returns a new router instance.
        /// </summary>
        /// <returns></returns>
        public override Router CreateRouter()
        {
            return this._createRouter.Invoke();
        }

        /// <summary>
        /// Returns true if this factory is ready.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return this._createRouter != null;
            }
        }
    }
}