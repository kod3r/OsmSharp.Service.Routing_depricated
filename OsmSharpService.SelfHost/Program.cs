using System;
using System.Collections.Generic;
using System.Configuration;
using OsmSharp.Tools.Output;
using OsmSharpService.Core;

namespace OsmSharpService.SelfHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // direct the output to the console.
            OsmSharp.Tools.Output.OutputStreamHost.RegisterOutputStream(
                new ConsoleOutputStream());

            // initializes the processor(s).
            List<IProcessor> processors = new List<IProcessor>();
            processors.Add(OperationProcessor.GetInstance());

            // start all the processor(s).
            foreach (IProcessor processor in processors)
            {
                processor.Start();
            }
            // get the hostname.
            string hostname = ConfigurationManager.AppSettings["hostname"];
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
