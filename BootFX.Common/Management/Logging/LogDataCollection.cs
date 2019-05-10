// BootFX - Application framework for .NET applications
// 
// File: LogDataCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Holds a collection of <see ref="LogData">LogData</see> instances.
	/// </summary>
	public class LogDataCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LogDataCollection()
		{
		}
		
		/// <summary>
		/// Adds a LogData instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(LogData item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of LogData instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(LogData[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of LogData instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(LogDataCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a LogData instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, LogData item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a LogData item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(LogData item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public LogData this[int index]
		{
			get
			{
				return (LogData)List[index];
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
		public int IndexOf(LogData item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(LogData item)
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
		public LogData[] ToArray()
		{
			return (LogData[])InnerList.ToArray(typeof(LogData));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(LogData[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
