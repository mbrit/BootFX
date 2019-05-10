// BootFX - Application framework for .NET applications
// 
// File: SqlStatementCreator.cs
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
using System.Runtime.Serialization;
using System.Collections;
using BootFX.Common.Crypto;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using System.Collections.Generic;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Base class for classes that create SQL statements.
    /// </summary>
    [Serializable]
    public abstract class SqlStatementCreator : SqlStatementSource, IEntityType, IEntitySource, ISqlStatementArrayBuilder
    {
        private int _aliasNumber = 0;

        private bool FirstApplySortCall { get; set; }

        /// <summary>
        /// Private value to support the <see cref="Distinct">Distinct</see> property.
        /// </summary>
        private bool _distinct = false;
        private bool _forceSortOnDistinct = false;

        /// <summary>
		/// Private field to support <c>Joins</c> property.
		/// </summary>
		private SqlJoinCollection _joins;

        //		/// <summary>
        //		/// Private field to support <see cref="Aliases"/> property.
        //		/// </summary>
        //		private SqlAliasCollection _aliases;

        /// <summary>
        /// Private field to support <c>Top</c> property.
        /// </summary>
        private int _top;

        /// <summary>
        /// Private field to support <see cref="IgnorePartitioning"/> property.
        /// </summary>
        private bool _ignorePartitioning = false;

        /// <summary>
        /// Private field to support <c>ExtraParameters</c> property.
        /// </summary>
        private SqlStatementParameterCollection _extraParameters = new SqlStatementParameterCollection("q");

        /// <summary>
        /// Private field to support <see cref="EntityType"/> property.
        /// </summary>
        private EntityType _entityType;

        /// <summary>
        /// Private field to support <c>SortOrder</c> property.
        /// </summary>
        private SortSpecificationCollection _sortOrder = new SortSpecificationCollection();

        /// <summary>
        /// Private field to support <c>Fields</c> property.
        /// </summary>
        private EntityFieldCollection _fields = null;

        public string StatementHash { get; set; }

        public int TimeoutSeconds { get; set; }

        public ArrayParameterType ArrayParameterType { get; set; }

        public IndentMode IndentMode { get; set; }

        public string ForcedIndex { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected SqlStatementCreator(Type type)
            : this(type, new EntityField[] { })
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected SqlStatementCreator(Type type, EntityField[] fields)
            : this(EntityType.GetEntityType(type, OnNotFound.ThrowException), fields)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected SqlStatementCreator(EntityType entityType)
            : this(entityType, new EntityField[] { })
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected SqlStatementCreator(EntityType entityType, EntityField[] fields)
        {
            if (entityType == null)
                throw new ArgumentNullException("entityType");
            if (fields == null)
                throw new ArgumentNullException("fields");

            // set...
            _entityType = entityType;

            // mjr - 12-10-2005 - setup the dialect...
            if (entityType.HasDatabaseName)
            {
                // mjr - 12-10-2005 - this needs to be refactored (not ideal)...				
                using (IConnection connection = Database.CreateConnection(entityType.DatabaseName))
                    this.Dialect = connection.Dialect;
            }
            else
                Dialect = Database.DefaultDialect;

            // set...
            if (Dialect == null)
                throw new InvalidOperationException("Dialect is null.");

            // do the fields...
            _fields = new EntityFieldCollection(entityType);

            // mbr - 16-10-2005 - now done on demand...			
            //			if(fields.Length == 0)
            //			{
            //				// get default fields...
            //				fields = entityType.GetCommonFields();
            //				if(fields.Length == 0)
            //				{
            //					fields = entityType.GetKeyFields();
            //					if(fields.Length == 0)
            //						throw new InvalidOperationException(string.Format("Entity type '{0}' has no common fields and no key fields.", entityType));
            //				}
            //			}

            // add the ones passed into the constructor...
            _fields.AddRange(fields);

            // order...
            _sortOrder = entityType.DefaultSortOrder.Clone();
            this.FirstApplySortCall = true;

            this.ArrayParameterType = Database.ArrayParameterType;
        }

        /// <summary>
        /// Resolves the fields for select.
        /// </summary>
        /// <returns>The fields that will be used in the select.</returns>
        /// <remarks>By default, if the <see cref="Fields"></see> collection is empty, this will return the common fields, failing that, the key fields.  Otherwise, 
        /// it will return the fields contained in the <see cref="Fields"></see> collection.</remarks>
        protected EntityField[] ResolveFieldsForSelect()
        {
            // if we have no fields, guess that all of the text fields on the table are free-text searchable...
            EntityFieldCollection toUse = null;
            if (this.Fields.Count == 0)
            {
                toUse = new EntityFieldCollection(this.EntityType);
                this.GetDefaultFields(toUse);

                // check...
                if (toUse.Count == 0)
                    throw new InvalidOperationException("No fields were returned from GetDefaultFields.");
            }
            else
                toUse = this.Fields;

            // mbr - 10-10-2007 - case 875 - make sure we have the key fields from any joined tables...
            if (this.HasJoins)
            {
                foreach (SqlJoin join in this.Joins)
                {
                    if (join.IncludeInTreeFetch)
                    {
                        if (join.TargetEntityType == null)
                            throw new InvalidOperationException("join.TargetEntityType is null.");

                        this.MergeFields(toUse, join.TargetEntityType.GetKeyFields());
                    }
                }
            }

            // mbr - 2016-09-25 -- what about sort?
            foreach (SortSpecification sort in this.SortOrder)
            {
                if (sort.Field != null && sort.Field.EntityType == this.EntityType)
                    this.MergeFields(toUse, new EntityField[] { sort.Field });
            }

            // return...
            return toUse.ToArray();
        }

        /// <summary>
        /// Merges in fields.
        /// </summary>
        /// <param name="fields"></param>
        /// <param name="newFields"></param>
        // mbr - 10-10-2007 - case 875 - added.
        private void MergeFields(IList fields, EntityField[] newFields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");
            if (newFields == null)
                throw new ArgumentNullException("newFields");

            // walk...
            foreach (EntityField newField in newFields)
            {
                int index = fields.IndexOf(newField);
                if (index == -1)
                    fields.Add(newField);
            }
        }

        /// <summary>
        /// Gets the default fields.
        /// </summary>
        /// <returns></returns>
        protected virtual void GetDefaultFields(EntityFieldCollection fields)
        {
            if (fields == null)
                throw new ArgumentNullException("fields");

            // common...
            fields.AddRange(this.EntityType.GetCommonFields());
            if (fields.Count == 0)
            {
                fields.AddRange(this.EntityType.GetKeyFields());
                if (fields.Count == 0)
                    throw new InvalidOperationException(string.Format("Entity type '{0}' has no common fields and no key fields.", this.EntityType));
            }

            // mbr - 10-10-2007 - case 875 - add joined names...
            if (this.HasJoins)
            {
                foreach (SqlJoin join in this.Joins)
                {
                    // mbr - 2011-08-30 - include?
                    if (join.IncludeInTreeFetch)
                    {
                        if (join.TargetEntityType == null)
                            throw new InvalidOperationException("join.TargetEntityType is null.");

                        this.MergeFields(fields, join.TargetEntityType.GetCommonFields());
                    }
                }
            }
        }

        /// <summary>
        /// Creates a statement.
        /// </summary>
        /// <returns></returns>
        public override SqlStatement GetStatement()
        {
            if (EntityType == null)
                throw new ArgumentNullException("EntityType");

            // mbr - 16-10-2005 - changed so that inheriting classes can override default field inclusion behaviour...			
            EntityField[] fields = this.ResolveFieldsForSelect();
            if (fields == null)
                throw new ArgumentNullException("fields");
            if (fields.Length == 0)
                throw new InvalidOperationException("No fields selected.");

            // create a statement...
            if (Dialect == null)
                throw new ArgumentNullException("Dialect");
            SqlStatement statement = new SqlStatement(this.EntityType, this.Dialect);

            if (this.TimeoutSeconds > 0)
                statement.TimeoutSeconds = this.TimeoutSeconds;

            // set...
            statement.SetOriginalCreator(this);

            // check...
            if (statement.SelectMap == null)
                throw new ArgumentNullException("statement.SelectMap");

            // set up the basic select text...
            StringBuilder builder = new StringBuilder();
            builder.Append(statement.Dialect.SelectKeyword);

            // mbr - 2010-03-03 - added...
            if (this.Distinct)
            {
                builder.Append(" ");
                builder.Append(statement.Dialect.DistinctKeyword);
            }

            // top...
            if (this.Top > 0 && statement.Dialect.TopAtTop)
            {
                builder.Append(" ");
                builder.Append(statement.Dialect.FormatTop(this.Top));
            }

            // fields...
            builder.Append(" ");
            for (int index = 0; index < fields.Length; index++)
            {
                // add...
                if (index > 0)
                    builder.Append(statement.Dialect.IdentifierSeparator);

                // field...
                EntityField field = fields[index];
                if (field.IsExtendedProperty)
                {
                    // mbr - 25-09-2007 - provider...			
                    //					AppendExtendedPropertyStatement(statement, builder, field);
                    if (Database.ExtensibilityProvider == null)
                        throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
                    Database.ExtensibilityProvider.AddExtendedPropertyToSelectStatement(this, statement, builder, field);
                }
                else
                {
                    // mbr - 2008-09-11 - changed for MySQL support...
                    //					builder.Append(statement.Dialect.FormatColumnName(field, this.UseFullyQualifiedNames));
                    builder.Append(statement.Dialect.FormatColumnNameForSelect(field, this.UseFullyQualifiedNames));

                    // mbr - 10-10-2007 - case 875 - do nothing if we don't have a join...
                    if (field.EntityType == this.EntityType)
                        statement.SelectMap.MapFields.Add(new SelectMapField(field, index));
                    else
                    {
                        // find it!
                        SqlJoin found = null;
                        foreach (SqlJoin join in this.Joins)
                        {
                            if (join.TargetEntityType == field.EntityType)
                            {
                                found = join;
                                break;
                            }
                        }

                        // found?
                        if (found == null)
                            throw new InvalidOperationException(string.Format("Could not find a join for field '{0}'.", field));

                        // set...
                        statement.SelectMap.MapFields.Add(new SelectMapField(field, index, found));
                    }
                }
            }

            this.AppendIndent(builder);
            builder.Append(statement.Dialect.FromKeyword);
            builder.Append(" ");
            builder.Append(statement.Dialect.FormatTableName(this.EntityType.NativeName));

            if (this.HasForcedIndex)
            {
                builder.Append(" ");
                builder.Append(statement.Dialect.GetForceIndexDirective(this.ForcedIndex));
            }

            // mbr - 25-09-2007 - joins...
            this.AppendJoins(statement, builder);

            // get the contraints...
            StringBuilder constraints = new StringBuilder();
            this.AppendConstraints(statement, constraints);

            // trim...
            string useConstraints = constraints.ToString().Trim();

            //// mbr - 13-10-2005 - rejigged to handle partitioning...
            //// mbr - 08-03-2006 - added supports...			
            // mbr - 2014-11-30 - removes partitioning...
            //if(!(this.IgnorePartitioning) && this.EntityType.SupportsPartitioning)
            //{
            //    // get the strategy....
            //    PartitioningStrategy strategy = this.EntityType.PartitioningStrategy;
            //    if(strategy == null)
            //        throw new InvalidOperationException("strategy is null.");

            //    // get the partition SQL...
            //    // mbr - 04-09-2007 - c7 - removed 'forReading'.
            //    //				useConstraints = strategy.RebuildConstraints(statement, useConstraints, true);
            //    useConstraints = strategy.RebuildConstraints(statement, useConstraints);

            //    // we have to get something back...
            //    if(useConstraints == null)
            //        throw new InvalidOperationException("'useConstraints' is null.");

            //    // mbr - 04-09-2007 - for c7 - a zero-length string might be ok...
            //    if(useConstraints.Length == 0 && !(strategy.IsZeroLengthIdSetOk))
            //        throw new InvalidOperationException("'useConstraints' is zero-length.");
            //}

            // mbr - 2010-03-10 - added a method to allow us to change the statement before it is executed...
            if (Database.StatementCallback != null)
                useConstraints = Database.StatementCallback.ReplaceContraints(statement, this, useConstraints);

            // do we have constraints?
            if (useConstraints.Length > 0)
            {
                // just add the constraints...
                this.AppendIndent(builder);
                builder.Append(this.Dialect.WhereKeyword);
                builder.Append(" ");
                builder.Append(useConstraints);
            }

            // mbr - 2010-03-08 - if not distinct...
            ///if (this.SortOrder.Count > 0)
            //if ((!(this.Distinct) || this.ForceSortOnDistinct) && this.SortOrder.Count > 0)
            // mbr - 2016-08-17 -- no idea why this was like this...
            if (this.SortOrder.Count > 0)
            {
                // append...
                this.AppendIndent(builder);
                builder.Append(statement.Dialect.OrderByKeyword);
                builder.Append(" ");

                // add...
                for (int index = 0; index < this.SortOrder.Count; index++)
                {
                    if (index > 0)
                        builder.Append(statement.Dialect.IdentifierSeparator);

                    // spec...
                    SortSpecification specification = this.SortOrder[index];

                    // mbr - 10-10-2007 - case 875 - fixed to qualify...					
                    //					builder.Append(statement.Dialect.FormatNativeName(specification.Field.NativeName));

                    if (specification.Field != null)
                        builder.Append(statement.Dialect.FormatColumnName(specification.Field, this.UseFullyQualifiedNames));
                    else
                        builder.Append(specification.FreeSort);

                    // direction?
                    builder.Append(" ");
                    builder.Append(statement.Dialect.GetSortDirectionKeyword(specification.Direction));
                }
            }

            // top...
            if (this.Top > 0 && !(statement.Dialect.TopAtTop))
            {
                this.AppendIndent(builder);
                builder.Append(statement.Dialect.FormatTop(this.Top));
            }

            if (this.SignalRecompile)
                builder.Append(" OPTION(RECOMPILE)");

            // setup...
            statement.CommandText = builder.ToString();
            statement.CommandType = CommandType.Text;

            // extras...
            statement.Parameters.AddRange(this.ExtraParameters);

            // return...
            return statement;
        }

        // mbr - 25-09-2007 - moved to provider.		
        //		/// <summary>
        //		/// Append SQL to select the extended properties from the extended table for the entity
        //		/// </summary>
        //		/// <param name="statement"></param>
        //		/// <param name="builder"></param>
        //		/// <param name="field"></param>
        //		private void AppendExtendedPropertyStatement(SqlStatement statement, StringBuilder builder, EntityField field )
        //		{
        //			builder.Append("(");
        //			builder.Append(statement.Dialect.SelectKeyword);
        //			builder.Append(" ");
        //			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetColumnNameForDbType(field.DBType)));
        //			builder.Append(" ");
        //			builder.Append(statement.Dialect.FromKeyword);
        //			builder.Append(" ");
        //			// mbr - 08-12-2005 - changed...			
        ////			builder.Append(statement.Dialect.FormatNativeName(ExtendedPropertySettings.GetExtendedNativeNameForEntityType(EntityType)));
        //			builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeNameExtended));
        //			builder.Append(" ");
        //			builder.Append(statement.Dialect.WhereKeyword);
        //			builder.Append(" ");
        //
        //			// where...
        //			this.AppendConstraints(statement, builder);
        //
        //			// create the param...
        //			SqlStatementParameter parameter = new SqlStatementParameter(string.Format("Extended{0}", field.NativeName), DbType.String,
        //				field.NativeName.Name);
        //
        //			// param...
        //			builder.Append(" ");
        //			builder.Append(statement.Dialect.AndKeyword);
        //			builder.Append(" ");
        //			builder.Append(statement.Dialect.FormatNativeName(new NativeName("Name")));
        //			builder.Append("=");
        //			builder.Append(statement.Dialect.FormatVariableNameForQueryText(parameter.Name));
        //
        //			builder.Append(") AS ");
        //			// mbr - 08-12-2005 - added format...
        ////			builder.Append(field.NativeName);
        //			builder.Append(statement.Dialect.FormatNativeName(field.NativeName));
        //
        //			// add...
        //			this.ExtraParameters.Add(parameter);
        //		}

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
        /// Gets a collection of EntityField objects.
        /// </summary>
        public EntityFieldCollection Fields
        {
            get
            {
                return _fields;
            }
        }

        /// <summary>
        /// Gets a collection of SortSpecification objects.
        /// </summary>
        public SortSpecificationCollection SortOrder
        {
            get
            {
                return _sortOrder;
            }
        }

        /// <summary>
        /// Adds constraints to the statement.
        /// </summary>
        /// <param name="statement"></param>
        /// <param name="builder"></param>
        // mbr - 25-09-2007 - made protected internal		
        //		protected virtual void AppendConstraints(SqlStatement statement, StringBuilder builder)
        protected internal virtual void AppendConstraints(SqlStatement statement, StringBuilder builder)
        {
            // no-op...
        }

        /// <summary>
        /// Gets a collection of SqlBuilderParameter objects.
        /// </summary>
        public SqlStatementParameterCollection ExtraParameters
        {
            get
            {
                return _extraParameters;
            }
        }

        /// <summary>
        /// Creates a parameter for the value value.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal SqlStatementParameter CreateParameter(EntityField field, object value, string parameterName)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (value == null)
                value = DBNull.Value;

            // should the field be encrypted?
            //if(field.IsEncrypted && !(value is DBNull))
            //	value = EncryptedValue.Create(field.EncryptionKeyName, value);

            // name...
            return new SqlStatementParameter(parameterName, field.DBType, value);
        }

        /// <summary>
        /// Gets the ignorepartitioning.
        /// </summary>
        public bool IgnorePartitioning
        {
            get
            {
                return _ignorePartitioning;
            }
            set
            {
                _ignorePartitioning = value;
            }
        }

        /// <summary>
        /// Gets or sets the top
        /// </summary>
        public int Top
        {
            get
            {
                return _top;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("value", value, "Value must be greater than or equal to zero.");

                // check to see if the value has changed...
                if (value != _top)
                {
                    // set the value...
                    _top = value;
                }
            }
        }

        /// <summary>
        /// Execute the key fields for the filter.
        /// </summary>
        /// <remarks>This method is NOT thread-safe.</remarks>
        /// <returns></returns>
        // mbr - 21-09-2007 - added c7.		
        public object[][] ExecuteKeyValues()
        {
            return ExecuteKeyValues(null);
        }

        /// <summary>
        /// Executes key values for the filter.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        // mbr - 26-11-2007 - for c7 - created this overload that accepts a connection.
        internal object[][] ExecuteKeyValues(IConnection connection)
        {
            // check...
            if (EntityType == null)
                throw new InvalidOperationException("EntityType is null.");

            // before?
            EntityField[] before = null;
            if (this.Fields.Count > 0)
                before = this.Fields.ToArray();

            try
            {
                // replace the fields...
                this.Fields.Clear();
                this.Fields.AddRange(this.EntityType.GetKeyFields());
                if (this.Fields.Count == 0)
                    throw new InvalidOperationException(string.Format("'{0}' does not have any key fields.", this.EntityType));

                // sql...
                SqlStatement sql = this.GetStatement();
                if (sql == null)
                    throw new InvalidOperationException("sql is null.");

                // mbr - 26-11-2007 - connection...
                bool disposeConnectionOnFinish = false;
                if (connection == null)
                {
                    // get...
                    connection = Database.CreateConnection(sql);
                    if (connection == null)
                        throw new InvalidOperationException("connection is null.");

                    // flag...
                    disposeConnectionOnFinish = true;
                }

                // run it...
                try
                {
                    // are we the right type?
                    if (!(connection is Connection))
                        throw new NotSupportedException(string.Format("Cannot handle '{0}'.", connection.GetType()));

                    // make sure we can open it...
                    ((Connection)connection).EnsureNativeConnectionOpen();

                    // command...
                    var command = connection.CreateCommand(sql);
                    try
                    {
                        using (IDataReader reader = command.ExecuteReader())
                        {
                            // walk...
                            ArrayList results = new ArrayList();
                            while (reader.Read())
                            {
                                object[] keys = new object[this.Fields.Count];
                                reader.GetValues(keys);

                                // add...
                                results.Add(keys);
                            }

                            // return...
                            return (object[][])results.ToArray(typeof(object[]));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ((Connection)connection).CreateCommandException("Failed to execute key values.", command, ex, null);
                    }
                    //finally
                    //{
                    //    if(command != null)
                    //    {
                    //        command.Dispose();
                    //        command = null;
                    //    }
                    //}
                }
                finally
                {
                    // mbr - 26-11-2007 - do we need to kill off the connection?					
                    if (disposeConnectionOnFinish)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                }
            }
            finally
            {
                // put it all back togehter...
                this.Fields.Clear();
                if (before != null)
                    this.Fields.AddRange(before);
            }
        }

        /// <summary>
        /// Appends joins to the query.
        /// </summary>
        private void AppendJoins(SqlStatement statement, StringBuilder builder)
        {
            // mbr - 25-09-2007 - the only place this is used right now is in the extensibility...			
            if (Database.ExtensibilityProvider == null)
                throw new InvalidOperationException("Database.ExtensibilityProvider is null.");

            // mbr - 10-10-2007 - case 875 - deal with joined types too...			
            //			if(!(this.EntityType.HasExtendedProperties))
            if (!(this.EntityType.HasExtendedProperties) && !(this.HasJoins))
                return;

            // ok...
            Database.ExtensibilityProvider.AppendJoinsToSelectStatement(statement, builder);

            // mbr - 10-10-2007 - case 875 - joined types...
            if (this.HasJoins)
            {
                foreach (SqlJoin join in this.Joins)
                {
                    this.AppendIndent(builder);
                    join.AppendJoin(statement, builder);
                }
            }
        }

        public IEnumerable<SqlJoin> GetJoins()
        {
            var results = new List<SqlJoin>();
            foreach (SqlJoin join in this.Joins)
                results.Add(join);
            return results;
        }

        public void AppendIndent(StringBuilder builder)
        {
            if (this.IndentMode == IndentMode.None)
                builder.Append(" ");
            else if (this.IndentMode == IndentMode.Indented)
                builder.Append("\r\n");
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", this.IndentMode));
        }

        //		/// <summary>
        //		/// Gets the aliases.
        //		/// </summary>
        //		private SqlAliasCollection Aliases
        //		{
        //			get
        //			{
        //				if(_aliases == null)
        //					_aliases = new SqlAliasCollection();
        //				return _aliases;
        //			}
        //		}
        //
        //		/// <summary>
        //		/// Gets the alias for the given native name.
        //		/// </summary>
        //		/// <param name="name"></param>
        //		/// <returns></returns>
        //		public SqlAlias GetAlias(NativeName name)
        //		{
        //			if(name == null)
        //				throw new ArgumentNullException("name");
        //			
        //			// check...
        //			string toFind = name.Name;
        //			if(toFind == null)
        //				throw new InvalidOperationException("'toFind' is null.");
        //			if(toFind.Length == 0)
        //				throw new InvalidOperationException("'toFind' is zero-length.");
        //
        //			// find it...
        //			foreach(SqlAlias alias in this.Aliases)
        //			{
        //				if(string.Compare(alias.ResolvedName, toFind, true, Cultures.System) == 0)
        //					return alias;
        //			}
        //
        //			// create a new one...
        //			string aliasName = "a" + this.Aliases.Count.ToString();
        //			SqlAlias newAlias = new SqlAlias(aliasName, name);
        //			this.Aliases.Add(newAlias);
        //
        //			// return...
        //			return newAlias;
        //		}

        /// <summary>
        /// Gets a collection of EntityType objects that represent joined types.
        /// </summary>
        // mbr - 10-10-2007 - case 875 - added.		
        private SqlJoinCollection Joins
        {
            get
            {
                if (_joins == null)
                    _joins = new SqlJoinCollection();
                return _joins;
            }
        }

        /// <summary>
        /// Returns true if we have joined types.
        /// </summary>
        private bool HasJoins
        {
            get
            {
                if (_joins != null && _joins.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Returns true if we need to use fully-qualified names.
        /// </summary>
        public bool UseFullyQualifiedNames
        {
            get
            {
                return this.HasJoins;
            }
        }

        public void AddJoin<T>()
            where T : Entity
        {
            this.AddJoin(typeof(T));
        }

        /// <summary>
        /// Adds a join to the given type.  This type must represent a parent type of the creator's entity type.
        /// </summary>
        /// <param name="type"></param>
        public void AddJoin(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // get...
            EntityType et = EntityType.GetEntityType(type, OnNotFound.ThrowException);
            if (et == null)
                throw new InvalidOperationException("et is null.");

            // defer...
            this.AddJoin(et);
        }

        /// <summary>
        /// Adds a join to the given type.  This type must represent a parent type of the creator's entity type.
        /// </summary>
        /// <param name="type"></param>
        public void AddJoin(EntityType et)
        {
            this.AddJoin(et, JoinType.InnerJoin);
        }

        public void AddJoin(EntityType et, JoinType type)
        {
            if (et == null)
                throw new ArgumentNullException("et");

            // find the link...
            foreach (ChildToParentEntityLink link in this.EntityType.Links)
            {
                if (link.ParentEntityType == et)
                {
                    // mbr - 2011-08-30 - always set distinct when using joins...
                    this.Distinct = true;

                    // add...
                    this.Joins.Add(new SqlJoin(GetNextJoinAlias(), type, link));
                    return;
                }
            }

            // stop...
            throw new InvalidOperationException(string.Format("Entity type '{0}' is not a parent of '{1}'.", et, this.EntityType));
        }

        public SqlJoin AddJoin(EntityType et, EntityField sourceField, EntityField targetField)
        {
            if (sourceField == null)
                throw new ArgumentNullException("sourceField");
            if (targetField == null)
                throw new ArgumentNullException("targetField");

            return this.AddJoin(et, sourceField, targetField, JoinType.InnerJoin);
        }

        public SqlJoin AddJoin(EntityType et, EntityField sourceField, string sourceAlias, EntityField targetField, JoinType type = JoinType.InnerJoin)
        {
            if (sourceField == null)
                throw new ArgumentNullException("sourceField");
            if (targetField == null)
                throw new ArgumentNullException("targetField");

            this.Distinct = true;

            // create...
            SqlJoin join = new SqlJoin(GetNextJoinAlias(), type, et, new EntityField[] { sourceField }, new EntityField[] { targetField })
            {
                SourceAlias = sourceAlias
            };
            this.AddJoin(join);
            return join;
        }

        public SqlJoin AddJoin(EntityType et, EntityField sourceField, EntityField targetField, JoinType type)
        {
            if (sourceField == null)
                throw new ArgumentNullException("sourceField");
            if (targetField == null)
                throw new ArgumentNullException("targetField");

            return this.AddJoin(et, new EntityField[] { sourceField }, new EntityField[] { targetField }, type);
        }

        public SqlJoin AddJoin(EntityType et, EntityField[] sourceFields, EntityField[] targetFields)
        {
            return this.AddJoin(et, sourceFields, targetFields, JoinType.InnerJoin);
        }

        public SqlJoin AddJoin(EntityType et, EntityField[] sourceFields, EntityField[] targetFields, JoinType type)
        {
            // mbr - 2011-08-30 - always set distinct when using joins...
            this.Distinct = true;

            // create...
            SqlJoin join = new SqlJoin(GetNextJoinAlias(), type, et, sourceFields, targetFields);
            this.AddJoin(join);
            return join;
        }

        public void AddJoin(SqlJoin join)
        {
            if (join == null)
                throw new ArgumentNullException("join");

            // add...
            this.Joins.Add(join);
        }

        public string GetNextJoinAlias()
        {
            _aliasNumber++;
            return "t" + _aliasNumber;
        }

        /// <summary>
        /// Gets the Distinct value.
        /// </summary>
        public bool Distinct
        {
            get
            {
                return _distinct;
            }
            set
            {
                _distinct = value;
            }
        }

        public bool HasJoin<T>()
            where T : Entity
        {
            return this.HasJoin(typeof(T).ToEntityType());
        }


        public bool HasJoin(EntityType et)
        {
            foreach (SqlJoin join in this.Joins)
            {
                if (join.TargetEntityType == et)
                    return true;
            }

            // nope...
            return false;
        }

        public bool ForceSortOnDistinct
        {
            get
            {
                return _forceSortOnDistinct;
            }
            set
            {
                _forceSortOnDistinct = value;
            }
        }

        public void ApplyIdSort(SortDirection direction = SortDirection.Ascending)
        {
            this.ApplySort(this.EntityType.GetKeyFields()[0], direction);
        }

        public void ApplySort(string name, SortDirection direction = SortDirection.Ascending)
        {
            var field = this.EntityType.Fields.GetField(name);
            this.ApplySort(field, direction);
        }

        public void ApplyFreeSort(string freeSort, SortDirection direction = SortDirection.Ascending)
        {
            this.ApplySortInternal(new SortSpecification(freeSort, direction));
        }

        public void ApplySort(EntityField field, SortDirection direction = SortDirection.Ascending)
        {
            this.ApplySortInternal(new SortSpecification(field, direction));
        }
        
        private void ApplySortInternal(SortSpecification sort)
        { 
            if (this.FirstApplySortCall)
            {
                this.SortOrder.Clear();
                this.FirstApplySortCall = false;
            }

            this.SortOrder.Add(sort);
        }

        SqlStatementParameterCollection ISqlStatementArrayBuilder.Parameters
        {
            get
            {
                return this.ExtraParameters;
            }
        }

        public bool HasForcedIndex
        {
            get
            {
                return !(string.IsNullOrEmpty(this.ForcedIndex));
            }
        }

        public virtual bool SignalRecompile
        {
            get
            {
                return false;
            }
            set
            {
                throw new NotImplementedException("This operation has not been implemented.");
            }
        }
    }
}

