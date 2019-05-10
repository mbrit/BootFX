// BootFX - Application framework for .NET applications
// 
// File: TypeEventArgs.cs
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
    public delegate void TypeEventHandler(object sender, TypeEventArgs e);

    public class TypeEventArgs : EventArgs
    {
        /// <summary>
        /// Private value to support the <see cref="Type">Type</see> property.
        /// </summary>
        private Type _type;

        public TypeEventArgs(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            _type = type;
        }

        /// <summary>
        /// Gets the Type value.
        /// </summary>
        public Type Type
        {
            get
            {
                return _type;
            }
        }
    }
}
