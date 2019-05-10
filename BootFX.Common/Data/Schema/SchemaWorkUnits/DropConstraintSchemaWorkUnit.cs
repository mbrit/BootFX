// BootFX - Application framework for .NET applications
// 
// File: DropConstraintSchemaWorkUnit.cs
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
	internal class DropConstraintSchemaWorkUnit : SchemaColumnWorkUnit
	{		

		private SqlDatabaseDefault _defaultExpression = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="column"></param>
		internal DropConstraintSchemaWorkUnit(EntityType entityType,SqlColumn column, SqlDatabaseDefault defaultExpression) : base(entityType,column)
		{
			_defaultExpression = defaultExpression;
		}

		/// <summary>
		/// Gets the defaultexpression.
		/// </summary>
		internal SqlDatabaseDefault DefaultExpression
		{
			get
			{
				return _defaultExpression;
			}
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(context.Dialect == null)
				throw new InvalidOperationException("context.Dialect is null.");

			return SqlStatement.CreateStatements(context.Dialect.GetDropConstraintStatement(this.Column,DefaultExpression));
		}
	}
}
