// BootFX - Application framework for .NET applications
// 
// File: RssReadResults.cs
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

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines an instance of <c>RssReadResults</c>.
	/// </summary>
	public class RssReadResults
	{
		/// <summary>
		/// Private field to support <see cref="channel"/> property.
		/// </summary>
		private RssChannel _channel;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="channel"></param>
		internal RssReadResults(RssChannel channel)
		{
			if(channel == null)
				throw new ArgumentNullException("channel");
			_channel = channel;
		}

		/// <summary>
		/// Gets the channel.
		/// </summary>
		public RssChannel Channel
		{
			get
			{
				return _channel;
			}
		}
	}
}
