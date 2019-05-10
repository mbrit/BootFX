// BootFX - Application framework for .NET applications
// 
// File: FormatLinkEventArgs.cs
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
	[Obsolete("Deprecated - use BootCMS instead.")]
	public delegate void FormatLinkEventHandler(object sender, FormatLinkEventArgs e);

	/// <summary>
	/// Summary description for FormatLinkEventArgs.
	/// </summary>
	[Obsolete("Deprecated - use BootCMS instead.")]
	public class FormatLinkEventArgs
	{
		/// <summary>
		/// Private field to support <c>Url</c> property.
		/// </summary>
		private string _url;

		/// <summary>
		/// Private field to support <c>Text</c> property.
		/// </summary>
		private string _text;
		
		internal FormatLinkEventArgs(string url, string text)
		{
			_url = url;
			_text = text;
		}

		/// <summary>
		/// Gets or sets the text
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _text)
				{
					// set the value...
					_text = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the url
		/// </summary>
		public string Url
		{
			get
			{
				return _url;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _url)
				{
					// set the value...
					_url = value;
				}
			}
		}
	}
}
