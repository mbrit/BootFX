// BootFX - Application framework for .NET applications
// 
// File: EntityMemberName.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for EntityMemberName.
	/// </summary>
	[Serializable()]
	internal struct EntityMemberName
	{
		/// <summary>
		/// Private field to support <see cref="EntityTypeName"/> property.
		/// </summary>
		private EntityTypeName _entityTypeName;
		
		/// <summary>
		/// Private field to support <c>Name</c> property.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Private field to support <see cref="MemberType"/> property.
		/// </summary>
		private EntityMemberType _memberType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal EntityMemberName(EntityMember member)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			// set...
			if(member.EntityType == null)
				throw new InvalidOperationException("member.EntityType is null.");
			_entityTypeName = member.EntityType.EntityTypeName;
			_name = member.Name;

			// check...
			if(member is EntityField)
				_memberType = EntityMemberType.Field;
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", member));
		}

		/// <summary>
		/// Gets the entitytypename.
		/// </summary>
		public EntityTypeName EntityTypeName
		{
			get
			{
				return _entityTypeName;
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				// returns the value...
				return _name;
			}
		}

		/// <summary>
		/// Gets the membertype.
		/// </summary>
		public EntityMemberType MemberType
		{
			get
			{
				return _memberType;
			}
		}
	}
}
