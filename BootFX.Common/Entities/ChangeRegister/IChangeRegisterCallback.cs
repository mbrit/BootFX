// BootFX - Application framework for .NET applications
// 
// File: IChangeRegisterCallback.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Entities
{
	public delegate void EntityChangedDelegate(ChangeNotificationContext context);

	/// <summary>
	/// Defines an instance of <c>IChangeRegisterCallback</c>.
	/// </summary>
	public interface IChangeRegisterCallback 
	{
		/// <summary>
		/// Called when an entity has been changed.
		/// </summary>
		/// <param name="context"></param>
		void EntityChanged(ChangeNotificationContext context);
	}
}
