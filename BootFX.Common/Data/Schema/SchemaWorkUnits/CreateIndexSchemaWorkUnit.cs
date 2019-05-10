// BootFX - Application framework for .NET applications
// 
// File: CreateIndexSchemaWorkUnit.cs
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
	internal class CreateIndexSchemaWorkUnit : SchemaWorkUnit
	{		
		/// <summary>
		/// Private field to support <see cref="Table"/> property.
		/// </summary>
		private SqlTable _table;

		/// <summary>
		/// Private field to support <see cref="Index"/> property.
		/// </summary>
		private SqlIndex _index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="index"></param>
		internal CreateIndexSchemaWorkUnit(EntityType entityType,SqlTable table, SqlIndex index) : base(entityType)
		{
			_index = index;	
			_table = table;
		}

		/// <summary>
		/// Gets the Table.
		/// </summary>
		internal SqlTable Table
		{
			get
			{
				return _table;
			}
		}

		
		/// <summary>
		/// Gets the column.
		/// </summary>
		internal SqlIndex Index
		{
			get
			{
				return _index;
			}
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(context.Dialect == null)
				throw new InvalidOperationException("context.Dialect is null.");
			return SqlStatement.CreateStatements(context.Dialect.GetCreateIndexStatement(this.Table,this.Index));
		}

		public override string ToString()
		{
			return string.Format("{0} [{1} on {2}]", base.ToString(), this.Index.NativeName, this.Table.NativeName);
		}
        protected override bool ContinueOnError
        {
            get
            {
                return true;
            }
        }
    }
}
