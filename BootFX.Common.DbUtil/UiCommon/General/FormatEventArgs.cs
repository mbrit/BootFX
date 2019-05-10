// BootFX - Application framework for .NET applications
// 
// File: FormatEventArgs.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Delegate for use with <see cref="FormatEventArgs"></see>.
	/// </summary>
	public delegate void FormatEventHandler(object sender, FormatEventArgs e);

	/// <summary>
	/// Summary description for FormatEventArgs.
	/// </summary>
	public class FormatEventArgs
	{
		/// <summary>
		/// Private field to support <c>SetValueCalled</c> property.
		/// </summary>
		private bool _setValueCalled = false;
		
		/// <summary>
		/// Private field to support <see cref="Value"/> property.
		/// </summary>
		private object _value;

		/// <summary>
		/// Private field to support <c>Text</c> property.
		/// </summary>
		private string _text = null;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="dbType"></param>
		public FormatEventArgs(object value)
		{
			_value = value;
		}

		/// <summary>
		/// Gets the type code of the value.
		/// </summary>
		public TypeCode TypeCode
		{
			get
			{
				if(this.Value != null)
					return Type.GetTypeCode(this.Value.GetType());
				else
					return TypeCode.Empty;
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

		/// <summary>
		/// Gets or sets the text
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				// set the value...
				_text = value;

				// flag...
				_setValueCalled = true;
			}
		}

		/// <summary>
		/// Gets or sets the setvaluecalled
		/// </summary>
		public bool SetValueCalled
		{
			get
			{
				return _setValueCalled;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _setValueCalled)
				{
					// set the value...
					_setValueCalled = value;
				}
			}
		}
	}
}
