// BootFX - Application framework for .NET applications
// 
// File: TreeNodeListViewItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>TreeNodeListViewItem</c>.
	/// </summary>
	internal class TreeNodeListViewItem : ListViewItem
	{
		/// <summary>
		/// Private field to support <see cref="Node"/> property.
		/// </summary>
		private TreeNode _node;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="node"></param>
		internal TreeNodeListViewItem(TreeNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			_node = node;

			// sub...
			if(node is ObjectTreeNode)
				((ObjectTreeNode)node).InnerObjectChanged += new EventHandler(TreeNodeListViewItem_InnerObjectChanged);

			// set...
			this.RefreshTextAndIcon();
		}

		private void RefreshTextAndIcon()
		{
			this.RefreshText();
			this.RefreshIcon();
		}

		private void RefreshText()
		{
			if(Node == null)
				throw new InvalidOperationException("Node is null.");
			this.Text = this.Node.Text;
		}

		private void RefreshIcon()
		{
			// get...
			this.ImageIndex = this.Node.ImageIndex;
		}

		/// <summary>
		/// Gets the node.
		/// </summary>
		internal TreeNode Node
		{
			get
			{
				return _node;
			}
		}

		internal ObjectTreeNode ObjectNode
		{
			get
			{
				return this.Node as ObjectTreeNode;
			}
		}

		private void TreeNodeListViewItem_InnerObjectChanged(object sender, EventArgs e)
		{
			this.RefreshTextAndIcon();
		}
	}
}
