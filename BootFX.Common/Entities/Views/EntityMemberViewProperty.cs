// BootFX - Application framework for .NET applications
// 
// File: EntityMemberViewProperty.cs
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
	/// Summary description for EntityMemberViewProperty.
	/// </summary>
	public class EntityMemberViewProperty : EntityViewProperty
	{
		/// <summary>
		/// Private field to support <see cref="Member"/> property.
		/// </summary>
		private EntityMember _member;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="member"></param>
		public EntityMemberViewProperty(EntityMember member) : base(member.EntityType)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			_member = member;
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		public EntityMember Member
		{
			get
			{
				return _member;
			}
		}

		public override System.ComponentModel.PropertyDescriptor GetPropertyDescriptor()
		{
			if(Member == null)
				throw new InvalidOperationException("Member is null.");
			return this.Member.GetPropertyDescriptor();
		}
	}
}
