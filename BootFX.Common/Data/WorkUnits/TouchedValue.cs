// BootFX - Application framework for .NET applications
// 
// File: TouchedValue.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for TouchedValue.
	/// </summary>
	public class TouchedValue
	{
		/// <summary>
		/// Private field to support <see cref="Field"/> property.
		/// </summary>
		private EntityField _field;
		
		/// <summary>
		/// Private field to support <see cref="OldValue"/> property.
		/// </summary>
		private object _oldValue;

		/// <summary>
		/// Private field to support <see cref="NewValue"/> property.
		/// </summary>
		private object _newValue;
		
		internal TouchedValue(EntityField field, object newValue)
			: this(field, null, newValue)
		{
		}

		internal TouchedValue(EntityField field, object oldValue, object newValue)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			_field = field;
			_oldValue = oldValue;
			_newValue = newValue;
		}

		/// <summary>
		/// Gets the newvalue.
		/// </summary>
		public object NewValue
		{
			get
			{
				return _newValue;
			}
		}
		
		/// <summary>
		/// Gets the oldvalue.
		/// </summary>
		public object OldValue
		{
			get
			{
				return _oldValue;
			}
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
	}
}
