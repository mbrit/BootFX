// BootFX - Application framework for .NET applications
// 
// File: TouchedValueCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;
using System.Collections;
using BootFX.Common;
using BootFX.Common.Xml;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Holds a collection of <see ref="TouchedValue">TouchedValue</see> instances.
	/// </summary>
	public class TouchedValueCollection : CollectionBase
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public TouchedValueCollection()
		{
		}
		
		/// <summary>
		/// Adds a TouchedValue instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(TouchedValue item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a set of TouchedValue instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(TouchedValue[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				Add(items[index]);
		}  
	
		/// <summary>
		/// Adds a set of TouchedValue instances to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(TouchedValueCollection items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Count; index++)
				Add(items[index]);
		}  
		
		/// <summary>
		/// Inserts a TouchedValue instance into the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, TouchedValue item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  
	
		/// <summary>
		/// Removes a TouchedValue item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(TouchedValue item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public TouchedValue this[int index]
		{
			get
			{
				return (TouchedValue)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}

		/// <summary>
		/// Gets the touched value for the given field.
		/// </summary>
		public TouchedValue this[EntityField field]
		{
			get
			{
				int index = this.IndexOf(field);
				if(index != -1)
					return this[index];
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the index of the given field.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public int IndexOf(EntityField field)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// walk...
			for(int index = 0; index < this.InnerList.Count; index++)
			{
				if(this[index].Field == field)
					return index;
			}

			// nope...
			return -1;
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(TouchedValue item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(TouchedValue item)
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
		public TouchedValue[] ToArray()
		{
			return (TouchedValue[])InnerList.ToArray(typeof(TouchedValue));
		}

		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public void CopyTo(TouchedValue[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			InnerList.CopyTo(items, index);
		}

		/// <summary>
		/// Populates an XML element with the values within the collection.
		/// </summary>
		/// <param name="element"></param>
		internal void PopulateXmlElement(XmlElement element, XmlNamespaceManagerEx manager)
		{
			if(element == null)
				throw new ArgumentNullException("element");			
			if(manager == null)
				throw new ArgumentNullException("manager");			

			// walk...
			foreach(TouchedValue value in this.InnerList)
			{
				XmlElement valueElement = element.OwnerDocument.CreateElement(
					XmlConvert.EncodeName(value.Field.Name));
				element.AppendChild(valueElement);

				// add...
				if(value.OldValue != null)
					this.AddValueElement(valueElement, "Old", value.OldValue);
				this.AddValueElement(valueElement, "New", value.NewValue);
			}
		}
		
		/// <summary>
		/// Adds a value element to the XML.
		/// </summary>
		/// <param name="valueElement"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		private void AddValueElement(XmlElement valueElement, string name, object value)
		{
			if(valueElement == null)
				throw new ArgumentNullException("valueElement");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// set...
			if(value == null || value is DBNull)
			{
				XmlElement childElement = valueElement.OwnerDocument.CreateElement(name);

				// set...
				if(value == null)
					XmlHelper.SetAttributeValue(childElement, "clrNull", true);
				else if(value is DBNull)
					XmlHelper.SetAttributeValue(childElement, "dbNull", true);
				else
					throw new InvalidOperationException("Value cannot be handled.");
			}
			else
				XmlHelper.AddElement(valueElement, name, value);
		}

		/// <summary>
		/// Gets the blocks data as plain-text.
		/// </summary>
		/// <param name="builder"></param>
		internal void ToPlainText(StringBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			// walk...
			foreach(TouchedValue value in this.InnerList)
			{
				if(builder.Length > 0)
					builder.Append("\r\n");

				// add...
				builder.Append(value.Field.Name);
				builder.Append(": ");

				// from, to...
				if(value.OldValue != null)
				{
					AppendPlainTextValue(builder, value.OldValue);
					builder.Append(" --> ");
					AppendPlainTextValue(builder, value.NewValue);
				}
				else
				{
					builder.Append("*NEW* ");
					AppendPlainTextValue(builder, value.NewValue);
				}
			}
		}

		/// <summary>
		/// Appends a value into the string.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="value"></param>
		private void AppendPlainTextValue(StringBuilder builder, object value)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			// quote...
			builder.Append("\"");

			// data...
			if(value is DateTime)
				builder.Append(((DateTime)value).ToString("dd/MMM/yyyy HH:mm:ss"));
			else
				builder.Append(value);

			// quote...
			builder.Append("\"");
		}
	}
}
