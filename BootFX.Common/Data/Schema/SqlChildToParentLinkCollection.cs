// BootFX - Application framework for .NET applications
// 
// File: SqlChildToParentLinkCollection.cs
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
	/// Holds a collection of <see ref="SqlChildToParentLink">SqlChildToParentLink</see> instances.
	/// </summary>
	public class SqlChildToParentLinkCollection : CollectionBase
	{
		/// <summary>
		/// Private field to support <see cref="Table"/> property.
		/// </summary>
		private SqlTable _table;

		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlChildToParentLinkCollection()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlChildToParentLinkCollection(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			_table = table;
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		public SqlTable Table
		{
			get
			{
				return _table;
			}
		}
		
		/// <summary>
		/// Adds a SqlChildToParentLink instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(SqlChildToParentLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			this.AssignItem(item);
			List.Add(item);
		}  

		/// <summary>
		/// Assigns an item.
		/// </summary>
		/// <param name="item"></param>
		private void AssignItem(SqlChildToParentLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			if(this.Table != null)
				item.SetTable(this.Table);
		}

		/// <summary>
		/// Adds a set of SqlChildToParentLink instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlChildToParentLink[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of SqlChildToParentLink instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlChildToParentLinkCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a SqlChildToParentLink instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SqlChildToParentLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			this.AssignItem(item);
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a SqlChildToParentLink item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SqlChildToParentLink item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlChildToParentLink this[int index]
		{
			get
			{
				return (SqlChildToParentLink)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				this.AssignItem(value);
				List[index] = value;
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(SqlChildToParentLink item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SqlChildToParentLink item)
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
		public SqlChildToParentLink[] ToArray()
		{
			return (SqlChildToParentLink[])InnerList.ToArray(typeof(SqlChildToParentLink));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(SqlChildToParentLink[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		public SqlChildToParentLink this[string nativeName]
		{
			get
			{
				foreach(SqlChildToParentLink link in this.InnerList)
				{
					if(string.Compare(link.NativeName, nativeName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
						return link;
				}
				return null;
			}
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
