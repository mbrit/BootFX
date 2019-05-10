// BootFX - Application framework for .NET applications
// 
// File: ConnectionStringBuilderAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for <see cref="ConnectionStringBuilderAttribute"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ConnectionStringBuilderAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private Type _type;
		
		/// <summary>
		/// Creates a new instance of <see cref="ConnectionStringBuilderAttribute"/>.
		/// </summary>
		public ConnectionStringBuilderAttribute(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			_type = type;
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
	}
}
