// BootFX - Application framework for .NET applications
// 
// File: SprocStatementSource.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that is used to call stored procedures.
	/// </summary>
	// mbr - 24-07-2008 - added.	
	public abstract class SprocStatementSource : SqlStatementSource
	{
		/// <summary>
		/// Private value to support the <see cref="NativeName">NativeName</see> property.
		/// </summary>
		private string _nativeName;

		/// <summary>
		/// Private value to support the <see cref="Parameters">Parameters</see> property.
		/// </summary>
		private SqlStatementParameterCollection _parameters = new SqlStatementParameterCollection();

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nativeName"></param>
		protected SprocStatementSource(string nativeName)
		{
			if (nativeName == null)
				throw new ArgumentNullException("nativeName");
			if (nativeName.Length == 0)
				throw new ArgumentException("'nativeName' is zero-length.");

			// set...
			_nativeName = nativeName;
		}

		/// <summary>
		/// Gets the NativeName value.
		/// </summary>
		private string NativeName
		{
			get
			{
				return _nativeName;
			}
		}

		/// <summary>
		/// Gets the Parameters value.
		/// </summary>
		public SqlStatementParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		public override SqlStatement GetStatement()
		{
			// create...
			SqlStatement sql = new SqlStatement();
            
			// command...
			sql.CommandType = CommandType.StoredProcedure;
			sql.CommandText = this.NativeName;

			// parameters...
			sql.Parameters.AddRange(this.Parameters);

			// return...
			return sql;
		}
	}
}
