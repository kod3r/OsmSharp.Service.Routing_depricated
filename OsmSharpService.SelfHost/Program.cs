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
using System.Collections.Generic;
using System.Configuration;
using OsmSharpService.Core;

namespace OsmSharpService.SelfHost
{
    /// <summary>
    /// Program containing the entry point of the application.
    /// </summary>
    public partial class Program
    {
        static void Main(string[] args)
        {
            // direct the output to the console.
            OsmSharp.Logging.Log.Enable();
            OsmSharp.Logging.Log.RegisterListener(
                new OsmSharp.WinForms.UI.Logging.ConsoleTraceListener());

            // check for command line options.
            var options = new Options();
            var parser = new CommandLine.Parser(with => with.HelpWriter = Console.Error);
            if (parser.ParseArgumentsStrict(args, options, () =>
                {
                    Console.ReadLine();
                    Environment.Exit(-2);
                }))
            {
                if (options.Type != null && options.Type.Length > 0)
                { // check format.
                    if(options.Format != null && options.Type != null)
                    { // the format is set.
                        OperationProcessor.Settings["graph." + options.Type + "." + options.Format] = 
                            options.File;
                    }
                    else
                    { // oeps, not format but a type?
                        throw new ArgumentOutOfRangeException("A type was given but not a format.");
                    }
                }
                else
                { // parsing was successfull.
                    OperationProcessor.Settings["pbf_file"] = options.File;
                }
            }

            // start application with parsed settings.
            Program.Start(options.Hostname);

            Console.ReadLine();
        }

        /// <summary>
        /// An alternative entrypoint to the application assuming OperationProcessor.Settings are already set.
        /// </summary>
        /// <remarks>This only exists to increase testability.</remarks>
        public static void Start(string hostname)
        {
            // initializes the processor(s).
            var processors = new List<IProcessor>();
            processors.Add(OperationProcessor.GetInstance());

            // start all the processor(s).
            foreach (IProcessor processor in processors)
            {
                processor.Start();
            }

            // get the hostname.
            if (string.IsNullOrWhiteSpace(hostname))
            {
                hostname = ConfigurationManager.AppSettings["hostname"];
            }
            if (string.IsNullOrWhiteSpace(hostname))
            {
                throw new ArgumentOutOfRangeException("hostname", "Hostname not configure! use -h or --host");
            }
            OsmSharp.Logging.Log.TraceEvent("Program", OsmSharp.Logging.TraceEventType.Information,
                "Service will listen to: {0}", hostname);

            // start the self-hosting.
            var host = new AppHost();
            host.Init();
            host.Start(hostname);

            OsmSharp.Logging.Log.TraceEvent("Program", OsmSharp.Logging.TraceEventType.Information,
                "OsmDataService started.");
        }
    }
}