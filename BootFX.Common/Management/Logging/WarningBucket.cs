// BootFX - Application framework for .NET applications
// 
// File: WarningBucket.cs
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

namespace BootFX.Common.Management
{
    public class WarningBucket : IUserMessages
    {
        public List<string> Warnings { get; private set; }

        public WarningBucket()
        {
            this.Warnings = new List<string>();
        }

        public bool HasWarnings
        {
            get
            {
                return this.Warnings.Any();
            }
        }

        public string[] GetUserMessages()
        {
            return this.Warnings.ToArray();
        }

        public void AddWarning(string message, Exception ex = null)
        {
            if (ex == null)
                this.Warnings.Add(message);
            else
                this.Warnings.Add(string.Format("{0} --> {1}", message, ex));
        }
    }
}
