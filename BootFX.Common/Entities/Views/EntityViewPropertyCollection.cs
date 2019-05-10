// BootFX - Application framework for .NET applications
// 
// File: EntityViewPropertyCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Holds a collection of <c ref="EntityViewProperty">EntityViewProperty</c> instances.
	/// </summary>
	public class EntityViewPropertyCollection : CollectionBase
	{
		/// <summary>
		/// Private field to support <c>EntityView</c> property.
		/// </summary>
		private EntityViewCollection _entityView;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal EntityViewPropertyCollection(EntityViewCollection entityView)
		{
			if(entityView == null)
				throw new ArgumentNullException("entityView");
			
			_entityView = entityView;
		}

		/// <summary>
		/// Gets the entityview.
		/// </summary>
		public EntityViewCollection EntityView
		{
			get
			{
				return _entityView;
			}
		}
		
		/// <summary>
		/// Adds a EntityViewProperty instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(EntityViewProperty item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  
		
		/// <summary>
		/// Adds a EntityViewProperty instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, EntityViewProperty item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  

		/// <summary>
		/// Adds the given expression.
		/// </summary>
		/// <param name="expression"></param>
		public EntityViewProperty Add(string expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			if(expression.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("expression");
			
			// add...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			EntityViewProperty newProperty = this.EntityType.CreateViewProperty(expression);

			// add...
			this.Add(newProperty);

			// return...
			return newProperty;
		}

		/// <summary>
		/// Adds the items to the list.
		/// </summary>
		/// <param name="itmes"></param>
		public void AddRange(IList items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}

		/// <summary>
		/// Adds the given expression.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public EntityViewProperty Add(object expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			
			// string?
			if(expression is string)
				return Add((string)expression);
			
			// member?
			if(expression is EntityMember)
			{
				// create...
				EntityViewProperty property = ((EntityMember)expression).GetViewProperty();
				if(property == null)
					throw new ArgumentNullException("property");

				// add...
				this.Add(property);

				// return...
				return property;
			}

			// nope...
			throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", expression.GetType()));
		}

		/// <summary>
		/// Adds a set of EntityViewProperty instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(EntityViewProperty[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of EntityViewProperty instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(EntityViewPropertyCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Removes a EntityViewProperty item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(EntityViewProperty item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityViewProperty this[int index]
		{
			get
			{
				return (EntityViewProperty)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(EntityViewProperty item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(EntityViewProperty item)
		{
			if(IndexOf(item) == -1)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public EntityViewProperty[] ToArray()
		{
			return (EntityViewProperty[])InnerList.ToArray(typeof(EntityViewProperty));
		}
	
		/// <summary>
		/// Gets the descriptors.
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptorCollection GetPropertyDescriptors()
		{
			PropertyDescriptorCollection results = new PropertyDescriptorCollection(new PropertyDescriptor[] {});
			foreach(EntityViewProperty property in this.InnerList)
				results.Add(property.GetPropertyDescriptor());

			// return...
			return results;
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		private EntityType EntityType
		{
			get
			{
				if(EntityView == null)
					throw new ArgumentNullException("EntityView");
				return this.EntityView.EntityType;
			}
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(EntityViewProperty[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}
	}
}
