// BootFX - Application framework for .NET applications
// 
// File: FilterMangler.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for FilterMangler.
	/// </summary>
	public class FilterMangler 
	{
		private string _manglerId = string.Empty;
		private ISqlFilterMangler _filterMangler = null;
		private SimpleXmlPropertyBag _settings = null;


		public FilterMangler(string manglerId, ISqlFilterMangler filter) : this(manglerId,new SimpleXmlPropertyBag(),filter)
		{
		}

		public FilterMangler(string manglerId, SimpleXmlPropertyBag settings, ISqlFilterMangler filter)
		{
			if(manglerId == null)
				throw new ArgumentNullException("manglerId");
			if(manglerId.Length == 0)
				throw new ArgumentOutOfRangeException("'manglerId' is zero-length.");
			if(settings == null)
				throw new ArgumentNullException("settings");
			if(filter == null)
				throw new ArgumentNullException("filter");
			
			// set...
			_manglerId = manglerId;
			_settings = settings;
			_filterMangler = filter;
		}

		/// <summary>
		/// Mangler ID
		/// </summary>
		public string ManglerId
		{
			get
			{
				return _manglerId;
			}
		}

		/// <summary>
		/// Settings from the mangler
		/// </summary>
		public SimpleXmlPropertyBag Settings
		{
			get
			{
				return _settings;
			}
		}

		/// <summary>
		/// Mangler ID
		/// </summary>
		public ISqlFilterMangler Filter
		{
			get
			{
				return _filterMangler;
			}
		}
	}
}
