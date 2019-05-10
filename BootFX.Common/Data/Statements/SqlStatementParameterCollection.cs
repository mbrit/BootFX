// BootFX - Application framework for .NET applications
// 
// File: SqlStatementParameterCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Data;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Holds a collection of <c ref="SqlStatementParameter">SqlStatementParameter</c> instances.
	/// </summary>
	[Serializable()]
	public class SqlStatementParameterCollection : CollectionBase
	{
		/// <summary>
		/// Private field to support <c>DefaultUniqueNamePrefix</c> property.
		/// </summary>
		private string _defaultUniqueNamePrefix = "z";
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlStatementParameterCollection()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 10-10-2007 - for c7 - added.		
		internal SqlStatementParameterCollection(string defaultUniqueNamePrefix)
		{
			if(defaultUniqueNamePrefix == null)
				throw new ArgumentNullException("defaultUniqueNamePrefix");
			if(defaultUniqueNamePrefix.Length == 0)
				throw new ArgumentOutOfRangeException("'defaultUniqueNamePrefix' is zero-length.");
			
			// set...
			_defaultUniqueNamePrefix = defaultUniqueNamePrefix;
		}

		/// <summary>
		/// Adds a SqlStatementParameter instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(SqlStatementParameter item)
		{
			if(item == null)
				throw new ArgumentNullException("item");

			if(!(IndexOf(item.Name) == -1))
				return;

			List.Add(item);
		}  

		// mbr - 06-01-2006 - added.
		public void Add(string name, DbType dbType, object value)
		{
			SqlStatementParameter parameter = new SqlStatementParameter(name, dbType, value);
			this.Add(parameter);
		}

		// mbr - 06-01-2006 - added.		
		public void Add(string name, DbType dbType, object value, ParameterDirection direction)
		{
			SqlStatementParameter parameter = new SqlStatementParameter(name, dbType, value, direction);
			this.Add(parameter);
		}

		/// <summary>
		/// Adds a parameter with a unique name.
		/// </summary>
		/// <param name="dbType"></param>
		/// <param name="value"></param>
		/// <returns>The new parameter's name.</returns>
		// mbr - 06-01-2006 - added.
		public string Add(DbType dbType, object value)
		{
			string name = this.GetUniqueParameterName();
			if(name == null)
				throw new InvalidOperationException("'name' is null.");
			if(name.Length == 0)
				throw new InvalidOperationException("'name' is zero-length.");

			// add...
			this.Add(name, dbType, value);

			// return...
			return name;
		}

		/// <summary>
		/// Adds a set of SqlStatementParameter instances to the collection.
		/// </summary>
		/// <param name="items">The item to add.</param>
		public void AddRange(SqlStatementParameter[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of SqlStatementParameter instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(SqlStatementParameterCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a SqlStatementParameter instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, SqlStatementParameter item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a SqlStatementParameter item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(SqlStatementParameter item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlStatementParameter this[int index]
		{
			get
			{
				return (SqlStatementParameter)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}
				
		/// <summary>
		/// Gets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public SqlStatementParameter this[string name]
		{
			get
			{
				return (SqlStatementParameter)this.GetParameter(name, OnNotFound.ReturnNull);
			}
		}

		/// <summary>
		/// Gets the parameter with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public SqlStatementParameter GetParameter(string name, OnNotFound onNotFound)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			int index = this.IndexOf(name);
			if(index != -1)
				return this[index];
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format("A parameter with name '{0}' was not found.", name));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(SqlStatementParameter item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// check...
			for(int index = 0; index < this.Count; index++)
			{
				if(string.Compare(this[index].Name, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				{
					return index;
				}
			}

			// nope...
			return -1;
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(SqlStatementParameter item)
		{
			if(IndexOf(item) == -1)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="name">The name of the parameter to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(string name)
		{
			if(IndexOf(name) == -1)
				return false;
			else
				return true;
		}
		

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public SqlStatementParameter[] ToArray()
		{
			return (SqlStatementParameter[])InnerList.ToArray(typeof(SqlStatementParameter));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(SqlStatementParameter[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		/// <summary>
		/// Gets or sets the defaultuniquenameprefix
		/// </summary>
		// mbr - 10-10-2007 - for c7 - changed.		
		private string DefaultUniqueNamePrefix
		{
			get
			{
				return _defaultUniqueNamePrefix;
			}
		}

		/// <summary>
		/// Gets a unique parameter name.
		/// </summary>
		/// <returns></returns>
		public string GetUniqueParameterName()
		{
			// mbr - 10-10-2007 - for c7 - changed			
//			return this.GetUniqueParameterName("z");
			return this.GetUniqueParameterName(this.DefaultUniqueNamePrefix);
		}

		/// <summary>
		/// Gets a unique parameter name.
		/// </summary>
		/// <returns></returns>
		public string GetUniqueParameterName(string seed)
		{
			if(seed == null)
				throw new ArgumentNullException("seed");
			if(seed.Length == 0)
				throw new ArgumentOutOfRangeException("'seed' is zero-length.");

			// get the id...
			int index = this.Count;
			while(true)
			{
				string name = seed + index.ToString();
				if(!(this.Contains(name)))
					return name;
				index++;
			}
		}
	}
}
