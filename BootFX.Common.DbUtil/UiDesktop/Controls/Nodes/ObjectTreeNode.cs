// BootFX - Application framework for .NET applications
// 
// File: ObjectTreeNode.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>ObjectTreeNode</c>.
	/// </summary>
	public class ObjectTreeNode : TreeNode, IEntityType, ICommandProvider
	{
		/// <summary>
		/// Private field to support <c>Commands</c> property.
		/// </summary>
		private CommandCollection _commands = new CommandCollection();
		
		/// <summary>
		/// Private field to support <c>CommonCommandHandler</c> property.
		/// </summary>
		private CommonCommandHandler _commonCommands;
		
		/// <summary>
		/// Private field to support <c>Wrapper</c> property.
		/// </summary>
		private TreeNodeCollectionWrapper _wrapper;
		
		/// <summary>
		/// Private field to support <c>ImageResourceName</c> property.
		/// </summary>
		private string _imageResourceName;
		
		/// <summary>
		/// Private field to support <c>ImageAssembly</c> property.
		/// </summary>
		private Assembly _imageResourceAssembly;
		
		/// <summary>
		/// Private field to support <c>AutoUpdateOnChange</c> property.
		/// </summary>
		private bool _autoUpdateOnChange = true;
		
		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Private field to support <c>FirstViewRequest</c> property.
		/// </summary>
		private bool _firstViewRequest = true;
		
		/// <summary>
		/// Private field to support <see cref="View"/> property.
		/// </summary>
		private Control _view = null;
		
		/// <summary>
		/// Raised when the <c>InnerObject</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the InnerObject property has changed.")]
		public event EventHandler InnerObjectChanged;
		
		/// <summary>
		/// Raised when node population is required.
		/// </summary>
		public event CancelEventHandler PopulateRequired;
		
		/// <summary>
		/// Raised before the node is selected.
		/// </summary>
		public event TreeViewCancelEventHandler BeforeSelect;
		
		/// <summary>
		/// Raised After the node is selected.
		/// </summary>
		public event TreeViewEventHandler AfterSelect;
		
		/// <summary>
		/// Raised before the node is expanded.
		/// </summary>
		public event TreeViewCancelEventHandler BeforeExpand;
		
		/// <summary>
		/// Private field to support <c>DemandLoad</c> property.
		/// </summary>
		private bool _demandLoad;

		/// <summary>
		/// Private field to support <see cref="InnerObject"/> property.
		/// </summary>
		private object _innerObject;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectTreeNode() : this(null)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public ObjectTreeNode(object innerObject)
		{
			// set...
			_wrapper = new TreeNodeCollectionWrapper(this, base.Nodes);

			// set...
			this.InnerObject = innerObject;

			// flag for demand load...
			this.DemandLoad = true;
		}

		/// <summary>
		/// Gets or sets the demandload
		/// </summary>
		public bool DemandLoad
		{
			get
			{
				return _demandLoad;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _demandLoad)
				{
					// set the value...
					_demandLoad = value;

					// update...
					if(this.Nodes.Count == 0 || (this.Nodes.Count > 0 && this.Nodes[0] is DemandLoadTreeNode))
					{
						// collapse and clear.
						this.Collapse();
						this.Nodes.Clear();

						// are we demand loading?
						if(this.DemandLoad)
							this.Nodes.Add(new DemandLoadTreeNode());
					}
					else
						throw new InvalidOperationException("Demand load property cannot be changed as the node has already been populated.");
				}
			}
		}

		/// <summary>
		/// Gets the innerobject.
		/// </summary>
		public object InnerObject
		{
			get
			{
				return _innerObject;
			}
			set
			{
				if(_innerObject != value)
					this.SetInnerObject(value);
			}
		}

		/// <summary>
		/// Sets the inner object.
		/// </summary>
		/// <param name="obj"></param>
		private void SetInnerObject(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			
			// set...
			_innerObject = obj;
			_entityType = null;

			// refresh...
			RefreshTextAndIcon();
	
			// set...
			this.OnInnerObjectChanged();			
		}

		/// <summary>
		/// Updates the text and the icon for the node.
		/// </summary>
		public void RefreshTextAndIcon()
		{
			this.RefreshText();
			this.RefreshIcon();
		}

		public void RefreshIcon()
		{
			this.ImageIndex = this.GetImageIndex();
			this.SelectedImageIndex = this.ImageIndex;
		}

		protected virtual int GetImageIndex()
		{
			if(this.TreeView == null || this.ImageResourceAssembly == null || this.ImageResourceName == null || this.ImageResourceName.Length == 0)
				return -1;

			// update...
			if(ObjectTreeView.ImageListEx == null)
				throw new InvalidOperationException("TreeView.ImageListEx is null.");
			return this.ObjectTreeView.ImageListEx.GetImageIndex(this.ImageResourceAssembly, this.ImageResourceName);
		}

		public void RefreshText()
		{
			this.Text = this.GetText();
		}

		protected virtual string GetText()
		{
			// default...
			if(this.InnerObject != null)
			{
				if(this.InnerObject is IConvertible)
					return ((IConvertible)this.InnerObject).ToString(Cultures.User);
				else
					return this.InnerObject.ToString();
			}
			else
				return string.Empty;
		}

		internal void HandleBeforeExpand(TreeViewCancelEventArgs e)
		{
			bool cancel = e.Cancel;
			this.EnsurePopulated(ref cancel);
			if(cancel)
			{
				e.Cancel = true;
				return;
			}

			// raise...
			this.OnBeforeExpand(e);
		}

		/// <summary>
		/// Raises the <c>BeforeExpand</c> event.
		/// </summary>
		protected virtual void OnBeforeExpand(TreeViewCancelEventArgs e)
		{
			// raise...
			if(BeforeExpand != null)
				BeforeExpand(this, e);
		}

		protected virtual void PopulateNodes(ref bool cancel)
		{
			CancelEventArgs e = new CancelEventArgs(cancel);
			this.OnPopulateRequired(e);
			cancel = e.Cancel;
		}

		internal void HandleBeforeSelect(TreeViewCancelEventArgs e)
		{
			// defer...
			this.OnBeforeSelect(e);
		}

		internal void HandleAfterSelect(TreeViewEventArgs e)
		{
			// defer...
			this.OnAfterSelect(e);
		}

		/// <summary>
		/// Raises the <c>BeforeSelect</c> event.
		/// </summary>
		protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e)
		{
			// raise...
			if(BeforeSelect != null)
				BeforeSelect(this, e);
		}

		/// <summary>
		/// Raises the <c>AfterSelect</c> event.
		/// </summary>
		protected virtual void OnAfterSelect(TreeViewEventArgs e)
		{
			// raise...
			if(AfterSelect != null)
				AfterSelect(this, e);
		}

		/// <summary>
		/// Raises the <c>PopulateRequired</c> event.
		/// </summary>
		protected virtual void OnPopulateRequired(CancelEventArgs e)
		{
			// raise...
			if(PopulateRequired != null)
				PopulateRequired(this, e);
		}

		/// <summary>
		/// Raises the <c>InnerObjectChanged</c> event.
		/// </summary>
		private void OnInnerObjectChanged()
		{
			OnInnerObjectChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>InnerObjectChanged</c> event.
		/// </summary>
		protected virtual void OnInnerObjectChanged(EventArgs e)
		{
			if(InnerObjectChanged != null)
				InnerObjectChanged(this, e);
		}

		internal void EnsurePopulated(ref bool cancel)
		{
			cancel = false;

			// demand load?
			if(!(this.IsPopulated))
			{
				this.Nodes.Clear();

				// populate...
				Cursor oldCursor = null;
				if(this.TreeView != null)
				{
					oldCursor = this.TreeView.Cursor;
					this.TreeView.Cursor = Cursors.WaitCursor;	
				}
				try
				{
					this.PopulateNodes(ref cancel);
				}
				finally
				{
					if(this.TreeView != null)
						this.TreeView.Cursor = oldCursor;
				}
			}
		}

		/// <summary>
		/// Gets the view.
		/// </summary>
		public Control View
		{
			get
			{
				if(this.FirstViewRequest)
				{
					try
					{
						_view = CreateView();
					}
					finally
					{
						_firstViewRequest = false;	
					}
				}

				// anything?
				if(_view != null)
					return _view;
				else if(this.ObjectTreeView != null)
					return this.ObjectTreeView.BuddyListView;
				else
					return null;
			}
		}

		public ObjectTreeView ObjectTreeView
		{
			get
			{
				return this.TreeView as ObjectTreeView;	
			}
		}

		/// <summary>
		/// Gets the firstviewrequest.
		/// </summary>
		private bool FirstViewRequest
		{
			get
			{
				// returns the value...
				return _firstViewRequest;
			}
		}

		/// <summary>
		/// Asks the node to create its own control view.
		/// </summary>
		/// <returns></returns>
		protected virtual Control CreateView()
		{			
			return null;
		}

		EntityType IEntityType.EntityType
		{
			get
			{
				return this.InnerObjectEntityType;
			}
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType InnerObjectEntityType
		{
			get
			{
				if(_entityType == null && this.InnerObject != null)
					_entityType = EntityType.GetEntityType(this.InnerObject.GetType(), OnNotFound.ReturnNull);
				return _entityType;
			}
		}

		/// <summary>
		/// Returns true if the node has been populated.
		/// </summary>
        // mbr - 2009-06-19 - made public...
        //internal bool IsPopulated
		public bool IsPopulated
		{
			get
			{
				if(this.DemandLoad && this.Nodes.Count > 0 && this.Nodes[0] is DemandLoadTreeNode)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Called when the entity that maps to the inner object has been changed.
		/// </summary>
		/// <param name="newEntity"></param>
		/// <param name="unit"></param>
		protected internal virtual void EntityChanged(object newEntity, WorkUnit unit)
		{
			if(newEntity == null)
				throw new ArgumentNullException("newEntity");
			if(unit == null)
				throw new ArgumentNullException("unit");
			
			// auto?
			if(this.AutoUpdateOnChange)
				this.SetInnerObject(newEntity);
		}

		/// <summary>
		/// Gets or sets the autoupdateonchange
		/// </summary>
		public bool AutoUpdateOnChange
		{
			get
			{
				return _autoUpdateOnChange;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _autoUpdateOnChange)
				{
					// set the value...
					_autoUpdateOnChange = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the imageassembly
		/// </summary>
		public Assembly ImageResourceAssembly
		{
			get
			{
				if(_imageResourceAssembly == null)
					return this.GetType().Assembly;
				else
					return _imageResourceAssembly;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageResourceAssembly)
				{
					// set the value...
					_imageResourceAssembly = value;

					// update...
					this.RefreshIcon();
				}
			}
		}

		/// <summary>
		/// Gets or sets the imageresourcename
		/// </summary>
		public string ImageResourceName
		{
			get
			{
				return _imageResourceName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageResourceName)
				{
					// set the value...
					_imageResourceName = value;

					// update...
					this.RefreshIcon();
				}
			}
		}

		/// <summary>
		/// Gets the wrapper.
		/// </summary>
		public new TreeNodeCollectionWrapper Nodes
		{
			get
			{
				// returns the value...
				return _wrapper;
			}
		}

		internal void Added()
		{
			this.RefreshIcon();
		}

		/// <summary>
		/// Gets commands for the node.
		/// </summary>
		/// <returns></returns>
		public virtual CommandCollection GetCommands()
		{
			// do we have an entity?
			CommandCollection commands = new CommandCollection();
			commands.AddRange(this.Commands);

			// entities...
			if(this.InnerObjectIsEntity)
				commands.AddRange(Command.GetCommandsFromCommandBundles(this.InnerObjectEntityType));

			// common..
			if(this._commonCommands != null)
				commands.AddRange(this.CommonCommands.GetCommands());

			// return...
			return commands;
		}

		/// <summary>
		/// Gets a collection of Command objects.
		/// </summary>
		public CommandCollection Commands
		{
			get
			{
				return _commands;
			}
		}

		/// <summary>
		/// Gets the commoncommandhandler.
		/// </summary>
		public CommonCommandHandler CommonCommands
		{
			get
			{
				// returns the value...
				if(_commonCommands == null)
					_commonCommands = new CommonCommandHandler();
				return _commonCommands;
			}
		}

		internal bool InnerObjectIsEntity
		{
			get
			{
				if(this.InnerObjectEntityType != null)
					return true;
				else
					return false;
			}
		}
	}
}
