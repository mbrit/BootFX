// BootFX - Application framework for .NET applications
// 
// File: SqlTableNode.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for TableNode.
	/// </summary>
	internal class SqlTableNode : SqlMemberNode
	{
		internal SqlTableNode(SqlTable table) : base(table)
		{
            table.ModifiersChanged += new EventHandler(table_ModifiersChanged);
			// demand load...
			this.DemandLoad = true;

			// reset...
			if(table.NativeName.Length > 30)
				this.BackColor = Color.Red;
		}

        void table_ModifiersChanged(object sender, EventArgs e)
        {
            this.SelectIcon();
            this.RefreshTextAndIcon();
        }

		protected override void PopulateNodes(ref bool cancel)
		{
			base.PopulateNodes(ref cancel);
			if(cancel)
				return;

			// children..
			SqlColumnsNode node = new SqlColumnsNode(this.Table);
			this.Nodes.Add(node);
			node.RefreshIcon();

			// update...
			if(Table.LinksToParents.Count > 0)
			{
				SqlChildToParentLinksNode linkNode = new SqlChildToParentLinksNode(Table);
				this.Nodes.Add(linkNode);
				linkNode.RefreshIcon();
			}
		}

		internal override void SelectIcon()
		{
            //if (this.Table.Modifiers == TableModifiers.Public)
            //{
            if (this.Member.Generate)
            {
                if(this.Table.GenerateDto)
                    this.ImageResourceName = "BootFX.Common.DbUtil.Resources.GenerateDto.ico";
                else
                    this.ImageResourceName = "BootFX.Common.DbUtil.Resources.Generate.ico";
            }
            else
                this.ImageResourceName = "BootFX.Common.DbUtil.Resources.NonGenerate.ico";
            //}
            //else
            //{
            //    if (this.Member.Generate)
            //        this.ImageResourceName = "BootFX.Common.DbUtil.Resources.GenerateInternal.ico";
            //    else
            //        this.ImageResourceName = "BootFX.Common.DbUtil.Resources.NonGenerateInternal.ico";
            //}
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		internal SqlTable Table
		{
			get
			{
				// returns the value...
				return (SqlTable) Member;
			}
		}

		private void table_GenerateWebServiceChanged(object sender, EventArgs e)
		{
			this.SelectIcon();
			this.RefreshTextAndIcon();
		}

		protected override string GetText()
		{
			string text = base.GetText();

			return text;
		}

	}
}
