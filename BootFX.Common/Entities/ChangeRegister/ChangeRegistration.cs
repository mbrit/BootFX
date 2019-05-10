// BootFX - Application framework for .NET applications
// 
// File: ChangeRegistration.cs
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

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>ChangeRegistration</c>.
	/// </summary>
	internal class ChangeRegistration
	{
		/// <summary>
		/// Private field to support <c>Block</c> property.
		/// </summary>
		private ChangeRegisterRegistrationsBlock _block;

		/// <summary>
		/// Private field to support <c>Callback</c> property.
		/// </summary>
		private IChangeRegisterCallback _callback;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="block"></param>
		/// <param name="callback"></param>
		/// <param name="state"></param>
		internal ChangeRegistration(ChangeRegisterRegistrationsBlock block, IChangeRegisterCallback callback, object state)
		{
			if(block == null)
				throw new ArgumentNullException("block");
			if(callback == null)
				throw new ArgumentNullException("callback");
			
			_block = block;
			_callback = callback;
		}

		/// <summary>
		/// Gets the callback.
		/// </summary>
		private IChangeRegisterCallback Callback
		{
			get
			{
				// returns the value...
				return _callback;
			}
		}
		
		/// <summary>
		/// Gets the block.
		/// </summary>
		private ChangeRegisterRegistrationsBlock Block
		{
			get
			{
				// returns the value...
				return _block;
			}
		}

		/// <summary>
		/// Notifies that a change has occured.
		/// </summary>
		/// <param name="context"></param>
		internal void Notify(ChangeNotificationContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// call...
			if(Callback == null)
				throw new InvalidOperationException("Callback is null.");
			this.Callback.EntityChanged(context);
		}
	}
}
