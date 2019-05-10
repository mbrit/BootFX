// BootFX - Application framework for .NET applications
// 
// File: CreateTableSchemaWorkUnit.cs
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
	/// Defines an instance of <c>SqlCreateTableWorkUnit</c>.
	/// </summary>
	public class CreateTableSchemaWorkUnit : SchemaWorkUnit
	{ 
		/// <summary>
		/// Private field to support <see cref="Table"/> property.
		/// </summary>
		private SqlTable _table;
		
		internal CreateTableSchemaWorkUnit(EntityType entityType, SqlTable table) : base(entityType)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			_table = table;
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		internal SqlTable Table
		{
			get
			{
				return _table;
			}
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(context.Dialect == null)
				throw new InvalidOperationException("context.Dialect is null.");

			// run...
			if(Table == null)
				throw new InvalidOperationException("Table is null.");
			return SqlStatement.CreateStatements(context.Dialect.GetCreateTableScript(this.Table, SqlTableScriptFlags.None));
		}

		public override string ToString()
		{
			return string.Format("{0} [{1}]", base.ToString(), this.Table.NativeName);
		}
	}
}
