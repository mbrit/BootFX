// BootFX - Application framework for .NET applications
// 
// File: BoundCache.cs
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
using System.Collections.Specialized;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that provides bound caching.
	/// </summary>
	internal class BoundCache
	{
		/// <summary>
		/// Private field to support <c>Usage</c> property.
		/// </summary>
		private int _usage = 0;
		
		/// <summary>
		/// Private field to support <c>Values</c> property.
		/// </summary>
		private IDictionary _values = new HybridDictionary();
		
		internal BoundCache()
		{
		}

		/// <summary>
		/// Gets a value from the bound cache.
		/// </summary>
		/// <param name="hash"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		internal bool GetValue(string hash, ref object result)
		{
			if(hash == null)
				throw new ArgumentNullException("hash");
			if(hash.Length == 0)
				throw new ArgumentOutOfRangeException("'hash' is zero-length.");
			
			// get...
			if(this.Values.Contains(hash))
			{
				result = this.Values[hash];

				// cloneable?
				if(result is ICloneable)
					result = ((ICloneable)result).Clone();

				// return...
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Sets the value in a bound cache.
		/// </summary>
		/// <param name="hash"></param>
		/// <param name="value"></param>
		public void SetValue(string hash, object value)
		{
			if(hash == null)
				throw new ArgumentNullException("hash");
			if(hash.Length == 0)
				throw new ArgumentOutOfRangeException("'hash' is zero-length.");
			
			// set...
			this.Values[hash] = value;
		}

		/// <summary>
		/// Gets the values.
		/// </summary>
		private IDictionary Values
		{
			get
			{
				// returns the value...
				return _values;
			}
		}

		/// <summary>
		/// Gets or sets the usage
		/// </summary>
		internal int Usage
		{
			get
			{
				return _usage;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _usage)
				{
					// set the value...
					_usage = value;
				}
			}
		}
	}
}
