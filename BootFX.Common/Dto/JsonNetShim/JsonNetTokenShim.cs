// BootFX - Application framework for .NET applications
// 
// File: JsonNetTokenShim.cs
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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Dto
{
    internal static class JsonNetTokenShim
    {
        private static Dictionary<string, int> Values { get; set; }

        static JsonNetTokenShim()
        {
            Values = new Dictionary<string,int>();

            // walk...
            var type = JsonNetMetadata.JsonTokenEnumType;
            foreach (var name in Enum.GetNames(type))
            {
                var value = (int)Enum.Parse(type, name);
                Values[name] = value;
            }
        }

        internal static int EndObject
        {
            get
            {
                return GetValue();
            }
        }

        internal static int PropertyName
        {
            get
            {
                return GetValue();
            }
        }

        internal static int String
        {
            get
            {
                return GetValue();
            }
        }

        internal static int Boolean
        {
            get
            {
                return GetValue();
            }
        }

        internal static int Integer
        {
            get
            {
                return GetValue();
            }
        }

        internal static int Float
        {
            get
            {
                return GetValue();
            }
        }

        internal static int Null
        {
            get
            {
                return GetValue();
            }
        }

        internal static int StartArray
        {
            get
            {
                return GetValue();
            }
        }

        internal static int EndArray
        {
            get
            {
                return GetValue();
            }
        }

        internal static int StartObject
        {
            get
            {
                return GetValue();
            }
        }

        internal static int Date
        {
            get
            {
                return GetValue();
            }
        }

        private static int GetValue([CallerMemberName] string name = null)
        {
            return Values[name];
        }
    }
}
