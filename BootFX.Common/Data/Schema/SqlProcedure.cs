// BootFX - Application framework for .NET applications
// 
// File: SqlProcedure.cs
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
using System.Data;
using System.Collections;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Defines an instance of <c>SqlSproc</c>.
	/// </summary>
	public class SqlProcedure : SqlMember, ISqlProgrammable
	{		
		/// <summary>
		/// Private field to support <c>Body</c> property.
		/// </summary>
		private string _body;
		
		/// <summary>
		/// Private field to support <c>Parameters</c> property.
		/// </summary>
		private SqlStatementParameterCollection _parameters = new SqlStatementParameterCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlProcedure(string nativeName) : this(nativeName, CodeDomHelper.SanitizeName(nativeName))
		{
			this.Initialize();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		private SqlProcedure(string nativeName, string name) : base(nativeName, name)
		{
			this.Initialize();
		}

		private void Initialize()
		{
			this.Generate = true;
		}

		/// <summary>
		/// Gets a collection of SqlStatementParameter objects.
		/// </summary>
		public SqlStatementParameterCollection Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Gets or sets the body
		/// </summary>
		public string Body
		{
			get
			{
				return _body;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _body)
				{
					// set the value...
					_body = value;
				}
			}
		}
	}
}
