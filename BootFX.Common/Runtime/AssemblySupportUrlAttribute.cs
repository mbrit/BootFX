// BootFX - Application framework for .NET applications
// 
// File: AssemblySupportUrlAttribute.cs
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
	///	 Defines an attribute for storing the support URL for the application.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class AssemblySupportUrlAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <c>Url</c> property.
		/// </summary>
		private string _url;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="url"></param>
		public AssemblySupportUrlAttribute(string url)
		{
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
		}
	}
}
