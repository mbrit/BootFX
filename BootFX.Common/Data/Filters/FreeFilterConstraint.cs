// BootFX - Application framework for .NET applications
// 
// File: FreeFilterConstraint.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Entities;
using System;
using System.Collections.Generic;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Describes a free test SQL constraint.
	/// </summary>
	public class FreeFilterConstraint : FilterConstraint
	{
		/// <summary>
		/// Private field to support <see cref="Sql"/> property.
		/// </summary>
		private string _sql;

        internal List<EntityField> ReferencedFields { get; private set; }
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal FreeFilterConstraint(SqlStatementCreator creator, string sql, IEnumerable<EntityField> referencedFields) : base(creator)
		{
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(sql.Length == 0)
				throw new ArgumentOutOfRangeException("'sql' is zero-length.");
			
			_sql = sql;
            this.ReferencedFields = new List<EntityField>(referencedFields);
		}

		/// <summary>
		/// Gets the sql.
		/// </summary>
		public string Sql
		{
			get
			{
				return _sql;
			}
		}

		public override void Append(FilterConstraintAppendContext context)
		{
			context.Sql.Append("(");
			context.Sql.Append(this.Sql);
			context.Sql.Append(")");
		}
	}
}
