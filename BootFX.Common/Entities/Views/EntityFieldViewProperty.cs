// BootFX - Application framework for .NET applications
// 
// File: EntityFieldViewProperty.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Provides access to a property descriptor for an <see cref="EntityField"></see>.
	/// </summary>
	public class EntityFieldViewProperty : EntityViewProperty
	{
		/// <summary>
		/// Private field to support <c>Field</c> property.
		/// </summary>
		private EntityField _field;

		/// <summary>
		/// Private field to support <c>Format</c> property.
		/// </summary>
		private string _format = string.Empty;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="field"></param>
		public EntityFieldViewProperty(EntityField field) : base(field.EntityType)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			// set...
			_field = field;
		}

		/// <summary>
		/// Constructor for entity field view with format
		/// </summary>
		/// <param name="field"></param>
		/// <param name="format"></param>
		public EntityFieldViewProperty(EntityField field, string format) : this(field)
		{
			_format = format;
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return _field;
			}
		}
		
		/// <summary>
		/// Gets the format.
		/// </summary>
		public string Format
		{
			get
			{
				return _format;
			}
		}

		/// <summary>
		/// Creates a property descriptor for this field.
		/// </summary>
		/// <returns></returns>
		public override System.ComponentModel.PropertyDescriptor GetPropertyDescriptor()
		{
			if(Field == null)
				throw new ArgumentNullException("Field");

			if(Format != string.Empty)
				return this.Field.GetPropertyDescriptor(Format);

			// mbr - 28-03-2006 - added support for display name...			
			PropertyDescriptor desc = this.Field.GetPropertyDescriptor();
			if(desc == null)
				throw new InvalidOperationException("desc is null.");

			// set...
			if(desc is PropertyDescriptorBase)
				((PropertyDescriptorBase)desc).CustomDisplayName = this.ResolvedDisplayName;

			// return...
			return desc;
		}

		public override string DisplayName
		{
			get
			{
				if(this.Field != null)
					return this.Field.Name;
				else
					return "(No field)";
			}
		}
	}
}
