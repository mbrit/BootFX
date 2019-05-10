// BootFX - Application framework for .NET applications
// 
// File: EntityTypeName.cs
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
	/// Defines the entity type name.
	/// </summary>
	[Serializable()]
	public struct EntityTypeName
	{
		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityTypeName(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// set...
			if(entityType.Type == null)
				throw new InvalidOperationException("entityType.Type is null.");
			_name = string.Format("{0}, {1}", entityType.Type.FullName, entityType.Type.Assembly.GetName().Name);
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
		/// Gets the CLR type.
		/// </summary>
		/// <returns></returns>
		public Type GetClrType()
		{
			if(Name == null)
				throw new InvalidOperationException("'Name' is null.");
			if(Name.Length == 0)
				throw new InvalidOperationException("'Name' is zero-length.");

			try
			{
				return Type.GetType(this.Name, true, true);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to load CLR type for '{0}'.", this.Name), ex);
			}
		}
	}
}
