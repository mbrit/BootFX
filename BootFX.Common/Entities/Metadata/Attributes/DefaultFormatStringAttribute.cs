// BootFX - Application framework for .NET applications
// 
// File: DefaultFormatStringAttribute.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities.Attributes
{
	/// <summary>
	/// Defines the default format string for an item. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class DefaultFormatStringAttribute : Attribute
	{
		/// <summary>
		/// Private field to support <see cref="FormatString"/> property.
		/// </summary>
		private string _formatString;
		
		/// <summary>
		/// Creates a new instance of <see cref="DefaultFormatStringAttribute"/>.
		/// </summary>
		public DefaultFormatStringAttribute(string formatString)
		{
			if(formatString == null)
				throw new ArgumentNullException("formatString");
			if(formatString.Length == 0)
				throw new ArgumentOutOfRangeException("'formatString' is zero-length.");
			_formatString = formatString;
		}

		/// <summary>
		/// Gets the formatstring.
		/// </summary>
		internal string FormatString
		{
			get
			{
				return _formatString;
			}
		}
	}
}
