// BootFX - Application framework for .NET applications
// 
// File: StringBuilderExtender.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace System.Text
{
    public static class StringBuilderExtender
    {
        public static void AppendIfNotEndsWith(this StringBuilder builder, char c)
        {
            builder.AppendIfNotEndsWith(new string(new char[] { c } ));
        }

        public static void AppendIfNotEndsWith(this StringBuilder builder, string buf)
        {
            if (!(builder.EndsWith(buf)))
                builder.Append(buf);
        }

        public static bool EndsWith(this StringBuilder builder, string buf)
        {
            return builder.ToString().EndsWith(buf);
        }


        public static bool StartsWith(this StringBuilder builder, string buf)
        {
            return builder.ToString().StartsWith(buf);
        }

        public static void AppendWithSeparator<T>(this StringBuilder builder, IEnumerable<T> items, Action<T> callback, string sep = ", ")
        {
            var first = true;
            foreach (var item in items)
            {
                if (first)
                    first = false;
                else
                    builder.Append(sep);

                callback(item);
            }
        }
    }
}
