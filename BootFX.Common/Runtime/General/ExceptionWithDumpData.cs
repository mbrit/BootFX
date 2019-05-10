// BootFX - Application framework for .NET applications
// 
// File: ExceptionWithDumpData.cs
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
	/// Defines an exception that includes additional dump data accessed through the <see cref="Dump"></see> property or <see cref="ToString"></see> method.
	/// </summary>
	[Serializable()]
	public abstract class ExceptionWithDumpData : ApplicationException
	{
		/// <summary>
		/// Private field to support <see cref="Dump"/> property.
		/// </summary>
		private string _dump;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		// TODO: Add a default message here.
		protected ExceptionWithDumpData() 
			: base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected ExceptionWithDumpData(string message) 
			: base(message)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ExceptionWithDumpData(string message, Exception innerException) 
			: base(message, innerException)
		{
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected ExceptionWithDumpData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
			_dump = info.GetString("_dump");
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
			info.AddValue("_dump", _dump);
		}

		/// <summary>
		/// Gets the string representation of the object include the dump data.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Cultures.Exceptions, "{0}\r\n---------------------------------\r\n{1}", base.ToString(), this.Dump);
		}

		/// <summary>
		/// Gets the dump.
		/// </summary>
		public string Dump
		{
			get
			{
				return _dump;
			}
		}	

		/// <summary>
		/// Sets dump information.
		/// </summary>
		/// <param name="dump"></param>
		protected void SetDump(string dump)
		{
			if(dump == null)
				dump = string.Empty;
			_dump = dump;
		}
	}
}
