// BootFX - Application framework for .NET applications
// 
// File: LookupItem.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Defines a class that holds an item in a <c>Lookup</c> instance.
    /// </summary>
    // mbr - 2010-01-20 - added...
    internal class LookupItem
    {
        /// <summary>
        /// Private value to support the <see cref="Value">Value</see> property.
        /// </summary>
        private object _value;

        /// <summary>
        /// Private value to support the <see cref="ExpiresUtc">ExpiresUtc</see> property.
        /// </summary>
        private DateTime _expiresUtc;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="value"></param>
        internal LookupItem(object value, DateTime expiresUtc)
        {
            _value = value;
            _expiresUtc = expiresUtc;
        }

        /// <summary>
        /// Gets the ExpiresUtc value.
        /// </summary>
        private DateTime ExpiresUtc
        {
            get
            {
                return _expiresUtc;
            }
        }

        /// <summary>
        /// Gets the Value value.
        /// </summary>
        internal object Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Returns true if the item has expired.
        /// </summary>
        internal bool HasExpired
        {
            get
            {
                if (this.ExpiresUtc == DateTime.MinValue)
                    return false;
                else if (this.ExpiresUtc < DateTime.UtcNow)
                    return true;
                else
                    return false;
            }
        }
    }
}
