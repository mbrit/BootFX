// BootFX - Application framework for .NET applications
// 
// File: SqlDialectAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for SqlDialectAttribute.
	/// </summary>
	// mbr - 2008-08-30 - added.
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class SqlDialectAttribute : Attribute
	{
		private Type _type;

		public SqlDialectAttribute(Type type)
		{
			_type = type;
		}

		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
}
