// BootFX - Application framework for .NET applications
// 
// File: PropertyBagByObjectBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Data;
using BootFX.Common.Management;
using System.Security;

namespace BootFX.Common
{
	/// <summary>
	/// Defines a class that holds objects via a string key and provides rich methods to access values.
	/// </summary>
	[Serializable()]
	public abstract class PropertyBagByObjectBase : Loggable, ISerializable
	{
		public PropertyBagByObjectBase()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected PropertyBagByObjectBase(SerializationInfo info, StreamingContext context)
		{
		}

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		/// <summary>
		/// Gets a string from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultValue"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public Type GetTypeValue(object key, Type defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (Type)this.GetValue(key, typeof(Type), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets a date/time from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public DateTime GetDateTimeValue(object key, DateTime defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (DateTime)this.GetValue(key, typeof(DateTime), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public short GetInt16Value(object key, short defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (short)this.GetValue(key, typeof(short), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public long GetInt64Value(object key, long defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (long)this.GetValue(key, typeof(long), defaultValue, provider, onNotFound);
		}
		
		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public decimal GetDecimalValue(object key, decimal defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (decimal)this.GetValue(key, typeof(decimal), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		// mbr - 07-11-2005 - added..		
		public Guid GetGuidValue(object key, Guid defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// mbr - 04-10-2006 - wasn't handling null.
			object asObject = this.GetValue(key, typeof(Guid), defaultValue, provider, onNotFound);
			if(asObject != null)
				return (Guid)asObject;
			else
				return defaultValue;
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public bool GetBooleanValue(object key, bool defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (bool)this.GetValue(key, typeof(bool), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public int GetInt32Value(object key, int defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (int)this.GetValue(key, typeof(int), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets a string from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultVaule"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public string GetStringValue(object key, string defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// return...
			return (string)this.GetValue(key, typeof(string), defaultValue, provider, onNotFound);
		}
		
		/// <summary>
		/// Gets an enumeration value from the settings.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="defaultValue"></param>
		/// <param key="enumerationType"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public object GetEnumerationValue(object key, object defaultValue, Type enumerationType, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(enumerationType == null)
				throw new ArgumentNullException("enumerationType");
			if(!(enumerationType.IsEnum))
				throw new InvalidOperationException(string.Format("Type '{0}' is not an enumeration type.", enumerationType));

			// get...
			string defaultAsString = null;
			if(defaultValue != null)
				defaultAsString = defaultValue.ToString();
			string asString = this.GetStringValue(key, defaultAsString, provider, onNotFound);
			if(asString != null && asString.Length > 0)
			{
				try
				{
					return Enum.Parse(enumerationType, asString, true);
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format("Failed to convert '{0}' to '{1}'.", asString, enumerationType), ex);
				}
			}
			else
				return null;
		}

		/// <summary>
		/// Gets a string array value.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public string[] GetStringArrayValue(object key, OnNotFound onNotFound)
		{
			if(key == null)
				throw new ArgumentNullException("key");
			
			// get...
			object value = this.GetValue(key, null, null, Cultures.System, onNotFound);
			if(value == null)
				throw new InvalidOperationException("value is null.");

			// what happened?
			return (string[])ToArray(value, typeof(string));
		}

		/// <summary>
		/// Converts the value to the given type.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private Array ToArray(object value, Type elementType)
		{
			if(value == null)
				throw new ArgumentNullException("value");
			if(elementType == null)
				throw new ArgumentNullException("elementType");
			
			// is it an enumerable?
			Array result = null;
			if(value is IEnumerable)
			{
				// only handled ArrayList at the moment...
				if(value is ArrayList)
					return ((ArrayList)value).ToArray(elementType);
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", value.GetType()));
			}
			else
			{
				// change...
				result = Array.CreateInstance(elementType, 1);
				result.SetValue(ConversionHelper.ChangeType(value, elementType, Cultures.System), 0);
			}

			// return...
			return result;
		}

		/// <summary>
		/// Gets a value.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="type"></param>
		/// <param key="defaultValue"></param>
		/// <param key="provider"></param>
		/// <param key="onNotFound"></param>
		/// <returns></returns>
		public abstract object GetValue(object key, Type type, object defaultValue, IFormatProvider provider, OnNotFound onNotFound);

		/// <summary>
		/// Sets the given value.
		/// </summary>
		/// <param key="key"></param>
		/// <param key="value"></param>
		public abstract void SetValue(object key, object value);
	}
}
