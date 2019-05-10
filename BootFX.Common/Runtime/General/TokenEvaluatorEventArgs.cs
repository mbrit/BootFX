// BootFX - Application framework for .NET applications
// 
// File: TokenEvaluatorEventArgs.cs
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
    public delegate void TokenEvaluatorEventHandler(object sender, TokenEvaluatorEventArgs e);

    public class TokenEvaluatorEventArgs : EventArgs
    {
        public string PropertyName { get; set; }
        private object _result;
        public bool WasResultSet { get; private set; }

        public TokenEvaluatorEventArgs(string propertyName)
        {
            this.PropertyName = propertyName;
        }

        public object Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                this.WasResultSet = true;
            }
        }
    }
}
