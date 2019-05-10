// BootFX - Application framework for .NET applications
// 
// File: EntityFieldAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using BootFX.Common.Data;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	///	 Defines a property of an class to be an <see cref="EntityField"></see>.
	/// </summary>
	/// <seealso cref="EntityType"></seealso>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class EntityFieldAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <c>NativeName</c> property.
		/// </summary>
		private string _nativeName;

		/// <summary>
		/// Private field to support <c>DbType</c> property.
		/// </summary>
		private DbType _dbType;

		/// <summary>
		/// Private field to support <c>Flags</c> property.
		/// </summary>
		private EntityFieldFlags _flags;

		/// <summary>
		/// Private field to support <c>Size</c> property.
		/// </summary>
		private long _size;

		/// <summary>
		/// Private field to support <c>SizeDefined</c> property.
		/// </summary>
		private bool _sizeDefined;
		
		/// <summary>
		/// Private field to support <c>Default</c> property.
		/// </summary>
		private CommonDefault _default;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityFieldAttribute(string nativeName, DbType dbType, EntityFieldFlags flags)
		{
			// set...
			_nativeName = nativeName;
			_dbType = dbType;
			_flags = flags;
			_sizeDefined = false;

			// default...
			_default = EntityField.GetDefaultDefault(dbType, flags);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityFieldAttribute(string nativeName, DbType dbType, EntityFieldFlags flags, long size) : this(nativeName, dbType, flags)
		{
			// set...
			_size = size;
			_sizeDefined = true;
		}

		/// <summary>
		/// Gets the sizedefined.
		/// </summary>
		public bool SizeDefined
		{
			get
			{
				return _sizeDefined;
			}
		}
		
		/// <summary>
		/// Gets the size.
		/// </summary>
		public long Size
		{
			get
			{
				return _size;
			}
		}
		
		/// <summary>
		/// Gets the flags.
		/// </summary>
		public EntityFieldFlags Flags
		{
			get
			{
				return _flags;
			}
		}
		
		/// <summary>
		/// Gets the dbtype.
		/// </summary>
		public DbType DBType
		{
			get
			{
				return _dbType;
			}
		}
		
		/// <summary>
		/// Gets the nativename.
		/// </summary>
		public string NativeName
		{
			get
			{
				return _nativeName;
			}
		}

		/// <summary>
		/// Gets or sets the default
		/// </summary>
		public CommonDefault Default
		{
			get
			{
				return _default;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _default)
				{
					// set the value...
					_default = value;
				}
			}
		}
	}
}
