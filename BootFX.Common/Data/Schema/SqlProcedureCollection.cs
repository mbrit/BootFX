// BootFX - Application framework for .NET applications
// 
// File: SqlProcedureCollection.cs
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
	/// Holds a collection of <see ref="SqlProcedure">SqlProcedure</see> instances.
	/// </summary>
	public class SqlProcedureCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlProcedureCollection()
		{
		}
		
		/// <summary>
		/// Adds a SqlProcedure instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(SqlProcedure item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of SqlProcedure instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlProcedure[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of SqlProcedure instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlProcedureCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a SqlProcedure instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SqlProcedure item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a SqlProcedure item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SqlProcedure item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlProcedure this[int index]
		{
			get
			{
				return (SqlProcedure)List[index];
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
		public int IndexOf(SqlProcedure item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SqlProcedure item)
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
		public SqlProcedure[] ToArray()
		{
			return (SqlProcedure[])InnerList.ToArray(typeof(SqlProcedure));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(SqlProcedure[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
	}
}
