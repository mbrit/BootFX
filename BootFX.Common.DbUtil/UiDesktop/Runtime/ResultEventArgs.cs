// BootFX - Application framework for .NET applications
// 
// File: ResultEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI.Desktop
{
	public delegate void ResultEventHandler(object sender, ResultEventArgs e);

	/// <summary>
	/// Summary description for ResultEventArgs.
	/// </summary>
	public class ResultEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Result"/> property.
		/// </summary>
		private object _result;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="result"></param>
		internal ResultEventArgs(object result)
		{
			_result = result;
		}

		/// <summary>
		/// Gets the result.
		/// </summary>
		public object Result
		{
			get
			{
				return _result;
			}
		}
	}
}
