// BootFX - Application framework for .NET applications
// 
// File: Lookup.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using BootFX.Common.Data;

namespace BootFX.Common
{
	/// <summary>
	/// Defines a thread-safe lookup class.
	/// </summary>
	/// <remarks>Essentially, this class wraps a dictionary but provides a way for the dictionary to automatically fetch values that
	/// are not already loaded into it through an event.</remarks>
	// mbr - 17-08-2006 - made to derive from PropertyBagByObjectBase.	
    // mbr - 2010-01-20 - changed this use wrapper classes for values, in order to support expiration...
	public class Lookup : PropertyBagByObjectBase, ILookup, IEnumerable, IDictionary
	{
        /// <summary>
        /// Private value to support the <see cref="ExpirationPeriod">ExpirationPeriod</see> property.
        /// </summary>
        private TimeSpan _expirationPeriod = TimeSpan.MinValue;

        /// <summary>
		/// Private field to support <see cref="CaseSensitive"/> property.
		/// </summary>
		private bool _caseSensitive;
		
		/// <summary>
		/// Raised when an item should be created.
		/// </summary>
		public event BootFX.Common.CreateLookupItemEventHandler CreateItemValue;

		/// <summary>
		/// Private field to support <c>InnerValues</c> property.
		/// </summary>
		private IDictionary _innerValues;

		/// <summary>
		/// Private field to support <c>Lock</c> property.
		/// </summary>
		private ReaderWriterLock _lock = new ReaderWriterLock();

		/// <summary>
		/// Constructor.
		/// </summary>
		public Lookup() 
            : this(true)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Lookup(bool caseSensitive)
		{
			_caseSensitive = caseSensitive;
			_innerValues = this.CreateDictionary();
		}

		/// <summary>
		/// Gets the casesensitive.
		/// </summary>
		private bool CaseSensitive
		{
			get
			{
				return _caseSensitive;
			}
		}

		private IDictionary CreateDictionary()
		{
			if(this.CaseSensitive)
				return new HybridDictionary();
			else
				return CollectionsUtil.CreateCaseInsensitiveHashtable();
		}

		/// <summary>
		/// Gets the lock.
		/// </summary>
		private ReaderWriterLock Lock
		{
			get
			{
				// returns the value...
				return _lock;
			}
		}
		
		/// <summary>
		/// Gets the values.
		/// </summary>
		private IDictionary InnerValues
		{
			get
			{
				// returns the value...
				return _innerValues;
			}
		}

		public override object GetValue(object key, Type type, object defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			// get it...
			object value = this.GetValue(key);

			// do we have a type?
			if(value != null)
			{
				if(type != null)
					return ConversionHelper.ChangeType(value, type, provider);
				else
					return value;
			}
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return defaultValue;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format("An item with key '{0}' ({1}) was not found.", key, key.GetType()));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}

