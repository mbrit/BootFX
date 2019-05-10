// BootFX - Application framework for .NET applications
// 
// File: IMemberEditor.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Interface for member editors.
	/// </summary>
	internal interface IMemberEditor
	{
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		object Value
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes the view.
		/// </summary>
		void Initializing(EntityMemberBox box, EntityMember member);

		/// <summary>
		/// Called when the view has been initialized.
		/// </summary>
		void Initialized();
	}
}
