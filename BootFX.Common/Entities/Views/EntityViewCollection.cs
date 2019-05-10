// BootFX - Application framework for .NET applications
// 
// File: EntityViewCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using BootFX.Common.Data;
using Comparers = BootFX.Common.Data.Comparers;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a collection of <see cref="EntityView"></see> instances.
	/// </summary>
	public class EntityViewCollection : ITypedList, IBindingList, IEntityType
	{
		/// <summary>
		/// Private field to support <c>Culture</c> property.
		/// </summary>
		private CultureInfo _culture = Cultures.User;
		
		/// <summary>
		/// Private field to support <c>IsReadOnly</c> property.
		/// </summary>
		private bool _isReadOnly = false;
		
		/// <summary>
		/// Private field to support <c>AllowEdit</c> property.
		/// </summary>
		private bool _allowEdit = true;
		
		/// <summary>
		/// Private field to support <c>AllowRemove</c> property.
		/// </summary>
		private bool _allowRemove = true;
		
		/// <summary>
		/// Private field to support <c>AllowNew</c> property.
		/// </summary>
		private bool _allowNew = true;
		
		/// <summary>
		/// Private field to support <c>Properties</c> property.
		/// </summary>
		private EntityViewPropertyCollection _properties;
		
		/// <summary>
		/// Private field to support <c>SortDirection</c> property.
		/// </summary>
		private SortDirection _sortDirection = SortDirection.Ascending;

		/// <summary>
		/// Private field to support <c>SortProperty</c> property.
		/// </summary>
		private PropertyDescriptor _sortProperty;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Raised when the list has changed.
		/// </summary>
		public event System.ComponentModel.ListChangedEventHandler ListChanged;

		/// <summary>
		/// Private field to support <c>InnerList</c> property.
		/// </summary>
		private ArrayList _innerList = new ArrayList();
		
		/// <summary>
		/// Private field to support <c>OriginalList</c> property.
		/// </summary>
		private object _originalList;

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityViewCollection(EntityCollection entities, IList expressions) : this(entities.EntityType, entities, ToObjectArray(expressions))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityViewCollection(EntityCollection entities, object[] expressions) : this(entities.EntityType, entities, expressions)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityViewCollection(EntityType entityType, object innerList, object[] expressions)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(innerList == null)
				throw new ArgumentNullException("innerList");
			if(expressions == null)
				throw new ArgumentNullException("expressions");

			// set...
			_properties = new EntityViewPropertyCollection(this);

			// check support...
			if(IsListSupported(innerList) == false)
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "The list '{0}' is not supported.  It must be an EntityCollection, IList or IListSource."));

			// set...
			_entityType = entityType;

			// set...
			IList innerIList = null;
			if(innerList is IList)
				innerIList = (IList)innerList;
			else if(innerList is IListSource)
				innerIList = ((IListSource)innerList).GetList();
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", innerList.GetType()));

			// set...
			_originalList = innerIList;

			// setup...
			this.IsReadOnly = innerIList.IsReadOnly;

			// copy...
			for(int index = 0; index < innerIList.Count; index++)
				this.Add(innerIList[index]);

			// set the properties...
			this.Properties.AddRange(expressions);
		}

		/// <summary>
		/// Converts the expressions to an object array.
		/// </summary>
		/// <param name="expressions"></param>
		/// <returns></returns>
		private static object[] ToObjectArray(IList expressions)
		{
			if(expressions == null)
				throw new ArgumentNullException("expressions");
			
			// create..
			object[] es = new object[expressions.Count];
			for(int index = 0; index < expressions.Count; index++)
				es[index] = expressions[index];

			// return...
			return es;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// Gets the innerlist.
		/// </summary>
		protected ArrayList InnerList
		{
			get
			{
				// returns the value...
				return _innerList;
			}
		}

		/// <summary>
		/// Gets the originallist.
		/// </summary>
		private object OriginalList
		{
			get
			{
				// returns the value...
				return _originalList;
			}
		}

		/// <summary>
		/// Gets the original list as an <c>IList</c>.
		/// </summary>
		private IList OriginalListAsIList
		{
			get
			{
				return this.OriginalList as IList;
			}
		}

		/// <summary>
		/// Returns true if the given list is supported.
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		/// <remarks>We either support an IList, or an EntityCollection.</remarks>
		public static bool IsListSupported(object list)
		{
			if(list == null)
				throw new ArgumentNullException("list");
		
			// check...
			if(list is IList || list is IListSource)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets item properties.
		/// </summary>
		/// <param name="listAccessors"></param>
		/// <returns></returns>
		PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
		{
			// return...
			PropertyDescriptorCollection properties = this.Properties.GetPropertyDescriptors();
			return properties;
		}

		string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
		{
			return this.GetListName(listAccessors);
		}

		/// <summary>
		/// Gets the name of the list.
		/// </summary>
		/// <param name="listAccessors"></param>
		/// <returns></returns>
		protected virtual string GetListName(PropertyDescriptor[] listAccessors)
		{
			if(this.EntityType != null)
				return this.EntityType.Name;
			else
				return "Entities";
		}

		bool IList.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		/// <summary>
		/// Gets or sets the isreadonly
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _isReadOnly)
				{
					// set the value...
					_isReadOnly = value;

					// update...
					this.AllowNew = !(this.IsReadOnly);
					this.AllowRemove = this.AllowNew;
					this.AllowEdit = this.AllowNew;
				}
			}
		}

		/// <summary>
		/// Gets or sets the item at the given index.
		/// </summary>
		public object this[int index]
		{
			get
			{
				return this.InnerList[index];
			}
			set
			{
				this.AssertEntity(value);
				if(!(value is EntityView))
					value = new EntityView(value);
				this.InnerList[index] = value;
			}
		}

		/// <summary>
		/// Gets the entity at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public object GetEntity(int index)
		{
			object value = this[index];
			if(value is EntityView)
				return ((EntityView)value).Entity;
			else
				return value;
		}

		/// <summary>
		/// Removes the item at the given index.
		/// </summary>
		/// <param name="index"></param>
		/// <remarks>This method will flag the entity in the underlying list as being deleted.</remarks>
		public void RemoveAt(int index)
		{
			// remove it...
			this.InnerList.RemoveAt(index);
		}

		public void Insert(int index, object entity)
		{
			this.AssertEntity(entity);

            throw new NotImplementedException("This operation has not been implemented.");
		}

		public void Remove(object entity)
		{
            throw new NotImplementedException("This operation has not been implemented.");
        }

		public bool Contains(object entity)
		{
            throw new NotImplementedException("This operation has not been implemented.");
        }

		public void Clear()
		{
			this.InnerList.Clear();
		}

		public int IndexOf(object entity)
		{
            throw new NotImplementedException("This operation has not been implemented.");
        }

		/// <summary>
		/// Adds an item.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public int Add(object entity)
		{
			this.AssertEntity(entity);
			return this.InnerList.Add(new EntityView(entity));
		}

		/// <summary>
		/// Asserts that we can support the provided entity.
		/// </summary>
		/// <param name="entity"></param>
		private void AssertEntity(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
                throw new ArgumentNullException("EntityType");
			EntityType.AssertIsOfType(entity);
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Gets the number of items in the list.
		/// </summary>
		public int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		/// <summary>
		/// Copies this list to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array)
		{
			if(InnerList == null)
				throw new ArgumentNullException("InnerList");
			this.InnerList.CopyTo(array);
		}

		/// <summary>
		/// Copies this list to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(Array array, int index)
		{
			if(InnerList == null)
				throw new ArgumentNullException("InnerList");
			this.InnerList.CopyTo(array, index);
		}

		/// <summary>
		/// Copies this list to an array.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="index"></param>
		public void CopyTo(int index, Array array, int arrayIndex, int count)
		{
			if(InnerList == null)
				throw new ArgumentNullException("InnerList");
			this.InnerList.CopyTo(index, array, arrayIndex, count);
		}

		/// <summary>
		/// Gets the sync root for the list.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return this.InnerList.SyncRoot;
			}
		}

		/// <summary>
		/// Raised the <c>ListChanged</c> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnListChanged(ListChangedEventArgs e)
		{
			if(ListChanged != null)
				ListChanged(this, e);
		}

		/// <summary>
		/// Gets the enumerator for the list.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return this.InnerList.GetEnumerator();
		}

		public void AddIndex(PropertyDescriptor property)
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		/// <summary>
		/// Returns true if we can add a new item to the list.
		/// </summary>
		bool IBindingList.AllowNew
		{
			get
			{
				return this.AllowNew;
			}
		}

		/// <summary>
		/// Gets or sets the allownew
		/// </summary>
		public bool AllowNew
		{
			get
			{
				return _allowNew;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _allowNew)
				{
					// set the value...
					_allowNew = value;
				}
			}
		}

		/// <summary>
		/// Sorts the list by the given property.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="direction"></param>
		public void ApplySort(PropertyDescriptor property, System.ComponentModel.ListSortDirection direction)
		{
			// are we trying to sort?
			if(property == null)
			{
				// set...
				_sortDirection = SortDirection.Ascending;
				_sortProperty = null;
				return;
			}

			// data...
			SortDirection useDirection = SortDirection.Ascending;
			if(direction == ListSortDirection.Descending)
				useDirection = SortDirection.Descending;

			// get a comparer...
			IComparer comparer = this.GetComparer(property);
			if(comparer == null)
				throw new ArgumentNullException("comparer");

            // mbr - 2010-04-05 - changed reversal method...
            //// set...
            //if(comparer is IComparerDirection)
            //    ((IComparerDirection)comparer).Direction = useDirection;
            IComparer toUse = comparer;
            if (useDirection == SortDirection.Descending)
                toUse = new ComparerReverser(comparer);

			// sort by the given property...
			this.InnerList.Sort(toUse);

			// set...
			_sortProperty = property;
			if(direction == ListSortDirection.Ascending)
				_sortDirection = SortDirection.Ascending;
			else
				_sortDirection = SortDirection.Descending;
		}

		/// <summary>
		/// Gets the comparer for the given property descriptor.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		private IComparer GetComparer(PropertyDescriptor property)
		{
			if(property == null)
				throw new ArgumentNullException("property");

			// do we know how to handle this?
			if(property is EntityFieldPropertyDescriptor)
				return ((EntityFieldPropertyDescriptor)property).Field.GetComparer(this.Culture);
			else
			{
				// get a default comparer...
				return new Comparers.StringComparer(this.Culture);
			}
		}

		/// <summary>
		/// Gets the sortproperty.
		/// </summary>
		public PropertyDescriptor SortProperty
		{
			get
			{
				// returns the value...
				return _sortProperty;
			}
		}

		/// <summary>
		/// Finds the given item in the list.
		/// </summary>
		/// <param name="property"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public int Find(PropertyDescriptor property, object key)
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		/// <summary>
		/// Returns true if the view supports sorting.
		/// </summary>
		public bool SupportsSorting
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Gets whether the list is sorted.
		/// </summary>
		public bool IsSorted
		{
			get
			{
				if(this.SortProperty == null)
					return false;
				else
					return true;
			}
		}

		bool IBindingList.AllowRemove
		{
			get
			{
				return this.AllowRemove;
			}
		}
		
		/// <summary>
		/// Gets or sets the allowremove
		/// </summary>
		public bool AllowRemove
		{
			get
			{
				return _allowRemove;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _allowRemove)
				{
					// set the value...
					_allowRemove = value;
				}
			}
		}

		/// <summary>
		/// Returns true if the view supports searching.
		/// </summary>
		public bool SupportsSearching
		{
			get
			{
				return false;
			}
		}
		
		ListSortDirection IBindingList.SortDirection
		{
			get
			{
				if(this.SortDirection == SortDirection.Ascending)
					return ListSortDirection.Ascending;
				else
					return ListSortDirection.Descending;
			}
		}

		/// <summary>
		/// Gets the sortdirection.
		/// </summary>
		public SortDirection SortDirection
		{
			get
			{
				// returns the value...
				return _sortDirection;
			}
		}

		/// <summary>
		/// Returns true if the item supports change notification.
		/// </summary>
		public bool SupportsChangeNotification
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Removes the sort.
		/// </summary>
		public void RemoveSort()
		{
			this.ApplySort(null, ListSortDirection.Ascending);
		}

		/// <summary>
		/// Adds a new item to the list.
		/// </summary>
		/// <returns></returns>
		public object AddNew()
		{
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// original...
			if(OriginalListAsIList == null)
				throw new ArgumentNullException("OriginalListAsIList");

			// create...
			object newEntity = EntityType.CreateInstance();
			int index = this.OriginalListAsIList.Add(newEntity);

			// add it to the list...
			this.Add(this.OriginalListAsIList[index]);

			// return...
			return newEntity;
		}

		/// <summary>
		/// Returns true if we can edit the list.
		/// </summary>
		bool IBindingList.AllowEdit
		{
			get
			{
				return this.AllowEdit;
			}
		}

		/// <summary>
		/// Gets or sets the allowedit
		/// </summary>
		public bool AllowEdit
		{
			get
			{
				return _allowEdit;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _allowEdit)
				{
					// set the value...
					_allowEdit = value;
				}
			}
		}

		public void RemoveIndex(PropertyDescriptor property)
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		/// <summary>
		/// Gets a collection of EntityViewProperty objects.
		/// </summary>
		public EntityViewPropertyCollection Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Gets or sets the culture
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _culture)
				{
					// set the value...
					_culture = value;
				}
			}
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(EntityView[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}

		/// <summary>
		/// Creates an entity collection from the items in the view collection.
		/// </summary>
		/// <returns></returns>
		public IList ToEntityCollection()
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// create...
			IList results = this.EntityType.CreateCollectionInstance();
			if(results == null)
				throw new InvalidOperationException("results is null.");

			// walk...
			foreach(EntityView view in this.InnerList)
				results.Add(EntityView.Unwrap(view));

			// return...
			return results;
		}
	}
}
