// BootFX - Application framework for .NET applications
// 
// File: EntitySearchControl.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using BootFX.Common;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EntitySearchControl.
	/// </summary>
	public class EntitySearchControl : System.Windows.Forms.UserControl
	{
		/// <summary>
		/// Raised when the search was completed.
		/// </summary>
		public event EventHandler SearchCompleted;
		
		/// <summary>
		/// Private field to support <c>MruItems</c> property.
		/// </summary>
		private MruList _mruItems = new MruList();
		
		/// <summary>
		/// Private field to support <see cref="ViewExpressions"/> property.
		/// </summary>
		private ArrayList _viewExpressions = new ArrayList();
		
		/// <summary>
		/// Raised when the user double-clicks an entity.
		/// </summary>
		public event EntityEventHandler EntityDoubleClick;

		private bool _editOnDoubleClick = true;
		private EntityType _entityType;
		private System.Windows.Forms.GroupBox groupBox2;
		private BootFX.Common.UI.Desktop.EntityListView listResults;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button buttonSearch;
		private System.Windows.Forms.ComboBox comboSearchFor;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EntitySearchControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.listResults = new BootFX.Common.UI.Desktop.EntityListView();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.buttonSearch = new System.Windows.Forms.Button();
			this.comboSearchFor = new System.Windows.Forms.ComboBox();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.listResults);
			this.groupBox2.Location = new System.Drawing.Point(4, 60);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(356, 164);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "&Results:";
			// 
			// listResults
			// 
			this.listResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.listResults.DataSource = null;
			this.listResults.FullRowSelect = true;
			this.listResults.HideSelection = false;
			this.listResults.Location = new System.Drawing.Point(12, 20);
			this.listResults.Name = "listResults";
			this.listResults.Size = new System.Drawing.Size(336, 136);
			this.listResults.TabIndex = 0;
			this.listResults.View = System.Windows.Forms.View.Details;
			this.listResults.EntityDoubleClick += new BootFX.Common.Entities.EntityEventHandler(this.listResults_EntityDoubleClick);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.buttonSearch);
			this.groupBox1.Controls.Add(this.comboSearchFor);
			this.groupBox1.Location = new System.Drawing.Point(4, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(356, 48);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Search &for:";
			// 
			// buttonSearch
			// 
			this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonSearch.Enabled = false;
			this.buttonSearch.Location = new System.Drawing.Point(272, 20);
			this.buttonSearch.Name = "buttonSearch";
			this.buttonSearch.TabIndex = 1;
			this.buttonSearch.Text = "&Search";
			this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
			// 
			// comboSearchFor
			// 
			this.comboSearchFor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboSearchFor.Enabled = false;
			this.comboSearchFor.Location = new System.Drawing.Point(12, 20);
			this.comboSearchFor.Name = "comboSearchFor";
			this.comboSearchFor.Size = new System.Drawing.Size(252, 21);
			this.comboSearchFor.TabIndex = 0;
			this.comboSearchFor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.comboSearchFor_KeyUp);
			this.comboSearchFor.SelectedIndexChanged += new System.EventHandler(this.comboSearchFor_SelectedIndexChanged);
			// 
			// EntitySearchControl
			// 
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "EntitySearchControl";
			this.Size = new System.Drawing.Size(364, 224);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void buttonSearch_Click(object sender, System.EventArgs e)
		{
			Search();
		}

		/// <summary>
		/// Searches the database.
		/// </summary>
		private void Search()
		{
			if(SqlSearcher.AreTermsValid(this.comboSearchFor.Text))
			{
				// create...
				SqlSearcher searcher = new SqlSearcher(this.EntityType, this.comboSearchFor.Text);
				IList results = searcher.ExecuteEntityCollection();
				if(results == null)
					throw new InvalidOperationException("results is null.");

				// show...
				if(this.ViewExpressions.Count == 0)
					this.listResults.DataSource = results;
				else
					this.listResults.DataSource = new EntityViewCollection(this.EntityType, results, (object[])this.ViewExpressions.ToArray(typeof(object)));

				// set...
				this.MruItems.Push(this.comboSearchFor.Text);
				this.RefreshMruItems();

				// ok...
				this.OnSearchCompleted();
			}
			else
				Alert.ShowWarning(this, "The search terms you specified are invalid.");
		}

		private void listResults_EntityDoubleClick(object sender, BootFX.Common.Entities.EntityEventArgs e)
		{
			HandleDoubleClick(e.Entity);
		}

		/// <summary>
		/// Handles an entity double-click.
		/// </summary>
		private void HandleDoubleClick(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// if...
			if(this.EditOnDoubleClick)
			{
				//				EntityDesktopCommandHelper.Current.EditEntity(this, entity);
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
			}
			else
				this.OnEntityDoubleClick(new EntityEventArgs(entity));
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
		/// Gets or sets the entitytype
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _entityType)
				{
					// set the value...
					_entityType = value;

					// set...
					bool enable = true;
					if(this.EntityType == null)
						enable = false;
					this.comboSearchFor.Enabled = enable;
					this.buttonSearch.Enabled = enable;
				}
			}
		}

		/// <summary>
		/// Gets or sets whether to edit the entity on a double-click.
		/// </summary>
		[Browsable(true), Category("Behavior"), Description("Gets or sets whether to edit the entity on a double-click."), DefaultValue(true)]
		public bool EditOnDoubleClick
		{
			get
			{
				return _editOnDoubleClick;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _editOnDoubleClick)
				{
					// set the value...
					_editOnDoubleClick = value;
				}
			}
		}

		/// <summary>
		/// Gets the viewexpressions.
		/// </summary>
		public ArrayList ViewExpressions
		{
			get
			{
				return _viewExpressions;
			}
		}

		/// <summary>
		/// Gets the mruitems.
		/// </summary>
		private MruList MruItems
		{
			get
			{
				// returns the value...
				return _mruItems;
			}
		}

		/// <summary>
		/// Updates the list with the current set of MRU items.
		/// </summary>
		private void RefreshMruItems()
		{
			if(MruItems == null)
				throw new InvalidOperationException("MruItems is null.");

			// clear...
			this.comboSearchFor.BeginUpdate();
			try
			{
				this.comboSearchFor.Items.Clear();
				foreach(string value in this.MruItems)
					this.comboSearchFor.Items.Add(value);
			}
			finally
			{
				this.comboSearchFor.EndUpdate();
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			if(this.DesignMode)
				return;

			// refresh...
			this.RefreshMruItems();
		}

		private void comboSearchFor_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.Search();
		}

		private void comboSearchFor_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			if(e.KeyCode == Keys.Return)
			{
				this.Search();
				e.Handled = true;
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
				return this.listResults.SelectedEntities;
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
				return this.listResults.SelectedEntity;
			}
		}

		/// <summary>
		/// Raises the <c>SearchCompleted</c> event.
		/// </summary>
		private void OnSearchCompleted()
		{
			OnSearchCompleted(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>SearchCompleted</c> event.
		/// </summary>
		protected virtual void OnSearchCompleted(EventArgs e)
		{
			// raise...
			if(SearchCompleted != null)
				SearchCompleted(this, e);
		}

		/// <summary>
		/// Selects all of the items in the list.
		/// </summary>
		public void SelectAll()
		{
			this.listResults.SelectAll();
		}

		/// <summary>
		/// Gets the found entities.
		/// </summary>
		public IList FoundEntities
		{
			get
			{
				return this.listResults.DataSource as IList;
			}
		}
	}
}
