// BootFX - Application framework for .NET applications
// 
// File: DefaultValueAttribute.cs
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
	/// Summary description for DefaultValueAttribute.
	/// </summary> 
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DefaultValueAttribute : Attribute
	{
		//propv DefaultValueType DefaultValueType
		public DefaultValueAttribute()
		{
		}

		public enum DefaultValueType
		{
			Primitive,
			DateTimeNow
		}
	}
}
