// BootFX - Application framework for .NET applications
// 
// File: NameValueStringPropertyBag.cs
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

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>NameValueStringPropertyBag</c>.
	/// </summary>
	public class NameValueStringPropertyBag : PropertyBag
	{
		public const char DefaultPairSeparator = ';';
		public const char DefaultValueSeparator = '=';

		/// <summary>
		/// Private field to support <see cref="ValueSeparator"/> property.
		/// </summary>
		private char _valueSeparator;

		/// <summary>
		/// Private field to support <see cref="PairSeparator"/> property.
		/// </summary>
		private char _pairSeparator;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nameValueString"></param>
		public NameValueStringPropertyBag()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nameValueString"></param>
		public NameValueStringPropertyBag(string nameValueString)
			: this(nameValueString, DefaultValueSeparator, DefaultPairSeparator)
		{
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="nameValueString"></param>
        // mbr - 2009-11-02 - need to support flags...
        public NameValueStringPropertyBag(string nameValueString, NameValueStringFlags flags)
            : this(nameValueString, DefaultValueSeparator, DefaultPairSeparator, flags)
        {
        }

        /// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nameValueString"></param>
		// mbr - 2009-11-02 - need to support flags...
        public NameValueStringPropertyBag(string nameValueString, char valueSeparator, char pairSeparator)
            : this(nameValueString, valueSeparator, pairSeparator, NameValueStringFlags.Normal)
        {
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="nameValueString"></param>
		// mbr - 25-07-2007 - added.		
		public NameValueStringPropertyBag(string nameValueString, char valueSeparator, char pairSeparator, NameValueStringFlags flags)
		{
			if(nameValueString == null)
				throw new ArgumentNullException("nameValueString");

			// set...
			_valueSeparator = valueSeparator;
			_pairSeparator = pairSeparator;

			// get...
			NameValueStringPropertyBag results = new NameValueStringPropertyBag();

            // mbr - 2009-11-02 - whitespace...
            string whitespace = @"\s";
            if((int)(flags & NameValueStringFlags.AllowWhitespaceInValues) != 0)
                whitespace = string.Empty;

			// mbr - 28-03-2006 - changed to: \s*(?<key>[^=;]*)\s*=?\s*(?<value>[^;\s]*)\s*
			Regex regex = new Regex(string.Format(@"\s*(?<key>[^{0}{1}]*)\s*{0}?\s*(?<value>[^{1}{2}]*)\s*", 
				valueSeparator, pairSeparator, whitespace), RegexOptions.Singleline | RegexOptions.IgnoreCase);
			
			// split it...
			MatchCollection matches = regex.Matches(nameValueString);
			if(matches == null)
				throw new InvalidOperationException("matches is null.");
			foreach(Match match in matches)
			{
				string key = match.Groups["key"].Value;
				if(key != null && key.Length > 0)
				{
					string value = match.Groups["value"].Value;
					this[key] = value;
				}
			}
		}
		
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			foreach(DictionaryEntry entry in this)
			{
				if(!(entry.Value is BootFX.Common.MissingItem))
				{
					if(builder.Length > 0)
						builder.Append(PairSeparator);
					builder.Append(ConversionHelper.ToString(entry.Key, Cultures.System));
					builder.Append(ValueSeparator);
					builder.Append(ConversionHelper.ToString(entry.Value, Cultures.System));
				}
			}

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Gets the pairseparator.
		/// </summary>
		private char PairSeparator
		{
			get
			{
				return _pairSeparator;
			}
		}
		
		/// <summary>
		/// Gets the valueseparator.
		/// </summary>
		private char ValueSeparator
		{
			get
			{
				return _valueSeparator;
			}
		}
	}
}
