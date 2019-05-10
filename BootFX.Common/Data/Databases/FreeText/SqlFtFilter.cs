// BootFX - Application framework for .NET applications
// 
// File: SqlFtFilter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of a class that can do free-text filtering.
	/// </summary>
	/// <remarks>This class is required as the default behaviour of free-text statements can be quite different from regular statements.</remarks>
	public class SqlFtFilter : SqlFilter
	{
		/// <summary>
		/// Private field to support <c>FreeTextFields</c> property.
		/// </summary>
		private EntityFieldCollection _freeTextFields;
		
		/// <summary>
		/// Private field to support <c>Terms</c> property.
		/// </summary>
		private string _terms;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		public SqlFtFilter(Type type) : base(type)
		{
			this.Initialize();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		public SqlFtFilter(EntityType type) : base(type)
		{
			this.Initialize();
		}

		private void Initialize()
		{
			_freeTextFields = new EntityFieldCollection(this.EntityType);
		}

		/// <summary>
		/// Gets or sets the terms
		/// </summary>
		public string Terms
		{
			get
			{
				return _terms;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _terms)
				{
					// set the value...
					_terms = value;
				}
			}
		}

		protected internal override void AppendConstraints(SqlStatement statement, StringBuilder builder)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(builder == null)
				throw new ArgumentNullException("builder");

			// check...
			if(Terms == null)
				throw new InvalidOperationException("'Terms' is null.");
			if(Terms.Length == 0)
				throw new InvalidOperationException("'Terms' is zero-length.");

			// do the base ones...
			StringBuilder existing = new StringBuilder();
			StringBuilder ft = new StringBuilder();
			base.AppendConstraints(statement, existing);
			
			// check...
			if(Dialect == null)
				throw new InvalidOperationException("Dialect is null.");

			// builder...
			if(this.FreeTextFields.Count == 0)
				throw new InvalidOperationException("No free-text fields were specified.");

			// param...
			string paramName = statement.Parameters.GetUniqueParameterName();
			if(paramName == null)
				throw new InvalidOperationException("'paramName' is null.");
			if(paramName.Length == 0)
				throw new InvalidOperationException("'paramName' is zero-length.");
			statement.Parameters.Add(new SqlStatementParameter(paramName, DbType.String, this.Terms));

			// walk...
			bool first = true;
			foreach(EntityField field in this.FreeTextFields)
			{
				// add...
				if(first)
					first = false;
				else
				{
					ft.Append(" ");
					ft.Append(this.Dialect.OrKeyword);
					ft.Append(" ");
				}

				// append...
				ft.Append("FREETEXT(");
				ft.Append(this.Dialect.FormatNativeName(field.NativeName));
				ft.Append(", ");
				ft.Append(this.Dialect.FormatVariableNameForQueryText(paramName));
				ft.Append(")");
			}

			// mangle the two constraints together...
			if(existing.Length == 0)
				builder.Append(ft);
			else
			{
				//return string.Format("({0}) {1} ({2})", useExisting, this.Dialect.AndKeyword, ft);
				builder.Append("(");
				builder.Append(existing);
				builder.Append(") ");
				builder.Append(this.Dialect.AndKeyword);
				builder.Append(" (");
				builder.Append(ft);
				builder.Append(")");
			}
		}

		/// <summary>
		/// Gets a collection of EntityField objects.
		/// </summary>
		public EntityFieldCollection FreeTextFields
		{
			get
			{
				return _freeTextFields;
			}
		}

		/// <summary>
		/// Adds all text fields to the to the fields to filter against.
		/// </summary>
		public void AddAllTextFields()
		{
			foreach(EntityField field in this.EntityType.Fields)
			{
				if(field.DBType == DbType.String || field.DBType == DbType.StringFixedLength ||
					field.DBType == DbType.AnsiString || field.DBType == DbType.AnsiStringFixedLength)
					this.Fields.Add(field);
			}
		}
	}
}
