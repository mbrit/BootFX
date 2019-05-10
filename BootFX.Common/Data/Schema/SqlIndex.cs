// BootFX - Application framework for .NET applications
// 
// File: SqlIndex.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using BootFX.Common.Entities;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Describes an index.
	/// </summary>
	public class SqlIndex : SqlMemberWithColumns
	{
		/// <summary>
		/// Private field to support <c>HasUniqueValues</c> property.
		/// </summary>
		private bool _hasUniqueValues;

        public SqlColumnCollection IncludedColumns { get; private set; }
        public SqlColumnCollection ComputedColumns { get; private set; }
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nativeName"></param>
		public SqlIndex(string nativeName) 
            : base(nativeName)
		{
            this.Initialize();
		}

		internal SqlIndex(SqlTable table, EntityIndex index) 
            : base(index.NativeName.Name)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			// set...
            this.Initialize();
            InitializeColumns(table, index.Fields, this.Columns);
            InitializeColumns(table, index.IncludedFields, this.IncludedColumns);
            InitializeColumns(table, index.ComputedFields, this.ComputedColumns);

			// set...
			this.Name = index.Name;
			this.HasUniqueValues = index.HasUniqueValues;
        }

        private void InitializeColumns(SqlTable table, EntityFieldCollection fields, SqlColumnCollection columns)
        {
            foreach (EntityField field in fields)
            {
                SqlColumn column = table.Columns[field.NativeName.Name];
                if (column == null)
                    throw new InvalidOperationException(string.Format("Failed to find column with native name '{0}' (field name '{1}')", field.NativeName, field.Name));

                // add...
                columns.Add(column);
            }
        }

        private void Initialize()
        {
            this.IncludedColumns = new SqlColumnCollection(this);
            this.ComputedColumns = new SqlColumnCollection(this);
        }

		/// <summary>
		/// Gets or sets the hasuniquevalues
		/// </summary>
		public bool HasUniqueValues
		{
			get
			{
				return _hasUniqueValues;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _hasUniqueValues)
				{
					// set the value...
					_hasUniqueValues = value;
				}
			}
		}

        public bool DoesMatch(SqlIndex index)
        {
            List<string> theseNames = this.Columns.GetNativeNamesSorted();
            List<string> otherNames = index.Columns.GetNativeNamesSorted();
            if (!(DoesMatch(theseNames, otherNames)))
                return false;

            theseNames = this.IncludedColumns.GetNativeNamesSorted();
            otherNames = index.IncludedColumns.GetNativeNamesSorted();
            if (!(DoesMatch(theseNames, otherNames)))
                return false;

            theseNames = this.ComputedColumns.GetNativeNamesSorted();
            otherNames = index.ComputedColumns.GetNativeNamesSorted();
            if (!(DoesMatch(theseNames, otherNames)))
                return false;

            // ok...
            return true;
        }

        private bool DoesMatch(List<string> a, List<string> b)
        {
            if (a.Count == b.Count)
            {
                for (int index = 0; index < a.Count; index++)
                {
                    if (string.Compare(a[index], b[index], true, Cultures.System) != 0)
                        return false;
                }

                // ok...
                return true;
            }
            else
                return false;
        }
    }
}
