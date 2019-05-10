// BootFX - Application framework for .NET applications
// 
// File: EntityViewProperty.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;
using System.ComponentModel;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Base class for providing property descriptors for entity members.
	/// </summary>
	public abstract class EntityViewProperty : IEntityType
	{
		/// <summary>
		/// Private field to support <c>CustomDisplayName</c> property.
		/// </summary>
		private string _customDisplayName;
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected EntityViewProperty(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}
	
		/// <summary>
		/// Creates a property descriptor.
		/// </summary>
		/// <returns></returns>
		public abstract PropertyDescriptor GetPropertyDescriptor();

		/// <summary>
		/// Creates a view property for the given expression.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static EntityViewProperty GetViewProperty(EntityType entityType, string expression)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(expression == null)
				throw new ArgumentNullException("expression");
			if(expression.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("expression");

			// is it a member?
			EntityMember member = entityType.GetMember(expression, OnNotFound.ReturnNull);
			if(member != null)
				return member.GetViewProperty();

			// is it a property?
			PropertyInfo property = entityType.Type.GetProperty(expression, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
			if(property != null)
				return new EntityBindViewProperty(entityType, property);

			// throw...
			throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot create expression for '{0}' on '{1}'.", expression, entityType));
		}

		/// <summary>
		/// Gets the display name of the property.
		/// </summary>
		public virtual string DisplayName
		{
			get
			{
				return base.ToString();
			}
		}

		public override string ToString()
		{
			return this.ResolvedDisplayName;
		}

		/// <summary>
		/// Gets or sets the customdisplayname
		/// </summary>
		// mbr - 28-03-2006 - added.
		public string CustomDisplayName
		{
			get
			{
				return _customDisplayName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _customDisplayName)
				{
					// set the value...
					_customDisplayName = value;
				}
			}
		}

		// mbr - 28-03-2006 - added.
		public string ResolvedDisplayName
		{
			get
			{
				if(this.CustomDisplayName != null && this.CustomDisplayName.Length > 0)
					return this.CustomDisplayName;
				else
					return this.DisplayName;
			}
		}
	}
}
