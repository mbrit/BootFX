// BootFX - Application framework for .NET applications
// 
// File: EntityListView.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Data.Comparers;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Extends <see cref="ListView"></see> to allow for data to be bound.
	/// </summary>
	/// <remarks>Despite the name, the data source used with this control does not need to map to entities.  For example, this will work with
	/// <see cref="DataTable"></see> instances.</remarks>
	// mbr - 06-04-2006 - changed to extend ListViewEx.	
	public class EntityListView : ListViewEx, IEntityType
	{
		/// <summary>
		/// Delegate for use with <see cref="RefreshItems"></see>.
		/// </summary>
		private delegate void RefreshItemsDelegate();
			
		/// <summary>
		/// Raised when the <c>Filter</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the Filter property has changed.")]
		public event EventHandler FilterChanged;
		
		/// <summary>
		/// Private field to support <c>Filter</c> property.
		/// </summary>
		private string _filter;
		
		/// <summary>
		/// Raised when an entity was right-clicked by the user.
		/// </summary>
		[Browsable(true), Description("Raised when an entity was right-clicked by the user."), Category("Behavior")]
		public event EntityEventHandler EntityRightClick;
		
		/// <summary>
		/// Raised when an entity was double clicked.
		/// </summary>
		[Browsable(true), Description("Raised when an entity was double clicked."), Category("Behavior")]
		public event EntityEventHandler EntityDoubleClick;
		
		/// <summary>
		/// Private field to support <c>LastButtonUp</c> property.
		/// </summary>
		private MouseButtons _lastButtonUp;
		
		/// <summary>
		/// Raised when the <c>DataSource</c> property has changed.
		/// </summary>
		[Browsable(true), Category("Property Changed"), Description("Raised when the DataSource property has changed.")]
		public event EventHandler DataSourceChanged;
		
		/// <summary>
		/// Private field to support <c>SortColumn</c> property.
		/// </summary>
		private EntityListViewColumnHeader _sortColumn;
		
		/// <summary>
		/// Private field to support <c>DataSource</c> property.
		/// </summary>
		private object _dataSource;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityListView()
		{
			this.View = View.Details;
			this.HideSelection = false;
		}

		/// <summary>
		/// Gets or sets the datasource
		/// </summary>
		[Browsable(false)]
		public object DataSource
		{
			get
			{
				return _dataSource;
			}
			set
			{
				// validate...
				if(value != null)
				{
					// list source?
					if(value is IListSource)
						value = ((IListSource)value).GetList();

					// check...
					if(!(value is ICollection))
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "An object of type '{0}' cannot be used as a data source.", value));
				}

				// check to see if the value has changed...
				if(value != _dataSource)
				{
					// set the value...
					_dataSource = value;

					// try...
					this.BeginUpdate();
					try
					{
						// setup...
						BindDataSource();

						// update...
						this.AutoSizeColumns();

						// after...
						this.OnDataSourceChanged();
					}
					finally
					{
						this.EndUpdate();
					}
				}
			}
		}

		/// <summary>
		/// Returns true if the list has a data source.
		/// </summary>
		protected bool HasDataSource
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
		/// Gets the data source as a IBindingList.
		/// </summary>
		protected IBindingList DataSourceAsIBindingList
		{
			get
			{
				return this.DataSource as IBindingList;
			}
		}

		/// <summary>
		/// Gets the data source as an IList.
		/// </summary>
		protected ICollection DataSourceAsICollection
		{
			get
			{
				return this.DataSource as ICollection;
			}
		}

		/// <summary>
		/// Gets the data source as an IList.
		/// </summary>
		protected IEntityCollection DataSourceAsIEntityCollection
		{
			get
			{
				return this.DataSource as IEntityCollection;
			}
		}

		/// <summary>
		/// Gets the data source as an IList.
		/// </summary>
		protected EntityViewCollection DataSourceAsEntityCollectionView
		{
			get
			{
				return this.DataSource as EntityViewCollection;
			}
		}

		/// <summary>
		/// Gets or sets the filter
		/// </summary>
		public string Filter
		{
			get
			{
				return _filter;
			}
			set
			{
				if(value != null && value.Length == 0)
					value = null;
				else
				{
					if(value != null)
						value = value.ToLower();
				}

				// check to see if the value has changed...
				if(value != _filter)
				{
					// set the value...
					_filter = value;
					this.OnFilterChanged();
					this.RefreshItems();
				}
			}
		}

		/// <summary>
		/// Raises the <c>FilterChanged</c> event.
		/// </summary>
		private void OnFilterChanged()
		{
			OnFilterChanged(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>FilterChanged</c> event.
		/// </summary>
		protected virtual void OnFilterChanged(EventArgs e)
		{
			if(FilterChanged != null)
				FilterChanged(this, e);
		}

		/// <summary>
		/// Binds the given data source.
		/// </summary>
		private void BindDataSource()
		{
			this.BeginUpdate();
			try
			{
				// setup the columns...
				RefreshColumns();

				// setup the items...
				RefreshItems();

				// make decisions...
				if(this.HasDataSource)
				{
					// sortable?
					bool allowSort = true;
					if(this.DataSourceAsIBindingList != null)
						allowSort = this.DataSourceAsIBindingList.SupportsSorting;

					// column header...
					if(allowSort == true)
						this.HeaderStyle = ColumnHeaderStyle.Clickable;
					else
						this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		/// <summary>
		/// Updates the items for the list.
		/// </summary>
		private void RefreshItems()
		{
			// flip...
			if(this.InvokeRequired)
			{
				// flip...
				RefreshItemsDelegate d = new RefreshItemsDelegate(this.RefreshItems);
				this.Invoke(d, new object[] {});
				return;
			}

			// try...
			this.BeginUpdate();
			try
			{
				this.Items.Clear();
				if(this.HasDataSource == false)
					return;

				// check...
				if(DataSourceAsICollection == null)
					throw new ArgumentNullException("DataSourceAsICollection");

				// walk...
				foreach(object entity in this.DataSourceAsICollection)
				{
					// should be displayed...
					EntityListViewItem item = new EntityListViewItem(this, entity);
					bool ok = false;
					if(this.HasFilter)
					{
						// check...
						foreach(ListViewItem.ListViewSubItem subItem in item.SubItems)
						{
							string text = subItem.Text;
							if(text != null && text.Length > 0)
							{
								if(text.ToLower().IndexOf(this.Filter) != -1)
								{
									ok = true;
									break;
								}
							}
						}
					}
					else
						ok = true;

					// add it...
					if(ok)
						this.Items.Add(item);
				}
			}
			finally
			{
				this.EndUpdate();
			}
		}

		/// <summary>
		/// Returns true if there is a filter.
		/// </summary>
		private bool HasFilter
		{
			get
			{
				if(this.Filter == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Updates the columns for the list.
		/// </summary>
		private void RefreshColumns()
		{
			// clear...
			if(this.HasDataSource == false)
				return;

			// update...
			this.BeginUpdate();
			try
			{
				// TODO: only clear the columns if the *property specification* has changed...
				this.Columns.Clear();

				// get the columns...
				PropertyDescriptorCollection properties = this.GetPropertyDescriptors();
				foreach(PropertyDescriptor property in properties)
					this.Columns.Add(new EntityListViewColumnHeader(property));
			}
			finally
			{
				this.EndUpdate();
			}
		}

		/// <summary>
		/// Gets the properties to display.
		/// </summary>
		/// <returns></returns>
		private PropertyDescriptorCollection GetPropertyDescriptors()
		{
			// get them...
			if(this.DataSource != null)
			{
				// binding?
				if(this.DataSource is ITypedList)
					return ((ITypedList)this.DataSource).GetItemProperties(new PropertyDescriptor[] {});
				else if(this.DataSource is IDictionary)
					return new PropertyDescriptorCollection(new PropertyDescriptor[] { new DictionaryPropertyDescriptor(true), new DictionaryPropertyDescriptor(false) });
				else if(this.DataSource is ICollection)
					return new PropertyDescriptorCollection(new PropertyDescriptor[] { new ListPropertyDescriptor() });
				else
					throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot get properties for '{0}'.", this.DataSource.GetType()));
			}
			else
				return new PropertyDescriptorCollection(new PropertyDescriptor[] {});
		}

		/// <summary>
		/// Sorts by the given column index.
		/// </summary>
		/// <param name="columnIndex"></param>
		public void Sort(int columnIndex)
		{
			ColumnHeader header = this.Columns[columnIndex];
			if(header == null)
				throw new ArgumentNullException("header");

			// sort...
			if(header is EntityListViewColumnHeader)
				this.Sort((EntityListViewColumnHeader)header);
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", header.GetType()));
		}

		/// <summary>
		/// Sorts by the given column index.
		/// </summary>
		/// <param name="header"></param>
		private void Sort(EntityListViewColumnHeader header)
		{
			if(header == null)
				throw new ArgumentNullException("header");

			// property...
			PropertyDescriptor property = header.Property;
			if(property == null)
				throw new ArgumentNullException("property");

			// get a comparer...
			IComparer comparer = null;
			if(property is EntityFieldPropertyDescriptor)
				comparer = ((EntityFieldPropertyDescriptor)property).Field.GetComparer(Cultures.User);
			else
				comparer = ComparerBase.GetComparer(property.PropertyType, Cultures.User);

			// check...
			SortOrder useOrder = SortOrder.Ascending;
			if(this.SortColumn == header)
			{
				if(this.Sorting == SortOrder.Ascending)
					useOrder = SortOrder.Descending;
				else
					useOrder = SortOrder.Ascending;
			}

			// sort...
			SortDirection direction = SortDirection.Ascending;
			if(useOrder == SortOrder.Descending)
				direction = SortDirection.Descending;
			this.ListViewItemSorter = new EntityListViewComparer(header, comparer, direction);
			this.Sorting = useOrder;
			_sortColumn = header;
		}

		/// <summary>
		/// Gets the sortcolumn.
		/// </summary>
		private EntityListViewColumnHeader SortColumn
		{
			get
			{
				// returns the value...
				return _sortColumn;
			}
		}

		/// <summary>
		/// Describes a class used for binding a list who only exposed information through <c>ToString</c>.
		/// </summary>
		private class DictionaryPropertyDescriptor : PropertyDescriptor
		{
			/// <summary>
			/// Private field to support <c>IsKey</c> property.
			/// </summary>
			private bool _isKey;
			
			/// <summary>
			/// Constructor.
			/// </summary>
			public DictionaryPropertyDescriptor(bool isKey) : base(isKey == true ? "Item" : "Value", new Attribute[] {})
			{
				_isKey = isKey;
			}

			/// <summary>
			/// Gets the iskey.
			/// </summary>
			public bool IsKey
			{
				get
				{
					return _isKey;
				}
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(IList);
				}
			}

			public override object GetValue(object component)
			{
				if(component is DictionaryEntry)
				{
					if(this.IsKey == true)
						return ((DictionaryEntry)component).Key;
					else
						return ((DictionaryEntry)component).Value;
				}
				else
					return null;
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(object);
				}
			}

			public override void ResetValue(object component)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException("The operation has not been implemented.");
			}

			public override void SetValue(object component, object value)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException("The operation has not been implemented.");
			}
			
			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}

		/// <summary>
		/// Describes a class used for binding a list who only exposed information through <c>ToString</c>.
		/// </summary>
		private class ListPropertyDescriptor : PropertyDescriptor
		{
			/// <summary>
			/// Constructor.
			/// </summary>
			public ListPropertyDescriptor() : base("Item", new Attribute[] {})
			{
			}

			public override bool CanResetValue(object component)
			{
				return false;
			}

			public override Type ComponentType
			{
				get
				{
					return typeof(IList);
				}
			}

			public override object GetValue(object component)
			{
				if(component != null)
					return component.ToString();
				else
					return null;
			}

			public override bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public override Type PropertyType
			{
				get
				{
					return typeof(object);
				}
			}

			public override void ResetValue(object component)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException("The operation has not been implemented.");
			}

			public override void SetValue(object component, object value)
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException("The operation has not been implemented.");
			}
			
			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the selected entities.
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		public IList CheckedEntities
		{
			get
			{
				return this.CreateEntityCollection(this.CheckedItems);
			}
			set
			{
				this.CheckNone();

				// update the selection...
				if(value != null && value.Count > 0)
				{
					// copy our items to a new array...
					ArrayList items = new ArrayList(this.Items);
					
					// walk the ones we were given...
					foreach(object checkEntity in value)
					{
						// walk our list...
						ArrayList toRemove = new ArrayList();
						foreach(EntityListViewItem item in items)
						{
							// match?
							object entity = item.Entity;
							if(EntityIdentification.Equals(entity, checkEntity) == true)
							{
								item.Checked = true;
								toRemove.Add(item);
							}
						}

						// remove the ones that we know about...
						foreach(EntityListViewItem item in toRemove)
							items.Remove(item);
					}
				}
			}
		}

		/// <summary>
		/// Gets the entities on the view.
		/// </summary>
		[Browsable(false)]
		public IList Entities
		{
			get
			{
				return this.CreateEntityCollection(this.Items);
			}
		}

		/// <summary>
		/// Gets the item for the given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		private EntityListViewItem GetItemForEntity(object entity)
		{
			foreach(EntityListViewItem item in this.Items)
			{
				if(item.Entity == entity)
					return item;
			}

			// nope....
			return null;
		}

		/// <summary>
		/// Refreshes the item for the given entity.
		/// </summary>
		/// <param name="entity"></param>
		public void RefreshItemForEntity(object entity)
		{
			EntityListViewItem item = this.GetItemForEntity(entity);
			if(item != null)
				RefreshItem(item);
		}

		/// <summary>
		/// Refreshes the given item.
		/// </summary>
		/// <param name="item"></param>
		public void RefreshItem(ListViewItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			if(!(item is EntityListViewItem))
				throw new InvalidOperationException(string.Format("Item is '{0}', not EntityListViewItem.", item));
			((EntityListViewItem)item).RefreshView();
		}

		/// <summary>
		/// Updates the selected items in response to the data being changed behind the item.
		/// </summary>
		public void RefreshSelectedItems()
		{
			ListViewItem[] items = new ListViewItem[this.SelectedItems.Count];
			for(int index = 0; index < this.SelectedItems.Count; index++)
				items[index] = this.SelectedItems[index];
			this.RefreshItems(items);
		}

		/// <summary>
		/// Updates the given items in response to the data being changed behind the item.
		/// </summary>
		public void RefreshItems(ListViewItem[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			
			// walk...
			foreach(EntityListViewItem item in items)
				item.RefreshView();
		}

		/// <summary>
		/// Gets the selected item.
		/// </summary>
		public ListViewItem SelectedItem
		{
			get
			{
				if(this.SelectedItems.Count > 0)
					return this.SelectedItems[0];
				else
					return null;
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
				IList list = this.SelectedEntities;
				if(list == null)
					throw new InvalidOperationException("list is null.");
				if(list.Count > 0)
					return list[0];
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the selected entities.
		/// </summary>
		/// <returns></returns>
		[Browsable(false)]
		public IList SelectedEntities
		{
			get
			{
				return this.CreateEntityCollection(this.SelectedItems);
			}
		}

		/// <summary>
		/// Creates an entity collection from the given items.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		private IList CreateEntityCollection(ICollection items)
		{
			ArrayList entities = new ArrayList();
			foreach(EntityListViewItem item in items)
				entities.Add(item.Entity);

			// return...
			return entities;
		}

		/// <summary>
		/// Gets the entity type, if any, of this collection.
		/// </summary>
		[Browsable(false)]
		public EntityType EntityType
		{
			get
			{
				if(this.DataSourceAsIEntityCollection != null)
					return this.DataSourceAsIEntityCollection.EntityType;
				if(this.DataSourceAsEntityCollectionView != null)
					return this.DataSourceAsEntityCollectionView.EntityType;
				else
					return null;
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

		// mbr - 06-04-2006 - implemented on base.		
//		/// <summary>
//		/// Sets all items in the list to be Selected.
//		/// </summary>
//		public void SelectAll()
//		{
//			ListViewHelper.SelectAll(this, true);
//		}
//
//		/// <summary>
//		/// Sets all items in the list to be unSelected.
//		/// </summary>
//		public void SelectNone()
//		{
//			ListViewHelper.SelectAll(this, false);
//		}
//
//		/// <summary>
//		/// Sets all items in the list to be checked.
//		/// </summary>
//		public void CheckAll()
//		{
//			ListViewHelper.CheckAll(this, true);
//		}
//
//		/// <summary>
//		/// Sets all items in the list to be unchecked.
//		/// </summary>
//		public void CheckNone()
//		{
//			ListViewHelper.CheckAll(this, false);
//		}

		// mbr - 06-04-2006 - implemented on base.		
//		/// <summary>
//		/// Autosizes all columns.
//		/// </summary>
//		public void AutoSizeColumns()
//		{
//			ListViewHelper.AutoSizeColumns(this);
//		}

		/// <summary>
		/// Autosizes the column.
		/// </summary>
		/// <param name="header"></param>
		public void AutoSizeColumn(ColumnHeader header)
		{
			ListViewHelper.AutoSizeColumn(this, header);
		}

		/// <summary>
		/// Gets the focused entity.
		/// </summary>
		public object FocusedEntity
		{
			get
			{
				EntityListViewItem item = this.FocusedItem as EntityListViewItem;
				if(item != null)
					return item.Entity;
				else
					return null;
			}
		}

		/// <summary>
		/// Selects the next item in the list.
		/// </summary>
		public void SelectNextItem()
		{
			if(this.Items.Count < 2)
				return;

			// any?
			ListViewItem current = this.SelectedItem;
			if(current != null)
			{
				// nope...
				current.Selected = false;

				// move...
				if(current.Index == this.Items.Count - 1)
					this.Items[0].Selected = true;
				else
					this.Items[current.Index + 1].Selected = true;
			}
			else
				this.Items[0].Selected = true;
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick (e);

			// check...
			if((this.LastButtonUp & MouseButtons.Left) != 0)
			{
				object entity = this.SelectedEntity;
				if(entity != null)
					this.OnEntityDoubleClick(new EntityEventArgs(entity));
			}
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

		protected override void OnMouseUp(MouseEventArgs e)
		{
			// set...
			_lastButtonUp = e.Button;

			// raise...
			base.OnMouseUp (e);

			// did we right click?
			if((this.LastButtonUp & MouseButtons.Right) != 0)
			{
				object entity = this.SelectedEntity;
				if(entity != null)
					this.OnEntityRightClick(new EntityEventArgs(entity));
			}
		}

		/// <summary>
		/// Raises the <c>EntityRightClick</c> event.
		/// </summary>
		protected virtual void OnEntityRightClick(EntityEventArgs e)
		{
			// nope?
			if(e.Entity is ICommandProvider)
			{
				// track and show...
				CommandUIObjectBuilder builder = new CommandUIObjectBuilder();
				if(e.Entity == null)
					throw new InvalidOperationException("e.Entity is null.");
				builder.CreateAndShowContextMenu(this, this.PointToClient(new Point(MousePosition.X, MousePosition.Y)), (ICommandProvider)e.Entity);
			}

			// raise...
			if(EntityRightClick != null)
				EntityRightClick(this, e);
		}

		/// <summary>
		/// Gets the lastbuttonup.
		/// </summary>
		private MouseButtons LastButtonUp
		{
			get
			{
				// returns the value...
				return _lastButtonUp;
			}
		}

		protected override void OnColumnClick(ColumnClickEventArgs e)
		{
			base.OnColumnClick (e);
			this.Sort(e.Column);
		}

		// mbr - 06-04-2006 - now on base.		
//		protected override void OnKeyDown(KeyEventArgs e)
//		{
//			base.OnKeyDown (e);
//
//			// handle...
//			ListViewHelper.HandleKeyDown(this, e);
//		}
	}
}

