// BootFX - Application framework for .NET applications
// 
// File: IErrorCollectorExtender.cs
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
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common
{
    public static class IErrorCollectorExtender
    {
        public static void AddErrors(this IErrorCollector collector, IEnumerable<string> errors)
        {
            foreach (var error in errors)
                collector.AddError(error);
        }

        public static void CopyErrorsFrom(this IErrorCollector collector, IUserMessages messages)
        {
            collector.AddErrors(messages.GetUserMessages());
        }

        public static void AddIsRequiredError(this IErrorCollector collector, string name)
        {
            collector.AddError(string.Format("'{0}' is required.", name));
        }
    }
}
