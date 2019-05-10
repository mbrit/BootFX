// BootFX - Application framework for .NET applications
// 
// File: AddColumnSchemaWorkUnit.cs
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
	internal class AddColumnSchemaWorkUnit : SchemaColumnWorkUnit
	{		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="column"></param>
		internal AddColumnSchemaWorkUnit(EntityType entityType,SqlColumn column) : base(entityType,column)
		{
		}

		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(context.Dialect == null)
				throw new InvalidOperationException("context.Dialect is null.");
			return SqlStatement.CreateStatements(context.Dialect.GetAddColumnStatement(this.Column,true));
		}
	}
}
