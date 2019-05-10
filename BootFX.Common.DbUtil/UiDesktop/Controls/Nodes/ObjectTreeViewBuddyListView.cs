// BootFX - Application framework for .NET applications
// 
// File: ObjectTreeViewBuddyListView.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>ObjectTreeViewBuddyListView</c>.
	/// </summary>
	internal class ObjectTreeViewBuddyListView : ListView, ICommandProvider, IEntityView
	{
		/// <summary>
		/// Private field to support <see cref="TreeView"/> property.
		/// </summary>
		private ObjectTreeView _treeView;

		/// <summary>
		/// Private field to support <c>ItemColumnHeader</c> property.
		/// </summary>
		private ColumnHeader _itemColumnHeader;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="treeView"></param>
		internal ObjectTreeViewBuddyListView(ObjectTreeView treeView)
		{
			if(treeView == null)
				throw new ArgumentNullException("treeView");
			_treeView = treeView;

			// image list...
			this.SmallImageList = treeView.ImageList;
			this.View = View.Details;
			this.HideSelection = false;

			// columns...
			this.Columns.Clear();
			_itemColumnHeader = new ColumnHeader();
			_itemColumnHeader.Text = "Item";
			this.Columns.Add(_itemColumnHeader);

			// update...
			this.RefreshView();

			// subscribe...
			treeView.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
		}

		/// <summary>
		/// Gets the treeview.
		/// </summary>
		internal ObjectTreeView TreeView
		{
			get
			{
				return _treeView;
			}
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			RefreshView();
		}

		protected override void Dispose(bool disposing)
		{
			// unsub...
			if(this._treeView != null)
			{
				_treeView.AfterSelect -= new TreeViewEventHandler(treeView_AfterSelect);	
				_treeView = null;
			}

			base.Dispose (disposing);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		private void RefreshView()
		{
			this.Items.Clear();

			// get...
			TreeNode selected = this.CurrentTreeNode;
			if(selected == null)
				return;

			// add...
			if(selected is ObjectTreeNode)
			{
				bool cancel = false;
				((ObjectTreeNode)selected).EnsurePopulated(ref cancel);
				if(cancel)
					return;
			}

			// walk...
			foreach(TreeNode node in selected.Nodes)
				this.Items.Add(new TreeNodeListViewItem(node));

			// size...
			ListViewHelper.AutoSizeColumns(this);
		}

		private TreeNode CurrentTreeNode
		{
			get
			{
				if(TreeView == null)
					throw new InvalidOperationException("TreeView is null.");
				return this.TreeView.SelectedNode;
			}
		}

		private TreeNodeListViewItem SelectedItem
		{
			get
			{
				if(this.SelectedItems.Count > 0)	
					return (TreeNodeListViewItem)this.SelectedItems[0];
				else
					return null;
			}
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick (e);
			this.SelectNode();
		}

		private void SelectNode()
		{
			// tree?
			if(TreeView == null)
				throw new InvalidOperationException("TreeView is null.");

			// show...
			TreeNodeListViewItem selected = this.SelectedItem;
			if(selected != null)
				this.TreeView.SelectedNode = selected.Node;
		}

		private TreeNode SelectedTreeNode
		{
			get
			{
				if(this.SelectedItem != null)
					return this.SelectedItem.Node;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the itemcolumnheader.
		/// </summary>
		private ColumnHeader ItemColumnHeader
		{
			get
			{
				// returns the value...
				return _itemColumnHeader;
			}
		}

		/// <summary>
		/// Gets or sets the caption for the default column.
		/// </summary>
		public string Caption
		{
			get
			{
				if(ItemColumnHeader == null)
					throw new InvalidOperationException("ItemColumnHeader is null.");
				return this.ItemColumnHeader.Text;
			}
			set
			{
				if(ItemColumnHeader	 == null)
					throw new InvalidOperationException("ItemColumnHeader	 is null.");
				this.ItemColumnHeader.Text = value;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);

			// context menu?
			if((int)(e.Button & MouseButtons.Right) != 0)
			{
				// show...
				CommandUIObjectBuilder builder = new CommandUIObjectBuilder();
				builder.CreateAndShowContextMenu(this, new Point(e.X, e.Y), this);
			}
		}

		public CommandCollection GetCommands()
		{
			if(this.SelectedTreeNode is ICommandProvider)
				return ((ICommandProvider)this.SelectedTreeNode).GetCommands();
			else
				return new CommandCollection();
		}

		public object FocusedEntity
		{
			get
			{
				TreeNodeListViewItem item = (TreeNodeListViewItem)this.FocusedItem;
				if(item != null && item.ObjectNode != null && item.ObjectNode.InnerObjectIsEntity)
					return item.ObjectNode.InnerObject;
				else
					return null;
			}
		}

		public IList SelectedEntities
		{
			get
			{
				ArrayList results = new ArrayList();

				// get...
				SelectedListViewItemCollection items = this.SelectedItems;
				if(items != null && items.Count > 0)
				{
					// create an arraylist and extract the selected items...
					foreach(TreeNodeListViewItem item in items)
					{
						if(item.ObjectNode != null && item.ObjectNode.InnerObjectIsEntity)
							results.Add(item.ObjectNode.InnerObject);
					}
				}

				// count?
				if(results.Count == 0)
					return results;
				else 
					return EntityType.CreateCollectionInstance(results);
			}
		}

		public int SelectedEntitiesCount
		{
			get
			{
				IList selected = this.SelectedEntities;
				if(selected != null)
					return selected.Count;
				else
					return 0;
			}
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);

			// handle...
			ListViewHelper.HandleKeyDown(this, e);

			// handled?
			if(!(e.Handled))
			{
				// return?  select the node and set focus back to us...
				if(e.KeyCode == Keys.Return)
				{
					SelectNode();
					this.Focus();
				}
			}
		}
	}
}
