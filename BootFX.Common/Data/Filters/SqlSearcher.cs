// BootFX - Application framework for .NET applications
// 
// File: SqlSearcher.cs
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
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Describes a class that can find entities that match a given specification.
	/// </summary>
	public class SqlSearcher : SqlStatementCreator
	{
		/// <summary>
		/// Private field to support <c>Constraints</c> property.
		/// </summary>
		private FilterConstraintCollection _constraints;
		
		/// <summary>
		/// Private field to support <c>Terms</c> property.
		/// </summary>
		private string[] _terms;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(Type type, string[] terms) : base(type)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(EntityType entityType, string[] terms) : base(entityType)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(Type type, string[] terms, EntityField[] fields) : base(type, fields)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(EntityType entityType, string[] terms, EntityField[] fields) : base(entityType, fields)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(Type type, string terms) : base(type)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(EntityType entityType, string terms) : base(entityType)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(Type type, string terms, EntityField[] fields) : base(type, fields)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlSearcher(EntityType entityType, string terms, EntityField[] fields) : base(entityType, fields)
		{
			this.Initialize(terms);
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		/// <param name="terms"></param>
		private void Initialize(string terms)
		{
			// defer...
			this.Initialize(SanitizeTerms(terms));
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		/// <param name="terms"></param>
		private void Initialize(string[] terms)
		{
			if(terms == null)
				throw new ArgumentNullException("terms");
			if(terms.Length == 0)
				throw new ArgumentOutOfRangeException("'terms' is zero-length.");
			_constraints = new FilterConstraintCollection(this);
			_terms = terms;
		}

		/// <summary>
		/// Sanitizes search terms.
		/// </summary>
		/// <param name="terms"></param>
		/// <returns></returns>
		private static string[] SanitizeTerms(string terms)
		{
			if(terms == null)
				return new string[] {};

			// \b\w*\b
			Regex regex = new Regex(@"\b\w*\b", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			MatchCollection matches = regex.Matches(terms);
			if(matches == null)
				throw new InvalidOperationException("matches is null.");

			// create...
			ArrayList results = new ArrayList();
			for(int index = 0; index < matches.Count; index++)
			{
				string value = matches[index].Value;
				if(value != null && value.Length > 0)
					results.Add(value);
			}

			// return...
			return (string[])results.ToArray(typeof(string));
		}

		/// <summary>
		/// Returns true if valid terms have been supplied.
		/// </summary>
		/// <param name="terms"></param>
		/// <returns></returns>
		public static bool AreTermsValid(string terms)
		{
			if(terms == null)
				throw new ArgumentNullException("terms");
			
			// get...
			return AreTermsValid(SanitizeTerms(terms));
		}

		/// <summary>
		/// Returns true if valid terms have been supplied.
		/// </summary>
		/// <param name="terms"></param>
		/// <returns></returns>
		public static bool AreTermsValid(string[] terms)
		{
			if(terms == null)
				throw new ArgumentNullException("terms");
			
			// check...
			if(terms.Length > 0)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the terms.
		/// </summary>
		private string[] Terms
		{
			get
			{
				// returns the value...
				return _terms;
			}
		}

		/// <summary>
		/// Adds constraints to the search.
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		protected internal override void AppendConstraints(SqlStatement statement, StringBuilder builder)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(builder == null)
				throw new ArgumentNullException("builder");

			// the sql we want is something like this...
			// (firstname like @term0 or firstname like @term1) or (lastname like @term0 or lastname like @term2)

			// check...
			if(Dialect == null)
				throw new InvalidOperationException("Dialect is null.");

			if(Terms == null)
				throw new InvalidOperationException("'Terms' is null.");
			if(Terms.Length == 0)
				throw new InvalidOperationException("'Terms' is zero-length.");

			// create parameters...
			for(int index = 0; index < this.Terms.Length; index++)
			{
				// term...
				string term = this.Dialect.AllWildcard + this.Terms[index] + this.Dialect.AllWildcard;

				// create...
				SqlStatementParameter parameter = new SqlStatementParameter("term" + index.ToString(), DbType.String, term);
				statement.Parameters.Add(parameter);
			}

			// mbr - 13-10-2005 - the creator now does this...			
//			// where...
//			builder.Append(" ");
//			builder.Append(this.Dialect.WhereKeyword);
//			builder.Append(" ");

			// walk each field in the type...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			bool first = true;
			builder.Append("(");
			foreach(EntityField field in this.EntityType.Fields)
			{
				if(this.IsCandidate(field))
				{
					// first?
					if(first == true)
						first = false;
					else
					{
						builder.Append(" ");
						builder.Append(this.Dialect.OrKeyword);
						builder.Append(" ");
					}

					// create...
					builder.Append("(");
					for(int index = 0; index < this.Terms.Length; index++)
					{
						if(index > 0)
						{
							builder.Append(" ");
							builder.Append(this.Dialect.OrKeyword);
							builder.Append(" ");
						}

						// add...
						builder.Append(this.Dialect.FormatNativeName(field.NativeName));
						builder.Append(" ");
						builder.Append(this.Dialect.LikeKeyword);
						builder.Append(" ");
						builder.Append(this.Dialect.FormatVariableNameForQueryText(statement.Parameters[index].Name));
					}
					builder.Append(")");
				}
			}
			builder.Append(")");

			// did we add any?
			if(first)
				throw new InvalidOperationException(string.Format(string.Format("No candidate fields were found on '{0}'.", this.EntityType)));

			// extra constraints...
			if(this.Constraints.Count > 0)
			{
				// create a context...
				FilterConstraintAppendContext context = new FilterConstraintAppendContext(this, statement, 0);

				// walk...
				foreach(FilterConstraint constraint in this.Constraints)
				{
					// add...
					context.ResetBuilder();
					constraint.Append(context);

					// join...
					builder.Append(" and ");
					builder.Append(context.Sql.ToString());
				}
			}
		}

		/// <summary>
		/// Returns true if the given field is a candidate.  
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>If the field is a string, and not large, then it's a candidate.</remarks>
		public bool IsCandidate(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// ext...
			if(field.IsExtendedProperty)
				return false;
			else
			{
				// check...
				switch(field.DBType)
				{
					case DbType.String:
					case DbType.StringFixedLength:
					case DbType.AnsiString:
					case DbType.AnsiStringFixedLength:

						if(field.IsLarge() == false)
							return true;
						else
							return false;

					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Gets a collection of FilterConstraint objects.
		/// </summary>
		public FilterConstraintCollection Constraints
		{
			get
			{
				return _constraints;
			}
		}
	}
}
