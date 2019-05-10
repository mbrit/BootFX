// BootFX - Application framework for .NET applications
// 
// File: LookupValue.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
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
    /// Defines the entity type for 'LookupValue'.
    /// </summary>
    /// <remarks>
    /// This entity maps to the 'LookupValuesBFX' table.  It has a minimum size of 263 byte(s) and 0 large column(s).
    /// </remarks>
    [Serializable()]
    [Entity(typeof(LookupValueCollection), "BfxLookups", true)]
    public class LookupValue : LookupValueBase
    {
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public LookupValue()
        {
        }
        
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected LookupValue(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
                base(info, context)
        {
        }

		/// <summary>
		/// Returns true if a lookup contains the given value.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="description"></param>
		/// <returns></returns>
		public static bool Contains(string name, string description)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(description == null)
				throw new ArgumentNullException("description");
			if(description.Length == 0)
				throw new ArgumentOutOfRangeException("'description' is zero-length.");

			// get...
			int result = ConversionHelper.ToInt32(Database.ExecuteScalar(new SqlStatement("select lookupid from bfxlookups where name=@p0 and description=@p1",
				new object[] { name, description })), Cultures.System);
			if(result == 0)
				return false;
			else
				return true;
		}

    	public static LookupValue CreateAndSave(string name, string description)
    	{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(description == null)
				throw new ArgumentNullException("description");
			if(description.Length == 0)
				throw new ArgumentOutOfRangeException("'description' is zero-length.");
			
			// has it already been used...
			if(Contains(name, description))
				throw new InvalidOperationException(string.Format("Lookup '{0}' already contains '{1}'.", name, description));

			// get...
			LookupValue item = new LookupValue();
			item.Name = name;
			item.Description = description;
			item.Value = GetNextValueForLookup(name);

			// save...
			item.SaveChanges();

			// return...
			return item;
    	}

		/// <summary>
		/// Gets the next available value for a lookup.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static int GetNextValueForLookup(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get the values...
			object[] rawValues = Database.ExecuteValuesVertical(new SqlStatement("select value from bfxlookups where name=@p0", 
				new object[] { name }));

			// create...
			ArrayList values = new ArrayList();
			foreach(object rawValue in rawValues)
				values.Add((int)rawValue);

			// sort...
			values.Sort();

			// find the next not used value...
			long value = 1;
			while(true)
			{
				// check...
				if(!(values.Contains((int)value)))
					break;

				// next...
				value = value * 2;	
				if(value > int.MaxValue)
					throw new InvalidOperationException(string.Format("No more space available in lookup '{0}'.", name));
			}

			// return...
			return (int)value;
		}
    }
}
