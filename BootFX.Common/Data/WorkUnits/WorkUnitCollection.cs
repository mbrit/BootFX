// BootFX - Application framework for .NET applications
// 
// File: WorkUnitCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common.Data.Schema;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Holds a collection of <c ref="IWorkUnit">IWorkUnit</c> instances.
	/// </summary>
	[Serializable()]
	public class WorkUnitCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public WorkUnitCollection()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorkUnitCollection(IWorkUnit unit)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");
			this.Add(unit);
		}
		
		/// <summary>
		/// Adds a IWorkUnit instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(IWorkUnit item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of IWorkUnit instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(IWorkUnit[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of IWorkUnit instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(WorkUnitCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a IWorkUnit instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, IWorkUnit item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a IWorkUnit item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(IWorkUnit item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public IWorkUnit this[int index]
		{
			get
			{
				return (IWorkUnit)List[index];
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
		public int IndexOf(IWorkUnit item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(IWorkUnit item)
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
		public IWorkUnit[] ToArray()
		{
			return (IWorkUnit[])InnerList.ToArray(typeof(IWorkUnit));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(IWorkUnit[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		public void Sort(IComparer comparer)
		{
			if(comparer == null)
				throw new ArgumentNullException("comparer");
			this.InnerList.Sort(comparer);
		}
	}
}
