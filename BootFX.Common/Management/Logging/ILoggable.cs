// BootFX - Application framework for .NET applications
// 
// File: ILoggable.cs
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
	/// Interface for loggable objects.  Describes a pattern for easier access to logging for a class.
	/// </summary>
	/// <remarks>Normally, you should derive from <see cref="Management.Loggable"/> rather than implementing this class directly.</remarks>
	public interface ILoggable : IIsLoggable
	{
		/// <summary>
		/// Gets the default logger for the object.
		/// </summary>
		ILog Log
		{
			get;
		}

		/// <summary>
		/// Gets the set of available logs for the object.
		/// </summary>
		ILogSet Logs
		{
			get;
		}
	}
}
