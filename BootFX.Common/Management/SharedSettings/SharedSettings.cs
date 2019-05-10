// BootFX - Application framework for .NET applications
// 
// File: SharedSettings.cs
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
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Data;
using BootFX.Common.Management;
using System.Runtime.CompilerServices;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>SharedSettings</c>.
	/// </summary>
	public class SharedSettings : PropertyBagByNameBase
	{
//		/// <summary>
//		/// Private field to support <c>CheckFlushSuspendCount</c> property.
//		/// </summary>
//		private int _checkFlushSuspendCount;
//		
//		/// <summary>
//		/// Defines the filename for installation settings.
//		/// </summary>
//		// mbr - 03-11-2005 - this is a .dat file because ultimately it will be encrypted.		
//		private const string SharedSettingsCacheFileName = "SharedSettings.dat";
//
//		/// <summary>
//		/// Private field to support <c>LastFlushedVersion</c> property.
//		/// </summary>
//		private long _lastFlushedVersion = -1;
		
		private string MasterVersionNumberKey = "!Version";

		/// <summary>
		/// Private field to support <c>EnsureCalled</c> property.
		/// </summary>
		private bool _ensureCalled;
		
		/// <summary>
		/// Private field to support <see cref="CompanyName"/> property.
		/// </summary>
		private string _companyName;

		/// <summary>
		/// Private field to support <see cref="ProductName"/> property.
		/// </summary>
		private string _productName;

		/// <summary>
		/// Private field to support <see cref="Caches"/> property.
		/// </summary>
		private Lookup _caches;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SharedSettings()
		{
			// mbr - 13-06-2008 - because in recent applications one service can talk to multiple default databases (through
			// defaultdatabaseprovider), I've added an extra level of indirection that caches the connection strings.

			// create...
//			_items = new Lookup();
//			_items.CreateItemValue += new CreateLookupItemEventHandler(_items_CreateItemValue);

			_caches = new Lookup();
			_caches.CreateItemValue += new CreateLookupItemEventHandler(_caches_CreateItemValue);
		}

		/// <summary>
		/// Gets the caches.
		/// </summary>
		private Lookup Caches
		{
			get
			{
				return _caches;
			}
		}

		/// <summary>
		/// Gets the productname.
		/// </summary>
		private string ProductName
		{
			get
			{
				CheckNamesLoaded();
				return _productName;
			}
		}
		
		/// <summary>
		/// Gets the companyname.
		/// </summary>
		private string CompanyName
		{
			get
			{
				CheckNamesLoaded();
				return _companyName;
			}
		}

		private void CheckNamesLoaded()
		{
			if(Runtime.Current.Application == null)
				throw new InvalidOperationException("Runtime.Current.Application is null.");

			// check...
			if(this._productName == null || this._productName.Length == 0)
			{
				_productName = Runtime.Current.Application.ProductName;
				if(_productName == null)
					throw new InvalidOperationException("'_productName' is null.");
				if(_productName.Length == 0)
					throw new InvalidOperationException("'_productName' is zero-length.");
			}

			// check...
			if(this._companyName == null || this._companyName.Length == 0)
			{
				_companyName = Runtime.Current.Application.ProductCompany;
				if(_companyName == null)
					throw new InvalidOperationException("'_companyName' is null.");
				if(_companyName.Length == 0)
					throw new InvalidOperationException("'_companyName' is zero-length.");
			}
		}

		public override void SetValue(string name, object value)
		{
			this.SetValueInternal(name, value, true);
		}

		private void SetValueInternal(string name, object value, bool incrementMasterVersionNumber)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// do we already have that as a config item?
			ConfigItem item = (ConfigItem)this.Items[name];
			if(item == null)
			{
				// mbr - 02-04-2006 - create a new one.				
				item = new ConfigItem();
				item.CompanyName = this.CompanyName;
				item.ProductName = this.ProductName;
				item.Name = name;
				item.Version = 0;

				// set...
				this.Items[name] = item;
			}

			// set...
			item.Data = ConversionHelper.ToString(value, Cultures.System);
			item.Version++;

			// save...
			item.SaveChanges();

			// get the master item...
			if(incrementMasterVersionNumber)
			{
				long existingVersion = MasterVersionNumber;
				this.SetValueInternal(MasterVersionNumberKey, existingVersion + 1, false);
			}
		}

		public override object GetValue(string name, Type type, object defaultValue, IFormatProvider provider, OnNotFound onNotFound)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

