// BootFX - Application framework for .NET applications
// 
// File: StringFormatterPropertyDescriptor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Management;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for <see cref="StringFormatterPropertyDescriptor"/>.
	/// </summary>
	internal class StringFormatterPropertyDescriptor : LoggablePropertyDescriptor
	{
		private const string Suffix = "AsString";

		/// <summary>
		/// Private field to support <c>FormatString</c> property.
		/// </summary>
		private string _formatString;
		
		/// <summary>
		/// Private field to support <c>InnerDescriptor</c> property.
		/// </summary>
		private PropertyDescriptor _innerDescriptor;
		
		/// <summary>
		/// Creates a new instance of <see cref="StringFormatterPropertyDescriptor"/>.
		/// </summary>
		internal StringFormatterPropertyDescriptor(PropertyDescriptor innerDescriptor, string formatString) 
			: base(innerDescriptor.Name + Suffix, new Attribute[] {})
		{
			if(innerDescriptor == null)
				throw new ArgumentNullException("innerDescriptor");
			_innerDescriptor = innerDescriptor;
			_formatString = formatString;
		}

		/// <summary>
		/// Gets the formatstring.
		/// </summary>
		private string FormatString
		{
			get
			{
				// returns the value...
				return _formatString;
			}
		}

		/// <summary>
		/// Gets the innerdescriptor.
		/// </summary>
		private PropertyDescriptor InnerDescriptor
		{
			get
			{
				// returns the value...
				return _innerDescriptor;
			}
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				if(InnerDescriptor == null)
					throw new InvalidOperationException("InnerDescriptor is null.");
				return this.InnerDescriptor.ComponentType;
			}
		}

		public override object GetValue(object component)
		{
			// inner...
			if(InnerDescriptor == null)
				throw new InvalidOperationException("InnerDescriptor is null.");
			object value = this.InnerDescriptor.GetValue(component);
			if(value == null || value is DBNull)
				return string.Empty;
			else if(value is DateTime)
			{
				// min?
				DateTime dateValue = (DateTime)value;
				if(dateValue == DateTime.MinValue)
					return string.Empty;
				else
				{
					if(this.HasFormatString)
						return string.Format("{0:" + this.FormatString + "}", dateValue);
					else
						return dateValue.ToString();
				}
			}
			else
			{
				if(this.HasFormatString)
					return string.Format("{0:" + this.FormatString + "}", value);
				else
					return value.ToString();
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return true;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(string);
			}
		}

		public override void ResetValue(object component)
		{
			
		}

		public override void SetValue(object component, object value)
		{
			
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		private bool HasFormatString
		{
			get
			{
				if(this.FormatString != null && this.FormatString.Length > 0)
					return true;
				else
					return false;
			}
		}
	}
}
