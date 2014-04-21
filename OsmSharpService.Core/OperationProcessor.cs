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
using OsmSharp.Logging;
using OsmSharp.Math.Geo;
using OsmSharp.Math.VRP.Core.Routes;
using OsmSharp.Routing;
using OsmSharp.Routing.TSP.Genetic;
using OsmSharpService.Core.Routing;
using OsmSharpService.Core.Routing.Primitives;
using OsmSharpService.Core.Routing.Primitives.GeoJSON;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace OsmSharpService.Core
{
    /// <summary>
    /// Processes routing service requests.
    /// </summary>
    public class OperationProcessor : IProcessor
    {
        /// <summary>
        /// Holds the router factory.
        /// </summary>
        private RouterFactoryBase _routerFactory;

        /// <summary>
        /// Creates a new operation processor.
        /// </summary>
        public OperationProcessor(RouterFactoryBase routerFactory)
        {
            _routerFactory = routerFactory;
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static OperationProcessor()
        {
            Vehicle.RegisterVehicles();
        }

        /// <summary>
        /// Processes a routing operation.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public object ProcessRoutingOperation(RoutingOperation operation)
        {
            // create the response object.
            RoutingResponse response;

            if (!this.IsReady())
            { // return that the service is not ready.
                response = new RoutingResponse
                               {
                                   Status = OsmSharpServiceResponseStatusEnum.Failed,
                                   StatusMessage = "Service is not ready!"
                               };

                return response;
            }

            if (operation.Hooks == null || operation.Hooks.Length == 0)
            { // there are no hooks!
                response = new RoutingResponse
                               {
                                   Status = OsmSharpServiceResponseStatusEnum.Failed,
                                   StatusMessage = "No hooks found!"
                               };

                return response;
            }

            try
            {
                // create the default edge matcher.
                IEdgeMatcher matcher = new LevenshteinEdgeMatcher();

                // create the router.
                var router = _routerFactory.CreateRouter();

                // execute the requested operation.
                switch (operation.Type)
                {
                    case RoutingOperationType.ManyToMany:
                        response = this.DoManyToMany(operation, router, matcher);
                        break;
                    case RoutingOperationType.Regular:
                        response = this.DoRegular(operation, router, matcher);
                        break;
                    case RoutingOperationType.TSP:
                        response = this.DoTSP(operation, router, matcher, false);
                        break;
                    case RoutingOperationType.OpenTSP:
                        response = this.DoTSP(operation, router, matcher, true);
                        break;
                    case RoutingOperationType.ToClosest:
                        response = this.DoToClosest(operation, router, matcher);
                        break;
                    default:
                        throw new Exception(string.Format("Invalid operation type:{0}", 
                            operation.Type));
                }

                // convert to the proper response.
                if (response.Route != null)
                {
                    switch (operation.ReturnType)
                    {
                        case RouteReturnType.Array:
                            response.RouteArray = response.Route.GetPoints().Select(x => new double[] { x.Latitude, x.Longitude }).ToArray();
                            response.Route = null;
                            break;
                        case RouteReturnType.LineString:
                            response.RouteLineString = Feature.FromRoute(response.Route);
                            response.Route = null;
                            break;
                    }
                }
            }
            catch (Exception ex)
            { // any exception was thrown.
                // create the response.
                response = new RoutingResponse
                               {
                                   Status = OsmSharpServiceResponseStatusEnum.Failed,
                                   StatusMessage = ex.Message
                               };
            }
            return response;
        }

        /// <summary>
        /// Calculates the route from the source to the closest point.
        /// </summary>
        /// <param name="operation">The operation request.</param>
        /// <param name="router">The router.</param>
        /// <param name="matcher">Contains an algorithm to match points to the route network.</param>
        /// <returns></returns>
        private RoutingResponse DoToClosest(
            RoutingOperation operation, Router router, IEdgeMatcher matcher)
        {
            // get the vehicle.
            var vehicle = Vehicle.GetByUniqueName(operation.Vehicle);

            // create the response.
            var response = new RoutingResponse();

            // resolve the points and do the routing.
            var hooksPerRouterPoints = new Dictionary<RouterPoint, List<RoutingHook>>();
            var routerPoints = new List<RouterPoint>();
            var unroutableHooks = new List<RoutingHook>(); // save the unroutable hooks.
            for (int idx = 0; idx < operation.Hooks.Length; idx++)
            {
                // routing hook tags.
                TagsCollectionBase tags = new TagsCollection(operation.Hooks[idx].Tags.ConvertToDictionary());

                // resolve the point.
                RouterPoint routerPoint = router.Resolve(vehicle, new GeoCoordinate(
                    operation.Hooks[idx].Latitude, operation.Hooks[idx].Longitude), matcher, tags);

                // check the result.
                if (routerPoint == null)
                { // this hook is not routable.
                    if (idx == 0)
                    { // the first point has to be valid!
                        throw new Exception("Cannot resolve source point of a CalculateToClosest() operation.");
                    }
                    unroutableHooks.Add(operation.Hooks[idx]);
                }
                else
                { // a point to hook on was found!
                    List<RoutingHook> hooksAtPoint;
                    if (!hooksPerRouterPoints.TryGetValue(routerPoint, out hooksAtPoint))
                    { // the router point does not exist yet.
                        // check if the router point is routable.
                        if (router.CheckConnectivity(vehicle, routerPoint, 200))
                        {// the point is routable.
                            // create the hooks list at this point.
                            hooksAtPoint = new List<RoutingHook>();
                            hooksPerRouterPoints.Add(routerPoint, hooksAtPoint);

                            // add the new router point to the list.
                            routerPoint.Tags.Add(new KeyValuePair<string, string>("id", routerPoints.Count.ToString(
                                System.Globalization.CultureInfo.InvariantCulture)));
                            routerPoint.Tags.Add(new KeyValuePair<string, string>("idx", idx.ToString(
                                System.Globalization.CultureInfo.InvariantCulture)));
                            routerPoints.Add(routerPoint);

                            // add the hook.
                            hooksAtPoint.Add(operation.Hooks[idx]);
                        }
                        else
                        {// this hook is not routable.
                            if (idx == 0)
                            { // the first point has to be valid!
                                throw new Exception("Cannot resolve source point of a CalculateToClosest() operation.");
                            }
                            unroutableHooks.Add(operation.Hooks[idx]);
                        }
                    }
                    else
                    {
                        // add the hook.
                        hooksAtPoint.Add(operation.Hooks[idx]);
                    }
                }
            }

            // add the unroutable hooks.
            response.UnroutableHooks = unroutableHooks.ToArray();

            // extract the first point.
            RouterPoint first = routerPoints[0];
            routerPoints.RemoveAt(0);

            // calculate all the weights.
            Route route = router.CalculateToClosest(vehicle, first, routerPoints.ToArray());

            if (route != null)
            {
                // add the routerpoint tags.
                string idxClosestString = route.Entries[route.Entries.Length - 1].Points[0].Tags[1].Value;
                int idxClosest = int.Parse(idxClosestString);

                // get the closest point.
                RoutingHook pointClosest = operation.Hooks[idxClosest];

                // get the routerpoint.
                RoutePoint routePoint = route.Entries[route.Entries.Length - 1].Points[0];

                // add the closest point tags.
                routePoint.Latitude = pointClosest.Latitude;
                routePoint.Longitude = pointClosest.Longitude;
                List<KeyValuePair<string, string>> tags = pointClosest.Tags.ConvertToList();
                tags.Add(new KeyValuePair<string, string>("id", pointClosest.Id.ToString())); // add the id-tag.
                routePoint.Tags = tags.ConvertFrom();

                response.Route = route;

                // set the response as successfull.
                response.Status = OsmSharpServiceResponseStatusEnum.Success;
            }

            return response;
        }

        /// <summary>
        /// Calculates the TSP.
        /// </summary>
        /// <param name="operation">The operation request.</param>
        /// <param name="router">The router.</param>
        /// <param name="matcher">Contains an algorithm to match points to the route network.</param>
        /// <param name="open">Flag indicating the type of TSP problem, open or not.</param>
        /// <returns></returns>
        private RoutingResponse DoTSP(
            RoutingOperation operation, Router router, IEdgeMatcher matcher, bool open)
        {
            // get the vehicle.
            var vehicle = Vehicle.GetByUniqueName(operation.Vehicle);

            // create the response.
            var response = new RoutingResponse();

            // resolve the points and do the routing.
            var hooksPerRouterPoints = new Dictionary<RouterPoint, List<RoutingHook>>();
            var routerPoints = new List<RouterPoint>();
            var unroutableHooks = new List<RoutingHook>(); // save the unroutable hooks.
            for (int idx = 0; idx < operation.Hooks.Length; idx++)
            {
                // routing hook tags.
                TagsCollectionBase tags = new TagsCollection(operation.Hooks[idx].Tags.ConvertToDictionary());

                // resolve the point.
                RouterPoint routerPoint = router.Resolve(vehicle, new GeoCoordinate(
                    operation.Hooks[idx].Latitude, operation.Hooks[idx].Longitude), matcher, tags);

                // check the result.
                if (routerPoint == null)
                { // this hook is not routable.
                    unroutableHooks.Add(operation.Hooks[idx]);
                }
                else
                { // a point to hook on was found!
                    List<RoutingHook> hooksAtPoint;
                    if (!hooksPerRouterPoints.TryGetValue(routerPoint, out hooksAtPoint))
                    { // the router point does not exist yet.
                        // check if the router point is routable.
                        if (router.CheckConnectivity(vehicle, routerPoint, 200))
                        {// the point is routable.
                            // create the hooks list at this point.
                            hooksAtPoint = new List<RoutingHook>();
                            hooksPerRouterPoints.Add(routerPoint, hooksAtPoint);

                            // add the new router point to the list.
                            routerPoint.Tags.Add(new KeyValuePair<string, string>("id", routerPoints.Count.ToString(
                                System.Globalization.CultureInfo.InvariantCulture)));  
                            routerPoints.Add(routerPoint);
                                
                            // add the hook.
                            hooksAtPoint.Add(operation.Hooks[idx]);
                        }
                        else
                        {// this hook is not routable.
                            unroutableHooks.Add(operation.Hooks[idx]);
                        }
                    }
                    else
                    {
                        // add the hook.
                        hooksAtPoint.Add(operation.Hooks[idx]);
                    }
                }
            }

            // add the unroutable hooks.
            response.UnroutableHooks = unroutableHooks.ToArray();

            // calculate all the weights.
            double[][] weights = router.CalculateManyToManyWeight(vehicle, routerPoints.ToArray(), routerPoints.ToArray());

            // calculate the TSP.
            var tspSolver = new RouterTSPAEXGenetic(300, 100);
            IRoute tspRoute = tspSolver.CalculateTSP(weights, routerPoints.Select(x => x.Location).ToArray(), 0, !open);
            
            // calculate the actual route.
            Route route = null;
            foreach (Edge edge in tspRoute.Edges())
            {
                // calculate the actual edge-route.
                Route edgeRoute = router.Calculate(vehicle, routerPoints[edge.From], routerPoints[edge.To]);

                // add the routing hook tags.
                List<RoutingHook> fromHooks = hooksPerRouterPoints[routerPoints[edge.From]];
                edgeRoute.Entries[0].Points = new RoutePoint[fromHooks.Count];
                for (int hookIdx = 0; hookIdx < fromHooks.Count; hookIdx++)
                {
                    var hookPoint = new RoutePoint
                                        {
                                            Latitude = fromHooks[hookIdx].Latitude,
                                            Longitude = fromHooks[hookIdx].Longitude,
                                            Tags = fromHooks[hookIdx].Tags.ConvertToList().ConvertFrom()
                                        };

                    edgeRoute.Entries[0].Points[hookIdx] = hookPoint;
                }
                List<RoutingHook> toHooks = hooksPerRouterPoints[routerPoints[edge.To]];
                edgeRoute.Entries[edgeRoute.Entries.Length - 1].Points = new RoutePoint[toHooks.Count];
                for (int hookIdx = 0; hookIdx < toHooks.Count; hookIdx++)
                {
                    var hookPoint = new RoutePoint
                                        {
                                            Latitude = toHooks[hookIdx].Latitude,
                                            Longitude = toHooks[hookIdx].Longitude,
                                            Tags = toHooks[hookIdx].Tags.ConvertToList().ConvertFrom()
                                        };

                    edgeRoute.Entries[edgeRoute.Entries.Length - 1].Points[hookIdx] = hookPoint;
                }

                // concatenate routes.
                if (route == null)
                {
                    route = edgeRoute;
                }
                else
                {
                    route = Route.Concatenate(route, edgeRoute);
                }
            }

            // assign the response.
            response.Route = route;

            // set the response as successfull.
            response.Status = OsmSharpServiceResponseStatusEnum.Success;

            return response;
        }
        
        /// <summary>
        /// Calculates a regular route.
        /// </summary>
        /// <param name="operation">The operation request.</param>
        /// <param name="router">The router.</param>
        /// <param name="matcher">Contains an algorithm to match points to the route network.</param>
        /// <returns></returns>
        private RoutingResponse DoRegular(
            RoutingOperation operation, Router router, IEdgeMatcher matcher)
        {
            // get the vehicle.
            var vehicle = Vehicle.GetByUniqueName(operation.Vehicle);

            // create the response.
            var response = new RoutingResponse();

            Route route = null;
            RouterPoint previous = null;
            var unroutableHooks = new List<RoutingHook>(); // keep a list of unroutable hooks.
            for (int idx = 0; idx < operation.Hooks.Length; idx++)
            {
                // routing hook tags.
                TagsCollectionBase tags = new TagsCollection(operation.Hooks[idx].Tags.ConvertToDictionary());

                // resolve the next point.
                RouterPoint next = router.Resolve(vehicle,
                                                  new GeoCoordinate(operation.Hooks[idx].Latitude,
                                                                    operation.Hooks[idx].Longitude), matcher,
                                                  tags);

                // check the routability.
                if (next != null &&
                    router.CheckConnectivity(vehicle, next, 100))
                { // the next point is routable.
                    if (previous != null)
                    {
                        // calculate the next part of the route.
                        Route routePart = router.Calculate(vehicle, previous, next);
                        // concatenate the route part.
                        if (route == null)
                        {
                            route = routePart;
                        }
                        else
                        {
                            route = Route.Concatenate(route, routePart);
                        }
                    }

                    // set the next to the previous.
                    previous = next;
                }
                else
                { // add the hook to the unroutable hooks list.
                   unroutableHooks.Add(operation.Hooks[idx]);
                }
            }

            // add the unroutable hooks.
            response.UnroutableHooks = unroutableHooks.ToArray();

            // set the response route.
            response.Route = route;

            // report succes!
            response.Status = OsmSharpServiceResponseStatusEnum.Success;
            response.StatusMessage = string.Empty;

            return response;
        }
       
        /// <summary>
        /// Calculates a matrix of weights between all given points.
        /// </summary>
        /// <param name="operation">The operation request.</param>
        /// <param name="router">The router.</param>
        /// <param name="matcher">Contains an algorithm to match points to the route network.</param>
        /// <returns></returns>
        private RoutingResponse DoManyToMany(
            RoutingOperation operation, Router router, IEdgeMatcher matcher)
        {
            // get the vehicle.
            var vehicle = Vehicle.GetByUniqueName(operation.Vehicle);

            // create the response.
            var response = new RoutingResponse();

            // resolve the points and do the routing.
            var routerPoints = new List<RouterPoint>();
            var unroutableHooks = new List<RoutingHook>(); // save the unroutable hooks.
            for (int idx = 0; idx < operation.Hooks.Length; idx++)
            {
                // routing hook tags.
                TagsCollectionBase tags = new TagsCollection(operation.Hooks[idx].Tags.ConvertToDictionary());

                // resolve the point.
                RouterPoint routerPoint = router.Resolve(vehicle, new GeoCoordinate(
                    operation.Hooks[idx].Latitude, operation.Hooks[idx].Longitude), matcher, tags);

                // check the result.
                if (routerPoint == null)
                {
                    // this hook is not routable.
                    unroutableHooks.Add(operation.Hooks[idx]);
                }
                else
                {
                    // a point to hook on was found!
                    // check if the router point is routable.
                    if (router.CheckConnectivity(vehicle, routerPoint, 200))
                    {
                        // the point is routable.

                        // add the new router point to the list.
                        routerPoint.Tags.Add(new KeyValuePair<string, string>("id", routerPoints.Count.ToString(
                            System.Globalization.CultureInfo.InvariantCulture)));
                        routerPoints.Add(routerPoint);
                    }
                    else
                    {
                        // this hook is not routable.
                        unroutableHooks.Add(operation.Hooks[idx]);
                    }
                }
            }

            // add the unroutable hooks.
            response.UnroutableHooks = unroutableHooks.ToArray();


            // calculate and fill the response.
            response.Weights = router.CalculateManyToManyWeight(vehicle, routerPoints.ToArray(),
                routerPoints.ToArray());

            // report succes!
            response.Status = OsmSharpServiceResponseStatusEnum.Success;
            response.StatusMessage = string.Empty;

            return response;
        }

        /// <summary>
        /// Processes a resolve operation.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public object ProcessResolveResource(ResolveOperation operation)
        {
            // create the response object.
            var response = new ResolveResponse();

            if (!this.IsReady())
            { // return that the service is not ready.
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = "Service is not ready!";
                return response;
            }

            if (operation.Hooks == null || operation.Hooks.Length == 0)
            { // there are no hooks!
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = "No hooks found!";
                return response;
            }

            try
            {
                // create the default edge matcher.
                IEdgeMatcher matcher = new LevenshteinEdgeMatcher();

                // create the router.
                var router = _routerFactory.CreateRouter();

                // create the coordinates list.
                response.ResolvedHooks = new RoutingHookResolved[operation.Hooks.Length];
                for (int idx = 0; idx < operation.Hooks.Length; idx++)
                {
                    // routing hook tags.
                    TagsCollectionBase tags = new TagsCollection(operation.Hooks[idx].Tags.ConvertToDictionary());

                    // resolve the point.
                    RouterPoint resolved = router.Resolve(operation.Vehicle, new GeoCoordinate(
                        operation.Hooks[idx].Latitude, operation.Hooks[idx].Longitude), matcher, tags);

                    if (resolved != null)
                    { // the point was resolved successfully.
                        response.ResolvedHooks[idx] = new RoutingHookResolved
                        {
                            Id = operation.Hooks[idx].Id,
                            Latitude = (float)resolved.Location.Latitude,
                            Longitude = (float)resolved.Location.Longitude,
                            Succes = true
                        };
                    }
                    else
                    { // the hook was unsuccessfully resolved.
                        response.ResolvedHooks[idx] = new RoutingHookResolved
                        {
                            Id = operation.Hooks[idx].Id,
                            Succes = false
                        };
                    }
                }

                // report succes!
                response.Status = OsmSharpServiceResponseStatusEnum.Success;
                response.StatusMessage = string.Empty;
            }
            catch (Exception ex)
            { // any exception was thrown.
                response.Status = OsmSharpServiceResponseStatusEnum.Failed;
                response.StatusMessage = ex.Message;
            }
            return response;
        }

        #region Singleton

        /// <summary>
        /// Holds the instance.
        /// </summary>
        private static OperationProcessor _instance;

        /// <summary>
        /// Returns an instance of this processor.
        /// </summary>
        /// <returns></returns>
        public static OperationProcessor GetInstance()
        {
            if (_instance == null)
            { // create the instance.
                // load router factory or choose default.
                var routerAssemblyName = ConfigurationManager.AppSettings["routerfactory.assembly"];
                var routerAssembly = Assembly.LoadFrom(routerAssemblyName);
                if(routerAssembly == null)
                { // router assembly not found.
                    throw new Exception(string.Format("Could not create router factory: Assembly {0} not found.",
                        routerAssemblyName));
                }
                var routerFactory = (from t in routerAssembly.GetTypes()
                                     where t.IsSubclassOf(typeof(RouterFactoryBase))
                                     select (RouterFactoryBase)Activator.CreateInstance(t)).First();
                if (routerFactory == null)
                { // router factory not found.
                    throw new Exception(string.Format("Could not create router factory: Router implementation not found in Assembly {0}.",
                        routerAssemblyName));
                }
                _instance = new OperationProcessor(routerFactory);
            }
            return _instance;
        }

        #endregion

        /// <summary>
        /// Starts this processor.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            var thread = new System.Threading.Thread(
                PrepareRouter);
            thread.Start();

            return true;
        }

        /// <summary>
        /// Returns true if this processor is ready.
        /// </summary>
        /// <returns></returns>
        public bool IsReady()
        {
            return _routerFactory != null &&
                _routerFactory.IsReady;
        }

        /// <summary>
        /// Stops this processor.
        /// </summary>
        public void Stop()
        {
            _routerFactory = null;
        }

        #region Preparation

        /// <summary>
        /// Prepares the router.
        /// </summary>
        private void PrepareRouter()
        {
            // initializes the router factory.
            _routerFactory.Initialize();
 
            // report initialization finished.
            OsmSharp.Logging.Log.TraceEvent("OperationProcessor", TraceEventType.Information,
                "Operation processor ready...");
        }

        #endregion

        #region Settings

        /// <summary>
        /// Holds the settings for the operation processor.
        /// </summary>
        private static readonly Dictionary<string, object> _settings = new Dictionary<string, object>(); 

        /// <summary>
        /// Returns the settings for the operation processor.
        /// </summary>
        public static Dictionary<string, object> Settings
        {
            get
            {
                return _settings;
            }
        }

        #endregion
    }
}