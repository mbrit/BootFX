// BootFX - Application framework for .NET applications
// 
// File: SqlIndexCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Holds a collection of <see ref="SqlIndex">SqlIndex</see> instances.
	/// </summary>
	public class SqlIndexCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlIndexCollection()
		{
		}
		
		/// <summary>
		/// Adds a SqlIndex instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(SqlIndex item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of SqlIndex instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlIndex[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of SqlIndex instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlIndexCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a SqlIndex instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SqlIndex item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a SqlIndex item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SqlIndex item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlIndex this[int index]
		{
			get
			{
				return (SqlIndex)List[index];
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
		public int IndexOf(SqlIndex item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SqlIndex item)
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
		public SqlIndex[] ToArray()
		{
			return (SqlIndex[])InnerList.ToArray(typeof(SqlIndex));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(SqlIndex[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		/// <summary>
		/// Sorts the collection by table name order.
		/// </summary>
		public void SortByNativeName()
		{
			this.InnerList.Sort(new SqlMemberNativeNameComparer());
		}

		public SqlIndex this[string name]
		{
			get
			{
				if(name == null)
					throw new ArgumentNullException("name");
				if(name.Length == 0)
					throw new ArgumentOutOfRangeException("'name' is zero-length.");
				
				foreach(SqlIndex index in this.InnerList)
				{
					if(string.Compare(index.Name, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
						return index;
				}

				// nope...
				return null;
			}
		}
	}
}
