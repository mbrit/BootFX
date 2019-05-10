// BootFX - Application framework for .NET applications
// 
// File: LayoutHint.cs
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
using System.Data;
using System.Collections;
using System.Globalization;
using BootFX.Common.Entities;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>LayoutHint</c>.
	/// </summary>
	[Serializable()]
	public class LayoutHint
	{
		/// <summary>
		/// Private field to support <see cref="DateTimeFormat"/> property.
		/// </summary>
		private string _dateTimeFormat = null;
		
		/// <summary>
		/// Private field to support <see cref="FormatHint"/> property.
		/// </summary>
		private string _formatHint = null;
		
		/// <summary>
		/// Private field to support <see cref="Multiline"/> property.
		/// </summary>
		private bool _multiline = false;
		
		/// <summary>
		/// Private field to support <see cref="MaxLength"/> property.
		/// </summary>
		private int _maxLength = -1;
		
		public const int SinglelineLimit = 128;

		/// <summary>
		/// Gets the multiline.
		/// </summary>
		public bool Multiline
		{
			get
			{
				return _multiline;
			}
		}

		/// <summary>
		/// Gets the maxlength.
		/// </summary>
		public int MaxLength
		{
			get
			{
				return _maxLength;
			}
		}

		/// <summary>
		/// Gets the formathint.
		/// </summary>
		public string FormatHint
		{
			get
			{
				return _formatHint;
			}
		}

		/// <summary>
		/// Gets the datetimeformat.
		/// </summary>
		public string DateTimeFormat
		{
			get
			{
				return _dateTimeFormat;
			}
		}
	}
}
