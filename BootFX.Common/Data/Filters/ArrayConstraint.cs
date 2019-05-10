// BootFX - Application framework for .NET applications
// 
// File: ArrayConstraint.cs
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
using System.Data;
using System.Linq;
using System.Text;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
    [Serializable]
    public class ArrayConstraint : FilterConstraint
    {
        internal EntityField Field { get; private set; }
        private List<object> Values { get; set; }
        private bool UseFullyQualifiedNames { get; set; }
        private SqlOperator Operator { get; set; }

        internal ArrayConstraint(SqlStatementCreator creator, EntityField field, IEnumerable values, bool useFullyQualifiedNames, SqlOperator op)
            : base(creator)
        {
            this.Field = field;
            this.Values = new List<object>();
            foreach (var value in values)
                this.Values.Add(value);

            this.UseFullyQualifiedNames = useFullyQualifiedNames;
            this.Operator = op;
        }

        public override void Append(FilterConstraintAppendContext context)
        {
            var builder = context.Sql;

            // create...
            var table = SqlStatementParameter.TableifyArrayInternal(this.Field.DBType, this.Values);

            // build...
            builder.Append(context.Creator.Dialect.FormatColumnNameForSelect(this.Field, this.UseFullyQualifiedNames));

            if (this.Operator == SqlOperator.EqualTo)
            {
                // no-op...
            }
            else if (this.Operator == SqlOperator.NotEqualTo)
                builder.Append("not ");
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", this.Operator));

            builder.Append(" in (select id from ");
            builder.Append(context.Creator.Dialect.FormatVariableNameForQueryText(((SqlFilter)context.Creator).ExtraParameters.Add(Field.DBType, table)));
            builder.Append(")");
        }
    }
}
