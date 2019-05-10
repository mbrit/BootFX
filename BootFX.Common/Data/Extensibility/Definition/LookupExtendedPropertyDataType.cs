// BootFX - Application framework for .NET applications
// 
// File: LookupExtendedPropertyDataType.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>LookupExtendedPropertyType</c>.
	/// </summary>
	// mbr - 25-09-2007 - renamed.	
//	public class LookupExtendedPropertyType : ExtendedPropertyType
	public class LookupExtendedPropertyDataType : ExtendedPropertyDataType
	{
		/// <summary>
		/// Private field to support <see cref="Lookup"/> property.
		/// </summary>
		private string _lookupName;
		
		public LookupExtendedPropertyDataType(ExtendedLookupDefinition lookup)
		{
			if(lookup == null)
				throw new ArgumentNullException("lookup");
			_lookupName = lookup.Name;
		}

		public LookupExtendedPropertyDataType(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// set...
			_lookupName = name;
		}

		/// <summary>
		/// Gets the lookup.
		/// </summary>
		public string LookupName
		{
			get
			{
				return _lookupName;
			}
		}

		public override Type Type
		{
			get
			{
				// return...
				return typeof(int);
			}
		}

		protected override string GetId()
		{
			return this.LookupName;
		}

		public override string Name
		{
			get
			{
				return this.LookupName;
			}
		}

		internal override EntityField GetEntityField(ExtendedPropertyDefinition property)
		{
			if(property == null)
				throw new ArgumentNullException("property");
			
			// check...
			if(LookupName == null)
				throw new InvalidOperationException("'LookupName' is null.");
			if(LookupName.Length == 0)
				throw new InvalidOperationException("'LookupName' is zero-length.");

			// create...
			return new EntityLookupField(property.Name, property.NativeName, property);
		}

		public override string DisplayName
		{
			get
			{
				return string.Format("{0} [Lookup]", base.DisplayName);
			}
		}

		public override bool SupportsMultiValue
		{
			get
			{
				return true;
			}
		}
	}
}
