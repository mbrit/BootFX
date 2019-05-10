// BootFX - Application framework for .NET applications
// 
// File: EntitySearchDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes a dialog that allows the user to search entities.
	/// </summary>
	public class EntitySearchDialog : BaseForm
	{
		/// <summary>
		/// Private field to support <c>ChoiceRequired</c> property.
		/// </summary>
		private bool _choiceRequired;
		
		/// <summary>
		/// Raised when the user double-clicks an entity.
		/// </summary>
		public event EntityEventHandler EntityDoubleClick;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private DialogBar buttonClose;
		private EntitySearchControl search;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntitySearchDialog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.buttonClose = new BootFX.Common.UI.Desktop.DialogBar();
			this.search = new BootFX.Common.UI.Desktop.EntitySearchControl();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			this.buttonClose.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.buttonClose.Location = new System.Drawing.Point(0, 386);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(404, 32);
			this.buttonClose.TabIndex = 2;
			// 
			// search
			// 
			this.search.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.search.EntityType = null;
			this.search.Location = new System.Drawing.Point(4, 8);
			this.search.Name = "search";
			this.search.Size = new System.Drawing.Size(396, 372);
			this.search.TabIndex = 3;
			this.search.SearchCompleted += new System.EventHandler(this.search_SearchCompleted);
			this.search.EntityDoubleClick += new BootFX.Common.Entities.EntityEventHandler(this.search_EntityDoubleClick);
			// 
			// EntitySearchDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(404, 418);
			this.Controls.Add(this.search);
			this.Controls.Add(this.buttonClose);
			this.Name = "EntitySearchDialog";
			this.Text = "Find Items";
			this.ResumeLayout(false);

		}
		#endregion

		private void search_EntityDoubleClick(object sender, BootFX.Common.Entities.EntityEventArgs e)
		{
			this.OnEntityDoubleClick(e);
		}

		/// <summary>
		/// Raises the <c>EntityDoubleClick</c> event.
		/// </summary>
		protected virtual void OnEntityDoubleClick(EntityEventArgs e)
		{
			// raise...
			if(EntityDoubleClick != null)
				EntityDoubleClick(this, e);
		}

		/// <summary>
		/// Gets or sets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return this.search.EntityType;
			}
			set
			{
				this.search.EntityType = value;
			}
		}

		/// <summary>
		/// Gets the viewexpressions.
		/// </summary>
		public ArrayList ViewExpressions
		{
			get
			{
				return search.ViewExpressions;
			}
		}

		/// <summary>
		/// Picks an entity from the list.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <param name="dataSource"></param>
		/// <param name="choiceRequired"></param>
		/// <returns></returns>
		public static object PickEntity(Control owner, string caption, Type entityType, bool choiceRequired, object[] viewExpressions)
		{
			if(caption == null)
				throw new ArgumentNullException("caption");
			if(caption.Length == 0)
				throw new ArgumentOutOfRangeException("'caption' is zero-length.");
			if(entityType == null)
				throw new ArgumentNullException("entityType");			
			if(viewExpressions == null)
				throw new ArgumentNullException("viewExpressions");

			// return...
			return PickEntity(owner, caption, EntityType.GetEntityType(entityType, OnNotFound.ThrowException), choiceRequired, viewExpressions);
		}

		/// <summary>
		/// Picks an entity from the list.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <param name="dataSource"></param>
		/// <param name="choiceRequired"></param>
		/// <returns></returns>
		public static object PickEntity(Control owner, string caption, EntityType entityType, bool choiceRequired, object[] viewExpressions)
		{
			if(caption == null)
				throw new ArgumentNullException("caption");
			if(caption.Length == 0)
				throw new ArgumentOutOfRangeException("'caption' is zero-length.");
			if(entityType == null)
				throw new ArgumentNullException("entityType");			

			// create...
			EntitySearchDialog dialog = new EntitySearchDialog();
			dialog.EntityType = entityType;
			dialog.Text = caption;
			dialog.ChoiceRequired = choiceRequired;
			dialog.ViewExpressions.AddRange(viewExpressions);
			if(dialog.ShowDialog(owner) == DialogResult.OK)
				return dialog.SelectedEntity;
			else
				return null;
		}

		protected override void OnApplying(CancelEventArgs e)
		{
			base.OnApplying (e);

			// check...
			if(this.SelectedEntities.Count == 0)
			{
				// required?
				if(this.ChoiceRequired)
				{
					Alert.ShowWarning(this, "You must select an item.");
					e.Cancel = true;
					return;
				}
			}
		}

		private void search_SearchCompleted(object sender, System.EventArgs e)
		{
			if(this.search.FoundEntities != null && this.search.FoundEntities.Count == 1)
				this.search.SelectAll();
		}

		/// <summary>
		/// Gets or sets whether the user much make a selection in order to click the OK button.
		/// </summary>
		[Browsable(false), Category("Behavior"), DefaultValue(false), Description("Gets or sets whether the user much make a selection in order to click the OK button.")]
		public bool ChoiceRequired
		{
			get
			{
				return _choiceRequired;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _choiceRequired)
				{
					// set the value...
					_choiceRequired = value;
				}
			}
		}

		/// <summary>
		/// Gets the selected entities.
		/// </summary>
		[Browsable(false)]
		public IList SelectedEntities
		{
			get
			{
				return this.search.SelectedEntities;
			}
		}

		/// <summary>
		/// Gets the selected entity.
		/// </summary>
		[Browsable(false)]
		public object SelectedEntity
		{
			get
			{
				return this.search.SelectedEntity;
			}
		}

		/// <summary>
		/// Finds entities of the given type.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <param name="entityType"></param>
		/// <param name="choiceRequired"></param>
		/// <param name="viewExpressions"></param>
		/// <returns></returns>
		public static IList FindEntities(Control owner, string caption, Type entityType, bool choiceRequired, IList viewExpressions)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			return FindEntities(owner, caption, EntityType.GetEntityType(entityType, OnNotFound.ThrowException), choiceRequired, viewExpressions);
		}

		/// <summary>
		/// Finds entities of the given type.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="caption"></param>
		/// <param name="entityType"></param>
		/// <param name="choiceRequired"></param>
		/// <param name="viewExpressions"></param>
		/// <returns></returns>
		public static IList FindEntities(Control owner, string caption, EntityType entityType, bool choiceRequired, IList viewExpressions)
		{
			if(caption == null)
				throw new ArgumentNullException("caption");
			if(caption.Length == 0)
				throw new ArgumentOutOfRangeException("'caption' is zero-length.");
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(viewExpressions == null)
				throw new ArgumentNullException("viewExpressions");
			
			// show...
			EntitySearchDialog dialog = new EntitySearchDialog();
			dialog.ViewExpressions.Clear();
			dialog.ViewExpressions.AddRange(viewExpressions);
			dialog.EntityType = entityType;
			dialog.Text = caption;
			dialog.ChoiceRequired = choiceRequired;
			if(dialog.ShowDialog(owner) == DialogResult.OK)
				return dialog.SelectedEntities;
			else
				return null;
		}
	}
}
