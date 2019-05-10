// BootFX - Application framework for .NET applications
// 
// File: ISqlFilterMangler.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for IConstraintControl.
	/// </summary>
	public interface ISqlFilterMangler
	{
		/// <summary>
		/// Gets the id of the mangler
		/// </summary>
		string ManglerId
		{
			get;
			set;
		}

		/// <summary>
		/// Appends an item to the filter.
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="context"></param>
		void MangleFilter(SqlFilter filter);

		/// <summary>
		/// Serializes values to a dictionary.
		/// </summary>
		/// <param name="bag"></param>
		void SerializeSqlSearchSettings(IDictionary bag);

		/// <summary>
		/// Serializes values to a dictionary.
		/// </summary>
		/// <param name="bag"></param>
		void DeserializeSqlSearchSettings(IDictionary bag);
	}
}
