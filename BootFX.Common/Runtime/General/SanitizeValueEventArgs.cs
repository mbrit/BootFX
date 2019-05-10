// BootFX - Application framework for .NET applications
// 
// File: SanitizeValueEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common
{
    public delegate void SanitizeValueEventHandler(object sender, SanitizeValueEventArgs e);

    public class SanitizeValueEventArgs : EventArgs
    {
        private string _value;

        public SanitizeValueEventArgs(string value)
        {
            this.Value = value;
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
