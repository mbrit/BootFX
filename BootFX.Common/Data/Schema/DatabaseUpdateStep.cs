// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateStep.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>DatabaseUpdateStep</c>.
	/// </summary>
	public abstract class DatabaseUpdateStep : Loggable, IEntityType
	{
		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private EntityType _entityType;
		
		protected DatabaseUpdateStep()
		{			
		}

		protected DatabaseUpdateStep(Type type) : this(EntityType.GetEntityType(type, OnNotFound.ThrowException))
		{			
		}

		protected DatabaseUpdateStep(EntityType entityType) : this()
		{			
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// Executes the step.
		/// </summary>
		/// <param name="context"></param>
        // mbr - 2009-07-21 - made public...
        //protected internal abstract void Execute(DatabaseUpdateContext context);
        public abstract void Execute(DatabaseUpdateContext context);

		public virtual void GetFriendlyWorkMessages(StringCollection messages)
		{
			if(messages == null)
				throw new ArgumentNullException("messages");

			if(!(this.IsUpToDate))
				messages.Add(this.ToString());
		}

		public virtual bool IsUpToDate
		{
			get
			{
				return false;
			}
		}

		protected bool IsTablePopulated(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// defer...
			return this.IsTablePopulated(new NativeName(name));
		}

		protected bool IsTablePopulated(NativeName name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			
			// check...
			if(Database.DoesTableExist(name))
			{
				// rows...
				SqlStatement sql = new SqlStatement();
				sql.CommandText = string.Format("select top 1 * from {0}", sql.Dialect.FormatTableName(name));

				// get...
				DataTable table = Database.ExecuteDataTable(sql);
				if(table == null)
					throw new InvalidOperationException("table is null.");

				// rows?
				if(table.Rows.Count > 0)
					return true;
				else
					return false;
			}
			else
				return false;
		}

		protected object AddRow(string name, string[] names, params object[] values)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// add...
			return this.AddRow(new NativeName(name), names, values);
		}

		protected object AddRow(NativeName name, string[] names, params object[] values)
		{
			if(names == null)
				throw new ArgumentNullException("names");
			if(names.Length == 0)
				throw new ArgumentOutOfRangeException("'names' is zero-length.");
			if(values == null)
				throw new ArgumentNullException("values");
			if(names.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'names' and 'values': {0} cf {1}.", names.Length, values.Length));

			// create...
			SqlStatement sql = new SqlStatement();

			// builder...
			StringBuilder builder = new StringBuilder();
			builder.Append("insert into ");
			builder.Append(sql.Dialect.FormatTableName(name));
			builder.Append(" (");
			for(int index = 0; index < names.Length; index++)
			{
				if(index > 0)
					builder.Append(", ");
				builder.Append(sql.Dialect.FormatColumnName(names[index]));
			}
			builder.Append(") values (");
			for(int index = 0; index < values.Length; index++)
			{
				if(index > 0)
					builder.Append(", ");

				// value...
				object value = values[index];
				if(value == null || (value is DateTime && (DateTime)value == DateTime.MinValue))
					value = DBNull.Value;

				// get it...
				DbType dbType = DbType.String;
				if(!(value is DBNull))
					dbType = ConversionHelper.GetDBTypeForClrType(value.GetType());

				// value...
				if(value is DBNull)
					builder.Append("null");
				else
				{
					// get and add...
					string paramName = sql.Parameters.GetUniqueParameterName();
					builder.Append(sql.Dialect.FormatVariableNameForQueryText(paramName));
					sql.Parameters.Add(paramName, dbType, value);
				}
			}
			builder.Append(")");
			builder.Append(sql.Dialect.StatementSeparator);

			// return...
			builder.Append("select ");
			builder.Append(sql.Dialect.LastInsertedIdVariableName);
			builder.Append(sql.Dialect.StatementSeparator);

			// run...
			sql.CommandText = builder.ToString();
			return Database.ExecuteScalar(sql);
		}
	}
}