//			// mbr - 07-03-2006 - do we need to flush?
//			CheckFlush();
			
			// get...
			ConfigItem item = (ConfigItem)this.Items[name];
			object useValue = null;
			if(item != null)
				useValue = item.Data;
			else
				useValue = defaultValue;

			// convert...?
			if(type != null)
				return ConversionHelper.ChangeType(useValue, type, Cultures.System);
			else
				return useValue;
		}

		/// <summary>
		/// Gets the items.
		/// </summary>
		private Lookup Items
		{
			get
			{
				// mbr - 13-06-2008 - changed to have a level of indirection through the connection string...
				string conn = Database.DefaultConnectionString;
				if(conn == null)
					throw new InvalidOperationException("'conn' is null.");
				if(conn.Length == 0)
					throw new InvalidOperationException("'conn' is zero-length.");

                // mbr - 2014-05-28 - added...
                if (Database.SystemDatabaseProvider != null)
                {
                    conn = Database.SystemDatabaseProvider.GetConnectionSettings().ConnectionString;
                    if (conn == null)
                        throw new InvalidOperationException("'conn' is null.");
                    if (conn.Length == 0)
                        throw new InvalidOperationException("'conn' is zero-length.");
                }

				// get...
				Lookup items = (Lookup)this.Caches[conn];
				if(items == null)
					throw new InvalidOperationException("items is null.");

				// return...
				return items;
			}
		}

		/// <summary>
		/// Gets the ensurecalled.
		/// </summary>
		private bool EnsureCalled
		{
			get
			{
				// returns the value...
				return _ensureCalled;
			}
		}

		private void PerDatabaseCreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			string name = (string)e.Key;

			// do we have it?
			if(!(_ensureCalled))
			{
				try
				{
					ConfigItem.EnsureConfigTableExists();
				}
				finally
				{
					_ensureCalled = true;
				}
			}

			// mbr - 10-03-2006 - check...
			if(CompanyName == null)
				throw new InvalidOperationException("'CompanyName' is null.");
			if(CompanyName.Length == 0)
				throw new InvalidOperationException("'CompanyName' is zero-length.");
			if(ProductName == null)
				throw new InvalidOperationException("'ProductName' is null.");
			if(ProductName.Length == 0)
				throw new InvalidOperationException("'ProductName' is zero-length.");

			// get...
			ConfigItem item = ConfigItem.GetByCompanyProductAndName(this.CompanyName, this.ProductName, name);

			// mbr - 02-04-2006 - don't create the item - just won't work if we do.			
//			if(item == null)
//			{
//				item = new ConfigItem();
//				item.CompanyName = this.CompanyName;
//				item.ProductName = this.ProductName;
//				item.Name = name;
//				item.Version = 0;
//			}

			// return...
			e.NewValue = item;
		}

		/// <summary>
		/// Gets the configuration version number.
		/// </summary>
		public long MasterVersionNumber
		{
			get
			{
				return this.GetInt64Value(MasterVersionNumberKey, 0, Cultures.System, OnNotFound.ReturnNull);
			}
		}

		/// <summary>
		/// Creates a data table of the latest settings.
		/// </summary>
		/// <returns></returns>
		public DataTable ToDataTable()
		{
			SqlFilter filter = ConfigItem.CreateFilter();
			filter.Fields.Clear();
			filter.Constraints.Add("name", SqlOperator.NotStartsWith, "!");

			// return...
			return filter.ExecuteDataTable();
		}

		/// <summary>
		/// Gets default shared settings based on classes loaded into the current appdomain.
		/// </summary>
		/// <returns></returns>
		internal static IDictionary GetDefaultSharedSettings()
		{
			// find...
			TypeFinder finder = new TypeFinder(typeof(object));
			finder.AddAttributeSpecification(typeof(SharedSettingsContainerAttribute), false);

			// get them...
			Type[] types = finder.GetTypes();
			if(types == null)
				throw new InvalidOperationException("types is null.");

			// walk each one and get the props...
			IDictionary results = CollectionsUtil.CreateCaseInsensitiveHashtable();
			foreach(Type type in types)
			{
				// get...
				PropertyInfo[] props = type.GetProperties();
				if(props == null)
					throw new InvalidOperationException("props is null.");

				// walk...
				foreach(PropertyInfo prop in props)
				{
					SharedSettingsItemAttribute[] attrs = (SharedSettingsItemAttribute[])prop.GetCustomAttributes(typeof(SharedSettingsItemAttribute), false);
					if(attrs == null)
						throw new InvalidOperationException("attrs is null.");

					// walk...
					foreach(SharedSettingsItemAttribute attr in attrs)
					{
						// add...
						results[prop.Name] = attr.DefaultValue;
					}
				}
			}

			// return...
			return results;
		}

