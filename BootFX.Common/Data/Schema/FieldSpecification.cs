// BootFX - Application framework for .NET applications
// 
// File: FieldSpecification.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Gets the field specification for the given column.
	/// </summary>
	public class FieldSpecification
	{
		/// <summary>
		/// Private field to support <see cref="DbType"/> property.
		/// </summary>
		private DbType _dbType;

		/// <summary>
		/// Private field to support <see cref="IsLarge"/> property.
		/// </summary>
		private bool _isLarge;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public FieldSpecification(DbType dbType, bool isLarge)
		{
			_dbType = dbType;
			_isLarge = isLarge;
		}

		/// <summary>
		/// Gets the islarge.
		/// </summary>
		public bool IsLarge
		{
			get
			{
				return _isLarge;
			}
		}
		
		/// <summary>
		/// Gets the dbtype.
		/// </summary>
		public DbType DbType
		{
			get
			{
				return _dbType;
			}
		}
	}
}
