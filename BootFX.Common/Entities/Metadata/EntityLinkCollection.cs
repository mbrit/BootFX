// BootFX - Application framework for .NET applications
// 
// File: EntityLinkCollection.cs
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
	/// Holds a collection of <c ref="EntityLink">EntityLink</c> instances.
	/// </summary>
	public class EntityLinkCollection : CollectionBase, IEntityType
	{
		/// <summary>
		/// Raised when a field is added.
		/// </summary>
		public event EntityLinkEventHandler LinkAdded;

		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkCollection(EntityType entityType)
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
		/// Adds a EntityLink instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(EntityLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			int index = List.Add(item);
			this.OnLinkAdded(new EntityLinkEventArgs(item));
			return index;
		}  

		/// <summary>
		/// Adds a EntityLink instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, EntityLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  

		/// <summary>
		/// Removes a EntityLink item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(EntityLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityLink this[int index]
		{
			get
			{
				return (EntityLink)List[index];
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
		public int IndexOf(EntityLink item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(EntityLink item)
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
		public EntityLink[] ToArray()
		{
			return (EntityLink[])InnerList.ToArray(typeof(EntityLink));
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
		public EntityLink GetLink(string name, OnNotFound onNotFound)
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
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Link with name '{0}' was not found on '{1}'.", name, this.EntityType));

					default:
						throw ExceptionHelper.CreateCannotHandleException(onNotFound);
				}
			}
		}

		/// <summary>
		/// Raises the <c>LinkAdded</c> event.
		/// </summary>
		protected virtual void OnLinkAdded(EntityLinkEventArgs e)
		{
			if(LinkAdded != null)
				LinkAdded(this, e);
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(EntityLink[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}
	}
}
