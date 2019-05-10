// BootFX - Application framework for .NET applications
// 
// File: ForeignKeyAttribute.cs
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
using BootFX.Common.Data.Schema;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines an instance of <c>IndexAttribute</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
	public sealed class ForeignKeyAttribute : Attribute
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
		/// Private field to support <see cref="NativeName"/> property.
		/// </summary>
		private string _parentNativeTableName;

		/// <summary>
		/// Private field to support <see cref="ColumnNativeNames"/> property.
		/// </summary>
		private string _columnNativeNames;
		
		/// <summary>
		/// Private field to support <see cref="ParentColumnNativeNames"/> property.
		/// </summary>
		private string _parentColumnNativeNames;

		public ForeignKeyAttribute(string name, string nativeName, string columnNativeNames, string parentNativeTableName, string parentColumnNativeNames)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'nativeName' is zero-length.");
			if(parentNativeTableName == null)
				throw new ArgumentNullException("parentNativeTableName");
			if(parentNativeTableName.Length == 0)
				throw new ArgumentOutOfRangeException("'parentNativeTableName' is zero-length.");
			if(columnNativeNames == null)
				throw new ArgumentNullException("columnNativeNames");
			if(columnNativeNames.Length == 0)
				throw new ArgumentOutOfRangeException("'columnNativeNames' is zero-length.");
			if(parentColumnNativeNames == null)
				throw new ArgumentNullException("parentColumnNativeNames");
			if(parentColumnNativeNames.Length == 0)
				throw new ArgumentOutOfRangeException("'parentColumnNativeNames' is zero-length.");
			
			_name = name;
			_nativeName = nativeName;
			_parentNativeTableName = parentNativeTableName;
			_parentColumnNativeNames = parentColumnNativeNames;
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
		/// Gets the parents native table name.
		/// </summary>
		internal string ParentNativeTableName
		{
			get
			{
				return _parentNativeTableName;
			}
		}

		/// <summary>
		/// Gets the columnnativenames.
		/// </summary>
		internal string ParentColumnNativeNames
		{
			get
			{
				return _parentColumnNativeNames;
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
