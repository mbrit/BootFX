// BootFX - Application framework for .NET applications
// 
// File: PropertyTabPageCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Holds a collection of <see ref="PropertyPage">PropertyPage</see> instances.
	/// </summary>
	internal class PropertyTabPageCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public PropertyTabPageCollection()
		{
		}
		
		/// <summary>
		/// Adds a PropertyPage instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(PropertyTabPage item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of PropertyPage instances to the collection.
		/// </summary>
		/// <param name="items">The items to add.</param>
		public void AddRange(PropertyTabPage[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of PropertyPage instances to the collection.
		/// </summary>
		/// <param name="items">The items to add.</param>
		public void AddRange(PropertyTabPageCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a PropertyPage instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, PropertyTabPage item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a PropertyPage item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(PropertyTabPage item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public PropertyTabPage this[int index]
		{
			get
			{
				return (PropertyTabPage)List[index];
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
		private PropertyTabPage this[string text]
		{
			get
			{
				foreach(PropertyTabPage page in this.InnerList)
				{
					if(string.Compare(page.Text, text, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
						return page;
				}
				return null;
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(PropertyTabPage item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(PropertyTabPage item)
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
		public PropertyTabPage[] ToArray()
		{
			return (PropertyTabPage[])InnerList.ToArray(typeof(PropertyTabPage));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(PropertyTabPage[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