//		/// <summary>
//		/// Gets the lastflushedversion.
//		/// </summary>
//		private long LastFlushedVersion
//		{
//			get
//			{
//				// returns the value...
//				return _lastFlushedVersion;
//			}
//		}
//
//		/// <summary>
//		/// Gets or sets the checkflushsuspendcount
//		/// </summary>
//		private int CheckFlushSuspendCount
//		{
//			get
//			{
//				return _checkFlushSuspendCount;
//			}
//			set
//			{
//				// check to see if the value has changed...
//				if(value != _checkFlushSuspendCount)
//				{
//					// set the value...
//					_checkFlushSuspendCount = value;
//				}
//			}
//		}
//
//		/// <summary>
//		/// Checks whether we need to flush configuration to the local disk.
//		/// </summary>
//		private void CheckFlush()
//		{
//			if(this.CheckFlushSuspendCount > 0)
//				return;
//
//			// update...
//			this.CheckFlushSuspendCount++;
//			try
//			{
//				// mbr - 07-03-2006 - not if we're not online.			
//				if(!(Runtime.Current.IsOnline))
//					return;
//
//				// not if we're up-to-date...
//				if(this.LastFlushedVersion >= this.MasterVersionNumber)
//					return;
//
//				try
//				{
//					// get them all...
//					ConfigItemCollection items = ConfigItem.GetAll();
//					if(items == null)
//						throw new InvalidOperationException("items is null.");
//
//					// dump it...
//					string path = this.SharedSettingsCacheFilePath;
//					if(path == null)
//						throw new InvalidOperationException("'path' is null.");
//					if(path.Length == 0)
//						throw new InvalidOperationException("'path' is zero-length.");
//
//					// output...
//					XmlDocument doc = items.ToXmlDocument();
//					if(doc == null)
//						throw new InvalidOperationException("doc is null.");
//
//					// save...
//					doc.Save(path);
//				}
//				catch(Exception ex)
//				{
//					this.ReportCacheError("Failed to flush cache to disk.", ex);
//				}
//				finally
//				{
//					// set...
//					_lastFlushedVersion = this.MasterVersionNumber;
//				}
//			}
//			finally
//			{
//				this.CheckFlushSuspendCount--;
//			}
//		}

		private void ReportCacheError(string message, Exception ex)
		{
			if(ex == null)
				throw new ArgumentNullException("ex");
			
			// log...
			if(this.Log.IsWarnEnabled)
				this.Log.Warn(message, ex);
		}

//		private string SharedSettingsCacheFilePath
//		{
//			get
//			{
//				// get...
//				return Path.Combine(Runtime.Current.AllUsersApplicationDataFolderPath, SharedSettingsCacheFileName);
//			}
		//		}

		private void _caches_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// create a new lookup...
			Lookup items = new Lookup();
			items.CreateItemValue += new CreateLookupItemEventHandler(PerDatabaseCreateItemValue);

			// set...
			e.NewValue = items;
		}

        public T GetValue<T>(T defaultValue, [CallerMemberName] string name = null)
        {
            return this.GetValue<T>(defaultValue, OnNotFound.ThrowException, name);
        }

        public T GetValue<T>(T defaultValue, OnNotFound onNotFound, [CallerMemberName] string name = null)
        {
            return (T)this.GetValue(name, typeof(T), defaultValue, Cultures.System, onNotFound);
        }
	}
}
