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
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace OsmSharpService.WindowsService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 &&
                args[0] != null &&
                (args[0] == "-i" || args[0] == "-u"))
            { // install the service.
                if (!(args[0] == "-i"))
                {
                    if (args[0] == "-u")
                    {
                        Install(true, args);
                        return;
                    }
                }
                else
                {
                    Install(false, args);
                    return;
                }
            }
            else
            { // runs the service.
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
			    { 
				    new OsmSharpService(),
				    new OsmSharpServiceDebugger() 
			    };
                ServiceBase.Run(ServicesToRun);
            }
        }

        /// <summary>
        /// Actually installs/uninstalls this service.
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="args"></param>
        private static void Install(bool undo, string[] args)
        {
            try
            {
                using (AssemblyInstaller installer = new AssemblyInstaller(
                    Assembly.GetEntryAssembly(), args))
                {
                    IDictionary savedState = new Hashtable();
                    installer.UseNewContext = true;
                    try
                    {
                        if (undo)
                        {
                            installer.Uninstall(savedState);
                        }
                        else
                        {
                            installer.Install(savedState);
                            installer.Commit(savedState);
                        }
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(savedState);
                        }
                        catch
                        {
                        }
                        throw;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(exception.Message);
            }
        }
        
    }
}