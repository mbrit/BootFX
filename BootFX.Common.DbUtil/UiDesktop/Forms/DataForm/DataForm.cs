// BootFX - Application framework for .NET applications
// 
// File: DataForm.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.UI.DataBinding;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.Xml;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines a form that provides access to a set of data.
	/// </summary>
	public class DataForm : BaseForm, IEntityDataControl
	{
		/// <summary>
		/// Raised when the <c>Position</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Position property has changed.")]
		public event EventHandler PositionChanged;
		
		/// <summary>
		/// Raised when the <c>Current</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Current property has changed.")]
		public event EventHandler CurrentChanged;
		
		/// <summary>
		/// Raised when the <c>MetaData</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the MetaData property has changed.")]
		public event EventHandler MetaDataChanged;
		
		/// <summary>
		/// Private field to support <c>EntityBindingManager</c> property.
		/// </summary>
		private EntityBindingManager _entityBindingManager;
		
		/// <summary>
		/// Raised when the <c>DataSource</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the DataSource property has changed.")]
		public event EventHandler DataSourceChanged;
		
		/// <summary>
		/// Private field to support <c>DataSource</c> property.
		/// </summary>
		private object _dataSource;
		private BootFX.Common.UI.Desktop.VcrButtons vcrbuttons;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuView;
		private System.Windows.Forms.MenuItem menuViewShowXml;
		private System.Windows.Forms.MenuItem menuFileSave;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public DataForm()
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
			this.vcrbuttons = new BootFX.Common.UI.Desktop.VcrButtons();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuFileSave = new System.Windows.Forms.MenuItem();
			this.menuView = new System.Windows.Forms.MenuItem();
			this.menuViewShowXml = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// vcrbuttons
			// 
			this.vcrbuttons.BindingManager = null;
			this.vcrbuttons.Dock = System.Windows.Forms.DockStyle.Top;
			this.vcrbuttons.Location = new System.Drawing.Point(0, 0);
			this.vcrbuttons.Name = "vcrbuttons";
			this.vcrbuttons.Position = 0;
			this.vcrbuttons.Size = new System.Drawing.Size(776, 36);
			this.vcrbuttons.TabIndex = 0;
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuView});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuFileSave});
			this.menuItem1.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuItem1.Text = "&File";
			// 
			// menuFileSave
			// 
			this.menuFileSave.Index = 0;
			this.menuFileSave.Text = "&Save";
			this.menuFileSave.Click += new System.EventHandler(this.menuFileSave_Click);
			// 
			// menuView
			// 
			this.menuView.Index = 1;
			this.menuView.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuViewShowXml});
			this.menuView.MergeOrder = 1;
			this.menuView.MergeType = System.Windows.Forms.MenuMerge.MergeItems;
			this.menuView.Text = "&View";
			// 
			// menuViewShowXml
			// 
			this.menuViewShowXml.Index = 0;
			this.menuViewShowXml.Text = "&Show XML...";
			this.menuViewShowXml.Click += new System.EventHandler(this.menuViewShowXml_Click);
			// 
			// DataForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(776, 478);
			this.Controls.Add(this.vcrbuttons);
			this.Menu = this.mainMenu1;
			this.Name = "DataForm";
			this.Text = "DataForm";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Gets or sets the datasource
		/// </summary>
		public object DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _dataSource)
				{
					// unbind...
					UnsubscribeBindingManager();

					// set the value...
					_dataSource = value;

					// unbind...
					SubscribeBindingManager();

					// update...
					UpdateDataBindings();

					// fire...
					this.OnDataSourceChanged();
					this.OnPositionChanged();
					this.OnCurrentChanged();
				}
			}
		}

		/// <summary>
		/// Subscribes to binding manager events.
		/// </summary>
		private void UnsubscribeBindingManager()
		{
			if(this.BindingManager == null)
				return;

			// add...
			this.BindingManager.PositionChanged -= new EventHandler(BindingManager_PositionChanged);
			this.BindingManager.CurrentChanged -= new EventHandler(BindingManager_CurrentChanged);

			// currency...
			if(this.CurrencyBindingManager != null)
			{
				this.CurrencyBindingManager.ItemChanged -= new ItemChangedEventHandler(CurrencyBindingManager_ItemChanged);
				this.CurrencyBindingManager.MetaDataChanged -= new EventHandler(CurrencyBindingManager_MetaDataChanged);
			}
		}

		/// <summary>
		/// Subscribes to binding manager events.
		/// </summary>
		private void SubscribeBindingManager()
		{
			if(this.BindingManager == null)
				return;

			// add...
			this.BindingManager.PositionChanged += new EventHandler(BindingManager_PositionChanged);
			this.BindingManager.CurrentChanged += new EventHandler(BindingManager_CurrentChanged);

			// currency...
			if(this.CurrencyBindingManager != null)
			{
				this.CurrencyBindingManager.ItemChanged += new ItemChangedEventHandler(CurrencyBindingManager_ItemChanged);
				this.CurrencyBindingManager.MetaDataChanged += new EventHandler(CurrencyBindingManager_MetaDataChanged);
			}
		}

		CurrencyManager IDataControl.CurrencyBindingManager
		{
			get
			{
				return this.CurrencyBindingManager;
			}
		}

		/// <summary>
		/// Gets the binding manager.
		/// </summary>
		protected CurrencyManager CurrencyBindingManager
		{
			get
			{
				return this.BindingManager as CurrencyManager;
			}
		}

		PropertyManager IDataControl.PropertyBindingManager
		{
			get
			{
				return this.PropertyBindingManager;
			}
		}

		/// <summary>
		/// Gets the binding manager.
		/// </summary>
		protected PropertyManager PropertyBindingManager
		{
			get
			{
				return this.BindingManager as PropertyManager;
			}
		}

		BindingManagerBase IDataControl.BindingManager
		{
			get
			{
				return this.BindingManager;
			}
		}

		/// <summary>
		/// Gets the binding manager.
		/// </summary>
		protected BindingManagerBase BindingManager
		{
			get
			{
				if(this.DataSource != null)
					return this.BindingContext[this.DataSource];
				else
					return null;
			}
		}

		/// <summary>
		/// Updates the data bindings for the form.
		/// </summary>
		private void UpdateDataBindings()
		{
			// set the bindingmanager...
			this.vcrbuttons.BindingManager = this.BindingManager;

			// set...
			_entityBindingManager = new EntityBindingManager(this.DataSource);

			// update the tag bindings..
			this.EntityBindingManager.BindControl(this);
		}

		EntityBindingManager IEntityDataControl.EntityBindingManager
		{
			get
			{
				return this.EntityBindingManager;
			}
		}

		/// <summary>
		/// Gets the entitybindingmanager.
		/// </summary>
		protected EntityBindingManager EntityBindingManager
		{
			get
			{
				return _entityBindingManager;
			}
		}

		private void menuViewShowXml_Click(object sender, System.EventArgs e)
		{
			ShowXml();
		}

		/// <summary>
		/// Shows the data source as XML.
		/// </summary>
		private void ShowXml()
		{
			Cursor oldCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			string xml = null;
			try
			{
				// convert...
				Type type = this.DataSource.GetType();
				XmlPersister persister = new XmlPersister(type, type.Name, Encoding.Unicode);
				xml = persister.ToXml(this.DataSource);
			}
			finally
			{
				this.Cursor = oldCursor;
			}

			// show...
			Alert.ShowInformation(this, xml);
		}

		private void menuFileSave_Click(object sender, System.EventArgs e)
		{
			try
			{
				Save();
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(this, "Failed to save changes.", ex);
			}
		}

		/// <summary>
		/// Saves changes to the database.
		/// </summary>
		private void Save()
		{
			if(this.DataSource == null)
				return;

			// cursor...
			Cursor oldCursor = this.Cursor;
			this.Cursor = Cursors.WaitCursor;
			try
			{
				// finish the edit...
				if(BindingManager == null)
					throw new ArgumentNullException("BindingManager");
				this.BindingManager.EndCurrentEdit();

				// save...
				EntityPersistenceHelper.Current.SaveChanges(this.DataSource);

				// refresh the bindings to get the reconciled values re-loaded...
				this.UpdateDataBindings();
			}
			finally
			{
				this.Cursor = oldCursor;
			}
		}

		/// <summary>
		/// Raises the <c>DataSourceChanged</c> event.
		/// </summary>
		private void OnDataSourceChanged()
		{
			OnDataSourceChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>DataSourceChanged</c> event.
		/// </summary>
		protected virtual void OnDataSourceChanged(EventArgs e)
		{
			if(DataSourceChanged != null)
				DataSourceChanged(this, e);
		}

		private void BindingManager_PositionChanged(object sender, EventArgs e)
		{
			this.OnPositionChanged();
		}

		/// <summary>
		/// Raises the <c>PositionChanged</c> event.
		/// </summary>
		private void OnPositionChanged()
		{
			OnPositionChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>PositionChanged</c> event.
		/// </summary>
		protected virtual void OnPositionChanged(EventArgs e)
		{
			if(PositionChanged != null)
				PositionChanged(this, e);
		}

		private void BindingManager_CurrentChanged(object sender, EventArgs e)
		{
			this.OnCurrentChanged();
		}

		/// <summary>
		/// Raises the <c>CurrentChanged</c> event.
		/// </summary>
		private void OnCurrentChanged()
		{
			OnCurrentChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>CurrentChanged</c> event.
		/// </summary>
		protected virtual void OnCurrentChanged(EventArgs e)
		{
			if(CurrentChanged != null)
				CurrentChanged(this, e);
		}

		private void CurrencyBindingManager_ItemChanged(object sender, ItemChangedEventArgs e)
		{

		}

		private void CurrencyBindingManager_MetaDataChanged(object sender, EventArgs e)
		{
			this.OnMetaDataChanged();
		}

		/// <summary>
		/// Raises the <c>MetaDataChanged</c> event.
		/// </summary>
		private void OnMetaDataChanged()
		{
			OnMetaDataChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>MetaDataChanged</c> event.
		/// </summary>
		protected virtual void OnMetaDataChanged(EventArgs e)
		{
			if(MetaDataChanged != null)
				MetaDataChanged(this, e);
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		public int Count
		{
			get
			{
				if(this.BindingManager != null)
					return this.BindingManager.Count;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		public int Position
		{
			get
			{
				if(this.BindingManager != null)
					return this.BindingManager.Position;
				else
					return 0;
			}
			set
			{
				if(this.BindingManager != null)
					this.BindingManager.Position = value;
			}
		}

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		public void MoveFirst()
		{
			if(this.HasDataSource)
				this.Position = 0;
		}

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		public void MoveLast()
		{
			if(this.HasDataSource)
				this.Position = this.Count - 1;
		}

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		public void MovePrevious()
		{
			if(this.HasDataSource)
				this.Position--;
		}

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		public void MoveNext()
		{
			if(this.HasDataSource)
				this.Position++;
		}

		IList IDataControlBase.DataSourceAsIList
		{
			get
			{
				return this.DataSourceAsIList;
			}
		}

		/// <summary>
		/// Gets the data source as an <c>IList</c>.
		/// </summary>
		protected IList DataSourceAsIList
		{
			get
			{
				return this.DataSource as IList;
			}
		}

		IBindingList IDataControlBase.DataSourceAsIBindingList
		{
			get
			{
				return this.DataSourceAsIBindingList;
			}
		}

		/// <summary>
		/// Gets the data source as an <c>IBindingList</c>.
		/// </summary>
		protected IBindingList DataSourceAsIBindingList
		{
			get
			{
				return this.DataSource as IBindingList;
			}
		}

		/// <summary>
		/// Returns true if the control has a datasource.
		/// </summary>
		public bool HasDataSource
		{
			get
			{
				if(this.DataSource != null)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				IEntityType deferredEntityType = this.DataSource as IEntityType;
				if(deferredEntityType != null)
					return deferredEntityType.EntityType;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public object Current
		{
			get
			{
				if(this.BindingManager != null)
					return this.BindingManager.Current;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the current entity.
		/// </summary>
		object IEntityDataControl.CurrentEntity
		{
			get
			{
				return this.CurrentEntity;
			}
		}

		/// <summary>
		/// Gets the current entity.
		/// </summary>
		protected object CurrentEntity
		{
			get
			{
				return EntityView.Unwrap(this.Current);
			}
		}

		/// <summary>
		/// Gets the entity type of <c>CurrentEntity</c>.  This will return null if the entity type is not found.
		/// </summary>
		EntityType IEntityDataControl.CurrentEntityType
		{
			get
			{
				return this.CurrentEntityType;
			}
		}

		/// <summary>
		/// Gets the entity type of <c>CurrentEntity</c>.  This will return null if the entity type is not found.
		/// </summary>
		protected EntityType CurrentEntityType
		{
			get
			{
				return this.GetCurrentEntityType(OnNotFound.ReturnNull);
			}
		}

		EntityType IEntityDataControl.GetCurrentEntityType(OnNotFound onNotFound)
		{
			return this.GetCurrentEntityType(onNotFound);
		}

		/// <summary>
		/// Gets the entity type of <c>CurrentEntity</c>.
		/// </summary>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		protected EntityType GetCurrentEntityType(OnNotFound onNotFound)
		{
			object entity = this.CurrentEntity;
			if(entity != null)
				return EntityType.GetEntityType(entity, onNotFound);
			else
				return null;
		}

		/// <summary>
		/// Returns true if the datasource is a list, false if the object is null, or the object is non-null and does not implement <see cref="IEnumerable"></see>.
		/// </summary>
		public bool IsBindingToList
		{
			get
			{
				if(this.DataSource != null && this.DataSource is IEnumerable)
					return true;
				else
					return false;
			}
		}
	}
}
