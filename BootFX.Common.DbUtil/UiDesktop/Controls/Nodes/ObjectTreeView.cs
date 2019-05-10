// BootFX - Application framework for .NET applications
// 
// File: ObjectTreeView.cs
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
	/// Defines an instance of <c>ObjectTreeView</c>.
	/// </summary>
	public class ObjectTreeView : TreeView, IChangeRegisterCallback, ICommandProvider, IEntityView
	{
		/// <summary>
		/// Private field to support <see cref="SelectOnRightClick"/> property.
		/// </summary>
		private bool _selectOnRightClick = true;
		
		/// <summary>
		/// Private field to support <c>Wrapper</c> property.
		/// </summary>
		private TreeNodeCollectionWrapper _wrapper;
		
		/// <summary>
		/// Raised when the control is loaded.
		/// </summary>
		public event EventHandler Load;
		
		/// <summary>
		/// Private field to support <see cref="ImageListEx"/> property.
		/// </summary>
		private ImageListEx _imageListEx = new ImageListEx();
		
		/// <summary>
		/// Private field to support <see cref="BuddyListView"/> property.
		/// </summary>
		private ListView _buddyListView;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectTreeView()
		{
			// wrapper...
			_wrapper = new TreeNodeCollectionWrapper(this, base.Nodes);

			// images...
			this.ImageList = this.ImageListEx.InnerImageList;

			// subscribe...
            //ChangeRegister.Current.Subscribe(null, this);
		}

		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			base.OnBeforeExpand (e);

			if(e.Node is ObjectTreeNode)
				((ObjectTreeNode)e.Node).HandleBeforeExpand(e);
		}

		protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			base.OnBeforeSelect (e);

			// defer...
			if(e.Node is ObjectTreeNode)
				((ObjectTreeNode)e.Node).HandleBeforeSelect(e);
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{
			base.OnAfterSelect (e);

			// defer...
			if(e.Node is ObjectTreeNode)
				((ObjectTreeNode)e.Node).HandleAfterSelect(e);
		}

		public ObjectTreeNode SelectedObjectNode
		{
			get
			{
				return this.SelectedNode as ObjectTreeNode;	
			}
		}

		/// <summary>
		/// Gets back to the main menu.
		/// </summary>
		protected Form ParentForm
		{
			get
			{
				// loop...
				Control parent = this.Parent;
				while(parent != null)
				{
					if(parent is Form)
						return (Form)parent;
					parent = parent.Parent;
				}

				// nope...
				return null;
			}
		}

		/// <summary>
		/// Gets the buddylistview.
		/// </summary>
		public ListView BuddyListView
		{
			get
			{
				if(_buddyListView == null)
					_buddyListView = new ObjectTreeViewBuddyListView(this);
				return _buddyListView;
			}
		}

		protected override void Dispose(bool disposing)
		{
			// image...
			if(_imageListEx != null)
			{
				_imageListEx.Dispose();
				_imageListEx = null;
			}

			// undo...
            //ChangeRegister.Current.Unsubscribe(this);

			base.Dispose (disposing);
		}

		/// <summary>
		/// Called when an entity has been changed.
		/// </summary>
		/// <param name="context"></param>
		void IChangeRegisterCallback.EntityChanged(ChangeNotificationContext context)
		{
			this.EntityChanged(context);	
		}

		private void EntityChanged(ChangeNotificationContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");

			// flip...
			if(this.InvokeRequired)
			{
				EntityChangedDelegate d = new EntityChangedDelegate(this.EntityChanged);
				this.Invoke(d, new object[] { context });
				return;
			}

			// notify...
			this.WalkNodesAndNotify(this.Nodes, context);	
		}

		private void WalkNodesAndNotify(TreeNodeCollectionWrapper nodes, ChangeNotificationContext context)
		{
			if(nodes == null)
				throw new ArgumentNullException("nodes");
			
			// defer...
			this.WalkNodesAndNotify(nodes.InnerList, context);
		}

		private void WalkNodesAndNotify(TreeNodeCollection nodes, ChangeNotificationContext context)
		{
			if(nodes == null)
				throw new ArgumentNullException("nodes");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// walk...
			foreach(TreeNode node in nodes)
			{
				bool doChildren = false;

				// get...
				ObjectTreeNode objectNode = node as ObjectTreeNode;
				if(objectNode != null)
				{
					// match?
					if(objectNode.InnerObjectEntityType != null && context.EntityType.IsOfType(context.EntityType))
					{
						if(EntityIdentification.CompareEntityKeyValues(objectNode.InnerObject, context.Entity))
						{
							// this one has changed...
							objectNode.EntityChanged(context.Entity, context.Unit);
						}
					}

					// children?
					if(objectNode.IsPopulated)
						doChildren = true;
				}
				else
					doChildren = true;

				// children...
				if(doChildren)
					this.WalkNodesAndNotify(node.Nodes, context);
			}
		}

		/// <summary>
		/// Gets the smartimagelist.
		/// </summary>
		public ImageListEx ImageListEx
		{
			get
			{
				return _imageListEx;
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl ();
			this.OnLoad();
		}

		/// <summary>
		/// Raises the <c>Load</c> event.
		/// </summary>
		private void OnLoad()
		{
			OnLoad(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Load</c> event.
		/// </summary>
		protected virtual void OnLoad(EventArgs e)
		{
			// update the icons...
			foreach(ObjectTreeNode node in this.Nodes)
			{
				if(node.ImageIndex == -1 || node.SelectedImageIndex == -1)
					node.RefreshIcon();
			}

			// raise...
			if(Load != null)
				Load(this, e);
		}

		internal void NodeAdded(TreeNode node)
		{
			if(node == null)
				throw new ArgumentNullException("node");
			
			// check...
			if(node is ObjectTreeNode)
				((ObjectTreeNode)node).Added();
		}

		/// <summary>
		/// Replaced version of nodes.
		/// </summary>
		public new TreeNodeCollectionWrapper Nodes
		{
			get
			{
				return _wrapper;
			}
		}

		/// <summary>
		/// Gets the selectonrightclick.
		/// </summary>
		public bool SelectOnRightClick
		{
			get
			{
				return _selectOnRightClick;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);

			if((int)(e.Button & MouseButtons.Right) != 0 && this.SelectOnRightClick)
			{
				// select...
				TreeNode node = this.GetNodeAt(e.X, e.Y);
				if(node != null)
					this.SelectedNode = node;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);

			// context menu?
			if((int)(e.Button & MouseButtons.Right) != 0)
				this.ShowContextMenu(e.X, e.Y);
		}

		private void ShowContextMenu()
		{
			Point pt = this.PointToClient(Control.MousePosition);
			this.ShowContextMenu(pt.X, pt.Y);
		}

		private void ShowContextMenu(int x, int y)
		{
			// show...
			CommandUIObjectBuilder builder = new CommandUIObjectBuilder();
			builder.CreateAndShowContextMenu(this, new Point(x, y), this);			
		}

		public CommandCollection GetCommands()
		{
			if(this.SelectedNode is ICommandProvider)
				return ((ICommandProvider)this.SelectedNode).GetCommands();
			else
				return null;
		}

		public object FocusedEntity
		{
			get
			{
				if(this.SelectedObjectNode != null && this.SelectedObjectNode.InnerObjectIsEntity)
					return this.SelectedObjectNode.InnerObject;
				else
					return null;
			}
		}

		public IList SelectedEntities
		{
			get
			{
				object focused = this.FocusedEntity;
				if(focused != null)
					return EntityType.CreateCollectionInstance(focused);
				else
					return new object[] {};
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

			if(e.KeyCode == Keys.Apps)
			{
				// show...
				ShowContextMenu();
				e.Handled = true;
			}
		}
	}
}
