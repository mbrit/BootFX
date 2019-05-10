// BootFX - Application framework for .NET applications
// 
// File: EnumComboBoxListItem.cs
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

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Summary description for EnumComboBoxListItem.
	/// </summary>
	internal class EnumComboBoxListItem
	{
		/// <summary>
		/// Private field to support <c>textRegex</c> property.
		/// </summary>
		// (?<word>(?<upper>[A-Z]{1})(?<remainder>[a-z]*))
		private static Regex _textRegex = new Regex(@"(?<word>(?<upper>[A-Z]{1})(?<remainder>[a-z]*))", 
			RegexOptions.Singleline);
		
		/// <summary>
		/// Private field to support <see cref="Text"/> property.
		/// </summary>
		private string _text;
		
		/// <summary>
		/// Private field to support <see cref="Value"/> property.
		/// </summary>
		private object _value;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EnumComboBoxListItem(object value)
		{
			_value = value;

			// split...
			if(value != null)
			{
				MatchCollection matches = TextRegex.Matches(value.ToString());
				StringBuilder builder = new StringBuilder();
				foreach(Match match in matches)
				{
					if(builder.Length > 0)
						builder.Append(" ");
					builder.Append(match.Value);
				}

				// set...
				_text = builder.ToString();
			}
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		public override string ToString()
		{
			return this.Text;
		}

		/// <summary>
		/// Gets the text.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
		}

		/// <summary>
		/// Gets the textRegex.
		/// </summary>
		private static Regex TextRegex
		{
			get
			{
				// returns the value...
				return _textRegex;
			}
		}
	}
}
