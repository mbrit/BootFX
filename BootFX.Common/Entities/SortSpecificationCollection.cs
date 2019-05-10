// BootFX - Application framework for .NET applications
// 
// File: SortSpecificationCollection.cs
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
	/// Holds a collection of <c ref="SortSpecification">SortSpecification</c> instances.
	/// </summary>
	public class SortSpecificationCollection : CollectionBase, ICloneable
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public SortSpecificationCollection()
		{
		}
		
		/// <summary>
		/// Adds a SortSpecification instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(SortSpecification item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a SortSpecification instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SortSpecification item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  

		/// <summary>
		/// Adds a SortSpecification instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SortSpecification[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				this.Add(items[index]);
		}  

		/// <summary>
		/// Adds a SortSpecification instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SortSpecificationCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				this.Add(items[index]);
		}  

		/// <summary>
		/// Removes a SortSpecification item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SortSpecification item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SortSpecification this[int index]
		{
			get
			{
				return (SortSpecification)List[index];
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
		public int IndexOf(SortSpecification item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SortSpecification item)
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
		public SortSpecification[] ToArray()
		{
			return (SortSpecification[])InnerList.ToArray(typeof(SortSpecification));
		}

		/// <summary>
		/// Copies the collection.
		/// </summary>
		/// <returns></returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}

		/// <summary>
		/// Copies the collection.
		/// </summary>
		/// <returns></returns>
		public SortSpecificationCollection Clone()
		{
			// new...
			SortSpecificationCollection newCollection = new SortSpecificationCollection();
			foreach(SortSpecification specification in this.InnerList)
				newCollection.Add(specification.Clone());

			// return...
			return newCollection;
		}

		/// <summary>
		/// Copies the values in this collection to the given array.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="index"></param>
		public void CopyTo(SortSpecification[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			this.InnerList.CopyTo(items, index);
		}
	}
}
