// BootFX - Application framework for .NET applications
// 
// File: EntityPropertyCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Holds a collection of <c ref="EntityProperty">EntityProperty</c> instances.
	/// </summary>
	public class EntityPropertyCollection : CollectionBase, IEntityType
	{
		/// <summary>
		/// Raised when a Property is added.
		/// </summary>
		public event EntityPropertyEventHandler PropertyAdded;
		
		/// <summary>
		/// Private Property to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal EntityPropertyCollection(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			_entityType = entityType;
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
		/// Adds a EntityProperty instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(EntityProperty item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			int index = List.Add(item);
			this.OnPropertyAdded(new EntityPropertyEventArgs(item));
			return index;
		}  

		/// <summary>
		/// Raises the <c>PropertyAdded</c> event.
		/// </summary>
		protected virtual void OnPropertyAdded(EntityPropertyEventArgs e)
		{
			if(PropertyAdded != null)
				PropertyAdded(this, e);
		}

		/// <summary>
		/// Adds a EntityProperty instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, EntityProperty item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  

		/// <summary>
		/// Adds a EntityProperty instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(EntityProperty[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			foreach(EntityProperty item in items)
				this.Add(item);
		}  

		/// <summary>
		/// Removes a EntityProperty item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(EntityProperty item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityProperty this[int index]
		{
			get
			{
				return (EntityProperty)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}

		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityProperty this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				if(name.Length == 0)
					throw ExceptionHelper.CreateZeroLengthArgumentException("name");
				
				// return...
				return this.GetProperty(name, OnNotFound.ReturnNull);
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(EntityProperty item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(EntityProperty item)
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
		public EntityProperty[] ToArray()
		{
			return (EntityProperty[])InnerList.ToArray(typeof(EntityProperty));
		}

		/// <summary>
		/// Gets the 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int IndexOf(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			
			// walk...
			for(int index = 0; index < this.Count; index++)
			{
				if(string.Compare(this[index].Name, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					return index;
			}

			// nope...
			return -1;
		}

		/// <summary>
		/// Gets the entity type for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public EntityProperty GetProperty(string name, OnNotFound onNotFound)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// index...
			int index = this.IndexOf(name);
			if(index != -1)
				return this[index];
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Property with name '{0}' was not found on '{1}'.", name, this.EntityType));

					default:
						throw ExceptionHelper.CreateCannotHandleException(onNotFound);
				}
			}
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(EntityProperty[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}
	}
}
