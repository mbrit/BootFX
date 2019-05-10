// BootFX - Application framework for .NET applications
// 
// File: FilterManglerCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Holds a collection of <see ref="FilterMangler">FilterMangler</see> instances.
	/// </summary>
	public class FilterManglerCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public FilterManglerCollection()
		{
		}
		
		/// <summary>
		/// Adds a FilterMangler instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(FilterMangler item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of FilterMangler instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(FilterMangler[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of FilterMangler instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(FilterManglerCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a FilterMangler instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, FilterMangler item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a FilterMangler item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(FilterMangler item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public FilterMangler this[int index]
		{
			get
			{
				return (FilterMangler)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}

		/// <summary>
		/// Gets or sets an item by its mangler id
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public FilterMangler this[string manglerId]
		{
			get
			{
				if(manglerId == null)
					throw new ArgumentNullException("manglerId");
				if(manglerId.Length == 0)
					throw new ArgumentOutOfRangeException("'manglerId' is zero-length.");
				
				int index = IndexOf(manglerId);
				if(index == -1)
					return null;

				return (FilterMangler)List[index];
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(FilterMangler item)
		{
			return IndexOf(item.ManglerId);
		}

		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(string manglerId)
		{
			if(manglerId == null)
				throw new ArgumentNullException("manglerId");
			if(manglerId.Length == 0)
				throw new ArgumentOutOfRangeException("'manglerId' is zero-length.");			

			for(int index = 0; index < List.Count; index ++)
				if(this[index].ManglerId == manglerId)
					return index;

			return -1;
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(FilterMangler item)
		{
			return Contains(item.ManglerId);
		}

		/// <summary>
		/// Discovers if the given mangler id is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(string manglerId)
		{
			if(manglerId == null)
				throw new ArgumentNullException("manglerId");
			if(manglerId.Length == 0)
				throw new ArgumentOutOfRangeException("'manglerId' is zero-length.");

			foreach(FilterMangler mangler in this.List)
				if(mangler.ManglerId == manglerId)
					return true;

			return false;
		}
		
		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public FilterMangler[] ToArray()
		{
			return (FilterMangler[])InnerList.ToArray(typeof(FilterMangler));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(FilterMangler[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
