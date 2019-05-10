// BootFX - Application framework for .NET applications
// 
// File: HttpHelperException.cs
// Build: 5.2.10321.2307
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
    /// <summary>
        /// Defines an exception message.
        /// </summary>
    [Serializable()]
    public class HttpHelperException : ApplicationException
    {
        public string ErrorText { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal HttpHelperException(string message, string errorText, Exception innerException)
            : base(message, innerException)
        {
            this.ErrorText = errorText;
        }

        /// <summary> 
        /// Deserialization constructor.
        /// </summary> 
        protected HttpHelperException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary> 
        /// Provides data to serialization.
        /// </summary> 
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            // base...
            base.GetObjectData(info, context);

            // store other items in the serialization bucket...
        }

        public static string GetErrorText(Exception ex)
        {
            var walk = ex;
            while (walk != null)
            {
                if (walk is HttpHelperException)
                    return ((HttpHelperException)walk).ErrorText;
                walk = walk.InnerException;
            }
            return null;
        }
    }

}
