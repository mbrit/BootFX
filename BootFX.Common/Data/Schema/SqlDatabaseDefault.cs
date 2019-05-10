// BootFX - Application framework for .NET applications
// 
// File: SqlDatabaseDefault.cs
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

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines an instance of <c>SqlDatabaseDefault</c>.
	/// </summary>
	public class SqlDatabaseDefault
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
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public SqlDatabaseDefault(SqlDatabaseDefaultType type, object value) : this(type,value,string.Empty)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="value"></param>
		public SqlDatabaseDefault(SqlDatabaseDefaultType type, object value, string name)
		{
			_type = type;
			_value = value;
			_name = name;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}
		
		/// <summary>
		/// Gets the value.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		public SqlDatabaseDefaultType Type
		{
			get
			{
				return _type;
			}
		}

		public override bool Equals(object obj)
		{
			if(obj == null)
				return false;

			// check...
			SqlDatabaseDefault other = (SqlDatabaseDefault)obj;
			if(other.Value == null && this.Value == null)
				return true;
			if(other.Value != null && this.Value == null)
				return false;
			if(other.Value == null && this.Value != null)
				return false;
			if(!other.Type.Equals(this.Type))
				return false;
			if(!other.Value.Equals(this.Value))
				return false;

			// ok...
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode ();
		}

	}
}
