// BootFX - Application framework for .NET applications
// 
// File: ErrorCollector.cs
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
    public class ErrorCollector : IErrorCollector
    {
        public List<string> Errors { get; set; }

        public ErrorCollector()
        {
            this.Errors = new List<string>();
        }

        public bool HasErrors
        {
            get
            {
                return this.Errors.Any();
            }
        }

        public void AddError(string error)
        {
            this.Errors.Add(error);
        }

        public string[] GetUserMessages()
        {
            return new List<string>(this.Errors).ToArray();
        }
    }
}
