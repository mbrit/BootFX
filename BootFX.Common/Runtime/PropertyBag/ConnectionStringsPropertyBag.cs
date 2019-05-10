// BootFX - Application framework for .NET applications
// 
// File: ConnectionStringsPropertyBag.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using BootFX.Common.Xml;
using BootFX.Common.Data;

namespace BootFX.Common
{
	/// <summary>
	/// Defines a class that can access connection strings stored in the .NET 2.0 <c>connectionStrings</c>
	/// .config section.
	/// </summary>
	public class ConnectionStringsPropertyBag : PropertyBag
	{
		public const string MainNameKey = "Main";

		/// <summary>
		/// Private field to hold the singleton instance.
		/// </summary>
		private static ConnectionStringsPropertyBag _current = null;
		
		/// <summary>
		/// Private constructor.
		/// </summary>
		private ConnectionStringsPropertyBag(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// does the file exist?  no, quit...
			if(!(File.Exists(path)))
				return;

			// try...
			try
			{
				// open...
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				// select...
				foreach(XmlElement element in doc.SelectNodes("configuration/connectionStrings/add"))
				{
					// get...
					string name = XmlHelper.GetAttributeString(element, "name", OnNotFound.ThrowException);
					string value =  XmlHelper.GetAttributeString(element, "connectionString", OnNotFound.ThrowException);

					// set...
					this[name] = value;
				}
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Parsing of '{0}' failed.", path), ex);
			}
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static ConnectionStringsPropertyBag Current
		{
			get
			{
				if(_current == null)
					throw new InvalidOperationException("The property bag has not been initialized.");
				return _current;
			}
		}

		/// <summary>
		/// Configures the database settings based on the values set in the .config.
		/// </summary>
		public void ConfigureDatabaseSettings()
		{
//			StringBuilder builder = new StringBuilder();			
			foreach(DictionaryEntry entry in this)
			{
				string name = (string)entry.Key;
//				builder.Append("-|-");
//				builder.Append(name);
//				builder.Append(": ");
//				builder.Append(entry.Value);

				// set...
				if(string.Compare(name, MainNameKey, true) == 0)
				{
					Database.SetDefaultDatabase(typeof(SqlServerConnection), (string)entry.Value);
//					builder.Append(" *** MAIN ***");
				}
				else
				{
					Database.Databases.Add(new NamedDatabase(name, 
						new ConnectionSettings(name, typeof(SqlServerConnection), (string)entry.Value)));
				}
			}

//			// throw...
//			throw new InvalidOperationException(builder.ToString());
		}

		/// <summary>
		/// Initialize.
		/// </summary>
		/// <param name="path"></param>
		internal static void Initialize(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// check...
			if(_current != null)
				throw new InvalidOperationException("The property bag has already been initialized");
			_current = new ConnectionStringsPropertyBag(path);
		}
		
		/// <summary>
		/// Clears down the item.
		/// </summary>
		// mbr - 21-09-2007 - added.
		internal static void Deinitialize()
		{
			_current = null;
		}
	}
}
