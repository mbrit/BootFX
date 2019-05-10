// BootFX - Application framework for .NET applications
// 
// File: IInEdit.cs
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
	/// Describes an interface for indicating whether an object is being edited.
	/// </summary>
	public interface IInEdit 
	{
		/// <summary>
		/// Raised when <c>InEdit</c> changes.
		/// </summary>
		event EventHandler InEditChanged;

		/// <summary>
		/// Gets whether the object is being edited.
		/// </summary>
		bool InEdit
		{
			get;
		}
	}
}
