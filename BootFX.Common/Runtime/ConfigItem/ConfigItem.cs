// BootFX - Application framework for .NET applications
// 
// File: ConfigItem.cs
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
	using BootFX.Common.Data.Schema;
	using BootFX.Common.Entities;
	using BootFX.Common.Entities.Attributes;
    
    
	/// <summary>
	/// Defines the entity type for 'BfxConfig'.
	/// </summary>
    // mbr - 2010-01-21 - added a optional database name...
	[Serializable()]
	[Entity(typeof(ConfigItemCollection), ConfigItem.NativeTableName, true)]
	[SortSpecification("name", SortDirection.Ascending)]
	public class ConfigItem : ConfigItemBase
	{
		internal const string NativeTableName = "BfxConfiguration";

		/// <summary>
		/// Constructor.
		/// </summary>
		public ConfigItem()
		{
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		protected ConfigItem(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
			base(info, context)
		{
		}

		public static void EnsureConfigTableExists()
		{		
			SchemaHelper.EnsureTableExists(typeof(ConfigItem));
		}

        public static bool DoesTableExist()
        {
            // mbr - 2010-03-30 - changed this to use a specific database...
            EntityType et = EntityType.GetEntityType(typeof(ConfigItem), OnNotFound.ThrowException);
            if (et == null)
                throw new InvalidOperationException("'et' is null.");

            using(var conn = Database.CreateConnection(et))
                return conn.DoesTableExist(NativeTableName);
        }

		/// <summary>
		/// Gets the value from the control.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object GetValue(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get...
			string data = this.Data;
			return ConversionHelper.ChangeType(data, type, Cultures.System, ConversionFlags.Safe);
		}

		public static ConfigItem GetByCompanyProductAndName(string company, string product, string name)
		{
			if(company == null)
				throw new ArgumentNullException("company");
			if(company.Length == 0)
				throw new ArgumentOutOfRangeException("'company' is zero-length.");
			if(product == null)
				throw new ArgumentNullException("product");
			if(product.Length == 0)
				throw new ArgumentOutOfRangeException("'product' is zero-length.");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// create...
			SqlFilter filter = CreateFilter();
			filter.Constraints.Add("companyname", company);
			filter.Constraints.Add("productname", product);
			filter.Constraints.Add("name", name);

			// return...
			return (ConfigItem)filter.ExecuteEntity();
		}
	}
}
