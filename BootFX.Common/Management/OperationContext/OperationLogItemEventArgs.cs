// BootFX - Application framework for .NET applications
// 
// File: OperationLogItemEventArgs.cs
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

namespace BootFX.Common.Management
{
	public delegate void OperationLogItemEventHandler(object sender, OperationLogItemEventArgs e);

	/// <summary>
	/// Defines an instance of <c>OperationLogItemEventArgs</c>.
	/// </summary>
	public class OperationLogItemEventArgs : EventArgs
	{
		/// <summary>
		/// Private field to support <see cref="item"/> property.
		/// </summary>
		private OperationLogItem _item;
		
		internal OperationLogItemEventArgs(OperationLogItem item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			_item = item;
		}

		/// <summary>
		/// Gets the item.
		/// </summary>
		public OperationLogItem Item
		{
			get
			{
				return _item;
			}
		}
	}
}
