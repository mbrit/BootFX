// BootFX - Application framework for .NET applications
// 
// File: TraceWindowListener.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Diagnostics;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Describes a listener for use with <see cref="TraceWindow"></see>.
	/// </summary>
	internal class TraceWindowListener : TraceListener
	{
		/// <summary>
		/// Raised when a line is written.
		/// </summary>
		public event LineEventHandler LineWritten;
		
		/// <summary>
		/// Private field to support <c>Line</c> property.
		/// </summary>
		private StringBuilder _line = new StringBuilder();

		/// <summary>
		/// Lock object for <c>Line</c>.
		/// </summary>
		private object lineLock = new object();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public TraceWindowListener()
		{
		}

		public override void Write(string message)
		{
			lock(this.lineLock)
			{
				Line.Append(message);
			}
		}

		public override void WriteLine(string message)
		{
			lock(this.lineLock)
			{
				this.Write(message);
				this.Flush();
			}
		}

		public override void Flush()
		{
			lock(this.lineLock)
			{
				// flush()...
				Flush(this.Line.ToString());

				// replace...
				_line = new StringBuilder();
			}
		}
		
		/// <summary>
		/// Gets the line.
		/// </summary>
		private StringBuilder Line
		{
			get
			{
				return _line;
			}
		}

		/// <summary>
		/// Flushes a line.
		/// </summary>
		private void Flush(string line)
		{
			if(line == null || line.Length == 0)
				return;
			this.OnLineWritten(new LineEventArgs(line));
		}

		/// <summary>
		/// Raises the <c>LineWritten</c> event.
		/// </summary>
		protected virtual void OnLineWritten(LineEventArgs e)
		{
			if(LineWritten != null)
				LineWritten(this, e);
		}
	}
}
