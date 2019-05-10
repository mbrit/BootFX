// BootFX - Application framework for .NET applications
// 
// File: IndexBuilder.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common.Data
{
    public class IndexBuilder : IEntityType
    {
        private NativeName TableOwner { get; set; }
        public EntityType EntityType { get; private set; }
        private SqlDialect Dialect { get; set; }

        private List<string> IndexColumns { get; set; }
        private List<string> IncludeColumns { get; set; }

        public IndexBuilder(NativeName tableOwner, SqlDialect dialect = null)
            : this(tableOwner, dialect, null)
        {
        }
        private IndexBuilder(NativeName tableOwner, SqlDialect dialect, EntityType et)
        {
            this.EntityType = et;
            this.TableOwner = tableOwner;

            if (dialect != null)
                this.Dialect = dialect;
            else
                this.Dialect = Database.DefaultDialect;

            this.IndexColumns = new List<string>();
            this.IncludeColumns = new List<string>();
        }

        public static IndexBuilder GetIndexBuilder<T>(SqlDialect dialect = null)
            where T : Entity
        {
            var et = typeof(T).ToEntityType();
            return GetIndexBuilder(et, dialect);
        }

        public static IndexBuilder GetIndexBuilder(EntityType et, SqlDialect dialect = null)
        {
            return new IndexBuilder(et.NativeName, dialect, et);
        }

        public void AddIndexColumn(EntityField field)
        {
            this.AddIndexColumn(field.NativeName.Name);
        }

        public void AddIndexColumn(string name)
        {
            name = this.MangleColumn(name);

            // if we add an index column, we can move it from the included columns...

            var index = this.IndexColumns.IndexOf(name, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
            {
                this.IndexColumns.Add(name);
                this.IndexColumns.Sort();
            }

            index = this.IncludeColumns.IndexOf(name, StringComparison.InvariantCultureIgnoreCase);
            if (index != -1)
                this.IncludeColumns.RemoveAt(index);
        }

        public void AddIncludeColumn(EntityField field)
        {
            this.AddIncludeColumn(field.NativeName.Name);
        }

        public void AddIncludeColumn(string name)
        {
            name = this.MangleColumn(name);

            // if we're already indexing this, we can't do anything with it...
            if (this.IsInIndexColumns(name))
                throw new InvalidOperationException(string.Format("The column '{0}' is already in the index set.", name));

            var index = this.IncludeColumns.IndexOf(name, StringComparison.InvariantCultureIgnoreCase);
            if (index == -1)
            {
                this.IncludeColumns.Add(name);
                this.IncludeColumns.Sort();
            }
        }

        internal bool IsInIndexColumns(EntityField field)
        {
            return this.IsInIndexColumns(field.NativeName.Name);
        }

        internal bool IsInIndexColumns(string name)
        {
            return this.IndexColumns.Contains(name, StringComparer.InvariantCultureIgnoreCase);
        }

        private string MangleColumn(string name)
        {
            if (this.EntityType != null)
            {
                var field = this.EntityType.Fields[name];
                if (field == null)
                    throw new InvalidOperationException(string.Format("The field '{0}' is not valid on '{1}'.", name, this.EntityType.Name));
                return field.NativeName.Name;
            }
            else
                return name;
        }

        internal string IndexName
        {
            get
            {
                var builder = new StringBuilder();
                this.GetIndexName(builder);
                return builder.ToString();
            }
        }

        private void GetIndexName(StringBuilder builder)
        {
            this.Validate();

            builder.Append(this.TableOwner.Name);
            builder.Append("_Bfx_");
            foreach (var name in this.IndexColumns)
                builder.Append(name);

            if (this.IncludeColumns.Any())
            {
                builder.Append("_Inc_");
                foreach (var name in this.IncludeColumns)
                    builder.Append(name);
            }
        }

        private void Validate()
        {
            if (!(this.IndexColumns.Any()))
                throw new InvalidOperationException("The index has no columns.");
        }

        public override string ToString()
        {
            this.Validate();

            StringBuilder builder = new StringBuilder();
            builder.Append(this.Dialect.CreateIndexKeyword);
            builder.Append(" ");
            builder.Append(this.Dialect.FormatIndexName(this.IndexName));
            builder.Append(" ON ");
            builder.Append(this.Dialect.FormatTableName(this.TableOwner));
            builder.Append(" (");
            builder.Append(this.Dialect.FormatColumnNamesList(this.IndexColumns));
            builder.Append(")");

            if (this.IncludeColumns.Any())
            {
                builder.Append(" INCLUDE (");
                builder.Append(this.Dialect.FormatColumnNamesList(this.IncludeColumns));
                builder.Append(")");
            }

            return builder.ToString();
        }
    }
}
