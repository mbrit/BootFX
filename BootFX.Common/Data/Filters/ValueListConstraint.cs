// BootFX - Application framework for .NET applications
// 
// File: ValueListConstraint.cs
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
    public class ValueListConstraint : FilterConstraint
    {
        private EntityField Field { get; set; }
        private List<object> Values { get; set; }
        private bool UseFullyQualifiedNames { get; set; }

        internal ValueListConstraint(SqlStatementCreator creator, EntityField field, IEnumerable values, bool useFullyQualifiedNames)
            : base(creator)
        {
            this.Field = field;
            this.Values = new List<object>();
            foreach (var value in values)
                this.Values.Add(value);

            this.UseFullyQualifiedNames = useFullyQualifiedNames;
        }

        public override void Append(FilterConstraintAppendContext context)
        {
            var builder = context.Sql;

            // build...
            builder.Append(context.Creator.Dialect.FormatColumnNameForSelect(this.Field, this.UseFullyQualifiedNames));
            builder.Append(" in (");
            var first = true;
            foreach(var value in this.Values)
            {
                if(first)
                    first = false;
                else
                    builder.Append(",");

                builder.Append(context.Creator.Dialect.FormatVariableNameForQueryText(((SqlFilter)context.Creator).ExtraParameters.Add(Field.DBType, value)));
            }
            builder.Append(")");
        }
    }
}
