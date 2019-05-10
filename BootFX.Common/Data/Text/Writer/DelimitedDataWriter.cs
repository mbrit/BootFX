// BootFX - Application framework for .NET applications
// 
// File: DelimitedDataWriter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Data.Text
{
	/// <summary>
	/// Defines an object that can write delimited text.
	/// </summary>
	public class DelimitedDataWriter : LineBasedDataWriter
	{
		/// <summary>
		/// Private field to support <see cref="Quote"/> property.
		/// </summary>
		private string _quote;
		
		/// <summary>
		/// Private field to support <see cref="Delimiter"/> property.
		/// </summary>
		private string _delimiter;
		
		public DelimitedDataWriter(TextWriter writer, string delimiter, string quote) : base(writer)
		{
			if(delimiter == null)
				throw new ArgumentNullException("delimiter");
			if(delimiter.Length == 0)
				throw new ArgumentOutOfRangeException("'delimiter' is zero-length.");
			
			_delimiter = delimiter;
			_quote = quote;
		}

		protected override void OnBeforeValueWritten(EventArgs e)
		{
			base.OnBeforeValueWritten (e);

			// field separator...
			if(this.FieldNumber > 0)
				this.Write(this.Delimiter);
		}

		/// <summary>
		/// Gets the delimiter.
		/// </summary>
		internal string Delimiter
		{
			get
			{
				return _delimiter;
			}
		}

		/// <summary>
		/// Gets the quote.
		/// </summary>
		internal string Quote
		{
			get
			{
				return _quote;
			}
		}

		protected override string FormatValue(object value)
		{
			string asString = base.FormatValue (value);
			if(asString == null)
				return string.Empty;

			// quote...
			if(this.Quote != null && this.Quote.Length > 0)
				asString = this.Quote + asString.Replace(this.Quote, this.Quote + this.Quote) + this.Quote;

			// return...
			return asString;
		}

	}
}
