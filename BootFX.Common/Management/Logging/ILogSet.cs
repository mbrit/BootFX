// BootFX - Application framework for .NET applications
// 
// File: ILogSet.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Management
{
	/// <summary>
	/// Describes a set of logs.
	/// </summary>
	public interface ILogSet
	{
		/// <summary>
		/// Gets the master log.
		/// </summary>
		ILog DefaultLog
		{
			get;
		}

		/// <summary>
		/// Gets the log for the given activity.
		/// </summary>
		ILog this[string activity]
		{
			get;
		}

		/// <summary>
		/// Returns the context ID.
		/// </summary>
		long ContextId
		{
			get;
		}
	}
}
