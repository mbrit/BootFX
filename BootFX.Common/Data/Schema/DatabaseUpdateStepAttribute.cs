// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateStepAttribute.cs
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
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>DatabaseUpdateStepAttribute</c>.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DatabaseUpdateStepAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="EntityType"/> property.
		/// </summary>
		private Type _entityType;

		public DatabaseUpdateStepAttribute()
		{
		}

		public DatabaseUpdateStepAttribute(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public Type EntityType
		{
			get
			{
				return _entityType;
			}
		}
	}
}
