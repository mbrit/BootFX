// BootFX - Application framework for .NET applications
// 
// File: SqlOperator.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Describes operators avaliable in relational databases.
	/// </summary>
	public enum SqlOperator
	{
		/// <summary>
		/// The two values must be equal, e.g. <c>foo = 27</c>.
		/// </summary>
		EqualTo = 0,

		/// <summary>
		/// The two values must not be equal, e.g. <c>foo = 27</c>.
		/// </summary>
		NotEqualTo = 1,

		/// <summary>
		/// The left value must start with the right value, e.g. <c>bar LIKE 'foo%'</c>.
		/// </summary>
		StartsWith = 2,

		/// <summary>
		/// The left value must not start with the right value, e.g. <c>bar NOT LIKE 'foo%'</c>.
		/// </summary>
		NotStartsWith = 3,

		/// <summary>
		/// The left value must end with the right value, e.g. <c>bar LIKE '%foo'</c>.
		/// </summary>
		/// <remarks>Use with caution as SQL Server cannot index for this type of query.</remarks>
		EndsWith = 4,

		/// <summary>
		/// The left value must not end with the right value, e.g. <c>bar NOT LIKE '%foo'</c>.
		/// </summary>
		/// <remarks>Use with caution as SQL Server cannot index for this type of query.</remarks>
		NotEndsWidth = 5,

		/// <summary>
		/// The right value must be contained within the right value, e.g. <c>bar LIKE '%foo%'</c>.
		/// </summary>
		/// <remarks>Use with caution as SQL Server cannot index for this type of query.</remarks>
		Contains = 6,

		/// <summary>
		/// The right value must be not contained within the right value, e.g. <c>bar NOT LIKE '%foo%'</c>.
		/// </summary>
		/// <remarks>Use with caution as SQL Server cannot index for this type of query.</remarks>
		NotContains = 7,

		/// <summary>
		/// The left value must be greater than the right value, e.g. <c>foo &gt; 27</c>.
		/// </summary>
		GreaterThan = 8,

		/// <summary>
		/// The left value must be greater than or equal to the right value, e.g. <c>foo &gt;= 27</c>.
		/// </summary>
		GreaterThanOrEqualTo = 9,

		/// <summary>
		/// The left value must be less than the right value, e.g. <c>foo &lt; 27</c>.
		/// </summary>
		LessThan = 10,

		/// <summary>
		/// The left value must be less than or equal to the right value, e.g. <c>foo &lt;= 27</c>.
		/// </summary>
		LessThanOrEqualTo = 11,

		/// <summary>
		/// Describes a between query, .e.g <c>foo BETWEEN 10 AND 20</c>
		/// </summary>
		Between = 12,

		/// <summary>
		/// Describes a not-between query, .e.g <c>foo NOT BETWEEN 10 AND 20</c>
		/// </summary>
		NotBetween = 13,

		/// <summary>
		/// Describes a bitwise and, .e.g <c>(CAST(foo as int) and 16) = 16</c>
		/// </summary>
		BitwiseAnd = 14
	}
}
