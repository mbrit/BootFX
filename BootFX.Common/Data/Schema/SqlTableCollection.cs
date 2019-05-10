// BootFX - Application framework for .NET applications
// 
// File: SqlTableCollection.cs
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
	/// Holds a collection of <see ref="SqlTable">SqlTable</see> instances.
	/// </summary>
	public class SqlTableCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlTableCollection()
		{
		}
		
		/// <summary>
		/// Adds a SqlTable instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(SqlTable item)
		{
			AssertNewItem(item);
			List.Add(item);
		}  

		/// <summary>
		/// Asserts the new item is valid.
		/// </summary>
		/// <param name="item"></param>
		private void AssertNewItem(SqlTable item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			
			// check...
			if(this.Contains(item.NativeName))
				throw new InvalidOperationException(string.Format("An item with the native name '{0}' already exists.", item.NativeName));
		}

		/// <summary>
		/// Adds a set of SqlTable instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlTable[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of SqlTable instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlTableCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a SqlTable instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SqlTable item)
		{
			this.AssertNewItem(item);
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a SqlTable item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SqlTable item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlTable this[int index]
		{
			get
			{
				return (SqlTable)List[index];
			}
			set
			{
				this.AssertNewItem(value);
				List[index] = value;
			}
		}
			
		/// <summary>
		/// Gets an item with the given name.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlTable this[string nativeName]
		{
			get
			{
				int index = this.IndexOf(nativeName, OnNotFound.ReturnNull);
				if(index != -1)
					return this[index];
				else
					return null;
			}
		}

		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(SqlTable item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SqlTable item)
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
		public SqlTable[] ToArray()
		{
			return (SqlTable[])InnerList.ToArray(typeof(SqlTable));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(SqlTable[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
		
		/// <summary>
		/// Gets the index of the item of the item with the given name.
		/// </summary>
		/// <returns>Returns the index of the item, or <c>-1</c> if not found.</returns>
		public int IndexOf(string nativeName, OnNotFound onNotFound)
		{
			for(int index = 0; index < Count; index++)
			{
				// check...
				if(string.Compare(nativeName, this[index].NativeName, true,
					System.Globalization.CultureInfo.InvariantCulture) == 0)
					return index;
			}
			
			// nope...
			switch(onNotFound)
			{
				case OnNotFound.ReturnNull:
					return -1;
				case OnNotFound.ThrowException:
					throw new InvalidOperationException(string.Format(Cultures.Exceptions,
						"An item with name '{0}' was not found.", nativeName));
				default:
					throw new NotSupportedException(string.Format(Cultures.Exceptions,
						"Cannot handle '{0}'.", onNotFound));
			}
		}
		
		/// <summary>
		/// Returns true if the collection contains an item with the given name.
		/// </summary>
		public bool Contains(string nativeName)
		{
			if(IndexOf(nativeName, OnNotFound.ReturnNull) != -1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Sorts the collection by table name order.
		/// </summary>
		public void SortByNativeName()
		{
			this.InnerList.Sort(new SqlMemberNativeNameComparer());
		}
	}
}
