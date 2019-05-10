// BootFX - Application framework for .NET applications
// 
// File: ProjectStoreCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Holds a collection of <see ref="ProjectStore">ProjectStore</see> instances.
	/// </summary>
	internal class ProjectStoreCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ProjectStoreCollection()
		{
		}
		
		/// <summary>
		/// Adds a ProjectStore instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		internal int Add(ProjectStore item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of ProjectStore instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		internal void AddRange(ProjectStore[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of ProjectStore instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		internal void AddRange(ProjectStoreCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a ProjectStore instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		internal void Insert(int index, ProjectStore item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a ProjectStore item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		internal void Remove(ProjectStore item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		internal ProjectStore this[int index]
		{
			get
			{
				return (ProjectStore)List[index];
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
		internal int IndexOf(ProjectStore item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		internal bool Contains(ProjectStore item)
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
		internal ProjectStore[] ToArray()
		{
			return (ProjectStore[])InnerList.ToArray(typeof(ProjectStore));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		internal void CopyTo(ProjectStore[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
