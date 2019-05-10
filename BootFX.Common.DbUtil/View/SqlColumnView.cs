// BootFX - Application framework for .NET applications
// 
// File: SqlColumnView.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlColumnView.
	/// </summary>
	public class SqlColumnView : SqlMemberView
	{
		public SqlColumnView(SqlColumn column) : base(column)
		{
		}

		[Category("Code Generation"), Description("Gets or sets the name of an enumeration type in your code that this property maps to.")]
		public string EnumerationTypeName
		{
			get
			{
				if(Column == null)
					throw new InvalidOperationException("Column is null.");
				return this.Column.EnumerationTypeName;
			}
			set
			{
				if(Column == null)
					throw new InvalidOperationException("Column is null.");
				this.Column.EnumerationTypeName = value;
			}
		}

        [Category("Behavior"), Description("Gets or sets the Modifiers for this column.")]
        public ColumnModifiers Modifiers
        {
            get
            {
                if (Column == null)
                    throw new InvalidOperationException("Column is null.");
                return this.Column.Modifiers;
            }
            set
            {
                if (Column == null)
                    throw new InvalidOperationException("Column is null.");
                this.Column.Modifiers = value;
            }
        }

		private SqlColumn Column
		{
			get
			{
				return (SqlColumn)base.Member;
			}
		}

        [Category("DTO"), Description("Gets or sets whether to include the member in the DTO object.")]
        public bool GenerateDtoField
        {
            get
            {
                return this.Column.GenerateDtoField;
            }
            set
            {
                this.Column.GenerateDtoField = value;
            }
        }


        [Category("DTO"), Description("Gets or sets the generated JSON name.")]
        public string JsonName
        {
            get
            {
                return this.Column.JsonName;
            }
            set
            {
                this.Column.JsonName = value;
            }
        }
    }
}
