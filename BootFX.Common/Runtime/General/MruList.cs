// BootFX - Application framework for .NET applications
// 
// File: MruList.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace BootFX.Common
{
	/// <summary>
	/// Describes a list that reorders itself according to usage.
	/// </summary>
	[Serializable()]
	public class MruList : IEnumerable, IList
	{
		/// <summary>
		/// Private field to support <c>Items</c> property.
		/// </summary>
		private ArrayList _items = new ArrayList();
		
		/// <summary>
		/// Private field to support <c>Capacity</c> property.
		/// </summary>
		private int _capacity;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public MruList()
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public MruList(int capacity)
		{
			this.Capacity = capacity;
		}

		/// <summary>
		/// Gets the items.
		/// </summary>
		private ArrayList Items
		{
			get
			{
				// returns the value...
				return _items;
			}
		}

		/// <summary>
		/// Adds an item onto the top of the MRU stack, removing it from below if it's already there.
		/// </summary>
		/// <param name="item"></param>
		public void Push(object item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			
			// contains...
			int index = ((IList)this).IndexOf(item);
			if(index == -1)
			{
				// add...
				this.Items.Insert(0, item);

				// remove?
				if(this.Count == this.Capacity)
					this.Items.RemoveAt(this.Capacity - 1);
			}
			else
			{
				// move...
				this.Items.RemoveAt(index);
				this.Items.Insert(0, item);
			}
		}

		/// <summary>
		/// Gets or sets the capacity
		/// </summary>
		public int Capacity
		{
			get
			{
				return _capacity;
			}
			set
			{
				// check...
				if(this.Count > 0)
					throw new InvalidOperationException("Cannot set capacity when the list contains items.");

				// check to see if the value has changed...
				if(value != _capacity)
				{
					// set the value...
					_capacity = value;
				}
			}
		}

		/// <summary>
		/// Clears the list.
		/// </summary>
		public void Clear()
		{
			this.Items.Clear();
		}

		/// <summary>
		/// Gets the number of items in the stack.
		/// </summary>
		public int Count
		{
			get
			{
				return this.Items.Count;
			}
		}

		/// <summary>
		/// Gets the index of the given item.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		int IList.IndexOf(object item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			
			// find it...
			for(int index = 0; index < this.Count; index++)
			{
				if(object.Equals(this.Items[index], item))
					return index;
			}

			// nope...
			return -1;
		}

		public IEnumerator GetEnumerator()
		{
			return this.Items.GetEnumerator();
		}

		bool IList.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public object this[int index]
		{
			get
			{
				return this.Items[index];
			}
			set
			{
				this.Items[index] = value;
			}
		}

		void IList.RemoveAt(int index)
		{
			this.Items.RemoveAt(index);
		}

		void IList.Insert(int index, object value)
		{
			this.Items.Insert(index, value);
		}

		void IList.Remove(object value)
		{
			this.Items.Remove(value);
		}

		public bool Contains(object value)
		{
			int index = ((IList)this).IndexOf(value);
			if(index == -1)
				return false;
			else
				return true;
		}

		int IList.Add(object value)
		{
			return this.Items.Add(value);
		}

		bool IList.IsFixedSize
		{
			get
			{
				return this.Items.IsFixedSize;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return this.Items.IsSynchronized;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.Items.CopyTo(array, index);
		}

		object ICollection.SyncRoot
		{
			get
			{
				return this.Items.SyncRoot;
			}
		}

		/// <summary>
		/// Gets the MRU list as XML.
		/// </summary>
		/// <returns></returns>
		public string ToXml()
		{
			SimpleXmlPropertyBag bag = new SimpleXmlPropertyBag();
			bag.Add("Capacity", this.Capacity);
			bag.Add("Count", this.Count);

			// items...
			for(int index = 0; index < this.Count; index++)
				bag.Add("Item" + index.ToString(), this[index]);

			// return...
			string xml = bag.ToXml("Mru");
			return xml;
		}

		public void Save(string xmlFilePath)
		{
			if(xmlFilePath == null)
				throw new ArgumentNullException("xmlFilePath");
			if(xmlFilePath.Length == 0)
				throw new ArgumentOutOfRangeException("'xmlFilePath' is zero-length.");
			
			// save...
			using(StreamWriter writer = new StreamWriter(xmlFilePath, false, Encoding.Unicode))
				writer.Write(this.ToXml());
		}

        /// <summary>
        /// Loads the MRU list from an XML stream.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static MruList LoadXml(string xml)
        {
            if (xml == null)
                throw new ArgumentNullException("xml");
            if (xml.Length == 0)
                throw new ArgumentException("'xml' is zero-length.");

            // load...
            SimpleXmlPropertyBag bag = SimpleXmlPropertyBag.FromXml(xml, typeof(SimpleXmlPropertyBag));
            if (bag == null)
                throw new InvalidOperationException("'bag' is null.");

            // defer...
            return LoadInternal(bag);
        }

        /// <summary>
        /// Loads the MRU list from a file.
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns></returns>
		public static MruList Load(string xmlFilePath)
		{
			if(xmlFilePath == null)
				throw new ArgumentNullException("xmlFilePath");
			if(xmlFilePath.Length == 0)
				throw new ArgumentOutOfRangeException("'xmlFilePath' is zero-length.");
			
			// get...
			SimpleXmlPropertyBag bag = SimpleXmlPropertyBag.Load(xmlFilePath, typeof(SimpleXmlPropertyBag));
			if(bag == null)
				throw new InvalidOperationException("bag is null.");

            // defer...
            return LoadInternal(bag);
        }

        /// <summary>
        /// Loads XML from a property bag.
        /// </summary>
        /// <param name="bag"></param>
        /// <returns></returns>
        private static MruList LoadInternal(SimpleXmlPropertyBag bag)
        {
            if (bag == null)
                throw new ArgumentNullException("bag");

			// get...
			MruList list = new MruList(bag.GetInt32Value("Capacity", 20, Cultures.System, OnNotFound.ReturnNull));

			// walk...
			int count = bag.GetInt32Value("Count", 0, Cultures.System, OnNotFound.ThrowException);

			// add...
			ArrayList values = new ArrayList();
			for(int index = 0; index < count; index++)
			{
				// get...
				string name = "Item" + index.ToString();
				values.Add(bag.GetValue(name, null, null, Cultures.System, OnNotFound.ReturnNull));
			}

			// set...
			list.BulkAdd(values);

			// return...
			return list;
		}

		private void BulkAdd(IList list)
		{
			if(list == null)
				throw new ArgumentNullException("list");
			
			// walk...
			this.Clear();
			this.Items.AddRange(list);
		}
	}
}
