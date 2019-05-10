// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateCheckResultsEventArgs.cs
// Build: 5.0.61009.900
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
using BootFX.Common.Data;

namespace BootFX.Common.UI.Desktop
{
	public delegate void DatabaseUpdateCheckResultsEventHandler(object sender, DatabaseUpdateCheckResultsEventArgs e);

	/// <summary>
	/// Defines an instance of <c>DatabaseUpdateCheckResultsEventArgs</c>.
	/// </summary>
	public class DatabaseUpdateCheckResultsEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Results"/> property.
		/// </summary>
		private DatabaseUpdateCheckResults _results;
		
		internal DatabaseUpdateCheckResultsEventArgs(DatabaseUpdateCheckResults results)
		{
			if(results == null)
				throw new ArgumentNullException("results");
			_results = results;
		}

		/// <summary>
		/// Gets the results.
		/// </summary>
		public DatabaseUpdateCheckResults Results
		{
			get
			{
				return _results;
			}
		}
	}
}
