// BootFX - Application framework for .NET applications
// 
// File: MailMessageRecipientCollection.cs
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
using Nm = System.Net.Mail;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Holds a collection of <see ref="MailMessageRecipient">MailMessageRecipient</see> instances.
	/// </summary>
	public class MailMessageRecipientCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public MailMessageRecipientCollection()
		{
		}
		
		/// <summary>
		/// Adds a MailMessageRecipient instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(MailMessageRecipient item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Add(item);
		}  

		/// <summary>
		/// Adds a set of MailMessageRecipient instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(MailMessageRecipient[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of MailMessageRecipient instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(MailMessageRecipientCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a MailMessageRecipient instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, MailMessageRecipient item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a MailMessageRecipient item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(MailMessageRecipient item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public MailMessageRecipient this[int index]
		{
			get
			{
				return (MailMessageRecipient)List[index];
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
		public int IndexOf(MailMessageRecipient item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(MailMessageRecipient item)
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
		public MailMessageRecipient[] ToArray()
		{
			return (MailMessageRecipient[])InnerList.ToArray(typeof(MailMessageRecipient));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(MailMessageRecipient[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		/// <summary>
		/// Populates the list from a string.
		/// </summary>
		/// <param name="value"></param>
		public void AddRange(string value)
		{
			// anything?
			if(value == null)
				return;
			value = value.Trim();
			if(value.Length == 0)
				return;

			// split...
			string[] parts = value.Split(';');
			foreach(string part in parts)
			{
				string usePart = part.Trim();
				if(usePart.Length > 0)
				{
					MailMessageRecipient recipient = MailMessageRecipient.Parse(usePart);
					if(recipient == null)
						throw new InvalidOperationException("recipient is null.");
					this.Add(recipient);
				}
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach(MailMessageRecipient recipient in this.InnerList)
			{
				if(builder.Length > 0)
					builder.Append("; ");
				builder.Append(recipient.ToString());
			}

			// return...
			return builder.ToString();
		}

		internal Nm.MailAddressCollection ToMailAddressCollection()
		{
			Nm.MailAddressCollection results = new Nm.MailAddressCollection();
			foreach(MailMessageRecipient recipient in this.InnerList)
				results.Add(recipient.ToMailAddress());

			// return...
			return results;
		}
	}
}
