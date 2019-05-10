// BootFX - Application framework for .NET applications
// 
// File: CreateForeignKeySchemaWorkUnit.cs
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
using BootFX.Common.Entities;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines a work unit that can add a column to a table.
	/// </summary>
	internal class CreateForeignKeySchemaWorkUnit : SchemaWorkUnit
	{		
		/// <summary>
		/// Private field to support <see cref="ForeignKey"/> property.
		/// </summary>
		private SqlChildToParentLink _foreignKey;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="foreignKey"></param>
		internal CreateForeignKeySchemaWorkUnit(EntityType entityType, SqlChildToParentLink foreignKey) : base(entityType)
		{
			_foreignKey = foreignKey;	
		}

		/// <summary>
		/// Gets the column.
		/// </summary>
		internal SqlChildToParentLink ForeignKey
		{
			get
			{
				return _foreignKey;
			}
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(context.Dialect == null)
				throw new InvalidOperationException("context.Dialect is null.");
			return SqlStatement.CreateStatements(context.Dialect.GetCreateForeignKeyStatement(this.ForeignKey));
		}

		public override string ToString()
		{
			return string.Format("{0} [{1} on {2}]", base.ToString(), this.ForeignKey.NativeName, this.ForeignKey.Table.NativeName);
		}
	}
}
