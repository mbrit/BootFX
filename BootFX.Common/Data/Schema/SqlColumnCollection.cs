// BootFX - Application framework for .NET applications
// 
// File: SqlColumnCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using BootFX.Common.Entities;
using System.Text;

namespace BootFX.Common.Data.Schema
{
	/// <summary>
	/// Holds a collection of <see ref="SqlColumn">SqlColumn</see> instances.
	/// </summary>
	public class SqlColumnCollection : CollectionBase
	{
		/// <summary>
		/// Private field to support <c>Owner</c> property.
		/// </summary>
		private SqlMemberWithColumns _owner;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal SqlColumnCollection(SqlMemberWithColumns owner)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");
			_owner = owner;
		}

		/// <summary>
		/// Gets the owner.
		/// </summary>
		private SqlMemberWithColumns Owner
		{
			get
			{
				// returns the value...
				return _owner;
			}
		}

		/// <summary>
		/// Gets the index.
		/// </summary>
		private SqlIndex Index
		{
			get
			{
				return this.Owner as SqlIndex;
			}
		}

		/// <summary>
		/// Gets the table.
		/// </summary>
		private SqlTable Table
		{
			get
			{
				return this.Owner as SqlTable;
			}
		}
		
		/// <summary>
		/// Adds a SqlColumn instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(SqlColumn item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			AssignItem(item);
			List.Add(item);
		}  

		/// <summary>
		/// Assigns the column to the given table.
		/// </summary>
		/// <param name="table"></param>
		private void AssignItem(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			
			// set...
			if(this.Table != null)
				column.SetTable(this.Table);
		}

		/// <summary>
		/// Adds a set of SqlColumn instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlColumn[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of SqlColumn instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlColumnCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a SqlColumn instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SqlColumn item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			this.AssignItem(item);
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a SqlColumn item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SqlColumn item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlColumn this[int index]
		{
			get
			{
				return (SqlColumn)List[index];
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
		/// Gets an item with the given name.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlColumn this[string nativeName]
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
		public int IndexOf(SqlColumn item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SqlColumn item)
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
		public SqlColumn[] ToArray()
		{
			return (SqlColumn[])InnerList.ToArray(typeof(SqlColumn));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(SqlColumn[] items, int index)
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
		/// Gets the columns defined as key columns.
		/// </summary>
		/// <returns></returns>
		internal SqlColumn[] GetColumnsWithFlags(EntityFieldFlags flags)
		{
			// walk...
			ArrayList results = new ArrayList();
			foreach(SqlColumn column in this.InnerList)
			{
				if(column.GetEntityFieldFlags(flags))
					results.Add(column);
			}

			return (SqlColumn[])results.ToArray(typeof(SqlColumn));
		}

		/// <summary>
		/// Sorts the collection by table name order.
		/// </summary>
		public void SortByNativeName()
		{
			this.InnerList.Sort(new SqlMemberNativeNameComparer());
		}

		/// <summary>
		/// Sorts the collection by table name order.
		/// </summary>
		public void SortByOrdinal()
		{
			this.InnerList.Sort(new SqlMemberNativeNameComparer(true));
		}

        public string GetNamesAsCsvString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (SqlColumn column in this.InnerList)
            {
                if (builder.Length > 0)
                    builder.Append(",");
                builder.Append(column.Name);
            }

            // return...
            return builder.ToString();
        }

        internal List<string> GetNativeNamesSorted()
        {
            List<string> names = new List<string>();
            foreach (SqlColumn column in this.InnerList)
                names.Add(column.NativeName);

            // return...
            names.Sort();
            return names;
        }
    }
}
