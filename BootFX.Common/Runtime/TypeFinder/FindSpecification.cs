// BootFX - Application framework for .NET applications
// 
// File: FindSpecification.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	/// <summary>
	/// Base class for <see cref="TypeFinder"></see> specifications.
	/// </summary>
	internal abstract class FindSpecification
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		protected FindSpecification()
		{
		}

		/// <summary>
		/// Returns true if the given type is a match.w
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public abstract bool Match(Type type);
	}
}
