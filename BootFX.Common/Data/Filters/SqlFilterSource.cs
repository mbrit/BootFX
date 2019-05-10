// BootFX - Application framework for .NET applications
// 
// File: SqlFilterSource.cs
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

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>SqlFilterSource</c>.
	/// </summary>
	public abstract class SqlFilterSource : SqlStatementSource
	{
		protected SqlFilterSource()
		{
		}

		/// <summary>
		/// Gets the filter.
		/// </summary>
		/// <returns></returns>
		public abstract SqlFilter GetSqlFilter();

		/// <summary>
		/// Gets the statement.
		/// </summary>
		/// <returns></returns>
		public override sealed SqlStatement GetStatement()
		{
			SqlFilter filter = this.GetSqlFilter();
			if(filter == null)
				throw new InvalidOperationException("filter is null.");

			// return...
			return filter.GetStatement();
		}
	}
}
