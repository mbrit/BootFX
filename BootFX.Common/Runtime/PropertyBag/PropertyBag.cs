// BootFX - Application framework for .NET applications
// 
// File: PropertyBag.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;
using System.Runtime.Serialization;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Describes a class that extends the prinicple of a dictionary with the notion of persistence.
	/// </summary>
	// mbr - 03-03-2006 - changed to extend PropertyBagBase.	
	[Serializable()]
	public class PropertyBag : PropertyBagByNameBase, IDisposable, IDictionary, ISerializable, ICloneable
	{
		private const string SerializationInfoPrefix = "__bfxPb";

		/// <summary>
		/// Raised when a value has been set.
		/// </summary>
		public event ValueSetEventHandler ValueSet;
		
		/// <summary>
		/// Private field to support <c>CaseInsensitive</c> property.
		/// </summary>
		private bool _caseInsensitive = true;
		
		/// <summary>
		/// Private field to support <c>IsInitialized</c> property.
		/// </summary>
		private bool _isInitialized = false;
		
		/// <summary>
		/// Private field to support <c>InnerValues</c> property.
		/// </summary>
		private IDictionary _innerValues = new HybridDictionary();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public PropertyBag()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected PropertyBag(SerializationInfo info, StreamingContext context)
		{
			// basics...
			_isInitialized = info.GetBoolean(SerializationInfoPrefix + "_isInitialized");
			_caseInsensitive = info.GetBoolean(SerializationInfoPrefix + "_caseInsensitive");

			// mbr - 20-10-2005 - check GetObjectData for an explanation of why this does not look as easy
			// as it could be...
			// values...
			string[] names = (string[])info.GetValue(SerializationInfoPrefix + "names", typeof(string[]));
			object[] values = (object[])info.GetValue(SerializationInfoPrefix + "values", typeof(object[]));
			if(names == null)
				throw new InvalidOperationException("names is null.");
			if(values == null)
				throw new InvalidOperationException("values is null.");
			if(names.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'names' and 'values': {0} cf {1}.", names.Length, values.Length));

			// add...
			for(int index = 0; index < names.Length; index++)
				this.SetValue(names[index], values[index]);
		}

		/// <summary>
		/// Finalizer.
		/// </summary>
		~PropertyBag()
		{
			try
			{
				this.Dispose(DisposeType.FromFinalizer);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Gets or sets the value with the given name.
		/// </summary>
		public object this[string name]
		{
			get
			{
				return GetValue(name, typeof(object), null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				SetValue(name, value);
			}
		}

		/// <summary>
		/// Sets the given value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public override void SetValue(string name, object value)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// ensure...
			this.EnsureInitialized();
			
			// key...
			object key = this.GetKey(name);
			this.InnerValues[key] = value;

			// raise...
			this.OnValueSet(new ValueSetEventArgs(name));
		}

		/// <summary>
		/// Raises the <c>ValueSet</c> event.
		/// </summary>
		protected virtual void OnValueSet(ValueSetEventArgs e)
		{
			// raise...
			if(ValueSet != null)
				ValueSet(this, e);
		}

		/// <summary>
		/// Ensures that the property bag is initialized.
		/// </summary>
		private void EnsureInitialized()
		{
			if(this.IsInitialized == false)
				this.Initialize();
		}

		/// <summary>
		/// Gets the given value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <param name="defaultValue"></param>
		/// <param name="onNotFound">In this case, <see cref="OnNotFound.ReturnNull"></see> means 'return default value'.</param>
		/// <returns></returns>
		public override object GetValue(string name, Type type, object defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// mbr - 21-07-2006 - type can be null.			
			if(type == null)
//				throw new ArgumentNullException("type");
				type = typeof(object);

			// ensure...
			this.EnsureInitialized();
			
			// look up the value...
			object key = this.GetKey(name);
			object value = null;
			bool found = false;
			if(this.InnerValues.Contains(key) == false)
			{
				// load it...
				value = this.LoadValue(name, ref found);

				// did we find it?
				if(found == false)
					value = MissingItem.Value;

				// add...
				this.InnerValues[key] = value;
			}
			else
				value = this.InnerValues[key];

			// found?
			if(value is MissingItem)
			{
				// now what?
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						value = defaultValue;
						break;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "A value with name '{0}' was not found.", name));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}

			// return it...
			if(type != typeof(object))
			{
				// were we asked for a type?
				if(typeof(Type).IsAssignableFrom(type) == true)
				{
					if(value is Type)
						return value;

					// value...
					value = Convert.ToString(value);
					if(value == null || ((string)value).Length == 0)
					{
						if(onNotFound == OnNotFound.ThrowException)
							throw new InvalidOperationException(string.Format(Cultures.System, "The value for '{0}' was an empty string, and an empty string cannot be used to load a type.", name));
						else
							return null;
					}

					// try and load it...
					try
					{
						return Type.GetType((string)value, true, true);
					}
					catch(Exception ex)
					{
						// mbr - 28-05-2006 - if we can't do it -- perhaps we're in the GAC?  look for the name...
						string[] parts = ((string)value).Split(',');
						if(parts.Length == 2)
						{
							if(parts[1].Trim().ToLower().StartsWith("BootFX.Common"))
							{
								AssemblyName asmName = this.GetType().Assembly.GetName();

								// append...
								StringBuilder builder = new StringBuilder();
								builder.Append(value);
								builder.Append(", Version=");
								builder.Append(asmName.Version.ToString());
								builder.Append(", Culture=neutral, PublicKeyToken=");
								foreach(byte b in asmName.GetPublicKeyToken())
									builder.Append(b.ToString("x2"));

								// try again...
								try
								{
									return Type.GetType(builder.ToString(), true, true);
								}
								catch(Exception ex2)
								{
									throw new InvalidOperationException(string.Format(Cultures.System, "Neither the value '{0}', nor the value '{1}' could be used to load a type.\r\n(Original exception: {2})", 
										value, builder, ex.Message), ex2);
								}
							}
						}

						// still failed?
						throw new InvalidOperationException(string.Format(Cultures.System, "The value '{0}' cannot be used to load a type.", value), ex);
					}
				}
				else
				{
					// convert it...
					return ConversionHelper.ChangeType(value, type, provider, ConversionFlags.Safe);
				}
			}
			else
			{
				// just return the value...
				return value;
			}
		}

		/// <summary>
		/// Gets the key for the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private object GetKey(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// mangle it if we're case insensitive...
			if(this.CaseInsensitive == true)
				return name.ToLower(Cultures.System);
			else
				return name;
		}

		/// <summary>
		/// Gets or sets the caseinsensitive
		/// </summary>
		public bool CaseInsensitive
		{
			get
			{
				return _caseInsensitive;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _caseInsensitive)
				{
					// set the value...
					_caseInsensitive = value;
				}
			}
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		protected IDictionary InnerValues
		{
			get
			{
				// returns the value...
				if(_innerValues == null)
					throw new ObjectDisposedException(this.GetType().FullName);

				// return...
				return _innerValues;
			}
		}

		ICollection IDictionary.Values
		{
			get
			{
				if(InnerValues == null)
					throw new InvalidOperationException("InnerValues is null.");
				return this.InnerValues.Values;
			}
		}

		/// <summary>
		/// Returns true if the property bag is read only.
		/// </summary>
		public virtual bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Loads a value from the underlying store.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		protected virtual object LoadValue(string name, ref bool found)
		{
			// not found...
			found = false;
			return null;
		}

		/// <summary>
		/// Returns true if the bag contains the given item.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get the value...
			object value = this.GetValue(name, typeof(object), MissingItem.Value, Cultures.System, OnNotFound.ReturnNull);
			if(value is MissingItem)
				return false;
			else
				return true;
		}

		/// <summary>
		/// Disposes the property bag.
		/// </summary>
		/// <remarks>This is used as a last chance commit of values to the underlying store, and therefore does not follow a classic
		/// dispose pattern as indicating that external resources are used.</remarks>
		public void Dispose()
		{
			this.Dispose(DisposeType.FromDispose);
		}

		/// <summary>
		/// Disposes the bag.
		/// </summary>
		protected virtual void Dispose(DisposeType disposeType)
		{
            // reset...
            if (_innerValues != null)
            {
                _innerValues.Clear();
                _innerValues = null;
            }

			// suppress finalization...
			GC.SuppressFinalize(this);
		}

