// BootFX - Application framework for .NET applications
// 
// File: SqlDeleteWorkUnit.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Runtime.Serialization;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a work unit that performs a SQL <c>DELETE</c> statement.
	/// </summary>
	[Serializable()]
	public class SqlDeleteWorkUnit : WorkUnit
	{
        //private SqlStatement[] _statements = null;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <param name="values"></param>
		public SqlDeleteWorkUnit(EntityType entityType, object entity, EntityField[] fields, object[] values) : base(entityType, entity, fields, values)
		{
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SqlDeleteWorkUnit(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Creates an Delete statement.
		/// </summary>
		/// <returns></returns>
		public override SqlStatement[] GetStatements(WorkUnitProcessingContext context)
		{
            // check...
            if (EntityType == null)
                throw new ArgumentNullException("EntityType");

            // create...
            SqlStatement statement = new SqlStatement(this.EntityType, this.Dialect);
            statement.OriginalWorkUnit = this;

            // sql...
            StringBuilder builder = new StringBuilder();
            builder.Append(statement.Dialect.DeleteFromKeyword);
            builder.Append(" ");
            builder.Append(statement.Dialect.FormatNativeName(this.EntityType.NativeName));
            builder.Append(" ");
            builder.Append(statement.Dialect.WhereKeyword);
            builder.Append(" ");

            // mbr - 13-10-2005 - rejigged to support partitioning...
            StringBuilder constraints = new StringBuilder();

            // key...
            EntityField[] fields = this.GetKeyFields();
            if (fields.Length == 0)
                throw new InvalidOperationException("Key fields are zero length.");

            // walk...
            for (int index = 0; index < fields.Length; index++)
            {
                // param...
                SqlStatementParameter parameter = this.CreateParameterForField(fields[index]);
                statement.Parameters.Add(parameter);

                // add...
                if (index > 0)
                {
                    constraints.Append(" ");
                    constraints.Append(statement.Dialect.AndKeyword);
                    constraints.Append(" ");
                }
                constraints.Append(statement.Dialect.FormatNativeName(fields[index].NativeName));
                constraints.Append("=");
                constraints.Append(statement.Dialect.FormatVariableNameForQueryText(parameter.Name));
            }

            // mbr - 13-10-2005 - rejigged to handle partitioning...
            string useConstraints = constraints.ToString();
            //if (this.EntityType.SupportsPartitioning)
            //{
            //    // get the strategy....
            //    PartitioningStrategy strategy = this.EntityType.PartitioningStrategy;
            //    if (strategy == null)
            //        throw new InvalidOperationException("strategy is null.");

            //    // mbr - 04-09-2007 - for c7 - can skip...
            //    if (strategy.ConstrianDeleteQuery)
            //    {
            //        // get the partition SQL...  (yes, this is for read, not for write.  for write really means 'for insert'.)
            //        // mbr - 04-09-2007 - for c7 - removed 'forReading'.
            //        //				useConstraints = strategy.RebuildConstraints(statement, useConstraints, true);
            //        useConstraints = strategy.RebuildConstraints(statement, useConstraints);

            //        // we have to get something back...
            //        if (useConstraints == null)
            //            throw new InvalidOperationException("'useConstraints' is null.");

            //        // mbr - 04-09-2007 - for c7 - zero length can be ok...				
            //        if (useConstraints.Length == 0 && !(strategy.IsZeroLengthIdSetOk))
            //            throw new InvalidOperationException("'useConstraints' is zero-length.");
            //    }
            //}

            // append...
            builder.Append(useConstraints);

            // return...
            statement.CommandText = builder.ToString();
            return new SqlStatement[] { statement };
		}

		public override WorkUnitType WorkUnitType
		{
			get
			{
				return WorkUnitType.Delete;
			}
		}
	}
}
