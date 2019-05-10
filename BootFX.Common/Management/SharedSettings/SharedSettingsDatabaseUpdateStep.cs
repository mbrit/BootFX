// BootFX - Application framework for .NET applications
// 
// File: SharedSettingsDatabaseUpdateStep.cs
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
using System.Data;
using System.Collections;
using System.Collections.Specialized;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that can populate BfxConfiguration when Database Update is run.
	/// </summary>
	[DatabaseUpdateStep()]
	public class SharedSettingsDatabaseUpdateStep : DatabaseUpdateStep
	{
		private const string LegacyConfigTableName = "BfxConfig";

		/// <summary>
		/// Constructor.
		/// </summary>
		public SharedSettingsDatabaseUpdateStep()
		{
		}

		public override bool IsUpToDate
		{
			get
			{
				// mbr - 10-03-2006 - do we have an old table?
				if(Database.DoesTableExist(LegacyConfigTableName))
					return false;

				// find out what settings we have...
				IDictionary settings = SharedSettings.GetDefaultSharedSettings();
				if(settings.Count == 0)
					return true;

				// check...
				IDictionary missing = this.GetMissingItems(settings);
				if(missing == null)
					throw new InvalidOperationException("missing is null.");

				// anything?
				if(missing.Count == 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets items that are missing.
		/// </summary>
		/// <param name="find"></param>
		/// <returns></returns>
		protected IDictionary GetMissingItems()
		{
			IDictionary settings = SharedSettings.GetDefaultSharedSettings();

			// anything?
			if(settings.Count > 0)
				return this.GetMissingItems(settings);
			else
				return settings;
		}

		/// <summary>
		/// Gets items that are missing.
		/// </summary>
		/// <param name="find"></param>
		/// <returns></returns>
		protected IDictionary GetMissingItems(IDictionary find)
		{
			if(find == null)
				throw new ArgumentNullException("find");
			
			// does the table exist?
			IDictionary results = CollectionsUtil.CreateCaseInsensitiveHashtable();
			if(ConfigItem.DoesTableExist())
			{
				// find it...
				foreach(DictionaryEntry entry in find)
				{
					string name = (string)entry.Key;
					
					// existing?
					ConfigItem item = ConfigItem.GetByName(name);
					if(item == null)
						results[name] = entry.Value;
				}
			}
			else
			{
				// copy it...
				foreach(DictionaryEntry entry in find)
					results[entry.Key] = entry.Value;
			}

			// return...
			return results;
		}

		/// <summary>
		/// Executes the update.
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(DatabaseUpdateContext context)
		{		
			// make sure it exists...
			ConfigItem.EnsureConfigTableExists();

			// create...
			IDictionary missing = this.GetMissingItems();
			if(missing == null)
				throw new InvalidOperationException("missing is null.");

			// run...
			foreach(DictionaryEntry entry in missing)
				Runtime.Current.SharedSettings.SetValue((string)entry.Key, entry.Value);
		}

		public override string ToString()
		{
			return string.Format("Update Shared Settings");
		}
	}
}
