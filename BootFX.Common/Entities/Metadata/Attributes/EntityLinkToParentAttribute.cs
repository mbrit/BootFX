// BootFX - Application framework for .NET applications
// 
// File: EntityLinkToParentAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines a property on a class to map to a <see cref="ChildToParentEntityLink"></see>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class EntityLinkToParentAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <c>LinkFieldNames</c> property.
		/// </summary>
		private string[] _linkFieldNames;
		
		/// <summary>
		/// Private field to support <c>ParentEntityType</c> property.
		/// </summary>
		private Type _parentEntityType;
		
		/// <summary>
		/// Private field to support <c>Name</c> property.
		/// </summary>
		private string _name;

		/// <summary>
		/// Private field to support <c>NativeName</c> property.
		/// </summary>
		private string _nativeName;

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkToParentAttribute(Type parentEntityType, string nativeName, string linkFieldName) : this(parentEntityType.Name, nativeName,parentEntityType, new string[] { linkFieldName })
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkToParentAttribute(Type parentEntityType,string nativeName, string[] linkFieldNames) : this(parentEntityType.Name, nativeName,parentEntityType, linkFieldNames)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkToParentAttribute(string name,string nativeName, Type parentEntityType, string linkFieldName) : this(name, nativeName,parentEntityType, new string[] { linkFieldName })
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkToParentAttribute(string name, string nativeName, Type parentEntityType, string[] linkFieldNames)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			if(nativeName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("nativeName");

			if(parentEntityType == null)
				throw new ArgumentNullException("parentEntityType");

			// set...
			_name = name;
			_nativeName = nativeName;
			_parentEntityType = parentEntityType;
			_linkFieldNames = linkFieldNames;
		}

		/// <summary>
		/// Gets the parententitytype.
		/// </summary>
		public Type ParentEntityType
		{
			get
			{
				return _parentEntityType;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string NativeName
		{
			get
			{
				return _nativeName;
			}
		}

		/// <summary>
		/// Gets the linkfieldnames.
		/// </summary>
		public string[] GetLinkFieldNames()
		{
			string[] newNames = new string[this._linkFieldNames.Length];
			this._linkFieldNames.CopyTo(newNames, 0);
			return newNames;
		}
	}
}
