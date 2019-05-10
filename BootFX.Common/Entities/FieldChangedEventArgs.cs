// BootFX - Application framework for .NET applications
// 
// File: FieldChangedEventArgs.cs
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
	/// Delegate for use with <see cref="FieldChangedEventArgs"></see>.
	/// </summary>
	public delegate void FieldChangedEventHandler(object sender, FieldChangedEventArgs e);

	/// <summary>
	/// Describes event args for a field change notification.
	/// </summary>
	public class FieldChangedEventArgs : EntityCancelEventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Field"/> property.
		/// </summary>
		private EntityField _field;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="field"></param>
		public FieldChangedEventArgs(object entity, EntityField field, object oldValue, object newValue) 
			: base(entity)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			_field = field;
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
