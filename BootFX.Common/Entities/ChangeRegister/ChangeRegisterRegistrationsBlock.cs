// BootFX - Application framework for .NET applications
// 
// File: ChangeRegisterRegistrationsBlock.cs
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
	/// Defines an instance of <c>ChangeRegisterRegistrationsBlock</c>.
	/// </summary>
	internal class ChangeRegisterRegistrationsBlock : IEntityType
	{
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Private field to support <c>Registrations</c> property.
		/// </summary>
		private ChangeRegistrationCollection _registrations = new ChangeRegistrationCollection();
		
		internal ChangeRegisterRegistrationsBlock(EntityType entityType)
		{
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				// returns the value...
				return _entityType;
			}
		}

		/// <summary>
		/// Subscribes a callback.
		/// </summary>
		/// <param name="callback"></param>
		internal void Subscribe(IChangeRegisterCallback callback, object state)
		{
			if(callback	 == null)
				throw new ArgumentNullException("callback	");

			// add...
			this.Registrations.Add(new ChangeRegistration(this, callback, state));
		}

		/// <summary>
		/// Gets a collection of ChangeRegistration objects.
		/// </summary>
		internal ChangeRegistrationCollection Registrations
		{
			get
			{
				return _registrations;
			}
		}
	}
}
