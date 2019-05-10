// BootFX - Application framework for .NET applications
// 
// File: EnumerationMappingAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines a mapping to an enumeration for a property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class EnumerationMappingAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="EnumerationType"/> property.
		/// </summary>
		private Type _enumerationType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EnumerationMappingAttribute(Type enumerationType)
		{
			if(enumerationType == null)
				throw new ArgumentNullException("enumerationType");
			_enumerationType = enumerationType;
		}

		/// <summary>
		/// Gets the enumerationtype.
		/// </summary>
		public Type EnumerationType
		{
			get
			{
				return _enumerationType;
			}
		}
	}
}
