// BootFX - Application framework for .NET applications
// 
// File: ConnectionSettings.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using BootFX.Common.Data.Formatters;
using System.Security;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines connection settings.
	/// </summary>
	[Serializable()]
	public class ConnectionSettings : ISerializable
	{
		private const string Key = "sdfrst9435p2o345hdfjhfgkljhasorwti";
		private const string IV = "#$*&#LKJHFD&*^SDFH#$LHLKSDF";

		/// <summary>
		/// Defines the key used for saved settings.
		/// </summary>
		private const string SavedSettingsPropertyBagKey = @"Core\SavedSettings";

		/// <summary>
		/// Private field to support <c>SavedSettings</c> property.
		/// </summary>
		private static ConnectionSettingsCollection _savedSettings = null;
		
		/// <summary>
		/// Defines the default settings name.
		/// </summary>
		public const string DefaultName = "Default Settings";

		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Private field to support <see cref="ConnectionType"/> property.
		/// </summary>
		private Type _connectionType;

		/// <summary>
		/// Private field to support <see cref="ConnectionString"/> property.
		/// </summary>
		private string _connectionString;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public ConnectionSettings(string name, Type connectionType, string connectionString)
		{
			if(connectionType == null)
				throw new ArgumentNullException("connectionType");
			if(typeof(Connection).IsAssignableFrom(connectionType) == false && 
				typeof(IConnection).IsAssignableFrom(connectionType) == false)
				throw new InvalidOperationException(string.Format("The connection type '{0}' does not inherit from 'Connection' or 'IConnection'.", connectionType));
			if(connectionString == null)
				throw new ArgumentNullException("connectionString");
			if(connectionString.Length == 0)
				throw new ArgumentOutOfRangeException("'connectionString' is zero-length.");

			// set...
			_name = name;
			_connectionType = connectionType;
			_connectionString = connectionString;
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ConnectionSettings(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// get the basics...
			_name = info.GetString("_name");

			// get the type name...
			string connectionTypeName = info.GetString("_connectionTypeName");
			if(connectionTypeName == null)
				throw new InvalidOperationException("'connectionTypeName' is null.");
			if(connectionTypeName.Length == 0)
				throw new InvalidOperationException("'connectionTypeName' is zero-length.");

			// find it...
			_connectionType = Type.GetType(connectionTypeName, true, true);
			if(_connectionType == null)
				throw new InvalidOperationException("_connectionType is null.");

			// get the string...
			string encryptedConnectionString = info.GetString("_connectionString");
			if(encryptedConnectionString == null)
				throw new InvalidOperationException("'encryptedConnectionString' is null.");
			if(encryptedConnectionString.Length == 0)
				throw new InvalidOperationException("'encryptedConnectionString' is zero-length.");
			_connectionString = DecryptConnectionString(encryptedConnectionString);
		}

		/// <summary>
		/// Gets the connectionstring.
		/// </summary>
		public string ConnectionString
		{
			get
			{
				return _connectionString;
			}
		}
		
		/// <summary>
		/// Gets the connectiontype.
		/// </summary>
		public Type ConnectionType
		{
			get
			{
				return _connectionType;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets a collection of ConnectionSettings objects.
		/// </summary>
		public static ConnectionSettingsCollection SavedSettings
		{
			get
			{
				if(_savedSettings == null)
				{
					// get...
					_savedSettings = (ConnectionSettingsCollection)Runtime.Current.UserSettings.GetValue(SavedSettingsPropertyBagKey, 
						typeof(ConnectionSettingsCollection), null, Cultures.System, OnNotFound.ReturnNull);
					if(_savedSettings == null)
						_savedSettings = new ConnectionSettingsCollection();
				}
				return _savedSettings;
			}
		}

		/// <summary>
		/// Saves the settings.
		/// </summary>
		public static void SaveSettings()
		{
			// save it...
			Runtime.Current.UserSettings[SavedSettingsPropertyBagKey] = _savedSettings;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(this.Name);
			builder.Append(" (");
			if(this.ConnectionType != null)
			{
				// get...
				if(this.ConnectionType.IsDefined(typeof(DescriptionAttribute), false))
				{
					DescriptionAttribute[] attrs = (DescriptionAttribute[])this.ConnectionType.GetCustomAttributes(typeof(DescriptionAttribute), false);
					if(attrs.Length > 0)
						builder.Append(attrs[0].Description);
					else
						builder.Append("??");
				}
				else
					builder.Append(this.ConnectionType.FullName);
			}
			else
				builder.Append("???");
			builder.Append(")");

			// return...
			return builder.ToString();
		}

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		/// <summary>
		/// Gets object data.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");

			// set...
			info.AddValue("_name", _name);
			if(_connectionType == null)
				throw new InvalidOperationException("_connectionType is null.");
			info.AddValue("_connectionTypeName", string.Format("{0}, {1}", _connectionType.FullName, _connectionType.Assembly.GetName().Name));
			info.AddValue("_connectionString", this.GetEncryptedConnectionString());
		}

		/// <summary>
		/// Decrypts the given connection string.
		/// </summary>
		private static string DecryptConnectionString(string connectionString)
		{
			if(connectionString == null)
				throw new ArgumentNullException("connectionString");
			if(connectionString.Length == 0)
				throw new ArgumentOutOfRangeException("'connectionString' is zero-length.");
			
			// create...
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			ICryptoTransform transform = des.CreateDecryptor(md5.ComputeHash(Encoding.Unicode.GetBytes(Key)), md5.ComputeHash(Encoding.Unicode.GetBytes(IV)));
			if(transform == null)
				throw new InvalidOperationException("transform is null.");

			// bytes...
			byte[] bs = Convert.FromBase64String(connectionString);
			using(MemoryStream stream = new MemoryStream(bs))
			{
				// create a buffer...
				byte[] encrypted = new byte[32];
				byte[] buf = new byte[encrypted.Length];
				int offset = 0;

				// create a stream...
				using(CryptoStream crypto = new CryptoStream(stream, transform, CryptoStreamMode.Read))
				{
					while(true)
					{
						// read...
						int read = crypto.Read(buf, 0, buf.Length);
						if(read == 0)
							break;

						// add...
						if(offset + read > encrypted.Length)
						{
							// flip...
							byte[] newEncrypted = new byte[encrypted.Length * 2];
							Array.Copy(encrypted, 0, newEncrypted, 0, offset);
							encrypted = newEncrypted;
						}

						// add...
						Array.Copy(buf, 0, encrypted, offset, read);
						offset += read;
					}
				}

				// return...
				byte[] toConvert = new byte[offset];
				Array.Copy(encrypted, 0, toConvert, 0, offset);
				string result = Encoding.Unicode.GetString(toConvert);
				return result;
			}
		}

		/// <summary>
		/// Gets the encrypted connection string.
		/// </summary>
		/// <returns></returns>
		private string GetEncryptedConnectionString()
		{
			// create...
			TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			ICryptoTransform transform = des.CreateEncryptor(md5.ComputeHash(Encoding.Unicode.GetBytes(Key)), md5.ComputeHash(Encoding.Unicode.GetBytes(IV)));
			if(transform == null)
				throw new InvalidOperationException("transform is null.");
			
			// convert...
			byte[] bs = Encoding.Unicode.GetBytes(this.ConnectionString);
			using(MemoryStream stream = new MemoryStream())
			{
				using(CryptoStream crypto = new CryptoStream(stream, transform, CryptoStreamMode.Write))
				{
					crypto.Write(bs, 0, bs.Length);
					crypto.FlushFinalBlock();

				    // get...
				    byte[] encrypted = new byte[stream.Length];
				    Array.Copy(stream.GetBuffer(), 0, encrypted, 0, stream.Length);
				
				    // return...
				    return Convert.ToBase64String(encrypted);
                }
            }
        }

		internal SqlDialect GetDefaultDialect()
		{
			if(ConnectionType == null)
				throw new InvalidOperationException("ConnectionType is null.");
			return Database.GetDefaultDialectForConnectionType(this.ConnectionType);
		}

		/// <summary>
		/// Gets the friendly name.
		/// </summary>
		public string FriendlyName
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				if(typeof(SqlServerConnection).IsAssignableFrom(this.ConnectionType))
				{
					// get...
					IDictionary parts = Database.GetConnectionStringParts(this.ConnectionString);
					if(parts == null)
						throw new InvalidOperationException("parts is null.");

					// set...
					builder.Append(parts["initial catalog"]);
					builder.Append(" on ");
					builder.Append(parts["data source"]);

					// type...
					builder.Append(" (SQL Server)");
				}
				else
				{
					builder.Append(this.ConnectionString);
					builder.Append(" (");
					builder.Append(this.ConnectionType);
					builder.Append(")");
				}

				// return...
				return builder.ToString();
			}
		}
	}
}
