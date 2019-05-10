// BootFX - Application framework for .NET applications
// 
// File: CheckoutFileListItem.cs
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

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Defines an instance of <c>CheckoutFileListItem</c>.
	/// </summary>
	internal class CheckoutFileListItem
	{
		internal CheckoutFileListItem(string[] data)
		{
			if(data == null)
				throw new ArgumentNullException("data");
			if(data.Length != 2)
				throw new ArgumentOutOfRangeException("'data' is wrong length.");
			
			_data = data;
		}

		/// <summary>
		/// Private field to support <c>Data</c> property.
		/// </summary>
		private string[] _data;
		
		/// <summary>
		/// Gets the data.
		/// </summary>
		private string[] Data
		{
			get
			{
				// returns the value...
				return _data;
			}
		}

		internal string TargetPath
		{
			get
			{
				return this.Data[0];
			}
		}

		internal string TempPath
		{
			get
			{
				return this.Data[1];
			}
		}

		public override string ToString()
		{
			return this.TargetPath;
		}
	}
}
