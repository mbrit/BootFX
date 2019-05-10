// BootFX - Application framework for .NET applications
// 
// File: SqlStatementParameter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using BootFX.Common.Crypto;
using BootFX.Common.Entities;
using System.Collections;
using System.Collections.Generic;

namespace BootFX.Common.Data
{
	/// <summary>
	///	 Describes a parameter used in a <see cref="SqlStatement"></see> instance.
	/// </summary>
	[Serializable()]
	public class SqlStatementParameter
	{
		/// <summary>
		/// Private field to support <c>Direction</c> property.
		/// </summary>
		private ParameterDirection _direction;
		
		/// <summary>
		/// Private field to support <c>Name</c> property.
		/// </summary>
		private string _name;

		/// <summary>
		/// Private field to support <c>Value</c> property.
		/// </summary>
		private object _value;

		/// <summary>
		/// Private field to support <c>DbType</c> property.
		/// </summary>
		private DbType _dbType;

        public FilterConstraint OriginalConstraint { get; internal set; }

        private EntityField _relatedField = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatementParameter()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatementParameter(string name, DbType dbType, object value)
			: this(name, dbType, value, ParameterDirection.Input)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatementParameter(string name, DbType dbType, object value, ParameterDirection direction)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			if(value == null)
				throw new ArgumentNullException("value");

			this.Name = name;
			this.DBType = dbType;
			this.Value = value;
			this.Direction = direction;
		}

        public static SqlStatementParameter CreateArrayParameter(string name, DbType dbType, IEnumerable values)
        {
            if (dbType == DbType.Int32)
                return CreateArrayParameter(name, (IEnumerable<int>)values);
            else if (dbType == DbType.Int64)
                return CreateArrayParameter(name, (IEnumerable<long>)values);
            else if (dbType == DbType.String)
                return CreateArrayParameter(name, (IEnumerable<string>)values);
            else if(dbType == DbType.Guid)
                return CreateArrayParameter(name, (IEnumerable<Guid>)values);
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", dbType));
        }

        public static SqlStatementParameter CreateArrayParameter(string name, IEnumerable<int> values)
        {
            var table = TableifyArray(values);
            return new SqlStatementParameter(name, DbType.Int32, table);
        }

        public static SqlStatementParameter CreateArrayParameter(string name, IEnumerable<long> values)
        {
            var table = TableifyArray(values);
            return new SqlStatementParameter(name, DbType.Int64, table);
        }

        public static SqlStatementParameter CreateArrayParameter(string name, IEnumerable<string> values)
        {
            var table = TableifyArray(values);
            return new SqlStatementParameter(name, DbType.String, table);
        }

        public static SqlStatementParameter CreateArrayParameter(string name, IEnumerable<Guid> values)
        {
            var table = TableifyArray(values);
            return new SqlStatementParameter(name, DbType.String, table);
        }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public object Value
		{
			get
			{
				return _value;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");

				// mbr - 08-12-2005 - ok, this is where life becomes a bit of a problem because basically, 
				// there are only certain types that are supported.
				value = MangleValueForSet(value);

				// check to see if the value has changed...
				if(value != _value)
				{
					// set the value...
					_value = value;
				}
			}
		}

		private object MangleValueForSet(object value)
		{
			// null?
			if(value == null || value is DBNull)
				return value;

			// primitive?
			Type type = value.GetType();
			if(type.Equals(typeof(Guid)))
			{
				// mbr - 09-01-2006 - I don't know why this was like this...
//				return ((Guid)value).ToByteArray();
				return value;
			}

			TypeCode typeCode = Type.GetTypeCode(type);
			if(typeCode == TypeCode.Object)
			{
				// if it's an object, only certain types are supported...
				if(typeof(byte[]).IsAssignableFrom(type) || typeof(DataTable).IsAssignableFrom(type))
					return value;

                // jm - 29-06-2006 - encrypted?
                //if(value is EncryptedValue)
                //	return ((EncryptedValue)value).ToBase64String();

                // otherwise, nope...
                return value;
			}
			else
				return value;
		}

        public static DataTable TableifyArray(IEnumerable<int> values)
        {
            return TableifyArrayInternal(DbType.Int32, values);
        }

        public static DataTable TableifyArray(IEnumerable<long> values)
        {
            return TableifyArrayInternal(DbType.Int64, values);
        }

        public static DataTable TableifyArray(IEnumerable<string> values)
        {
            return TableifyArrayInternal(DbType.String, values);
        }

        public static DataTable TableifyArray(IEnumerable<Guid> values)
        {
            return TableifyArrayInternal(DbType.Guid, values);
        }

        internal static DataTable TableifyArrayInternal(DbType dbType, IEnumerable values)
        {
            var table = new DataTable();
            var column = table.Columns.Add("Value", ConversionHelper.GetClrTypeForDBType(dbType));
            foreach (var value in values)
                table.Rows.Add(value);

            return table;
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				if(value.Length == 0)
					throw ExceptionHelper.CreateZeroLengthArgumentException("value");

				// check to see if the value has changed...
				if(value != _name)
				{
					// set the value...
					_name = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the direction
		/// </summary>
		public ParameterDirection Direction
		{
			get
			{
				return _direction;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _direction)
				{
					// set the value...
					_direction = value;
				}
			}
		}

		/// <summary>
		/// Gets the dbtype.
		/// </summary>
		public DbType DBType
		{
			get
			{
				return _dbType;
			}
			set
			{
				if(value != _dbType)
					_dbType = value;
			}
		}

        public EntityField RelatedField
        {
            get
            {
                if (_relatedField != null)
                    return _relatedField;
                else if(this.OriginalConstraint is EntityFieldFilterConstraint)
                    return ((EntityFieldFilterConstraint)this.OriginalConstraint).Field;
                else
                    return null;
            }
            set
            {
                _relatedField = value;
            }
        }

        public bool IsStructured
        {
            get
            {
                return this.Value is DataTable || this.Value is StructuredParameterData;
            }
        }
	}
}
