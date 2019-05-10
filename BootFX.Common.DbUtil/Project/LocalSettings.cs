// BootFX - Application framework for .NET applications
// 
// File: LocalSettings.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using BootFX.Common.Data;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines a class for holding local project settings.
	/// </summary>
	[Serializable()]
	public class LocalSettings : SimpleXmlPropertyBag
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public LocalSettings()
		{
		}

		/// <summary>
		/// Gets the connection settings.
		/// </summary>
		internal ConnectionSettings ConnectionSettings
		{
			get
			{
				if(this.ConnectionType != null && this.ConnectionString != null && this.ConnectionString.Length > 0)
					return new ConnectionSettings(ConnectionSettings.DefaultName, this.ConnectionType, this.ConnectionString);
				else
					return null;
			}
		}

		internal string ConnectionString
		{
			get
			{
				return this.GetStringValue("ConnectionString", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this["ConnectionString"] = value;
			}
		}

		internal Type ConnectionType
		{
			get
			{
				return this.GetTypeValue("ConnectionType", typeof(SqlServerConnection), Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this["ConnectionType"] = value;
			}
		}

		internal string ProceduresFolderPath
		{
			get
			{
				if(this.IsEntitiesFolderPathSet)
					return Path.Combine(Path.GetDirectoryName(this.EntitiesFolderPath), "Procedures");
				else
					return null;
			}
		}

		internal string EntitiesFolderPath
		{
			get
			{
				return this.GetStringValue("EntitiesFolderPath", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this["EntitiesFolderPath"] = value;
			}
		}

		internal string DtoFolderPath
		{
			get
			{
				return this.GetStringValue("DtoFolderPath", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
                this["DtoFolderPath"] = value;
			}
		}

		// mbr - 26-07-2007 - case 343 - changed this.		
		internal bool AreFolderPathsSet()
		{
			return this.IsEntitiesFolderPathSet && this.IsDtoFolderPathSet;
		}

		internal bool IsDtoFolderPathSet
		{
			get
			{
				if(this.DtoFolderPath != null && this.DtoFolderPath.Length > 0)
					return true;
				else
					return false;
			}
		}

		internal bool IsEntitiesFolderPathSet
		{
			get
			{
				if(this.EntitiesFolderPath != null && this.EntitiesFolderPath.Length > 0)
					return true;
				else
					return false;
			}
		}

		internal static LocalSettings Load2(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// load it...
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				IFormatter formatter = new BinaryFormatter();
				return (LocalSettings)formatter.Deserialize(stream);
			}
		}

		internal void Save2(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// save...
			using(FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
			{
				IFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, this);
			}		
		}
	}
}
