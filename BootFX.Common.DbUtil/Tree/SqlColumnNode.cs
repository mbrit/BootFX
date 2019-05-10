// BootFX - Application framework for .NET applications
// 
// File: SqlColumnNode.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlColumnNode.
	/// </summary>
	internal class SqlColumnNode : SqlMemberNode
	{
		internal SqlColumnNode(SqlColumn column) : base(column)
		{
			this.DemandLoad = false;
			column.EnumerationTypeNameChanged += new EventHandler(column_EnumerationTypeNameChanged);
            column.ModifiersChanged += new EventHandler(column_ModifiersChanged);
			column.FlagsChanged += new EventHandler(column_FlagsChanged);
		}

        void column_ModifiersChanged(object sender, EventArgs e)
        {
            this.SelectIcon();
            this.RefreshTextAndIcon();
        }

		protected override string GetText()
		{
			string text = base.GetText();
			if(text == null)
				throw new InvalidOperationException("text is null.");

			// add...
			if(Column == null)
				throw new InvalidOperationException("Column is null.");

			// builder...
			StringBuilder builder = new StringBuilder();
			builder.Append(text);
			builder.Append(", ");
			builder.Append(this.Column.DbType);
			if(this.Column.IsFixedLength)
			{
				builder.Append(", ");
				if(this.Column.IsLarge)
					builder.Append("large");
				else
					builder.Append(this.Column.Length);
			}
			if(this.Column.IsNullable)
				builder.Append(", nullable");
			if(this.Column.IsKey)
				builder.Append(", key");

			// return...
			return builder.ToString();
		}

		internal override void SelectIcon()
		{
		    string image = string.Empty;
			if(this.Member.Generate)
			{
                if (this.Column.IsKey)
                {
                    if (this.Column.GenerateDtoField)
                        image = "BootFX.Common.DbUtil.Resources.{0}KeyColumnGenerateDto.ico";
                    else
                        image = "BootFX.Common.DbUtil.Resources.{0}KeyColumnGenerate.ico";
                }
                else if (this.Column.EnumerationTypeName != null && this.Column.EnumerationTypeName != string.Empty)
                {
                    if (this.Column.GenerateDtoField)
                        image = "BootFX.Common.DbUtil.Resources.{0}EnumColumnGenerateDto.ico";
                    else
                        image = "BootFX.Common.DbUtil.Resources.{0}EnumColumnGenerate.ico";
                }
                else
                {
                    if (this.Column.GenerateDtoField)
                        image = "BootFX.Common.DbUtil.Resources.{0}ColumnGenerateDto.ico";
                    else
                        image = "BootFX.Common.DbUtil.Resources.{0}ColumnGenerate.ico";
                }
			}
			else
                image = "BootFX.Common.DbUtil.Resources.{0}NonColumnGenerate.ico";

            ImageResourceName = string.Format(image, Column.Modifiers);
		}

		private SqlColumn Column
		{
			get
			{
				return (SqlColumn)base.Member;
			}
		}

		private void column_EnumerationTypeNameChanged(object sender, EventArgs e)
		{
			this.SelectIcon();
			this.RefreshTextAndIcon();
		}

		private void column_FlagsChanged(object sender, EventArgs e)
		{
			this.SelectIcon();
			this.RefreshTextAndIcon();
		}
	}
}