		/// <summary>
		/// Gets the item with the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private object GetValue(object key)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			Lock.AcquireReaderLock(-1);
			try
			{
				// find it...
				if(!(InnerValues.Contains(key)))
				{
                    // mbr - 2010-01-20 - moved to method...
                    //Lock.UpgradeToWriterLock(-1);

                    //// try again...
                    //if(!(InnerValues.Contains(key)))
                    //{
                    //    // create it...
                    //    CreateLookupItemEventArgs e = new CreateLookupItemEventArgs(key);
                    //    this.OnCreateItemValue(e);

                    //    // add...
                    //    // mbr - 2010-01-20 - added wrap...
                    //    //InnerValues[key] = e.NewValue;
                    //    InnerValues[key] = Wrap(e.NewValue);
                    //}
                    LoadItem(key, false);
				}

				// return...
                // mbr - 2010-01-20 - added wrap...
                //return InnerValues[key];
                bool expired = false;
                object value = Unwrap(InnerValues[key], ref expired);

                // mbr - 2010-01-20 - expired...
                if (expired)
                {
                    // load and reload...
                    LoadItem(key, true);
                    value = Unwrap(InnerValues[key], ref expired);
                }

                // return...
                return value;
            }
			finally
			{
				Lock.ReleaseLock();
			}
		}

        /// <summary>
        /// Demand loads the item.
        /// </summary>
        /// <param name="key"></param>
        // mbr - 2010-01-20 - broken into separate method...
        private void LoadItem(object key, bool forceReload)
        {
            Lock.UpgradeToWriterLock(-1);

            // try again...
            if (forceReload || !(InnerValues.Contains(key)))
            {
                // create it...
                CreateLookupItemEventArgs e = new CreateLookupItemEventArgs(key);
                this.OnCreateItemValue(e);

                // add...
                // mbr - 2010-01-20 - added wrap...
                //InnerValues[key] = e.NewValue;
                InnerValues[key] = Wrap(e.NewValue);
            }
        }

        private object Unwrap(object value, ref bool expired)
        {
            if (value == null)
                return null;

            // get...
            if (!(value is LookupItem))
                throw new InvalidOperationException(string.Format("Item in collection is of type '{0}', not LookupItem.", value.GetType()));

            // return...
            return Unwrap((LookupItem)value, ref expired);
        }

        private object Unwrap(LookupItem item, ref bool expired)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            // return...
            expired = item.HasExpired;
            return item.Value;
        }

        /// <summary>
        /// Wraps a value for storage.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private LookupItem Wrap(object value)
        {
            // create...
            DateTime expires = DateTime.MinValue;
            if (this.HasExpiration)
                expires = DateTime.UtcNow.Add(this.ExpirationPeriod);

            // create...
            return new LookupItem(value, expires);
        }

		/// <summary>
		/// Raised the <see cref="CreateItemValue"/> event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnCreateItemValue(CreateLookupItemEventArgs e)
		{
			if(this.CreateItemValue != null)
				this.CreateItemValue(this, e);
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		public int Count
		{
			get
			{
				Lock.AcquireReaderLock(-1);
				try
				{
					return _innerValues.Count;
				}
				finally
				{
					Lock.ReleaseLock();
				}
			}
		}

		/// <summary>
		/// Clears the collection.
		/// </summary>
		public void Clear()
		{
			Lock.AcquireWriterLock(-1);
			try
			{
                // mbr - 2010-01-20 - this was wrong...
				//_innerValues = new HybridDictionary();
                _innerValues = this.CreateDictionary();
			}
			finally
			{
				Lock.ReleaseLock();
			}
		}

		/// <summary>
		/// Gets a value.
		/// </summary>
		public object this[object key]
		{
			get
			{
				return this.GetValue(key);
			}
			set
			{
				this.SetValue(key, value);
			}
		}

		public override void SetValue(object key, object value)
		{
			if(key == null)
				throw new ArgumentNullException("key");
				
			// set...
			this.Lock.AcquireWriterLock(-1);
			try
			{
                // mbr - 2010-01-20 - added wrap...
				//this.InnerValues[key] = value;
                this.InnerValues[key] = Wrap(value);
            }
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		/// <summary>
		/// Returns true if the lookup already contains the given key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(object key)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			this.Lock.AcquireReaderLock(-1);
			try
			{
				return this.InnerValues.Contains(key);
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		/// <summary>
		/// Gets the values in the lookup as an array.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object[] ToArray(Type type)
		{
			return this.ToArray(type, true);
		}

		/// <summary>
		/// Gets the values in the lookup as an array.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		// mbr - 10-10-2007 - for c7 - added.
		internal object[] ToArray(Type type, bool includeNulls)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// lock...
			this.Lock.AcquireReaderLock(-1);
			try
			{
				// create results...
				ArrayList results = new ArrayList();

				// walk...
				foreach(DictionaryEntry entry in this.InnerValues)
				{
                    // mbr - 2010-01-20 - unwrap not needed here, as the indexer has already done it...
					object value = this[entry.Key];

					// ok...
					bool ok = true;
					if(!(includeNulls) && value == null)
						ok = false;

					// ok?
					if(ok)
						results.Add(value);
				}

				// return...
				return (object[])results.ToArray(type);
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		/// <summary>
		/// Gets the values in the lookup as a dictionary.
		/// </summary>
		/// <returns></returns>
		public IDictionary ToDictionary()
		{
			IDictionary results = this.CreateDictionary();
			if(results == null)
				throw new InvalidOperationException("results is null.");

			this.Lock.AcquireReaderLock(-1);
			try
			{
                foreach (DictionaryEntry entry in this.InnerValues)
                {
                    // mbr - 2010-01-20 - added unwrap...
                    //results[entry.Key] = entry.Value;
                    bool expired = false;
                    results[entry.Key] = Unwrap(entry.Value, ref expired);
                }
			}
			finally
			{
				this.Lock.ReleaseLock();
			}

			// return...
			return results;
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return (IDictionaryEnumerator)this.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			IDictionary dictionary = this.ToDictionary();
			if(dictionary == null)
				throw new InvalidOperationException("dictionary is null.");

			// return...
			return dictionary.GetEnumerator();
		}

		public bool ContainsValue(object value)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			
			// walk...
			this.Lock.AcquireReaderLock(-1);
			try
			{
				foreach(DictionaryEntry entry in this.InnerValues)
				{
                    // mbr - 2010-01-20 - added unwrap...
                    //if (object.Equals(entry.Value, value))
                    bool expired = false;
                    object checkValue = Unwrap(entry.Value, ref expired);
                    if (object.Equals(checkValue, value))
						return true;
				}
				
				// nope...
				return false;
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public void Remove(object key)
		{
			this.Lock.AcquireWriterLock(-1);
			try
			{
				this.InnerValues.Remove(key);
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		public void Add(object key, object value)
		{
			this.Lock.AcquireWriterLock(-1);
			try
			{
                // mbr - 2010-01-20 - added wrap...
				//this.InnerValues.Add(key, value);
                this.InnerValues.Add(key, Wrap(value));
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		public ICollection Keys
		{
			get
			{
				return this.GetCollection(false);
			}
		}

		public ICollection Values
		{
			get
			{
				return this.GetCollection(true);
			}
		}

		private ICollection GetCollection(bool values)
		{
			this.Lock.AcquireReaderLock(-1);
			try
			{
				// create...
				ArrayList results = new ArrayList();
				foreach(DictionaryEntry entry in this.InnerValues)
				{
                    if (values)
                    {
                        // mbr - 2010-01-20 - added unwrap...
                        //results.Add(entry.Value);
                        bool expired = false;
                        results.Add(Unwrap(entry.Value, ref expired));
                    }
                    else
                        results.Add(entry.Key);
				}

				// return...
				return results;
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public void CopyTo(Array array, int index)
		{
			this.Lock.AcquireReaderLock(-1);
			try
			{
				this.InnerValues.CopyTo(array, index);
			}
			finally
			{
				this.Lock.ReleaseLock();
			}
		}

		public object SyncRoot
		{
			get
			{
				return null;
			}
		}

        /// <summary>
        /// Gets the ExpirationPeriod value.
        /// </summary>
        // mbr - 2010-01-20 - added...
        public TimeSpan ExpirationPeriod
        {
            get
            {
                return _expirationPeriod;
            }
            set
            {
                _expirationPeriod = value;
            }
        }

        /// <summary>
        /// Returns true if the class supports expiration of values.
        /// </summary>
        // mbr - 2010-01-20 - added...
        private bool HasExpiration
        {
            get
            {
                if (this.ExpirationPeriod == TimeSpan.MinValue)
                    return false;
                else
                    return true;
            }
        }
	}
}
