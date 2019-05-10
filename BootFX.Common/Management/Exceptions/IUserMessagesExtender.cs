// BootFX - Application framework for .NET applications
// 
// File: IUserMessagesExtender.cs
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

namespace BootFX.Common
{
    public static class IUserMessagesExtender
    {
        public static string GetUserMessagesSeparatedByCrLf(this IUserMessages messages)
        {
            return messages.GetUserMessagesSeparatedByInternal("\r\n");
        }

        public static string GetUserMessagesSeparatedByBr(this IUserMessages messages)
        {
            return messages.GetUserMessagesSeparatedByInternal("<br />");
        }

        private static string GetUserMessagesSeparatedByInternal(this IUserMessages messages, string sep)
        {
            StringBuilder builder = new StringBuilder();
            bool first = true;
            foreach (var message in messages.GetUserMessages())
            {
                if (first)
                    first = false;
                else
                    builder.Append(sep);
                builder.Append(message);
            }

            return builder.ToString();
        }
    }
}
