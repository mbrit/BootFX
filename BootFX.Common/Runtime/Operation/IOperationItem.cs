// BootFX - Application framework for .NET applications
// 
// File: IOperationItem.cs
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
	/// Defines an item that makes up an operation.
	/// </summary>
	public interface IOperationItem
	{
		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		string Status
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the progress maximum,
		/// </summary>
		int ProgressMaximum
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the progress minimum.
		/// </summary>
		int ProgressMinimum
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the progress value.
		/// </summary>
		int ProgressValue
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the last error.
		/// </summary>
		Exception LastError
		{
			get;
		}

		/// <summary>
		/// Sets the last error.
		/// </summary>
		/// <param name="error"></param>
		/// <remarks>Implementers may care to make this protected and LastError public.</remarks>
		void SetLastError(string status, Exception error);

		/// <summary>
		/// Resets the operation.
		/// </summary>
		void Reset();

		/// <summary>
		/// Gets the last time that the progress bar, status bar or cancel flag was changed.
		/// </summary>
		DateTime LastUpdateTime
		{
			get;
		}

		/// <summary>
		/// Cancels the operation.
		/// </summary>
		void Cancel();

		/// <summary>
		/// Returns true if the operation is cancelled.
		/// </summary>
		/// <returns></returns>
		bool IsCancelled
		{
			get;
		}

		/// <summary>
		/// Increments the progress bar.
		/// </summary>
		void IncrementProgress();

		/// <summary>
		/// Raised when the item has been finished.
		/// </summary>
		event EventHandler Finished;

		/// <summary>
		/// Raised when the item is cancelled.
		/// </summary>
		event EventHandler Cancelled;

		/// <summary>
		/// Raised when an error occurs.
		/// </summary>
		/// <remarks>The error that occured is in the LastError property.</remarks>
		event EventHandler Error;
	}
}
