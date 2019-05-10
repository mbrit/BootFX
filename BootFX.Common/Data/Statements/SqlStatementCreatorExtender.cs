// BootFX - Application framework for .NET applications
// 
// File: SqlStatementCreatorExtender.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BootFX.Common.Data
{
    public static class SqlStatementCreatorExtender
    {
        public static void AddValueList<T>(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable<T> values)
        {
            sql.AddValueList<T>(builder, values, sql.ArrayParameterType);
        }

        public static void AddValueList<T>(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable<T> values, ArrayParameterType type)
        {
            sql.AddValueList(builder, values, ConversionHelper.GetDBTypeForClrType(typeof(T)), type);
        }

        public static void AddValueList(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable values, DbType dbType)
        {
            sql.AddValueList(builder, values, dbType, sql.ArrayParameterType);
        }

        public static void AddValueList(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable values, DbType dbType, ArrayParameterType type)
        {
            if (type == ArrayParameterType.Array)
                sql.AddValueListTableType(builder, values, dbType);
            else if (type == ArrayParameterType.BunchOfParameters)
                sql.AddValueListLegacy(builder, values, dbType);
            else if (type == ArrayParameterType.Xml)
                sql.AddValueListXml(builder, values, dbType);
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", sql.ArrayParameterType));
        }

        private static void AddValueListXml(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable values, DbType dbType)
        {
            string xml = null;
            using (var writer = new StringWriter())
            {
                var xmlWriter = new XmlTextWriter(writer);
                xmlWriter.WriteStartElement("a");
                foreach (var value in values)
                {
                    xmlWriter.WriteStartElement("b");
                    xmlWriter.WriteValue(value);
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();

                xml = writer.ToString();
            }

            var name = sql.Parameters.GetUniqueParameterName();
            var alias = "x_" + name;
            var p = new SqlStatementParameter(name, DbType.Xml, xml);
            sql.Parameters.Add(p);

            //SELECT x.node.value('@value', 'int')
            //    FROM @xml.nodes('/values/value') as x(node)

            builder.Append("select ");
            builder.Append(alias);
            builder.Append(".node.value('./node()[1]', '");
            if (dbType == DbType.Int32 || dbType == DbType.Int16 || dbType == DbType.Boolean)
                builder.Append("int");
            else if(dbType == DbType.Int64)
                builder.Append("bigint");
            else if (dbType == DbType.String)
                builder.Append("nvarchar(max)");
            else if (dbType == DbType.Guid)
                builder.Append("uniqueidentider");
            else
                throw new NotSupportedException(string.Format("Cannot handle '{0}'.", dbType));
            builder.Append("') id from ");
            builder.Append(sql.Dialect.FormatVariableNameForQueryText(p.Name));
            builder.Append(".nodes('/a/b') as ");
            builder.Append(alias);
            builder.Append("(node)");
        }

        private static void AddValueListTableType(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable values, DbType dbType)
        {
            var name = sql.Parameters.GetUniqueParameterName();
            var p = SqlStatementParameter.CreateArrayParameter(name, dbType, values);
            sql.Parameters.Add(p);

            builder.Append("select id from ");
            builder.Append(sql.Dialect.FormatVariableNameForQueryText(name));
        }

        private static void AddValueListLegacy(this ISqlStatementArrayBuilder sql, StringBuilder builder, IEnumerable values, DbType dbType)
        {
            bool first = true;
            foreach (var value in values)
            {
                if (first)
                    first = false;
                else
                    builder.Append(", ");

                builder.Append(sql.Dialect.FormatVariableNameForQueryText(sql.Parameters.Add(dbType, value)));
            }
        }
    }
}
