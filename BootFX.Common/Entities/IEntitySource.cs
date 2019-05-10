// BootFX - Application framework for .NET applications
// 
// File: IEntitySource.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an interface that provides a way of reading entities from a database.
	/// </summary>
	public interface IEntitySource : ISqlStatementSource, IEntityType
	{
		/// <summary>
		/// Gets the fields that will be returned.
		/// </summary>
		EntityFieldCollection Fields
		{
			get;
		}

		/// <summary>
		/// Gets the keys of the entities in the source.
		/// </summary>
		/// <returns></returns>
		object[][] ExecuteKeyValues();
	}
}
