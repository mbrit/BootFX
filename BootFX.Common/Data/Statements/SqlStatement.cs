// BootFX - Application framework for .NET applications
// 
// File: SqlStatement.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;
using System.Security;
using System.IO;
using System.Xml;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Represents a SQL statement.
	/// </summary>
	// mbr - 15-06-2006 - no longer sealed.	
	[Serializable()]
	public class SqlStatement : ISqlStatementSource, ISerializable, ISqlStatementArrayBuilder
	{
        /// <summary>
        /// Private value to support the <see cref="TimeoutSeconds">TimeoutSeconds</see> property.
        /// </summary>
        private int _timeoutSeconds;

        ///// <summary>
        ///// Private field to support <c>Md5</c> property.
        ///// </summary>
        //private static MD5CryptoServiceProvider _md5 = new MD5CryptoServiceProvider();
		
		/// <summary>
		/// Private field to support <c>OriginalCreator</c> property.
		/// </summary>
		[NonSerialized()]
		private SqlStatementCreator _originalCreator = null;
		
		/// <summary>
		/// Private field to support <c>DatabaseName</c> property.
		/// </summary>
		private string _databaseName = null;
		
		/// <summary>
		/// Private field to support <c>Dialect</c> property.
		/// </summary>
		[NonSerialized()]
		private SqlDialect _dialect = null;

		/// <summary>
		/// Private field to support <c>Parameters</c> property.
		/// </summary>
		private SqlStatementParameterCollection _parameters = new SqlStatementParameterCollection();
		
		/// <summary>
		/// Private field to support <c>CommandText</c> property.
		/// </summary>
		private string _commandText = string.Empty;

		/// <summary>
		/// Private field to support <c>CommandType</c> property.
		/// </summary>
		private CommandType _commandType = CommandType.Text;
		
		/// <summary>
		/// Private field to support <c>SelectMap</c> property.
		/// </summary>
		private SelectMap _selectMap;

        public WorkUnit OriginalWorkUnit { get; internal set; }

        public string Tag { get; set; }

        public ArrayParameterType ArrayParameterType { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement()
		{
			this.Dialect = Database.DefaultDialect;
            this.Initialize();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement(SqlDialect dialect)
		{
			this.Dialect = dialect;
            this.Initialize();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public SqlStatement(EntityType entityType) 
            : this(entityType, entityType.Dialect)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement(EntityType entityType, SqlDialect dialect)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			this.SetEntityType(entityType, dialect);
            this.Initialize();
        }

        private void Initialize()
        {
            this.ArrayParameterType = Database.ArrayParameterType;
        }

        /// <summary>
        /// Gets data for serialization.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        private SqlStatement(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// get...
			Type dialectType = (Type)info.GetValue("_dialectType", typeof(Type));
			if(dialectType != null)
				_dialect = (SqlDialect)Activator.CreateInstance(dialectType);

			// get...
			_databaseName = info.GetString("_databaseName");
			_parameters = (SqlStatementParameterCollection)info.GetValue("_parameters", typeof(SqlStatementParameterCollection));
			_commandText = info.GetString("_commandText");
			_commandType = (CommandType)info.GetValue("_commandType", typeof(CommandType));
			_selectMap = (SelectMap)info.GetValue("_selectMap", typeof(SelectMap));
            this.Tag = (string)info.GetValue("Tag", typeof(string));
            this.ArrayParameterType = (ArrayParameterType)info.GetValue("ArrayParameterType", typeof(ArrayParameterType));
		}

		private void QuickConfigureSelectMap(string[] names)
		{
			if(names == null)
				throw new ArgumentNullException("names");
			if(names.Length == 0)
				throw new ArgumentOutOfRangeException("'names' is zero-length.");
			
			// set...
			if(SelectMap == null)
				throw new InvalidOperationException("SelectMap is null.");
			if(this.SelectMap.MapFields.Count > 0)
				throw new InvalidOperationException("Select map has already been populated.");

			// check...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// setup...
			for(int index = 0; index < names.Length; index++)
			{
				this.SelectMap.MapFields.Add(new SelectMapField(this.EntityType.Fields.GetField(names[index], OnNotFound.ThrowException), 
					index));
			}
		}

		protected void SetEntityType(Type type, params string[] names)
		{
			this.SetEntityType(type);
			this.QuickConfigureSelectMap(names);
		}

		protected void SetEntityType(EntityType type, params string[] names)
		{
			this.SetEntityType(type);
			this.QuickConfigureSelectMap(names);
		}

		// mbr - 15-06-2006 - added.		
		protected void SetEntityType(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// set...
			EntityType et = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// defer...
			this.SetEntityType(et);
		}

		// mbr - 15-06-2006 - added.		
		protected void SetEntityType(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// defer...
			this.SetEntityType(entityType, entityType.Dialect);
		}

		// mbr - 15-06-2006 - added.		
		private void SetEntityType(EntityType entityType, SqlDialect dialect)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(dialect == null)
				throw new ArgumentNullException("dialect");
			
			// have we?
			if(_selectMap != null)
				throw new InvalidOperationException("The select map for this entity has already been defined and bound to an entity.");
			_selectMap = new SelectMap(entityType);
			this.Dialect = dialect;
			this.DatabaseName = entityType.DatabaseName;
		}
	
		/// <summary>
		/// Takes an array of string statements and converts them to a an  SqlStatement array
		/// </summary>
		/// <param name="statements"></param>
		/// <returns></returns>
		public static SqlStatement[] CreateStatements(string[] statements)
		{
			ArrayList sqlStatements = new ArrayList();
			foreach(string statement in statements)
				sqlStatements.Add(new SqlStatement(statement));

			return (SqlStatement[]) sqlStatements.ToArray(typeof (SqlStatement));
		}

        public bool HasEntityType
        {
            get
            {
                return this._selectMap != null && this.SelectMap.EntityType != null;
            }
        }

		public EntityType EntityType
		{
			get
			{
				if(SelectMap == null)
					throw new InvalidOperationException("SelectMap is null.");
				return this.SelectMap.EntityType;
			}
		}

		/// <summary>
		/// Gets the selectmap.
		/// </summary>
		public SelectMap SelectMap
		{
			get
			{
				// mbr - 15-06-2006 - added check.
				this.EnsureMapPopulated();
				return _selectMap;
			}
		}

		// mbr - 15-06-2006 - added.		
		internal void EnsureMapPopulated()
		{
			if(_selectMap == null)
			{
				// run...
				this.PopulateMap();

				// check again...
				if(_selectMap == null)
					throw new InvalidOperationException("The map was not populated even though 'PopulateMap' completed successfull.");
			}
		}

		/// <summary>
		/// Sets up the map for the statement.
		/// </summary>
		protected virtual void PopulateMap()
		{
			throw new NotSupportedException("Entities are not supported for this statement.");
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement(string commandText, SqlDialect dialect = null)
		{
			this.CommandText = commandText;

            if (dialect != null)
                this.Dialect = dialect;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement(string commandText, object[] parameters, SqlDialect dialect = null) 
            : this(commandText, dialect)
		{
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			
			this.InitializeParameters(parameters);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement(string commandText, CommandType commandType, SqlDialect dialect = null) 
            : this(commandText)
		{
			this.CommandType = commandType;

            if (dialect != null)
                this.Dialect = dialect;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatement(string commandText, CommandType commandType, object[] parameters, SqlDialect dialect = null) 
            : this(commandText, commandType, dialect)
		{
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			
			this.InitializeParameters(parameters);
		}

		/// <summary>
		/// Initializes the parameters.
		/// </summary>
		/// <param name="parameters"></param>
		private void InitializeParameters(object[] parameters)
		{
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			
			// use...
			object[] useParameters = new object[parameters.Length];

			// walk...
			for(int index = 0; index < parameters.Length; index++)
			{
				// convert it?
				if(parameters[index] is SqlStatementParameter)
					this.Parameters.Add((SqlStatementParameter)parameters[index]);
				else
				{
					DbType dbType = DbType.String;

					// mbr - 27-10-2005 - handled dbnull.
					useParameters[index] = parameters[index];
					if(useParameters[index] == null)
						useParameters[index] = DBNull.Value;

                    if (!(useParameters[index] is DBNull))
                    {
                        if (useParameters[index] is DataTable)
                            dbType = ConversionHelper.GetDBTypeForClrType(((DataTable)useParameters[index]).Columns[0].DataType);
                        else
                            dbType = ConversionHelper.GetDBTypeForClrType(useParameters[index].GetType());
                    }

					// mbr - 27-10-2005 - added debugging...
					string name = "p" + index.ToString(Cultures.System);
					try
					{
                        this.Parameters.Add(new SqlStatementParameter(name, dbType, useParameters[index]));
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format("Failed to initialize parameter '{0}'.", name), ex);
					}
				}
			}
		}

		/// <summary>
		/// Gets or sets the commandtype
		/// </summary>
		public CommandType CommandType
		{
			get
			{
				return _commandType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _commandType)
				{
					// set the value...
					_commandType = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the commandtext
		/// </summary>
		public string CommandText
		{
			get
			{
				return _commandText;
			}
			set
			{
				// check to see if the value has changed...
				if(value == null)
					value = string.Empty;
				if(value != _commandText)
				{
					// set the value...
					_commandText = value;
				}
			}
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

		public override string ToString()
		{
			return ToString(Cultures.System);
		}

		public string ToString(IFormatProvider formatProvider)
		{
			return string.Format(formatProvider, "{0}: {1} ({2})", base.ToString(), this.CommandText, this.CommandType);
		}

		/// <summary>
		/// Gets or sets the dialect
		/// </summary>
		public SqlDialect Dialect
		{
			get
			{
				if(_dialect == null)
					return Database.DefaultDialect;
				else
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

		/// <summary>
		/// Gets or sets the databasename
		/// </summary>
		public string DatabaseName
		{
			get
			{
				return _databaseName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _databaseName)
				{
					// set the value...
					_databaseName = value;
				}
			}
		}

		/// <summary>
		/// Gets the originalfilter.
		/// </summary>
		public SqlStatementCreator OriginalCreator
		{
			get
			{
				// returns the value...
				return _originalCreator;
			}
		}

		/// <summary>
		/// Gets the original filter.
		/// </summary>
		public SqlFilter OriginalFilter
		{
			get
			{
				// returns the value...
				return this.OriginalCreator as SqlFilter;
			}
		}

		/// <summary>
		/// Gets the original searcher.
		/// </summary>
		public SqlSearcher OriginalSearcher
		{
			get
			{
				// returns the value...
				return this.OriginalCreator as SqlSearcher;
			}
		}

		/// <summary>
		/// Sets the original filter.
		/// </summary>
		/// <param name="originalFilter"></param>
		internal void SetOriginalCreator(SqlStatementCreator originalCreator)
		{
			_originalCreator = originalCreator;
		}

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{			
			// save...
			Type dialectType = null;
			if(_dialect != null)
				dialectType = _dialect.GetType();
			info.AddValue("_dialectType", dialectType);

			// others...
			info.AddValue("_databaseName", _databaseName);
			info.AddValue("_parameters", _parameters);
			info.AddValue("_commandText", _commandText);
			info.AddValue("_commandType", _commandType);
			info.AddValue("_selectMap", _selectMap);
            info.AddValue("Tag", this.Tag);
            info.AddValue("ArrayParameterType", this.ArrayParameterType);
        }

        /// <summary>
        /// Gets the statement complete with parameters.
        /// </summary>
        /// <remarks>This method is for debugging and troubleshooting.</remarks>
        /// <returns></returns>
        public string GetDump()
        {
			StringBuilder builder = new StringBuilder();
			builder.Append(this.CommandText);
			builder.Append("\r\nType: ");
			builder.Append(this.CommandType);
			builder.Append("\r\nDatabase: ");
			if(this.DatabaseName == null || this.DatabaseName.Length == 0)
				builder.Append("(Default)");
			else
				builder.Append(this.DatabaseName);
			builder.Append("\r\n");
			if(this.Parameters.Count > 0)
			{
				builder.Append(this.Parameters.Count);
				builder.Append(" parameter(s):");
				foreach(SqlStatementParameter param in this.Parameters)
				{
					builder.Append("\r\n\t");
					builder.Append(param.Name);
					builder.Append(", value: ");
					DatabaseException.FormatParameterValueForDump(param.Value, builder, true);
					builder.Append(", ");
					builder.Append(param.DBType);
					builder.Append(", ");
					builder.Append(param.Direction);
				}
			}
			else
				builder.Append("0 parameters");

			// return...
			return builder.ToString();
		}

        public string GetTsqlSetupDump()
        {
            var builder = new StringBuilder();
            this.GetTsqlSetupDumpInternal(builder);
            return builder.ToString();
        }

        private void GetTsqlSetupDumpInternal(StringBuilder builder)
        {
            foreach (SqlStatementParameter param in this.Parameters)
            {
                builder.Append("declare ");
                builder.Append(this.Dialect.FormatVariableNameForQueryText(param.Name));
                builder.Append(" ");

                // select...
                if (!(param.IsStructured))
                {
                    long length = -1;
                    if (param.Value is string)
                        length = ((string)param.Value).Length;
                    builder.Append(this.Dialect.GetColumnTypeNativeName(param.DBType, false, length));
                    builder.Append("\r\n");

                    builder.Append("select ");
                    builder.Append(this.Dialect.FormatVariableNameForQueryText(param.Name));
                    builder.Append("=");
                    AppendParameterValueForTsqlDump(builder, param.Value);
                    builder.Append("\r\n");
                }
                else
                {
                    if (param.DBType == DbType.Int64)
                        builder.Append("__bfxIdsInt64");
                    else if (param.DBType == DbType.Int32)
                        builder.Append("__bfxIdsInt32");
                    else if (param.DBType == DbType.String || param.DBType == DbType.AnsiString ||
                        param.DBType == DbType.StringFixedLength || param.DBType == DbType.AnsiStringFixedLength)
                    {
                        builder.Append("__bfxIdsString");
                    }
                    else if (param.DBType == DbType.Guid)
                        builder.Append("__bfxIdsGuid");
                    else if (param.DBType == DbType.Object)
                    {
                        if (param.Value == null)
                            throw new InvalidOperationException("'param.Value' is null.");

                        if (param.Value is StructuredParameterData)
                        {
                            var data = (StructuredParameterData)param.Value;
                            builder.Append(data.NativeTypeName);
                        }
                        else
                            throw new NotSupportedException(string.Format("Cannot handle '{0}' for object parameter.", param.Value.GetType()));
                    }
                    else
                        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", param.DBType));
                    builder.Append("\r\n");

                    // sql server lets you do insert into foo (bar) values (1), (2), (3)...
                    if (this.Dialect.SupportsMultipleInsertValues)
                    {
                        // this can only handle 1000 parameters at a time...
                        if (param.Value is DataTable)
                        {
                            var rows = new List<DataRow>();
                            foreach (DataRow row in ((DataTable)param.Value).Rows)
                                rows.Add(row);

                            var pages = rows.SplitIntoPages(1000);
                            foreach (var page in pages)
                            {
                                builder.Append("insert into ");
                                builder.Append(this.Dialect.FormatVariableNameForQueryText(param.Name));
                                builder.Append(" (id) values ");
                                var first = true;
                                foreach (DataRow row in page)
                                {
                                    if (first)
                                        first = false;
                                    else
                                        builder.Append(", ");
                                    builder.Append("(");
                                    AppendParameterValueForTsqlDump(builder, row[0]);
                                    builder.Append(")");
                                }
                                builder.Append("\r\n");
                            }
                        }
                        else if (param.Value is StructuredParameterData)
                        {
                            var data = (StructuredParameterData)param.Value;

                            var rows = new List<DataRow>();
                            foreach (DataRow row in data.Data.Rows)
                                rows.Add(row);

                            var pages = rows.SplitIntoPages(1000);
                            foreach (var page in pages)
                            {
                                builder.Append("insert into ");
                                builder.Append(this.Dialect.FormatVariableNameForQueryText(param.Name));
                                builder.Append(" (");
                                var first = true;
                                foreach (DataColumn column in data.Data.Columns)
                                {
                                    if (first)
                                        first = false;
                                    else
                                        builder.Append(", ");
                                    builder.Append(this.Dialect.FormatColumnName(column.ColumnName));
                                }

                                builder.Append(") values ");
                                first = true;
                                foreach (DataRow row in page)
                                {
                                    if (first)
                                        first = false;
                                    else
                                        builder.Append(", ");

                                    builder.Append("(");
                                    var firstColumn = true;
                                    foreach (DataColumn column in data.Data.Columns)
                                    {
                                        if (firstColumn)
                                            firstColumn = false;
                                        else
                                            builder.Append(", ");
                                        AppendParameterValueForTsqlDump(builder, row[column]);
                                    }
                                    builder.Append(")");
                                }
                                builder.Append("\r\n");
                            }
                        }
                        else
                            throw new NotSupportedException(string.Format("Cannot handle '{0}'.", param.Value));
                    }
                    else
                    {
                        foreach (DataRow row in ((DataTable)param.Value).Rows)
                        {
                            builder.Append("insert into ");
                            builder.Append(this.Dialect.FormatVariableNameForQueryText(param.Name));
                            builder.Append(" (id) values (");
                            AppendParameterValueForTsqlDump(builder, row[0]);
                            builder.Append(")\r\n");
                        }
                    }
                }
            }
        }

        public static void AppendParameterValueForTsqlDump(StringBuilder builder, object value)
        {
            if (value == null || value is DBNull)
                builder.Append("null");
            else if (value is Boolean)
            {
                if ((bool)value)
                    builder.Append("1");
                else
                    builder.Append("0");
            }
            else if (value is string)
            {
                var asString = (string)value;
                builder.Append("'");
                builder.Append(asString.Replace("'", "''"));
                builder.Append("'");
            }
            else if (value is DateTime)
            {
                builder.Append("'");
                DateTime dt = (DateTime)value;
                builder.Append(dt.ToString("dd-MMM-yyyy HH:mm:ss"));
                builder.Append("'");
            }
            else if (value.GetType().IsEnum)
            {
                long enumValue = ConversionHelper.ToInt64(value, Cultures.System);
                builder.Append(enumValue);
            }
            else
                builder.Append(value);
        }

        public string GetTsqlDump()
        {
            StringBuilder builder = new StringBuilder();
            this.GetTsqlSetupDumpInternal(builder);

            // next...
            builder.Append(this.CommandText);

            // return...
            return builder.ToString();
        }

		/// <summary>
		/// Gets the hash of the statement.
		/// </summary>
		/// <returns></returns>
		public SqlStatementHash GetHash(bool includeParameterValues)
		{
			StringBuilder builder = new StringBuilder();

			// default...
            if (this.CommandType != System.Data.CommandType.Text)
            {
                builder.Append(this.CommandType);
            }

            // type...
			builder.Append("|");
			builder.Append(this.CommandText);

            // get...
            if (includeParameterValues)
            {
                var paramBuilder = new StringBuilder();
                for (int index = 0; index < this.Parameters.Count; index++)
                {
                    SqlStatementParameter parameter = this.Parameters[index];

                    // do we want parameter values? if we're caching results, we do, if we're caching the command, we do not...
                    if(index > 0)
                        paramBuilder.Append("|");
                    paramBuilder.Append(parameter.Value);
                }

                // erturn...
                return new SqlStatementHash(builder.ToString(), paramBuilder.ToString());
            }
            else
                return new SqlStatementHash(builder.ToString(), null);

			// hash it...
            //byte[] bs = Encoding.Unicode.GetBytes(builder.ToString());

            //// return...
            //if(Md5 == null)
            //    throw new InvalidOperationException("Md5 is null.");
            //byte[] hash = Md5.ComputeHash(bs);

            //// again...
            //builder = new StringBuilder();
            //for(int index = 0; index < hash.Length; index++)
            //    builder.Append(hash[index].ToString("x2"));

            //// return...
            //string result = builder.ToString();
            //return result;

            // mbr - 2014-11-30 - changed...
            //return HashHelper.GetMd5HashOfStringAsBase64(builder.ToString());
		}

        ///// <summary>
        ///// Gets the md5.
        ///// </summary>
        //private static MD5CryptoServiceProvider Md5
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _md5;
        //    }
        //}

        internal bool HasTimeout
        {
            get
            {
                if (this.TimeoutSeconds != 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Gets the TimeoutSeconds value.
        /// </summary>
        public int TimeoutSeconds
        {
            get
            {
                return _timeoutSeconds;
            }
            set
            {
                _timeoutSeconds = value;
            }
        }

        //public void AddValueList<T>(StringBuilder builder, IEnumerable<T> values)
        //{
        //    this.AddValueList(builder, values, ConversionHelper.GetDBTypeForClrType(typeof(T)));
        //}

        //private void AddValueList(StringBuilder builder, IEnumerable values, DbType dbType)
        //{
        //    if (this.ArrayParameterType == ArrayParameterType.Array)
        //        this.AddValueListTableType(builder, values, dbType);
        //    else if (this.ArrayParameterType == ArrayParameterType.BunchOfParameters)
        //        this.AddValueListLegacy(builder, values, dbType);
        //    else if (this.ArrayParameterType == ArrayParameterType.Xml)
        //        this.AddValueListXml(builder, values, dbType);
        //    else
        //        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", this.ArrayParameterType));
        //}

        //private void AddValueListXml(StringBuilder builder, IEnumerable values, DbType dbType)
        //{
        //    string xml = null;
        //    using (var writer = new StringWriter())
        //    {
        //        var xmlWriter = new XmlTextWriter(writer);
        //        xmlWriter.WriteStartElement("a");
        //        foreach(var value in values)
        //        {
        //            xmlWriter.WriteStartElement("b");
        //            xmlWriter.WriteValue(value);
        //            xmlWriter.WriteEndElement();
        //        }
        //        xmlWriter.WriteEndElement();

        //        xml = writer.ToString();
        //    }

        //    var name = this.Parameters.GetUniqueParameterName();
        //    var alias = "x_" + name;
        //    var p = new SqlStatementParameter(name, DbType.Xml, xml);
        //    this.Parameters.Add(p);

        //    //SELECT x.node.value('@value', 'int')
        //    //    FROM @xml.nodes('/values/value') as x(node)

        //    builder.Append("select ");
        //    builder.Append(alias);
        //    builder.Append(".node.value('./node()[1]', '");
        //    if (dbType == DbType.Int32)
        //        builder.Append("int");
        //    else if (dbType == DbType.String)
        //        builder.Append("nvarchar(max)");
        //    else if (dbType == DbType.Guid)
        //        builder.Append("uniqueidentider");
        //    else
        //        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", dbType));
        //    builder.Append("') id from ");
        //    builder.Append(this.Dialect.FormatVariableNameForQueryText(p.Name));
        //    builder.Append(".nodes('/a/b') as ");
        //    builder.Append(alias);
        //    builder.Append("(node)");
        //}

        //private void AddValueListTableType(StringBuilder builder, IEnumerable values, DbType dbType)
        //{
        //    var name = this.Parameters.GetUniqueParameterName();
        //    var p = SqlStatementParameter.CreateArrayParameter(name, dbType, values);

        //    throw new NotImplementedException("This operation has not been implemented.");
        //}

        //private void AddValueListLegacy(StringBuilder builder, IEnumerable values, DbType dbType)
        //{ 
        //    bool first = true;
        //    foreach (var value in values)
        //    {
        //        if (first)
        //            first = false;
        //        else
        //            builder.Append(", ");

        //        builder.Append(this.Dialect.FormatVariableNameForQueryText(this.Parameters.Add(dbType, value)));
        //    }
        //}

        public void AddParameter(StringBuilder builder, DbType dbType, object value)
        {
            var name = this.Parameters.Add(dbType, value);
            builder.Append(this.Dialect.FormatVariableNameForQueryText(name));
        }

        SqlStatement ISqlStatementSource.GetStatement()
        {
            return this;
        }

        public void AppendTableName<T>(StringBuilder builder)
            where T : Entity
        {
            builder.Append(this.Dialect.FormatTableName<T>());
        }

        public void AppendTableName<T>(StringBuilder builder, string name)
            where T : Entity
        {
            builder.Append(this.Dialect.FormatTableName(name));
        }

        public void AppendColumnName(StringBuilder builder, string name)
        {
            builder.Append(this.Dialect.FormatColumnName(name));
        }
    }
}
	