//		/// <summary>
//		/// Writes the values in the bag to XML.
//		/// </summary>
//		/// <param name="xml"></param>
//		/// <param name="context"></param>
//		protected override void WriteXml(System.Xml.XmlWriter xml, WriteXmlContext context)
//		{
//			if(xml == null)
//				throw new ArgumentNullException("xml");
//			if(context == null)
//				throw new ArgumentNullException("context");
//			
//			// create a persister...
//			XmlPersister persister = new XmlPersister(this.InnerValues.GetType(), "InnerValues", context.Encoding);
//			persister.ToXml(this.InnerValues, xml);
//		}

		/// <summary>
		/// Gets whether the object is initialized.
		/// </summary>
		protected bool IsInitialized
		{
			get
			{
				// returns the value...
				return _isInitialized;
			}
		}

		/// <summary>
		/// Initializes the object.
		/// </summary>
		private void Initialize()
		{
			if(this.IsInitialized)
				return;

			// run...
			this.DoInitialize();

			// set...
			_isInitialized = true;
		}

		/// <summary>
		/// Performs initialization of the object.
		/// </summary>
		protected virtual void DoInitialize()
		{
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerator GetEnumerator()
		{
			return this.InnerValues.GetEnumerator();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return this.InnerValues.GetEnumerator();
		}

		/// <summary>
		/// Replaces the values internally stored with these.
		/// </summary>
		/// <param name="newValues"></param>
		protected internal void ReplaceValues(IDictionary newValues)
		{
			if(newValues == null)
				throw new ArgumentNullException("newValues");

			// clear...
			this.Clear();
			foreach(DictionaryEntry entry in newValues)
				this.InnerValues.Add(entry.Key, entry.Value);
		}

		void IDictionary.Remove(object key)
		{
			this.AssertKey(key);
			this.Remove((string)key);
		}

		public void Remove(string key)
		{
			if(InnerValues == null)
				throw new InvalidOperationException("InnerValues is null.");
			this.InnerValues.Remove(key);
		}

		public void Clear()
		{
			if(InnerValues == null)
				throw new InvalidOperationException("InnerValues is null.");
			this.InnerValues.Clear();
		}

//		public ICollection InnerValues
//		{
//			get
//			{
//				if(InnerValues == null)
//					throw new InvalidOperationException("InnerValues is null.");
//				return this.InnerValues.InnerValues;
//			}
//		}

		private void AssertKey(object key)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			if(!(key is string))
				throw new ArgumentException(string.Format("Key is of type '{0}', and only strings are supported.", key.GetType()));
		}

		void IDictionary.Add(object key, object value)
		{
			this.AssertKey(key);
		
			// ad...
			if(InnerValues == null)
				throw new InvalidOperationException("InnerValues is null.");
			this.Add((string)key, value);
		}

		public ICollection Keys
		{
			get
			{
				if(InnerValues == null)
					throw new InvalidOperationException("InnerValues is null.");
				return this.InnerValues.Keys;
			}
		}

		public string[] AllKeys
		{
			get
			{
				ArrayList list = new ArrayList(this.InnerValues.Keys);
				return (string[])list.ToArray(typeof(string));
			}
		}

		public object[] AllValues
		{
			get
			{
				ArrayList list = new ArrayList(this.InnerValues.Values);
				return (object[])list.ToArray(typeof(object));
			}
		}

		public bool IsFixedSize
		{
			get
			{
				if(InnerValues == null)
					throw new InvalidOperationException("InnerValues is null.");
				return this.InnerValues.IsFixedSize;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				if(InnerValues == null)
					throw new InvalidOperationException("InnerValues is null.");
				return this.InnerValues.IsSynchronized;
			}
		}

		public int Count
		{
			get
			{
				if(InnerValues == null)
					throw new InvalidOperationException("InnerValues is null.");
				return this.InnerValues.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			if(InnerValues == null)
				throw new InvalidOperationException("InnerValues is null.");
			this.InnerValues.CopyTo(array, index);
		}

		public object SyncRoot
		{
			get
			{
				if(InnerValues == null)
					throw new InvalidOperationException("InnerValues is null.");
				return this.InnerValues.SyncRoot;
			}
		}

		bool IDictionary.Contains(object key)
		{
			this.AssertKey(key);
			return this.Contains((string)key);
		}

		public void Add(string name, object value)
		{
			if(InnerValues == null)
				throw new InvalidOperationException("InnerValues is null.");
			this.InnerValues.Add(name, value);
		}

		object IDictionary.this[object key]
		{
			get
			{
				this.AssertKey(key);
				return this[(string)key];
			}
			set
			{
				this.AssertKey(key);
				this[(string)key] = value;
			}
		}

		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue(SerializationInfoPrefix + "_isInitialized", _isInitialized);
			info.AddValue(SerializationInfoPrefix + "_caseInsensitive", _caseInsensitive);

			// mbr - 20-10-2005 - for inner values, we create arrays of the keys and values and serialize those.  the Hashtable appears to 
			// be slightly evil when it comes to loading its values back after deserialization, and this appears to be the safeest way of making
			// the deserialized values available in the constructor.
			string[] names = new string[this.Count];
			object[] values = new object[this.Count];
			int index = 0;
			foreach(DictionaryEntry entry in this.InnerValues)
			{
				names[index] = (string)entry.Key;
				values[index] = entry.Value;
				index++;
			}

			// set...
			info.AddValue(SerializationInfoPrefix + "names", names);
			info.AddValue(SerializationInfoPrefix + "values", values);
		}

		object ICloneable.Clone()
		{
			return this.Clone();
		}

		public PropertyBag Clone()
		{
			// create a new one...
			PropertyBag newBag = (PropertyBag)Activator.CreateInstance(this.GetType());
			newBag.ReplaceValues(this.InnerValues);

			// return...
			return newBag;
		}
		
		/// <summary>
		/// Gets the names of the items in the bag.
		/// </summary>
		/// <returns></returns>
		public string[] GetNames()
		{
			string[] results = new string[this.Count];
			int index = 0;
			foreach(DictionaryEntry entry in this.InnerValues)
			{
				results[index] = (string)entry.Key;
				index++;
			}

			// return...
			return results;
		}
	}
}
