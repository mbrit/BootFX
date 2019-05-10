// BootFX - Application framework for .NET applications
// 
// File: IEntityReadState.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for ILookaheadReader.
	/// </summary>
	public interface IEntityReadState
	{
		/// <summary>
		/// Gets the select map that's being used.
		/// </summary>
		SelectMap SelectMap
		{
			get;
		}

		/// <summary>
		/// Gets all values at the current point.
		/// </summary>
		/// <returns></returns>
		object[] GetValues();

		/// <summary>
		/// Gets a value from the current point.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		object GetValue(int index);
	}
}
