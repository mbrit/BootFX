// BootFX - Application framework for .NET applications
// 
// File: BadTransactionException.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Defines an exception message.
    /// </summary>
    [Serializable()]
    public class BadTransactionException : ApplicationException
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal BadTransactionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal BadTransactionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary> 
        /// Deserialization constructor.
        /// </summary> 
        protected BadTransactionException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary> 
        /// Provides data to serialization.
        /// </summary> 
        [SecurityCritical]
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // base...
            base.GetObjectData(info, context);

            // store other items in the serialization bucket...
        }
    }

}
