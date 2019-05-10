// BootFX - Application framework for .NET applications
// 
// File: SqlStatementSource.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that acts as a SQL statement source, and also implements common execute methods.
	/// </summary>
	public abstract class SqlStatementSource : Loggable, ISqlStatementSource
	{
		/// <summary>
		/// Private field to support <c>Dialect</c> property.
		/// </summary>
		private SqlDialect _dialect;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected SqlStatementSource()
		{
		}

		/// <summary>
		/// Gets the statement.
		/// </summary>
		/// <returns></returns>
		public abstract SqlStatement GetStatement();

		/// <summary>
		/// Runs a non-query statement.
		/// </summary>
		/// <returns></returns>
		// mbr - 24-07-2008 - added.		
		public int ExecuteNonQuery()
		{
			return Database.ExecuteNonQuery(this);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public virtual IList ExecuteEntityCollection()
		{
			return Database.ExecuteEntityCollection(this);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		/// <remarks>This method defers to the overload of this method passing <see cref="OnNotFound.ReturnNull"></see>.</remarks>
		public object ExecuteEntity()
		{
			// defer...
			return this.ExecuteEntity(OnNotFound.ReturnNull);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public virtual object ExecuteEntity(OnNotFound onNotFound)
		{
			return Database.ExecuteEntity(this, onNotFound);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public object ExecuteScalar()
		{
			return Database.ExecuteScalar(this);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public DataSet ExecuteDataSet()
		{
			return Database.ExecuteDataSet(this);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public object[] ExecuteValues()
		{
			return Database.ExecuteValues(this);
		}

        public IEnumerable<T> ExecuteValuesVertical<T>()
        {
            return Database.ExecuteValuesVertical<T>(this);
        }

        /// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public object[] ExecuteValuesVertical()
		{
			return Database.ExecuteValuesVertical(this);
		}

		/// <summary>
		/// Loads the entities for the filter.
		/// </summary>
		/// <returns></returns>
		public DataTable ExecuteDataTable()
		{
			return Database.ExecuteDataTable(this);
		}

		/// <summary>
		/// Gets or sets the dialect
		/// </summary>
		public SqlDialect Dialect
		{
			get
			{
				if(_dialect == null)
					_dialect = Database.DefaultDialect;
				return _dialect;
			}
			set
			{
				if(value == null)
					value = Database.DefaultDialect;

				// check to see if the value has changed...
				if(value != _dialect)
				{
					// set the value...
					_dialect = value;
				}
			}
		}

		public override string ToString()
		{
			return GetStatement().ToString();
		}
	}
}
