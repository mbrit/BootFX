// BootFX - Application framework for .NET applications
// 
// File: EntityMemberBinding.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop.DataBinding
{
	/// <summary>
	/// Base class for binding to an entity member.
	/// </summary>
	public abstract class EntityMemberBinding : BindingBase
	{
		/// <summary>
		/// Private field to support <see cref="Member"/> property.
		/// </summary>
		private EntityMember _member;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected EntityMemberBinding(EntityMember member, string propertyName, object dataSource, string dataMember) : base(propertyName, dataSource, dataMember)
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
	}
}
