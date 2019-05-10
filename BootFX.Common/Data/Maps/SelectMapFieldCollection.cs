// BootFX - Application framework for .NET applications
// 
// File: SelectMapFieldCollection.cs
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
	/// Holds a collection of <c ref="SelectMapField">SelectMapField</c> instances.
	/// </summary>
	[Serializable()]
	public class SelectMapFieldCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public SelectMapFieldCollection()
		{
		}
		
		/// <summary>
		/// Adds a SelectMapField instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(SelectMapField item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  
	
		/// <summary>
		/// Removes a SelectMapField item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SelectMapField item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SelectMapField this[int index]
		{
			get
			{
				return (SelectMapField)List[index];
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
		public int IndexOf(SelectMapField item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SelectMapField item)
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
		public SelectMapField[] ToArray()
		{
			return (SelectMapField[])InnerList.ToArray(typeof(SelectMapField));
		}
	}
}
