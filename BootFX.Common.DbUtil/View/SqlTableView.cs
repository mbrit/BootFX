// BootFX - Application framework for .NET applications
// 
// File: SqlTableView.cs
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
	public class SqlTableView : SqlMemberView
	{
		public SqlTableView(SqlTable table) : base(table)
		{
		}

        //[Category("Behavior"), Description("Gets or sets the Modifiers for this table.")]
        //public TableModifiers Modifiers
        //{
        //    get
        //    {
        //        if (Table == null)
        //            throw new InvalidOperationException("Table is null.");
        //        return this.Table.Modifiers;
        //    }
        //    set
        //    {
        //        if (Table == null)
        //            throw new InvalidOperationException("Table is null.");
        //        this.Table.Modifiers = value;
        //    }
        //}

        [Category("Code Generation"), Description("Gets or sets whether to create a DTO for the entity.")]
        public bool GenerateDto
        {
            get
            {
                return this.Table.GenerateDto;
            }
            set
            {
                this.Table.GenerateDto = value;
            }
        }

		private SqlTable Table
		{
			get
			{
				return (SqlTable)base.Member;
			}
		}
	}
}
