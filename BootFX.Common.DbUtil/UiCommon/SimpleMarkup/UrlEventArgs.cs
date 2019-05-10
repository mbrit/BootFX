// BootFX - Application framework for .NET applications
// 
// File: UrlEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI
{
	public delegate void UrlEventHandler(object sender, UrlEventArgs e);

	/// <summary>
	/// Summary description for ImageUrlEventArgs.
	/// </summary>
	public class UrlEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="Url"/> property.
		/// </summary>
		private string _url;
		
		public UrlEventArgs(string url)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			_url = url;
		}

		/// <summary>
		/// Gets the url.
		/// </summary>
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				_url = value;
			}
		}
	}
}
