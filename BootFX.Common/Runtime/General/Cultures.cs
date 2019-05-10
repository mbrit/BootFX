// BootFX - Application framework for .NET applications
// 
// File: Cultures.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Globalization;

namespace BootFX.Common
{
	/// <summary>
	/// Provides quick access to cultures.
	/// </summary>
	public sealed class Cultures
	{
		/// <summary>
		/// Private constructor.
		/// </summary>
		private Cultures()
		{
		}

		/// <summary>
		/// Returns the culture used for UI-specific comparisons, etc.
		/// </summary>
		public static CultureInfo User
		{
			get
			{
				return CultureInfo.CurrentUICulture;
			}
		}

		/// <summary>
		/// Returns the culture used for UI-agnostic (system) comparisons, etc.
		/// </summary>
		public static CultureInfo System
		{
			get
			{
				return CultureInfo.InvariantCulture;
			}
		}

        internal static CultureInfo Default
        {
            get
            {
                return System;
            }
        }

		/// <summary>
		/// Returns the culture used for exceptions.
		/// </summary>
		public static CultureInfo Exceptions
		{
			get
			{
				return System;
			}
		}

		/// <summary>
		/// Returns the culture used for logging.
		/// </summary>
		public static CultureInfo Log
		{
			get
			{
				return System;
			}
		}
	}
}
