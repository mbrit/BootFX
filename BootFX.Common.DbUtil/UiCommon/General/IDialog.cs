// BootFX - Application framework for .NET applications
// 
// File: IDialog.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Common
{
	/// <summary>
	/// Common dialog functionality.
	/// </summary>
	public interface IDialog
	{
		/// <summary>
		/// Applies changes to the dialog.
		/// </summary>
		/// <returns></returns>
		bool Apply();

		/// <summary>
		/// Applies changes to the dialog.
		/// </summary>
		/// <returns></returns>
		bool Apply(ApplyReason reason);

		/// <summary>
		/// Called when the Apply button is pressed.
		/// </summary>
		/// <returns></returns>
		bool DialogApply();

		/// <summary>
		/// Called when the OK button is pressed.
		/// </summary>
		/// <returns></returns>
		bool DialogOK();

		/// <summary>
		/// Called when the Cancel button is pressed.
		/// </summary>
		void DialogCancel();
	}
}
