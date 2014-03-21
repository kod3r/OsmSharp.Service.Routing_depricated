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

using System.Collections.Generic;
using System.Configuration;
using System.ServiceProcess;
using OsmSharpService.Core;

namespace OsmSharpService.WindowsService
{
    /// <summary>
    /// The OsmSharp service host.
    /// </summary>
    public partial class OsmSharpService : ServiceBase
    {
        /// <summary>
        /// Holds the list of processes.
        /// </summary>
        private readonly List<IProcessor> _processors;

        /// <summary>
        /// Holds the services host.
        /// </summary>
        private AppHost _host;

        /// <summary>
        /// Creates the service.
        /// </summary>
        public OsmSharpService()
        {
            InitializeComponent();

            _processors = new List<IProcessor>();
        }

        /// <summary>
        /// Called when the service needs to start.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            // initialize the settings.
            OperationProcessor.Settings["pbf_file"] = ConfigurationManager.AppSettings["pbf_file"];

            // initializes the processor(s).
            _processors.Add(OperationProcessor.GetInstance());

            // start all the processor(s).
            foreach (IProcessor processor in _processors)
            {
                processor.Start();
            }

            // start the self-hosting.
            _host = new AppHost();
            _host.Init();
            _host.Start(ConfigurationManager.AppSettings["hostname"]);
        }

        /// <summary>
        /// Called when the services needs to stop.
        /// </summary>
        protected override void OnStop()
        {
            // stops the http listener.
            _host.Stop();

            // stop all the processor(s).
            foreach (IProcessor processor in _processors)
            {
                processor.Stop();
            }
        }
    }
}