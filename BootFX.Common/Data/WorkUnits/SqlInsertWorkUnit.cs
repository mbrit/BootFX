// BootFX - Application framework for .NET applications
// 
// File: SqlInsertWorkUnit.cs
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
using System.Runtime.Serialization;
using System.Collections;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a work unit that performs a SQL <c>INSERT</c> statement.
	/// </summary>
	[Serializable()]
	public class SqlInsertWorkUnit : WorkUnit
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlInsertWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values) : base(entityType, entity, fields, values)
		{
		}
		
		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SqlInsertWorkUnit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Appends the insert portion to the query.
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		private void AppendInsertPortion(SqlStatement statement, StringBuilder builder)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(builder == null)
				throw new ArgumentNullException("builder");

			// create a block...
			TouchedValueCollection block = this.AddTouchedValueBlock();
            EntityField[] autoIncrementFields = GetAutoIncrementFields();
            if (autoIncrementFields.Length > 0 && statement.Dialect.LastInsertedIdMode == LastInsertedIdMode.Parameter)
                statement.Parameters.Add(statement.Dialect.GetLastInsertedIdParameter());

			// append...
			builder.Append(statement.Dialect.InsertIntoKeyword);
			builder.Append(" ");
			builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeName));
			builder.Append(" (");
			EntityField[] fields = this.GetNonAutoIncrementFields();
			for(int index = 0; index < fields.Length; index++)
			{
				if(index > 0)
					builder.Append(statement.Dialect.IdentifierSeparator);
				builder.Append(statement.Dialect.FormatNativeName(fields[index].NativeName));
			}
			builder.Append(") ");
			builder.Append(statement.Dialect.ValuesKeyword);
			builder.Append(" (");
			for(int index = 0; index < fields.Length; index++)
			{
				// create the param...
				SqlStatementParameter param = this.CreateParameterForField(fields[index]);
				statement.Parameters.Add(param);
                param.RelatedField = fields[index];

				// add...
				if(index > 0)
					builder.Append(statement.Dialect.IdentifierSeparator);
				builder.Append(statement.Dialect.FormatVariableNameForQueryText(param.Name));

				// mbr - 2007-04-02 - touched value...
				block.Add(new TouchedValue(fields[index], param.Value));
			}
			builder.Append(")");
            if (autoIncrementFields.Length > 0 && statement.Dialect.LastInsertedIdMode == LastInsertedIdMode.Parameter)
                builder.Append(statement.Dialect.AppendLastInsertedIdParameterAccess(GetAutoIncrementFields())); 
            builder.Append(statement.Dialect.StatementSeparator);
		}

		private void AppendSelectIdPortion(SqlStatement statement, StringBuilder builder)
		{
			builder.Append("select ");
			builder.Append(statement.Dialect.LastInsertedIdVariableName);
			builder.Append(statement.Dialect.StatementSeparator);
		}

		/// <summary>
		/// Creates a statement for this work unit.
		/// </summary>
		/// <param name="statements"></param>
		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
            if (context == null)
                throw new ArgumentNullException("context");

            //// check...
            //if (context.Connection == null)
            //    throw new InvalidOperationException("context.Connection is null.");
            //switch (context.Connection.SqlMode)
            //{
            //    case SqlMode.AdHoc:
                    return new SqlStatement[] { GetAdhocStatement(context) };

                // mbr - 10-10-2007 - removed.					
                //				case SqlMode.Sprocs:
                //					return new SqlStatement[] { GetSprocStatement(context) };

            //    default:
            //        throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", context.Connection.SqlMode, context.Connection.SqlMode.GetType()));
            //}
		}

		// mbr - 10-10-2007 - removed.		
