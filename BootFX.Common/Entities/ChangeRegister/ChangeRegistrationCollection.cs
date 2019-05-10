// BootFX - Application framework for .NET applications
// 
// File: ChangeRegistrationCollection.cs
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
	/// Holds a collection of <see ref="ChangeRegistration">ChangeRegistration</see> instances.
	/// </summary>
	internal class ChangeRegistrationCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public ChangeRegistrationCollection()
		{
		}
		
		/// <summary>
		/// Adds a ChangeRegistration instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(ChangeRegistration item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of ChangeRegistration instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(ChangeRegistration[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of ChangeRegistration instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(ChangeRegistrationCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a ChangeRegistration instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, ChangeRegistration item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a ChangeRegistration item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(ChangeRegistration item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public ChangeRegistration this[int index]
		{
			get
			{
				return (ChangeRegistration)List[index];
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
		public int IndexOf(ChangeRegistration item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(ChangeRegistration item)
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
		public ChangeRegistration[] ToArray()
		{
			return (ChangeRegistration[])InnerList.ToArray(typeof(ChangeRegistration));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(ChangeRegistration[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
