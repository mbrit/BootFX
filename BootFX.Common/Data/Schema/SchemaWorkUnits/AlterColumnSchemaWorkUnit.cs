// BootFX - Application framework for .NET applications
// 
// File: AlterColumnSchemaWorkUnit.cs
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
using System.Data;
using System.Collections;
using BootFX.Common.Entities;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a work unit that can add a column to a table.
	/// </summary>
	internal class AlterColumnSchemaWorkUnit : SchemaColumnWorkUnit
	{		
	
		/// <summary>
		/// Private field to support <see cref="Type"/> property.
		/// </summary>
		private DbType _type;

		/// <summary>
		/// Private field to support <see cref="Length"/> property.
		/// </summary>
		private long _length;

		/// <summary>
		/// Private field to support <see cref="IsLarge"/> property.
		/// </summary>
		private bool _isLarge;

		/// <summary>
		/// Private field to support <see cref="IsNullable"/> property.
		/// </summary>
		private bool _isNullable;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="column"></param>
		internal AlterColumnSchemaWorkUnit(EntityType entityType,SqlColumn column, DbType type, long length, 
			bool isLarge, bool isNullable, SqlDatabaseDefault defaultExpression, string reason) : base(entityType,column)
		{
			_type = type;
			_length = length;
			_isLarge = isLarge;
			_isNullable = isNullable;

			// set...
			SetReason(reason);
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(context.Dialect == null)
				throw new InvalidOperationException("context.Dialect is null.");

			return SqlStatement.CreateStatements(context.Dialect.GetAddColumnStatement(this.Column,false)) ;
		}

		/// <summary>
		/// Gets the isnullable.
		/// </summary>
		internal bool IsNullable
		{
			get
			{
				return _isNullable;
			}
		}
		
		/// <summary>
		/// Gets the islarge.
		/// </summary>
		internal bool IsLarge
		{
			get
			{
				return _isLarge;
			}
		}
		
		/// <summary>
		/// Gets the length.
		/// </summary>
		internal long Length
		{
			get
			{
				return _length;
			}
		}
		
		/// <summary>
		/// Gets the type.
		/// </summary>
		internal DbType Type
		{
			get
			{
				return _type;
			}
		}
	}
}
