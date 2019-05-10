// BootFX - Application framework for .NET applications
// 
// File: EntityAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines an attribute for indicating that a given class is actually an entity.
	/// </summary>
	/// <seealso cref="EntityType"></seealso>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class EntityAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <c>IsSystemTable</c> property.
		/// </summary>
		private bool _isSystemTable = false;
		
		/// <summary>
		/// Private field to support <c>DatabaseName</c> property.
		/// </summary>
		private string _databaseName;
		
		/// <summary>
		/// Private field to support <c>CollectionType</c> property.
		/// </summary>
		private Type _collectionType;
		
		/// <summary>
		/// Private field to support <c>NativeName</c> property.
		/// </summary>
		private string _nativeName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nativeName"></param>
		public EntityAttribute(Type collectionType, string nativeName)
		{
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("nativeName");
			
			// set..
			_collectionType = collectionType;
			if(_collectionType == null)
				_collectionType = typeof(ArrayList);
			else
			{
				// check...
				if(typeof(IList).IsAssignableFrom(_collectionType) == false)
					throw new InvalidOperationException(string.Format("'{0}' does not implement IList and cannot be used as a collection type.", 
						_collectionType));
			}

			// set the native name...
			_nativeName = nativeName;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nativeName"></param>
		internal EntityAttribute(Type collectionType, string nativeName, bool isSystemTable) : this(collectionType, nativeName)
		{
			_isSystemTable = isSystemTable;
		}

		/// <summary>
		/// Sets the service interface type.
		/// </summary>
		/// <param name="serviceInterfaceType"></param>
		private void SetServiceInterfaceType(Type serviceInterfaceType)
		{
			if(serviceInterfaceType == null)
				throw new ArgumentNullException("serviceInterfaceType");
			if(serviceInterfaceType.IsInterface == false)
				throw new InvalidOperationException(string.Format("'{0}' is not an interface.", serviceInterfaceType));
		}

		/// <summary>
		/// Gets the collectiontype.
		/// </summary>
		public Type CollectionType
		{
			get
			{
				// returns the value...
				return _collectionType;
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
		/// Gets or sets the databasename
		/// </summary>
		public string DatabaseName
		{
			get
			{
				return _databaseName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _databaseName)
				{
					// set the value...
					_databaseName = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the issystemtable
		/// </summary>
		internal bool IsSystemTable
		{
			get
			{
				return _isSystemTable;
			}
		}
	}
}
