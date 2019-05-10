// BootFX - Application framework for .NET applications
// 
// File: SqlColumnsNode.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for SqlColumnsName.
	/// </summary>
	internal class SqlColumnsNode : ObjectTreeNode
	{
		/// <summary>
		/// Private field to support <c>Table</c> property.
		/// </summary>
		private SqlTable _table;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="table"></param>
		internal SqlColumnsNode(SqlTable table) : base(table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// set...
			_table = table;
            _table.ModifiersChanged += new EventHandler(_table_ModifiersChanged);
			this.Text = "Columns";
			this.ImageResourceName = "BootFX.Common.DbUtil.Resources.Columns.ico";
			this.DemandLoad = true;
		}

        void _table_ModifiersChanged(object sender, EventArgs e)
        {
            this.RefreshTextAndIcon();
        }

		protected override void PopulateNodes(ref bool cancel)
		{
			cancel = false;
			base.PopulateNodes(ref cancel);
			if(cancel)
				return;

			// walk...
			if(Table == null)
				throw new InvalidOperationException("Table is null.");
			foreach(SqlColumn column in this.Table.Columns)
				this.Nodes.Add(new SqlColumnNode(column));
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		private SqlTable Table
		{
			get
			{
				// returns the value...
				return _table;
			}
		}
	}
}
