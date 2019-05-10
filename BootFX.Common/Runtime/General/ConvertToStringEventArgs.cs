// BootFX - Application framework for .NET applications
// 
// File: ConvertToStringEventArgs.cs
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
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common
{
    public delegate void ConvertToStringEventHandler(object sender, ConvertToStringEventArgs e);

    public class ConvertToStringEventArgs : EventArgs
    {
        private object _value;
        private string _asString;

        public ConvertToStringEventArgs(object value)
        {
            this.Value = value;

            // convert...
            if (value is Entity)
                _asString = value.ToString();
            else
                _asString = ConversionHelper.ToString(value, Cultures.User);
        }

        public object Value
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

        public string AsString
        {
            get
            {
                return _asString;
            }
            set
            {
                _asString = value;
            }
        }
    }
}
