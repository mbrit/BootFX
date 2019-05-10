// BootFX - Application framework for .NET applications
// 
// File: LogArgs.cs
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
    public class LogArgs
    {
        private string _message;
        private Exception _exception;
        private string _system;
        private string _category;
        private ConsoleColor _color;
        internal bool HasColor { get; private set; }

        public LogArgs(string message, Exception ex)
        {
            _message = message;
            _exception = ex;
            _system = null;
            _category = "Log";
            _color = ConsoleColor.Gray;
            this.HasColor = false;
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
            }
        }

        public string System
        {
            get
            {
                return _system;
            }
            set
            {
                _system = value;
            }
        }

        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        public bool HasException
        {
            get
            {
                return this.Exception != null;
            }
        }    

        public ConsoleColor Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                this.HasColor = true;
            }
        }
    }
}
