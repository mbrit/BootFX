// BootFX - Application framework for .NET applications
// 
// File: ErrorBucket.cs
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
    public class ErrorBucket : IUserMessages
    {
        public List<string> Errors { get; private set; }

        public ErrorBucket()
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

        public string[] GetUserMessages()
        {
            return this.Errors.ToArray();
        }

        public void AddError(string message, Exception ex = null)
        {
            if (ex == null)
                this.Errors.Add(message);
            else
                this.Errors.Add(string.Format("{0} --> {1}", message, ex));
        }

        public void AddErrors(IEnumerable<string> messages)
        {
            foreach (var message in messages)
                this.AddError(message);
        }

        public void AddErrors(IUserMessages messages)
        {
            var theMessages = messages.GetUserMessages();
            this.AddErrors(theMessages);
        }
    }
}
