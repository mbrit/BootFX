// BootFX - Application framework for .NET applications
// 
// File: ConnectionSettingsCollection.cs
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
	/// Holds a collection of <see ref="ConnectionSettings">ConnectionSettings</see> instances.
	/// </summary>
	[Serializable()]
	public class ConnectionSettingsCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectionSettingsCollection()
		{
		}
		
		/// <summary>
		/// Adds a ConnectionSettings instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(ConnectionSettings item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of ConnectionSettings instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(ConnectionSettings[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of ConnectionSettings instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(ConnectionSettingsCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a ConnectionSettings instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, ConnectionSettings item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a ConnectionSettings item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(ConnectionSettings item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public ConnectionSettings this[int index]
		{
			get
			{
				return (ConnectionSettings)List[index];
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
		public int IndexOf(ConnectionSettings item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(ConnectionSettings item)
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
		public ConnectionSettings[] ToArray()
		{
			return (ConnectionSettings[])InnerList.ToArray(typeof(ConnectionSettings));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(ConnectionSettings[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
