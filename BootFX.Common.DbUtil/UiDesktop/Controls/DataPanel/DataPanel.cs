// BootFX - Application framework for .NET applications
// 
// File: DataPanel.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Collections;
using System.ComponentModel;
using BootFX.Common.UI.DataBinding;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes a control that allows for data binding.
	/// </summary>
	public class DataPanel : UserControl, IEntityDataControl
	{
		/// <summary>
		/// Raised when the <c>DataSource</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the DataSource property has changed.")]
		public event EventHandler DataSourceChanged;
		
		/// <summary>
		/// Delegate for <see cref="OnMetaDataChanged"/>.
		/// </summary>
		private delegate void OnMetaDataChangedDelegate(EventArgs e);
		
		/// <summary>
		/// Raised when the metadata for the item changes.
		/// </summary>
		public event EventHandler MetaDataChanged;
		
		/// <summary>
		/// Delegate for <see cref="OnCurrentChanged"/>.
		/// </summary>
		private delegate void OnCurrentChangedDelegate(EventArgs e);
		
		/// <summary>
		/// Raised when the current item changes.
		/// </summary>
		public event EventHandler CurrentChanged;
		
		/// <summary>
		/// Delegate for <see cref="OnPositionChanged"/>.
		/// </summary>
		private delegate void OnPositionChangedDelegate(EventArgs e);
		
		/// <summary>
		/// Raised when the position in the list changes.
		/// </summary>
		public event EventHandler PositionChanged;
		
		/// <summary>
		/// Private field to support <c>EntityBindingManager</c> property.
		/// </summary>
		private EntityBindingManager _entityBindingManager;
		
		/// <summary>
		/// Private field to support <c>DataSource</c> property.
		/// </summary>
		private object _dataSource;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public DataPanel()
		{
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
			// flip...
			if(InvokeRequired)
			{
				OnMetaDataChangedDelegate d = new OnMetaDataChangedDelegate(OnMetaDataChanged);
				Invoke(d, new object[] { e });
				return;
			}
		
			// raise...
			if(MetaDataChanged != null)
				MetaDataChanged(this, e);
		}

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
				{
					if(BindingContext == null)
						throw new InvalidOperationException("BindingContext is null.");
					return this.BindingContext[this.DataSource];
				}
				else
					return null;
			}
		}

		/// <summary>
		/// Updates the data bindings for the form.
		/// </summary>
		private void UpdateDataBindings()
		{
			// set...
			_entityBindingManager = new EntityBindingManager(this.DataSource);

			// update the tag bindings..
			this.EntityBindingManager.BindControl(this);
		}

		/// <summary>
		/// Gets the number of items in the datasource.
		/// </summary>
		public int Count
		{
			get
			{
				if(this.DataSource != null)
				{
					if(this.IsBindingToList)
					{
						// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
						throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
					}
					else
						return 1;
				}
				else
					return 0;
			}
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
			// flip...
			if(InvokeRequired)
			{
				OnCurrentChangedDelegate d = new OnCurrentChangedDelegate(OnCurrentChanged);
				Invoke(d, new object[] { e });
				return;
			}
		
			// raise...
			if(CurrentChanged != null)
				CurrentChanged(this, e);
		}

		/// <summary>
		/// Returns true if a data source is set.
		/// </summary>
		public bool HasDataSource
		{
			get
			{
				if(this.DataSource == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		public int Position
		{
			get
			{
				if(this.DataSource != null)
				{
					if(this.IsBindingToList)
					{
						// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
						throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
					}
					else
						return 0;
				}
				else
					return 0;
			}
			set
			{
			}
		}

		public void MoveFirst()
		{
			if(this.IsBindingToList)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
			}
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
			// flip...
			if(InvokeRequired)
			{
				OnPositionChangedDelegate d = new OnPositionChangedDelegate(OnPositionChanged);
				Invoke(d, new object[] { e });
				return;
			}
		
			// raise...
			if(PositionChanged != null)
				PositionChanged(this, e);
		}

		IBindingList IDataControlBase.DataSourceAsIBindingList
		{
			get
			{
				return DataSourceAsIBindingList;
			}
		}

		/// <summary>
		/// Gets the data source as a binding list.
		/// </summary>
		protected IBindingList DataSourceAsIBindingList
		{
			get
			{
				return this.DataSource as IBindingList;
			}
		}

		public void MovePrevious()
		{
			if(this.IsBindingToList)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
			}
		}

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public object Current
		{
			get
			{
				if(this.DataSource != null)
				{
					if(this.IsBindingToList)
					{
						// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
						throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
					}
					else
						return this.DataSource;
				}
				else
					return null;
			}
		}

		IList IDataControlBase.DataSourceAsIList
		{
			get
			{
				return this.DataSourceAsIList;
			}
		}

		/// <summary>
		/// Gets the data source as a list.
		/// </summary>
		protected IList DataSourceAsIList
		{
			get
			{
				return this.DataSource as IList;
			}
		}

		public void MoveLast()
		{
			if(this.IsBindingToList)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
			}
		}

		public void MoveNext()
		{
			if(this.IsBindingToList)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
			}
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

		private void BindingManager_CurrentChanged(object sender, EventArgs e)
		{
			this.OnCurrentChanged();
		}

		private void BindingManager_PositionChanged(object sender, EventArgs e)
		{
			this.OnPositionChanged();
		}

		private void CurrencyBindingManager_ItemChanged(object sender, ItemChangedEventArgs e)
		{

		}

		private void CurrencyBindingManager_MetaDataChanged(object sender, EventArgs e)
		{
			this.OnMetaDataChanged();
		}
	}
}
