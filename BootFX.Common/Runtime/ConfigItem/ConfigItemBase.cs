// BootFX - Application framework for .NET applications
// 
// File: ConfigItemBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	using System;
	using System.IO;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Data;
	using System.Collections;
	using System.Collections.Specialized;
	using BootFX.Common;
	using BootFX.Common.Data;
	using BootFX.Common.Entities;
	using BootFX.Common.Entities.Attributes;
    
    
	/// <summary>
	/// Defines the base entity type for 'BfxConfig'.
	/// </summary>
	[Serializable()]
	public abstract class ConfigItemBase : BootFX.Common.Entities.Entity
	{
        
		/// <summary>
		/// Constructor.
		/// </summary>
		protected ConfigItemBase()
		{
		}
        
		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected ConfigItemBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
			base(info, context)
		{
		}
        
		/// <summary>
		/// Gets or sets the value for 'ConfigId'.
		/// </summary>
		/// <remarks>
		/// This property maps to the 'ConfigId' column.
		/// </remarks>
		[EntityField("ConfigId", System.Data.DbType.Int32, ((BootFX.Common.Entities.EntityFieldFlags.Key | BootFX.Common.Entities.EntityFieldFlags.Common) 
			 | BootFX.Common.Entities.EntityFieldFlags.AutoIncrement))]
		public int ConfigId
		{
			get
			{
				return ((int)(this["ConfigId"]));
			}
			set
			{
				this["ConfigId"] = value;
			}
		}
        
		/// <summary>
		/// Gets or sets the value for 'CompanyName'.
		/// </summary>
		/// <remarks>
		/// This property maps to the 'CompanyName' column.
		/// </remarks>
		[EntityField("CompanyName", System.Data.DbType.String, BootFX.Common.Entities.EntityFieldFlags.Common, 96)]
		public string CompanyName
		{
			get
			{
				return ((string)(this["CompanyName"]));
			}
			set
			{
				this["CompanyName"] = value;
			}
		}
		        
		/// <summary>
		/// Gets or sets the value for 'ProductName'.
		/// </summary>
		/// <remarks>
		/// This property maps to the 'ProductName' column.
		/// </remarks>
		[EntityField("ProductName", System.Data.DbType.String, BootFX.Common.Entities.EntityFieldFlags.Common, 96)]
		public string ProductName
		{
			get
			{
				return ((string)(this["ProductName"]));
			}
			set
			{
				this["ProductName"] = value;
			}
		}

		/// <summary>
		/// Gets or sets the value for 'Name'.
		/// </summary>
		/// <remarks>
		/// This property maps to the 'Name' column.
		/// </remarks>
		[EntityField("Name", System.Data.DbType.String, BootFX.Common.Entities.EntityFieldFlags.Common, 96)]
		public string Name
		{
			get
			{
				return ((string)(this["Name"]));
			}
			set
			{
				this["Name"] = value;
			}
		}
        
		/// <summary>
		/// Gets or sets the value for 'Data'.
		/// </summary>
		/// <remarks>
		/// This property maps to the 'Data' column.
		/// </remarks>
		[EntityField("Data", System.Data.DbType.String, BootFX.Common.Entities.EntityFieldFlags.Large | BootFX.Common.Entities.EntityFieldFlags.Common)]
		public string Data
		{
			get
			{
				return ((string)(this["Data"]));
			}
			set
			{
				this["Data"] = value;
			}
		}
        
		/// <summary>
		/// Gets or sets the value for 'Version'.
		/// </summary>
		/// <remarks>
		/// This property maps to the 'Version' column.
		/// </remarks>
		[EntityField("Version", System.Data.DbType.Int32, BootFX.Common.Entities.EntityFieldFlags.Common)]
		public int Version
		{
			get
			{
				return ((int)(this["Version"]));
			}
			set
			{
				this["Version"] = value;
			}
		}
        
		/// <summary>
		/// Creates an SqlFilter for an instance of 'ConfigItem'.
		/// </summary>
		public static BootFX.Common.Data.SqlFilter CreateFilter()
		{
			return new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
		}       
        
		/// <summary>
		/// Sets properties on an instance of 'ConfigItem'.
		/// </summary>
		public static void SetProperties(int configId, string[] propertyNames, object[] propertyValues)
		{
			ConfigItem entity = BootFX.Common.ConfigItem.GetById(configId);
			entity.SetProperties(entity, propertyNames, propertyValues);
			entity.SaveChanges();
		}
        
		/// <summary>
		/// Get all <see cref="ConfigItem"/> entities.
		/// </summary>
		public static ConfigItemCollection GetAll()
		{
			BootFX.Common.Data.SqlFilter filter = BootFX.Common.Data.SqlFilter.CreateGetAllFilter(typeof(ConfigItem));
			return ((ConfigItemCollection)(filter.ExecuteEntityCollection()));
		}
        
		/// <summary>
		/// Gets the <see cref="ConfigItem"/> entity with the given ID.
		/// </summary>
		/// <bootfx>
		/// CreateEntityFilterEqualToMethod - ConfigItem
		/// </bootfx>
		public static ConfigItem GetById(int configId)
		{
			return BootFX.Common.ConfigItem.GetById(configId, BootFX.Common.Data.SqlOperator.EqualTo);
		}
        
		/// <summary>
		/// Gets the <see cref="ConfigItem"/> entity where the ID matches the given specification.
		/// </summary>
		public static ConfigItem GetById(int configId, BootFX.Common.Data.SqlOperator configIdOperator)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("ConfigId", configIdOperator, configId);
			return ((ConfigItem)(filter.ExecuteEntity()));
		}
        
		/// <summary>
		/// Gets the <see cref="ConfigItem"/> entity with the given ID.
		/// </summary>
		/// <bootfx>
		/// CreateEntityFilterEqualToMethod - ConfigItem
		/// </bootfx>
		public static ConfigItem GetById(int configId, BootFX.Common.OnNotFound onNotFound)
		{
			return BootFX.Common.ConfigItem.GetById(configId, BootFX.Common.Data.SqlOperator.EqualTo, onNotFound);
		}
        
		/// <summary>
		/// Gets the <see cref="ConfigItem"/> entity where the ID matches the given specification.
		/// </summary>
		public static ConfigItem GetById(int configId, BootFX.Common.Data.SqlOperator configIdOperator, BootFX.Common.OnNotFound onNotFound)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("ConfigId", configIdOperator, configId);
			ConfigItem results = ((ConfigItem)(filter.ExecuteEntity()));
			return results;
		}
        
		/// <summary>
		/// Gets entities where Name is equal to the given value.
		/// </summary>
		/// <bootfx>
		/// CreateEntityFilterEqualToMethod - ConfigItem
		/// </bootfx>
		/// <remarks>
		/// Created for column <c>Name</c>
		/// </remarks>
		public static ConfigItem GetByName(string name)
		{
			return BootFX.Common.ConfigItem.GetByName(name, BootFX.Common.Data.SqlOperator.EqualTo);
		}
        
		/// <summary>
		/// Gets entities where Name matches the given specification.
		/// </summary>
		/// <remarks>
		/// Created for column <c>Name</c>
		/// </remarks>
		public static ConfigItem GetByName(string name, BootFX.Common.Data.SqlOperator nameOperator)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("Name", nameOperator, name);
			return ((ConfigItem)(filter.ExecuteEntity()));
		}
        
		/// <summary>
		/// Gets entities where Name matches the given specification.
		/// </summary>
		/// <remarks>
		/// Created for column <c>Name</c>
		/// </remarks>
		public static ConfigItemCollection GetByName(string name, BootFX.Common.Data.SqlOperator nameOperator, BootFX.Common.OnNotFound onNotFound)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("Name", nameOperator, name);
			ConfigItemCollection results = ((ConfigItemCollection)(filter.ExecuteEntityCollection()));
			return results;
		}
        
		/// <summary>
		/// Gets entities where Data is equal to the given value.
		/// </summary>
		/// <bootfx>
		/// CreateEntityFilterEqualToMethod - ConfigItem
		/// </bootfx>
		/// <remarks>
		/// Created for column <c>Data</c>
		/// </remarks>
		public static ConfigItemCollection GetByData(string data)
		{
			return BootFX.Common.ConfigItem.GetByData(data, BootFX.Common.Data.SqlOperator.EqualTo);
		}
        
		/// <summary>
		/// Gets entities where Data matches the given specification.
		/// </summary>
		/// <remarks>
		/// Created for column <c>Data</c>
		/// </remarks>
		public static ConfigItemCollection GetByData(string data, BootFX.Common.Data.SqlOperator dataOperator)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("Data", dataOperator, data);
			return ((ConfigItemCollection)(filter.ExecuteEntityCollection()));
		}
        
		/// <summary>
		/// Gets entities where Data matches the given specification.
		/// </summary>
		/// <remarks>
		/// Created for column <c>Data</c>
		/// </remarks>
		public static ConfigItemCollection GetByData(string data, BootFX.Common.Data.SqlOperator dataOperator, BootFX.Common.OnNotFound onNotFound)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("Data", dataOperator, data);
			ConfigItemCollection results = ((ConfigItemCollection)(filter.ExecuteEntityCollection()));
			return results;
		}
        
		/// <summary>
		/// Gets entities where Version is equal to the given value.
		/// </summary>
		/// <bootfx>
		/// CreateEntityFilterEqualToMethod - ConfigItem
		/// </bootfx>
		/// <remarks>
		/// Created for column <c>Version</c>
		/// </remarks>
		public static ConfigItemCollection GetByVersion(int version)
		{
			return BootFX.Common.ConfigItem.GetByVersion(version, BootFX.Common.Data.SqlOperator.EqualTo);
		}
        
		/// <summary>
		/// Gets entities where Version matches the given specification.
		/// </summary>
		/// <remarks>
		/// Created for column <c>Version</c>
		/// </remarks>
		public static ConfigItemCollection GetByVersion(int version, BootFX.Common.Data.SqlOperator versionOperator)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("Version", versionOperator, version);
			return ((ConfigItemCollection)(filter.ExecuteEntityCollection()));
		}
        
		/// <summary>
		/// Gets entities where Version matches the given specification.
		/// </summary>
		/// <remarks>
		/// Created for column <c>Version</c>
		/// </remarks>
		public static ConfigItemCollection GetByVersion(int version, BootFX.Common.Data.SqlOperator versionOperator, BootFX.Common.OnNotFound onNotFound)
		{
			BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(ConfigItem));
			filter.Constraints.Add("Version", versionOperator, version);
			ConfigItemCollection results = ((ConfigItemCollection)(filter.ExecuteEntityCollection()));
			return results;
		}
        
		/// <summary>
		/// Searches for <see cref="ConfigItem"/> items with the given terms.
		/// </summary>
		public static ConfigItemCollection Search(string terms)
		{
			BootFX.Common.Data.SqlSearcher searcher = new BootFX.Common.Data.SqlSearcher(typeof(ConfigItem), terms);
			return ((ConfigItemCollection)(searcher.ExecuteEntityCollection()));
		}
        
		/// <summary>
		/// Returns the value in the 'Name' property.
		/// </summary>
		public override string ToString()
		{
			return this.Name;
		}
	}
}
