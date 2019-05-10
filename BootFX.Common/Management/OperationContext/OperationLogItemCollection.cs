// BootFX - Application framework for .NET applications
// 
// File: OperationLogItemCollection.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Collections;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Holds a collection of <see ref="OperationLogItem">OperationLogItem</see> instances.
	/// </summary>
	// mbr - 28-02-2008 - added implementation of IUserMessages.
	public class OperationLogItemCollection : CollectionBase, IUserMessages
	{
		/// <summary>
		/// Raised when an item is added.
		/// </summary>
		public event OperationLogItemEventHandler ItemAdded;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public OperationLogItemCollection()
		{
		}
		
		public int Add(LogLevel level, string message, Exception ex)
		{
			return this.Add(new OperationLogItem(level, message, ex));
		}

		/// <summary>
		/// Adds a OperationLogItem instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(OperationLogItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of OperationLogItem instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(OperationLogItem[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of OperationLogItem instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(OperationLogItemCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a OperationLogItem instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, OperationLogItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a OperationLogItem item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(OperationLogItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public OperationLogItem this[int index]
		{
			get
			{
				return (OperationLogItem)List[index];
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
		public int IndexOf(OperationLogItem item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(OperationLogItem item)
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
		public OperationLogItem[] ToArray()
		{
			return (OperationLogItem[])InnerList.ToArray(typeof(OperationLogItem));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(OperationLogItem[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		public OperationLogItemCollection GetDebugItems(bool andAbove)
		{
			return this.FilterItems(LogLevel.Debug, andAbove);
		}

		public OperationLogItemCollection GetInfoItems(bool andAbove)
		{
			return this.FilterItems(LogLevel.Info, andAbove);
		}

		public OperationLogItemCollection GetWarningItems(bool andAbove)
		{
			return this.FilterItems(LogLevel.Warn, andAbove);
		}

		public OperationLogItemCollection GetErrorItems(bool andAbove)
		{
			return this.FilterItems(LogLevel.Error, andAbove);
		}

		public OperationLogItemCollection GetFatalItems(bool andAbove)
		{
			return this.FilterItems(LogLevel.Fatal, andAbove);
		}

		public OperationLogItemCollection FilterItems(LogLevel level, bool andAbove)
		{
			OperationLogItemCollection results = new OperationLogItemCollection();
			foreach(OperationLogItem item in this.InnerList)
			{
				if(((int)item.Level > (int)level && !(andAbove)) || ((int)item.Level >= (int)level && andAbove))
					results.Add(item);
			}

			// return...
			return results;
		}

		public override string ToString()
		{
			return this.ToString(OperationLogItem.DefaultFormatFlags);
		}

		public string ToString(OperationLogItemFormatFlags flags)
		{
			string sep = "\r\n";
			if((int)(flags & OperationLogItemFormatFlags.UseBrTagSeparator) != 0)
				sep = "<BR />";

			// build...
			StringBuilder builder = new StringBuilder();
			foreach(OperationLogItem item in this.InnerList)
			{
				if(builder.Length > 0)
					builder.Append(sep);
				builder.Append(item.ToString(flags));
			}

			// return...
			return builder.ToString();
		}
		
		/// <summary>
		/// Raises the <c>ItemAdded</c> event.
		/// </summary>
		protected virtual void OnItemAdded(OperationLogItemEventArgs e)
		{
			// raise...
			if(ItemAdded != null)
				ItemAdded(this, e);
		}

		protected override void OnInsertComplete(int index, object value)
		{
			base.OnInsertComplete (index, value);

			// ok...
			if(value == null)
				throw new ArgumentNullException("value");			
			this.OnItemAdded(new OperationLogItemEventArgs((OperationLogItem)value));
		}

		/// <summary>
		/// Gets the user messages.
		/// </summary>
		/// <returns></returns>
		// mbr - 28-02-2008 - added implementation of IUserMessages.
		public string[] GetUserMessages()
		{
			string[] results = new string[this.Count];
			for(int index = 0; index < this.Count; index++)
				results[index] = this[index].ToString();

			// return...
			return results;
		}
	}
}
