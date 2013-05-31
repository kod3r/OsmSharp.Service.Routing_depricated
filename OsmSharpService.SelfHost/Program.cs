using System;
using System.Collections.Generic;
using System.Configuration;
using OsmSharp.Tools.Output;
using OsmSharpService.Core;

namespace OsmSharpService.SelfHost
{
    /// <summary>
    /// Program containing the entry point of the application.
    /// </summary>
    partial class Program
    {
        static void Main(string[] args)
        {
            // direct the output to the console.
            OsmSharp.Tools.Output.OutputStreamHost.RegisterOutputStream(
                new ConsoleOutputStream());

            // check for command line options.
            var options = new Options();
            var parser = new CommandLine.Parser(with => with.HelpWriter = Console.Error);

            if (parser.ParseArgumentsStrict(args, options, () => Environment.Exit(-2)))
            {
                // parsing was successfull.
                OperationProcessor.Settings["pbf_file"] = options.File;
            }

            // initializes the processor(s).
            var processors = new List<IProcessor>();
            processors.Add(OperationProcessor.GetInstance());

            // start all the processor(s).
            foreach (IProcessor processor in processors)
            {
                processor.Start();
            }

            // get the hostname.
            string hostname = options.Hostname;
            if (string.IsNullOrWhiteSpace(hostname))
            {
                hostname = ConfigurationManager.AppSettings["hostname"];
            }
            if (string.IsNullOrWhiteSpace(hostname))
            {
                throw new ArgumentOutOfRangeException("hostname", "Hostname not configure! use -h or --host");
            }
            OsmSharp.Tools.Output.OutputStreamHost.WriteLine("Service will listen to: {0}",
                hostname);

            // start the self-hosting.
            var host = new AppHost();
            host.Init();
            host.Start(hostname);

            OsmSharp.Tools.Output.OutputStreamHost.WriteLine("OsmDataService started.");
            Console.ReadLine();
        }
    }
}
