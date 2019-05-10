// BootFX - Application framework for .NET applications
// 
// File: EntityTypeCollection.cs
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
	/// Holds a collection of <c ref="EntityType">EntityType</c> instances.
	/// </summary>
	internal class EntityTypeCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityTypeCollection()
		{
		}
		
		/// <summary>
		/// Adds a EntityType instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(EntityType item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}
  
		/// <summary>
		/// Adds a type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public int Add(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get...
			EntityType et = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// return...
			return this.Add(et);
		}

		/// <summary>
		/// Adds a EntityType instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, EntityType item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			this.List.Insert(index, item);
		}  

		/// <summary>
		/// Adds a EntityType instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(EntityType[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  

		/// <summary>
		/// Removes a EntityType item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(EntityType item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public EntityType this[int index]
		{
			get
			{
				return (EntityType)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}

//		/// <summary>
//		/// Gets the given entity type.
//		/// </summary>
//		[FxCopNote("DesignRules.dll", "OnlyIntegralValuesOrStringsShouldBeUsedForIndexers", "Valid in this case.")]
//		public EntityType this[Type type]
//		{
//			get
//			{
//				if(type == null)
//					throw new ArgumentNullException("type");
//				
//				// return...
//				return this.GetEntityType(type, OnNotFound.ReturnNull);
//			}
//		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(EntityType item)
		{
			return List.IndexOf(item);
		}

//		/// <summary>
//		/// Gets the 
//		/// </summary>
//		/// <param name="type"></param>
//		/// <returns></returns>
//		public int IndexOf(Type type)
//		{
//			if(type == null)
//				throw new ArgumentNullException("type");
//			
//			// walk...
//			for(int index = 0; index < this.Count; index++)
//			{
//				if(this[index].Type == type)
//					return index;
//			}
//
//			// nope...
//			return -1;
//		}
//
//		/// <summary>
//		/// Gets the entity type for the given type.
//		/// </summary>
//		/// <param name="type"></param>
//		/// <param name="onNotFound"></param>
//		/// <returns></returns>
//		internal EntityType GetEntityType(EntityTypeName name, OnNotFound onNotFound)
//		{
//			// get the type...
//			Type type = name.GetClrType();
//			if(type == null)
//				throw new InvalidOperationException("type is null.");
//
//			// defer...
//			return GetEntityType(type, onNotFound);
//		}
//
//		/// <summary>
//		/// Gets the entity type for the given type.
//		/// </summary>
//		/// <param name="type"></param>
//		/// <param name="onNotFound"></param>
//		/// <returns></returns>
//		public EntityType GetEntityType(Type type, OnNotFound onNotFound)
//		{
//			if(type == null)
//				throw new ArgumentNullException("type");
//			
//			// index...
//			int index = this.IndexOf(type);
//			if(index != -1)
//				return this[index];
//			else
//			{
//				// demand load...
//				EntityType entityType = this.DemandLoadEntityType(type);
//				if(entityType != null)
//					return entityType;
//
//				// now what?
//				switch(onNotFound)
//				{
//					case OnNotFound.ReturnNull:
//						return null;
//
//					case OnNotFound.ThrowException:
//						throw new InvalidOperationException(string.Format(Cultures.Exceptions, StringHelper.GetString("EntityTypeForP0WasNotFound", OnStringNotFound.ReturnName), type));
//								// EntityTypeForP0WasNotFound = 'Entity type for '{0}' was not found.'
//
//					default:
//						throw ExceptionHelper.CreateCannotHandleException(onNotFound);
//				}
//			}
//		}
//
//		/// <summary>
//		/// Demand loads the given entity type.
//		/// </summary>
//		/// <param name="type"></param>
//		/// <returns></returns>
//		private EntityType DemandLoadEntityType(Type type)
//		{
//			if(type == null)
//				throw new ArgumentNullException("type");
//			
//			// load...
//			return EntityType.LoadFromAttributes(type, OnNotFound.ReturnNull);
//		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(EntityType item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			if(IndexOf(item) == -1)
				return false;
			else
				return true;
		}

//		/// <summary>
//		/// Discovers if the given item is in the collection.
//		/// </summary>
//		/// <param name="item">The item to find.</param>
//		/// <returns>Returns true if the given item is in the collection.</returns>
//		public bool Contains(Type entityType)
//		{
//			if(entityType == null)
//				throw new ArgumentNullException("entityType");
//			
//			if(IndexOf(entityType) == -1)
//				return false;
//			else
//				return true;
//		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public EntityType[] ToArray()
		{
			return (EntityType[])InnerList.ToArray(typeof(EntityType));
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(EntityType[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}
	}
}
