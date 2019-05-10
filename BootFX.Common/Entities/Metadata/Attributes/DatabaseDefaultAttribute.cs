// BootFX - Application framework for .NET applications
// 
// File: DatabaseDefaultAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines an instance of <c>DatabaseDefaultAttribute</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DatabaseDefaultAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private SqlDatabaseDefaultType _type;

		/// <summary>
		/// Private field to support <see cref="Value"/> property.
		/// </summary>
		private object _value;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public DatabaseDefaultAttribute(SqlDatabaseDefaultType type) : this(type, null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public DatabaseDefaultAttribute(SqlDatabaseDefaultType type, object value)
		{
			_type = type;
			_value = value;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		internal object Value
		{
			get
			{
				return _value;
			}
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		internal SqlDatabaseDefaultType Type
		{
			get
			{
				return _type;
			}
		}
	}
}
