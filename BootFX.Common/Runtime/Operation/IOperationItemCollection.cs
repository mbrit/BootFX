// BootFX - Application framework for .NET applications
// 
// File: IOperationItemCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common
{
	/// <summary>
	/// Holds a collection of <see ref="IOperationItem">IOperationItem</see> instances.
	/// </summary>
	public class IOperationItemCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public IOperationItemCollection()
		{
		}
		
		/// <summary>
		/// Adds a IOperationItem instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(IOperationItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of IOperationItem instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(IOperationItem[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of IOperationItem instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(IOperationItemCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a IOperationItem instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, IOperationItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a IOperationItem item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(IOperationItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			// remove it... this is a safer version that doesn't throw if the item is not found...
			int index = this.IndexOf(item);
			if(index != -1)
				this.RemoveAt(index);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public IOperationItem this[int index]
		{
			get
			{
				return (IOperationItem)List[index];
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
		public int IndexOf(IOperationItem item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(IOperationItem item)
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
		public IOperationItem[] ToArray()
		{
			return (IOperationItem[])InnerList.ToArray(typeof(IOperationItem));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(IOperationItem[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
