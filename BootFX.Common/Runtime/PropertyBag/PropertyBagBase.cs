// BootFX - Application framework for .NET applications
// 
// File: PropertyBagBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Management;
using System.Security;

namespace BootFX.Common
{
	/// <summary>
	/// Defines a class that holds objects via a string key and provides rich methods to access values.
	/// </summary>
	[Serializable()]
	public abstract class PropertyBagByNameBase : Loggable, ISerializable
	{
		protected PropertyBagByNameBase()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		protected PropertyBagByNameBase(SerializationInfo info, StreamingContext context)
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
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public Type GetTypeValue(string name, Type defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (Type)this.GetValue(name, typeof(Type), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets a date/time from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public DateTime GetDateTimeValue(string name, DateTime defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (DateTime)this.GetValue(name, typeof(DateTime), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public short GetInt16Value(string name, short defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (short)this.GetValue(name, typeof(short), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public long GetInt64Value(string name, long defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (long)this.GetValue(name, typeof(long), defaultValue, provider, onNotFound);
		}
		
		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public decimal GetDecimalValue(string name, decimal defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (decimal)this.GetValue(name, typeof(decimal), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		// mbr - 07-11-2005 - added..		
        public Guid GetGuidValue(string name, Guid defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// mbr - 04-10-2006 - wasn't handling null.
			object asObject = this.GetValue(name, typeof(Guid), defaultValue, provider, onNotFound);
			if(asObject != null)
				return (Guid)asObject;
			else
				return defaultValue;		
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public bool GetBooleanValue(string name, bool defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (bool)this.GetValue(name, typeof(bool), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets an integer from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public int GetInt32Value(string name, int defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (int)this.GetValue(name, typeof(int), defaultValue, provider, onNotFound);
		}

		/// <summary>
		/// Gets a string from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultVaule"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
        public string GetStringValue(string name, object defaultValue, IFormatProvider provider = null, OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			return (string)this.GetValue(name, typeof(string), defaultValue, provider, onNotFound);
		}

        public T GetValue<T>(string name, bool throwIfNotfound = true)
        {
            return (T)this.GetValue(name, typeof(T), default(T), Cultures.System, throwIfNotfound ? OnNotFound.ThrowException : OnNotFound.ReturnNull);
        }
		
		/// <summary>
		/// Gets an enumeration value from the settings.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultValue"></param>
		/// <param name="enumerationType"></param>
		/// <param name="provider"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public object GetEnumerationValue(string name, object defaultValue, Type enumerationType, IFormatProvider provider = null, 
            OnNotFound onNotFound = OnNotFound.ReturnNull)
		{
			if(enumerationType == null)
				throw new ArgumentNullException("enumerationType");
			if(!(enumerationType.IsEnum))
				throw new InvalidOperationException(string.Format("Type '{0}' is not an enumeration type.", enumerationType));

			// get...
			string defaultAsString = null;
			if(defaultValue != null)
				defaultAsString = defaultValue.ToString();
			string asString = this.GetStringValue(name, defaultAsString, provider, onNotFound);
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

        public T GetEnumerationValue<T>(string name, T defaultValue = default(T), bool throwItNotFound = false, IFormatProvider provider = null)
        {
            return (T)GetEnumerationValue(name, defaultValue, typeof(T), provider, throwItNotFound ? OnNotFound.ThrowException : OnNotFound.ReturnNull);
        }

        public abstract void SetValue(string name, object value);
        public abstract object GetValue(string name, Type type, object defaultValue, IFormatProvider provider = null, 
            OnNotFound onNotFound = OnNotFound.ReturnNull);

        public T GetValue<T>(string name, T defaultValue = default(T), IFormatProvider provider = null,
            OnNotFound onNotFound = OnNotFound.ReturnNull)
        {
            return (T)GetValue(name, typeof(T), defaultValue, provider, onNotFound);
        }

        public T GetValue<T>(string name, T defaultValue = default(T), bool throwIfNotFound = false)
        {
            return (T)GetValue(name, typeof(T), defaultValue, null, throwIfNotFound ? OnNotFound.ThrowException : OnNotFound.ReturnNull);
        }

        /// <summary>
        /// Gets the default encryption container name.
        /// </summary>
        // mbr - 2009-05-01 - added.
        private string DefaultEncryptionContainerName
        {
            get
            {
                // mbr - 2009-08-07 - added domain/username...
                //return string.Format("{0}_{1}", Runtime.Current.Application.ProductCompany, Runtime.Current.Application.ProductName);
                return "__bfxContainer_" + HashHelper.GetMd5HashOfStringAsBase64(string.Format("{0}|{1}|{2}|{3}",
                    Runtime.Current.Application.ProductCompany, Runtime.Current.Application.ProductName, Environment.UserDomainName,
                    Environment.UserName));
            }
        }

        /// <summary>
        /// Gets an encrypted string from the property bag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="provider"></param>
        /// <param name="onNotFound"></param>
        /// <returns></returns>
        // mbr - 2009-05-01 - added.
        public string GetEncryptedStringValue(string name, OnNotFound onNotFound)
        {
            return this.GetEncryptedStringValue(name, this.DefaultEncryptionContainerName, onNotFound);
        }

        /// <summary>
        /// Gets an encrypted string from the property bag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerName"></param>
        /// <param name="defaultValue"></param>
        /// <param name="provider"></param>
        /// <param name="onNotFound"></param>
        /// <returns></returns>
        // mbr - 2009-05-01 - added.
        public string GetEncryptedStringValue(string name, string containerName, OnNotFound onNotFound)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");

            // get...
            string base64 = Runtime.Current.UserSettings.GetStringValue(name, null, Cultures.System, onNotFound);
            if (string.IsNullOrEmpty(base64))
                return base64;
            
            // bytes...
            byte[] bs = Convert.FromBase64String(base64);

            // get...
            RSACryptoServiceProvider crypto = this.GetCrypto(containerName);
            if (crypto == null)
                throw new InvalidOperationException("'crypto' is null.");

            // decrypt...
            byte[] decrypted = crypto.Decrypt(bs, true);

            // return...
            string result = Encoding.Unicode.GetString(decrypted);
            return result;
        }

        /// <summary>
        /// Sets an encrypted string in the property bag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerName"></param>
        /// <param name="value"></param>
        // mbr - 2009-05-01 - added.
        public void SetEncryptedStringValue(string name, string value)
        {
            this.SetEncryptedStringValue(name, this.DefaultEncryptionContainerName, value);
        }

        /// <summary>
        /// Sets an encrypted string in the property bag.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="containerName"></param>
        /// <param name="value"></param>
        // mbr - 2009-05-01 - added.
        public void SetEncryptedStringValue(string name, string containerName, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("'name' is zero-length.");

            // get...
            RSACryptoServiceProvider crypto = this.GetCrypto(containerName);
            if (crypto == null)
                throw new InvalidOperationException("'crypto' is null.");

            // save...
            byte[] bs = Encoding.Unicode.GetBytes(value);
            byte[] encrypted = crypto.Encrypt(bs, true);

            // save...
            string base64 = Convert.ToBase64String(encrypted);
            this.SetValue(name, base64);
        }

        private RSACryptoServiceProvider GetCrypto(string containerName)
        {
            if (containerName == null)
                throw new ArgumentNullException("containerName");
            if (containerName.Length == 0)
                throw new ArgumentException("'containerName' is zero-length.");

            // get...
            CspParameters args = new CspParameters();
            args.Flags = CspProviderFlags.UseMachineKeyStore;
            args.KeyContainerName = "__bfx_" + containerName;

            // return...
            return new RSACryptoServiceProvider(args);
        }
	}
}
