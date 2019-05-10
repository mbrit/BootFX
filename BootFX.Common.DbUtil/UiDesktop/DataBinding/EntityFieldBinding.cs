// BootFX - Application framework for .NET applications
// 
// File: EntityFieldBinding.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop.DataBinding
{
	/// <summary>
	/// Extends Windows Forms data binding for use with fields.
	/// </summary>
	/// <remarks>Specifically, this handles formatting and parsing the values to and from strings in accordance
	/// with Windows Forms rules and the rules of the field itself.</remarks>
	public class EntityFieldBinding : EntityMemberBinding
	{
		/// <summary>
		/// Private field to support <c>Culture</c> property.
		/// </summary>
		private CultureInfo _culture = Cultures.User;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityFieldBinding(EntityField field, object dataSource, string propertyName) : base(field, propertyName, dataSource, field.Name)
		{
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return (EntityField)this.Member;
			}
		}

		/// <summary>
		/// Private field to support <c>Converter</c> property.
		/// </summary>
		private TypeConverter _converter;
		
		/// <summary>
		/// Gets the type converter for the binding.
		/// </summary>
		public TypeConverter Converter
		{
			get
			{
				EnsureConverterCreated();
				return _converter;
			}
		}
		
		/// <summary>
		/// Returns  Converter.
		/// </summary>
		private bool IsConverterCreated()
		{
			if(_converter == null)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Ensures that Converter has been created.
		/// </summary>
		private void EnsureConverterCreated()
		{
			if(IsConverterCreated() == false)
				_converter = CreateConverter();
		}
		
		/// <summary>
		/// Creates an instance of Converter.
		/// </summary>
		/// <remarks>This does not assign the instance to the _converter field</remarks>
		private TypeConverter CreateConverter()
		{
			if(Field == null)
				throw new InvalidOperationException("Field is null.");
			return this.Field.GetConverter();
		}

		protected override void OnFormat(ConvertEventArgs e)
		{
			// get a converter...
			if(Converter == null)
				throw new InvalidOperationException("Converter is null.");
			e.Value = this.Converter.ConvertToString(null, this.Culture, e.Value);
		}

		protected override void OnParse(ConvertEventArgs e)
		{
			// convert...
			if(Field == null)
				throw new InvalidOperationException("Field is null.");

			// target...
			Type targetType = this.Field.Type;

			// don't do anything if 
			if(e.Value == null || targetType.IsAssignableFrom(e.Value.GetType()) == false)
			{
				if(Converter == null)
					throw new InvalidOperationException("Converter is null.");
				e.Value = this.Converter.ConvertTo(null, this.Culture, e.Value, this.Field.Type);
			}
		}

		/// <summary>
		/// Gets the culture.
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
		}
	}
}
