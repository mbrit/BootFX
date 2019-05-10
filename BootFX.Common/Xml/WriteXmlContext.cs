// BootFX - Application framework for .NET applications
// 
// File: WriteXmlContext.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Globalization;

namespace BootFX.Common.Xml
{
	/// <summary>
	/// Context for writing XML.
	/// </summary>
	public class WriteXmlContext
	{
		/// <summary>
		/// Private field to support <c>Culture</c> property.
		/// </summary>
		private CultureInfo _culture = Cultures.System;
		
		/// <summary>
		/// Private field to support <c>Encoding</c> property.
		/// </summary>
		private Encoding _encoding;

		/// <summary>
		/// Contructor.
		/// </summary>
		/// <param name="encoding"></param>
		internal WriteXmlContext(Encoding encoding)
		{
			if(encoding == null)
				throw new ArgumentNullException("encoding");
			
			_encoding = encoding;
		}

		/// <summary>
		/// Gets the encoding.
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				return _encoding;
			}
		}

		/// <summary>
		/// Gets or sets the culture
		/// </summary>
		public CultureInfo Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _culture)
				{
					// set the value...
					_culture = value;
				}
			}
		}
	}
}
