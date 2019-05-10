// BootFX - Application framework for .NET applications
// 
// File: NamedDatabaseCollection.cs
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
	/// Holds a collection of <see ref="NamedDatabase">NamedDatabase</see> instances.
	/// </summary>
	public class NamedDatabaseCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public NamedDatabaseCollection()
		{
		}
		
		/// <summary>
		/// Adds a NamedDatabase instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(NamedDatabase item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of NamedDatabase instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(NamedDatabase[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of NamedDatabase instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(NamedDatabaseCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a NamedDatabase instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, NamedDatabase item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a NamedDatabase item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(NamedDatabase item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public NamedDatabase this[int index]
		{
			get
			{
				return (NamedDatabase)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}
			
		/// <summary>
		/// Gets an item with the given name.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public NamedDatabase this[string name]
		{
			get
			{
				int index = this.IndexOf(name, OnNotFound.ReturnNull);
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
		public int IndexOf(NamedDatabase item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(NamedDatabase item)
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
		public NamedDatabase[] ToArray()
		{
			return (NamedDatabase[])InnerList.ToArray(typeof(NamedDatabase));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(NamedDatabase[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}
		
		/// <summary>
		/// Gets the index of the item of the item with the given name.
		/// </summary>
		/// <returns>Returns the index of the item, or <c>-1</c> if not found.</returns>
		public int IndexOf(string name, OnNotFound onNotFound)
		{
			for(int index = 0; index < Count; index++)
			{
				// check...
				if(string.Compare(name, this[index].Name, true,
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
						"An item with name '{0}' was not found.", name));
				default:
					throw new NotSupportedException(string.Format(Cultures.Exceptions,
						"Cannot handle '{0}'.", onNotFound));
			}
		}
		
		/// <summary>
		/// Returns true if the collection contains an item with the given name.
		/// </summary>
		public bool Contains(string name)
		{
			if(IndexOf(name, OnNotFound.ReturnNull) != -1)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the database with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public NamedDatabase GetDatabase(string name, OnNotFound onNotFound)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			int index = this.IndexOf(name, onNotFound);
			if(index != -1)
				return this[index];
			else
				return null;
		}
	}
}
