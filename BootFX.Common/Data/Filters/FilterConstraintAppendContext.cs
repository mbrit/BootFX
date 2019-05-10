// BootFX - Application framework for .NET applications
// 
// File: FilterConstraintAppendContext.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Describes the context used for appending constraints for use with a <see cref="SqlFilter"></see>.
	/// </summary>
	public class FilterConstraintAppendContext
	{
		/// <summary>
		/// Private field to support <c>Filter</c> property.
		/// </summary>
		private SqlStatementCreator _creator;
		
		/// <summary>
		/// Private field to support <c>ConstraintNumberSeed</c> property.
		/// </summary>
		private int _constraintNumberSeed;
		
		/// <summary>
		/// Private field to support <c>LastConstraint</c> property.
		/// </summary>
		private FilterConstraint _lastConstraint = null;
		
		/// <summary>
		/// Private field to support <c>ConstraintNumber</c> property.
		/// </summary>
		private int _constraintNumber = 0;
		
		/// <summary>
		/// Private field to support <c>Statement</c> property.
		/// </summary>
		private SqlStatement _statement;

		/// <summary>
		/// Private field to support <c>Sql</c> property.
		/// </summary>
		private StringBuilder _sql;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="sql"></param>
		public FilterConstraintAppendContext(SqlStatementCreator creator, SqlStatement statement, int constraintNumberSeed)
		{
			if(creator == null)
				throw new ArgumentNullException("creator");
			if(statement == null)
				throw new ArgumentNullException("statement");
			
			_creator = creator;
			_statement = statement;
			_constraintNumberSeed = constraintNumberSeed;

			// reset...
			this.ResetBuilder();
		}

		/// <summary>
		/// Gets the filter.
		/// </summary>
		public SqlStatementCreator Creator
		{
			get
			{
				return _creator;
			}
		}

		/// <summary>
		/// Gets the constraintnumberseed.
		/// </summary>
		public int ConstraintNumberSeed
		{
			get
			{
				return _constraintNumberSeed;
			}
		}

		/// <summary>
		/// Gets the sql.
		/// </summary>
		public StringBuilder Sql
		{
			get
			{
				return _sql;
			}
		}
		
		/// <summary>
		/// Gets the statement.
		/// </summary>
		public SqlStatement Statement
		{
			get
			{
				return _statement;
			}
		}

		/// <summary>
		/// Gets or sets the constraintnumber
		/// </summary>
		private int ConstraintNumber
		{
			get
			{
				return _constraintNumber;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _constraintNumber)
				{
					// set the value...
					_constraintNumber = value;
				}
			}
		}

		/// <summary>
		/// Creates the name for a constraint.
		/// </summary>
		/// <returns></returns>
		public string CreateConstraintName()
		{
			this.ConstraintNumber++;

            // mbr - 2014-11-27 - finally changeds this as it was bugging me...
            if (this.ConstraintNumberSeed == 0)
                return string.Format("w" + this.ConstraintNumber);
            else
                return string.Format(Cultures.System, "w{0}_{1}", this.ConstraintNumberSeed, this.ConstraintNumber);
		}

		/// <summary>
		/// Gets the lastconstraint.
		/// </summary>
		public FilterConstraint LastConstraint
		{
			get
			{
				return _lastConstraint;
			}
		}

		/// <summary>
		/// Sets the last constraint.
		/// </summary>
		/// <param name="constraint"></param>
		internal void SetLastConstraint(FilterConstraint lastConstraint)
		{
			_lastConstraint = lastConstraint;
		}

		/// <summary>
		/// Resets the string builder.
		/// </summary>
		internal void ResetBuilder()
		{
			_sql = new StringBuilder();
		}
	}
}
