// BootFX - Application framework for .NET applications
// 
// File: DBNullEquivalentAttribute.cs
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
	/// Attribute for decorating propreties to set  DB null equivalents.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DBNullEquivalentAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="DBNullEquivalent"/> property.
		/// </summary>
		private object _dbNullEquivalent;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="DBNullEquivalent"></param>
		public DBNullEquivalentAttribute(object DBNullEquivalent)
		{
			_dbNullEquivalent = DBNullEquivalent;
		}

		/// <summary>
		/// Gets the dbnullequivalent.
		/// </summary>
		public object DBNullEquivalent
		{
			get
			{
				return _dbNullEquivalent;
			}
		}
	}
}
