// BootFX - Application framework for .NET applications
// 
// File: FindSpecificationCollection.cs
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
	/// Holds a collection of <c ref="FindSpecification">FindSpecification</c> instances.
	/// </summary>
	internal class FindSpecificationCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public FindSpecificationCollection()
		{
		}
		
		/// <summary>
		/// Adds a FindSpecification instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(FindSpecification item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  
	
		/// <summary>
		/// Removes a FindSpecification item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(FindSpecification item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public FindSpecification this[int index]
		{
			get
			{
				return (FindSpecification)List[index];
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
		public int IndexOf(FindSpecification item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(FindSpecification item)
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
		public FindSpecification[] ToArray()
		{
			return (FindSpecification[])InnerList.ToArray(typeof(FindSpecification));
		}
	}
}
