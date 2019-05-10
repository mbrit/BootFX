// BootFX - Application framework for .NET applications
// 
// File: JsonNetMetadata.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    internal static class JsonNetMetadata
    {
        internal static Type JsonPropertyAttributeType { get; set; }
        internal static Type JsonTokenEnumType { get; set; }

        static JsonNetMetadata()
        {
            JsonPropertyAttributeType = Type.GetType("Newtonsoft.Json.JsonPropertyAttribute, Newtonsoft.Json");
            if (JsonPropertyAttributeType == null)
                throw new InvalidOperationException("JSON.NET types not found. Check the package is referenced.");

            JsonTokenEnumType = Type.GetType("Newtonsoft.Json.JsonToken, Newtonsoft.Json");
        }

        internal static string GetPropertyNameFromAttribute(object attr)
        {
            var prop = JsonPropertyAttributeType.GetProperty("PropertyName");
            return (string)prop.GetValue(attr);
        }
    }
}
