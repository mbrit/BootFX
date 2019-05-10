// BootFX - Application framework for .NET applications
// 
// File: ConnectionType.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Holds connection types for use with <see cref="ConnectToDatabaseDialog"></see>.
	/// </summary>
	internal class ConnectionType
	{
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private Type _type;

		/// <summary>
		/// Private field to support <see cref="Description"/> property.
		/// </summary>
		private string _description;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="description"></param>
		public ConnectionType(Type type, string description)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			_type = type;
			_description = description;
		}

		/// <summary>
		/// Gets the description.
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		public override string ToString()
		{
			if(this.Description != null && this.Description.Length > 0)
				return this.Description;
			if(this.Type != null)
				return this.Type.ToString();
			return base.ToString();
		}
	}
}
