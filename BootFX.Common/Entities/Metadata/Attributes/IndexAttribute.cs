// BootFX - Application framework for .NET applications
// 
// File: IndexAttribute.cs
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

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines an instance of <c>IndexAttribute</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public sealed class IndexAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;

		/// <summary>
		/// Private field to support <see cref="NativeName"/> property.
		/// </summary>
		private string _nativeName;

		/// <summary>
		/// Private field to support <see cref="HasUniqueValues"/> property.
		/// </summary>
		private bool _hasUniqueValues;

		/// <summary>
		/// Private field to support <see cref="ColumnNativeNames"/> property.
		/// </summary>
		private string _columnNativeNames;

        public string IncludedColumns { get; set; }
        public string ComputedColumns { get; set; }

		public IndexAttribute(string name, string nativeName, bool hasUniqueValues, string columnNativeNames)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			if(columnNativeNames == null)
				throw new ArgumentNullException("columnNativeNames");
			if(columnNativeNames.Length == 0)
				throw new ArgumentOutOfRangeException("'columnNativeNames' is zero-length.");
			
			_name = name;
			_nativeName = nativeName;
			_hasUniqueValues = hasUniqueValues;
			_columnNativeNames = columnNativeNames;
		}

		/// <summary>
		/// Gets the columnnativenames.
		/// </summary>
		internal string ColumnNativeNames
		{
			get
			{
				return _columnNativeNames;
			}
		}
		
		/// <summary>
		/// Gets the hasuniquevalues.
		/// </summary>
		internal bool HasUniqueValues
		{
			get
			{
				return _hasUniqueValues;
			}
		}
		
		/// <summary>
		/// Gets the nativename.
		/// </summary>
		internal string NativeName
		{
			get
			{
				return _nativeName;
			}
		}
		
		/// <summary>
		/// Gets the name.
		/// </summary>
		internal string Name
		{
			get
			{
				return _name;
			}
		}
	}
}
