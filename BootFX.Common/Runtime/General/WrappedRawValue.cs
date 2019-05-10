// BootFX - Application framework for .NET applications
// 
// File: WrappedRawValue.cs
// Build: 5.0.61009.900
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
using System.Globalization;
using BootFX.Common.Data;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>WrappedRawValue</c>.
	/// </summary>
	public abstract class WrappedRawValue
	{
		/// <summary>
		/// Private field to support <see cref="RawValue"/> property.
		/// </summary>
		private object _rawValue;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value"></param>
		protected WrappedRawValue(object rawValue)
		{
			this.RawValue = rawValue;
		}

		/// <summary>
		/// Gets the rawvalue.
		/// </summary>
		protected object RawValue
		{
			get
			{
				return _rawValue;
			}
			set
			{
				_rawValue = value;
			}
		}

		/// <summary>
		/// Gets the value as a string.
		/// </summary>
		/// <returns></returns>
		public string GetStringValue()
		{
			return GetStringValue(Cultures.System);
		}

		/// <summary>
		/// Gets the value as a string.
		/// </summary>
		/// <returns></returns>
		public string GetStringValue(IFormatProvider provider)
		{
			return (string)this.GetValue(typeof(string), provider);
		}

		/// <summary>
		/// Gets the value converting to the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object GetValue(Type type, IFormatProvider provider)
		{
			// get...
			if(type != null)
				return ConversionHelper.ChangeType(this.RawValue, type, provider);
			else
				return this.RawValue;
		}
	}
}
