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

using System.Collections.Generic;

namespace OsmSharp.Service.Routing.Routing.Primitives
{
    /// <summary>
    /// Represents a string pair, key-value, as a pair of tags for a routing hook.
    /// </summary>
    public class RoutingHookTag
    {
        /// <summary>
        /// The key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The value.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Containts extension methods for the RoutingHook-class.
    /// </summary>
    public static class RoutingHookExtensions
    {
        /// <summary>
        /// Converts the tags in the given dictionary to RoutingHookTags.
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        public static RoutingHookTag[] ConvertToRoutingHookTags(this IDictionary<string, string> tags)
        {
            var hookTags = new RoutingHookTag[tags.Count];
            int idx = 0;
            foreach (KeyValuePair<string, string> tag in tags)
            { // loop over all the tags.
                hookTags[idx] = new RoutingHookTag()
                {
                    Key = tag.Key,
                    Value = tag.Value
                };

                idx++;
            }
            return hookTags;
        }

        /// <summary>
        /// Converts the RoutingHookTags to a regular tags dictionary.
        /// </summary>
        /// <param name="hookTags"></param>
        /// <returns></returns>
        public static IDictionary<string, string> ConvertToDictionary(this RoutingHookTag[] hookTags)
        {
            IDictionary<string, string> tags = new Dictionary<string, string>();
            if (hookTags != null)
            {
                for (int idx = 0; idx < hookTags.Length; idx++)
                {
                    tags[hookTags[idx].Key] = hookTags[idx].Value;
                }
            }
            return tags;
        }

        /// <summary>
        /// Converts the RoutingHookTags to a regular tags dictionary.
        /// </summary>
        /// <param name="hookTags"></param>
        /// <returns></returns>
        public static List<KeyValuePair<string, string>> ConvertToList(this RoutingHookTag[] hookTags)
        {
            var tags = new List<KeyValuePair<string, string>>();
            if (hookTags != null)
            {
                for (int idx = 0; idx < hookTags.Length; idx++)
                {
                    tags.Add(new KeyValuePair<string, string>(hookTags[idx].Key, hookTags[idx].Value));
                }
            }
            return tags;
        }
    }
}