// BootFX - Application framework for .NET applications
// 
// File: SchemaColumnWorkUnit.cs
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
	/// Defines an instance of <c>SchemaColumnWorkUnit</c>.
	/// </summary>
	internal abstract class SchemaColumnWorkUnit : SchemaWorkUnit
	{
		/// <summary>
		/// Private field to support <see cref="Column"/> property.
		/// </summary>
		private SqlColumn _column;

		internal SchemaColumnWorkUnit(EntityType entityType, SqlColumn column) : base(entityType)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			_column = column;
		}

		/// <summary>
		/// Gets the column.
		/// </summary>
		internal SqlColumn Column
		{
			get
			{
				return _column;
			}
		}

		internal SqlTable Table
		{
			get
			{
				if(Column == null)
					throw new InvalidOperationException("Column is null.");
				return this.Column.Table;
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();			
			builder.AppendFormat("{0} [{1} on {2}]", base.ToString(), this.Column.NativeName, this.Table.NativeName);

			// reason?
			if(this.Reason != null && this.Reason.Length > 0)
			{
				builder.Append(" - ");
				builder.Append(this.Reason);
			}

			// return...
			return builder.ToString();
		}
	}
}