//		private SqlStatement GetSprocStatement(WorkUnitProcessingContext context)
//		{
//			// check...
//			if(EntityType == null)
//				throw new ArgumentNullException("EntityType");
//			if(Dialect == null)
//				throw new InvalidOperationException("Dialect is null.");
//
//			// create a statement...
//			SqlStatement statement = new SqlStatement(this.EntityType, this.Dialect);
//
//			// command text...
//			statement.CommandText = this.Dialect.FormatSprocName(SqlTable.SprocPrefix + "I" + this.EntityType.NativeName.Name);
//			statement.CommandType = CommandType.StoredProcedure;
//
//			// setup the parameters...
//			foreach(EntityField field in this.EntityType.Fields)
//			{
//				// create...
//				SqlStatementParameter parameter = this.CreateParameterForField(field, false, OnNotFound.ReturnNull);
//				statement.Parameters.Add(parameter);
//
//				// auto?
//				// mbr - 04-07-2007 - changed.				
////				if(field.IsAutoIncrement())
//				if(field.IsAutoIncrement)
//					parameter.Direction = ParameterDirection.Output;
//			}
//
//			// return...
//			return statement;
//		}

		private SqlStatement GetAdhocStatement(WorkUnitProcessingContext context)
		{
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(Dialect == null)
				throw new InvalidOperationException("Dialect is null.");

			// create...
			SqlStatement statement = new SqlStatement(this.EntityType, this.Dialect);
            statement.OriginalWorkUnit = this;

			// command text...
			StringBuilder builder = new StringBuilder();
			this.AppendInsertPortion(statement, builder);
			if(context.Connection.SupportsMultiStatementQueries && context.Dialect.LastInsertedIdMode == LastInsertedIdMode.Scalar)
				this.AppendSelectIdPortion(statement, builder);

			// set...
			statement.CommandText = builder.ToString();

			// return...
			return statement;
		}

		/// <summary>
		/// Creates a statement for this work unit.
		/// </summary>
		/// <param name="statements"></param>
		private SqlStatement CreateSelectIdStatement(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");			

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// create...
			if(context.Connection == null)
				throw new InvalidOperationException("context.Connection is null.");
			if(context.Connection.Dialect == null)
				throw new InvalidOperationException("context.Connection.Dialect is null.");
			SqlStatement statement = new SqlStatement(this.EntityType, context.Connection.Dialect);

			// builder...
			StringBuilder builder = new StringBuilder();
			this.AppendSelectIdPortion(statement, builder);

			// set...
			statement.CommandText = builder.ToString();

			// return...
			return statement;
		}

		/// <summary>
		/// Creates a statement for this work unit.
		/// </summary>
		/// <param name="statements"></param>
		private SqlStatement CreateInsertStatement(WorkUnitProcessingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");			

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// create...
			if(context.Connection == null)
				throw new InvalidOperationException("context.Connection is null.");
			if(context.Connection.Dialect == null)
				throw new InvalidOperationException("context.Connection.Dialect is null.");
			SqlStatement statement = new SqlStatement(this.EntityType, context.Connection.Dialect);

			// builder...
			StringBuilder builder = new StringBuilder();
			this.AppendInsertPortion(statement, builder);

			// set...
			statement.CommandText = builder.ToString();

			// return...
			return statement;
		}

		/// <summary>
		/// Processes the work unit.
		/// </summary>
		/// <param name="context"></param>
		public override void Process(WorkUnitProcessingContext context, ITimingBucket timings)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(context.Connection == null)
				throw new InvalidOperationException("context.Connection is null.");

			// insert...
            SqlStatement[] statements = null;
            using (timings.GetTimer("GetStatements"))
            {
                statements = this.GetStatements(context);
                if (statements == null)
                    throw new InvalidOperationException("'statements' is null.");
                if (statements.Length == 0)
                    throw new InvalidOperationException("'statements' is zero-length.");
            }

			// get...
			SqlStatement statement = statements[0];
			if(statement == null)
				throw new InvalidOperationException("statement is null.");
				
			// run it...
			object[] result = null;

			// mbr - 27-10-2005 - if we're sproc mode, get the result differently...
			if(context.Connection.SqlMode == SqlMode.AdHoc)
			{
				if(context.Connection.SupportsMultiStatementQueries)
				{
					// debug?
                    using(timings.GetTimer("SingletonExecute"))
					    result = this.Execute(context, statement);
				}
				else
				{
                    using (timings.GetTimer("MultipleExecutes"))
                    {
                        // run...
                        context.Connection.ExecuteNonQuery(statement);

                        // id...
                        if (context.Dialect.LastInsertedIdMode == LastInsertedIdMode.Scalar)
                        {
                            statement = this.CreateSelectIdStatement(context);
                            if (statement == null)
                                throw new InvalidOperationException("statement is null.");

                            // run...
                            result = new object[] { context.Connection.ExecuteScalar(statement) };
                        }
                        else if (context.Dialect.LastInsertedIdMode == LastInsertedIdMode.Parameter)
                        {
                            EntityField[] autos = this.GetAutoIncrementFields();
                            if (autos == null)
                                throw new InvalidOperationException("'autos' is null.");
                            if (autos.Length == 0)
                                throw new InvalidOperationException("'autos' is zero-length.");

                            SqlStatementParameter parameter = statement.Parameters[context.Dialect.LastInsertedIdVariableName];
                            if (parameter == null)
                                throw new InvalidOperationException("parameter is null.");

                            // run...
                            result = new object[] { parameter.Value };
                        }
                        else
                            throw new NotSupportedException(string.Format("Cannot handle {0}.", context.Dialect.LastInsertedIdMode));
                    }
                }
			}
				// mbr - 10-10-2007 - removed.				
//			else if(context.Connection.SqlMode == SqlMode.Sprocs)
//			{
//				// run it...
//				context.Connection.ExecuteNonQuery(statement);
//
//				// get the return parameter...
//				EntityField[] autoFields = this.EntityType.GetAutoIncrementFields();
//				if(autoFields == null)
//					throw new InvalidOperationException("autoFields is null.");
//				if(autoFields.Length == 1)
//				{
//					// find it...
//					SqlStatementParameter parameter = statement.Parameters[autoFields[0].NativeName.Name];
//					if(parameter == null)
//						throw new InvalidOperationException("parameter is null.");
//
//					// return...
//					result = new object[] { parameter.Value };
//				}
//				else if(autoFields.Length > 1)
//					throw new NotSupportedException(string.Format("Tables with '{0}' auto-increment fields are not supported.", autoFields.Length));
//			}
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", context.Connection.SqlMode));
 
			// set...
            using (timings.GetTimer("Passback"))
            {
                if (result != null)
                    context.Bag.LastCreatedId = result[0];
            }
		}

		/// <summary>
		/// Executes the statement;
		/// </summary>
		/// <param name="context"></param>
		/// <param name="statement"></param>
		/// <returns></returns>
		private object[] Execute(WorkUnitProcessingContext context, SqlStatement statement)
		{
			if(context == null)
				throw new ArgumentNullException("context");			
			if(statement == null)
				throw new ArgumentNullException("statement");
			
			// check...
			if(context.Connection == null)
				throw new InvalidOperationException("context.Connection is null.");

			// run it...
			return new object[] { context.Connection.ExecuteScalar(statement) };
		}

		public override WorkUnitType WorkUnitType
		{
			get
			{
				return WorkUnitType.Insert;
			}
		}
	}
}
