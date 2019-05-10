// BootFX - Application framework for .NET applications
// 
// File: MdacInstallationException.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Security;

namespace BootFX.Common
{
	/// <summary>
	/// Thrown if there is a problem with the installed MDAC version.
	/// </summary>
	[Serializable()]
	public class MdacInstallationException : ApplicationException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		// TODO: Add a default message here.
		internal MdacInstallationException() 
			: base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal MdacInstallationException(string message) 
			: base(message)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal MdacInstallationException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected MdacInstallationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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

			// add code here to stream extra state into 'info'.
			// remember to update the deserializtion constructor.
		}
	}
}
