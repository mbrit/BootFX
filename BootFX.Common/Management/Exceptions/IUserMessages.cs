// BootFX - Application framework for .NET applications
// 
// File: IUserMessages.cs
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
	/// Defines an interface for classes that hold an array of user-facing messages.
	/// </summary>
	public interface IUserMessages
	{
		/// <summary>
		/// Gets the user messages.
		/// </summary>
		/// <returns></returns>
		string[] GetUserMessages();
    }
}
