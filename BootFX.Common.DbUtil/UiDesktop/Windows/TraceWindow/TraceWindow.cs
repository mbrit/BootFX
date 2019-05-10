// BootFX - Application framework for .NET applications
// 
// File: TraceWindow.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	///	 Defines a form that contains a window for use with <see cref="Trace"></see>.
	/// </summary>
	public class TraceWindow : System.Windows.Forms.Form
	{
		/// <summary>
		/// Delegate for <c>AddToLog</c>.
		/// </summary>
		private delegate void AddToLogDelegate(string line);

		/// <summary>
		/// Private field to support <c>Listener</c> property.
		/// </summary>
		private TraceWindowListener _listener;
		
		private System.Windows.Forms.RichTextBox textTrace;
		private System.Windows.Forms.ContextMenu menuContext;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuLogTestMessage;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TraceWindow()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			this.TopMost = true;
			this.StartPosition = FormStartPosition.Manual;
			this.Location = new Point(System.Windows.Forms.SystemInformation.WorkingArea.Left, 
					System.Windows.Forms.SystemInformation.WorkingArea.Top);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textTrace = new System.Windows.Forms.RichTextBox();
			this.menuContext = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuLogTestMessage = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// textTrace
			// 
			this.textTrace.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textTrace.ContextMenu = this.menuContext;
			this.textTrace.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textTrace.Font = new System.Drawing.Font("Courier New", 8F);
			this.textTrace.Location = new System.Drawing.Point(0, 0);
			this.textTrace.Name = "textTrace";
			this.textTrace.ReadOnly = true;
			this.textTrace.Size = new System.Drawing.Size(632, 170);
			this.textTrace.TabIndex = 0;
			this.textTrace.Text = "";
			this.textTrace.WordWrap = false;
			// 
			// menuContext
			// 
			this.menuContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItem1,
																						this.menuItem2,
																						this.menuLogTestMessage});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "&Clear";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
			// 
			// menuLogTestMessage
			// 
			this.menuLogTestMessage.Index = 2;
			this.menuLogTestMessage.Text = "&Log Test Message";
			this.menuLogTestMessage.Click += new System.EventHandler(this.menuLogTestMessage_Click);
			// 
			// TraceWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(632, 170);
			this.Controls.Add(this.textTrace);
			this.MaximizeBox = false;
			this.Name = "TraceWindow";
			this.Text = "Trace";
			this.ResumeLayout(false);

		}
		#endregion

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated (e);

			// listen...
			Trace.Listeners.Add(this.Listener);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed (e);

			// dispose...
			DisposeListener();
		}

		/// <summary>
		/// Disposes the listener.
		/// </summary>
		private void DisposeListener()
		{
			if(_listener != null)
			{
				_listener.Dispose();
				_listener = null;
			}
		}

		/// <summary>
		/// Gets the listener.
		/// </summary>
		private TraceWindowListener Listener
		{
			get
			{
				if(_listener == null)
				{
					_listener = new TraceWindowListener();
					this.Listener.LineWritten += new LineEventHandler(Listener_LineWritten);
				}
				return _listener;
			}
		}

		private void Listener_LineWritten(object sender, LineEventArgs e)
		{
			AddToLog(e.Line);
		}

		/// <summary>
		/// Adds a line to the log.
		/// </summary>
		/// <param name="line"></param>
		private void AddToLog(string line)
		{
			if(this.InvokeRequired == true)
			{
				this.BeginInvoke(new AddToLogDelegate(AddToLog), new object[] { line });
				return;
			}

			// trim...
			if(line == null)
				return;

			// trim the crlf...
			while(line.EndsWith("\r\n") == true)
				line = line.Substring(0, line.Length - 2);
			if(line.Length == 0)
				return;

			// colorize the line...
			line = this.ColorizeLine(line);

			// add...
			string newLine = string.Format(Cultures.System, "{0}\r\n{1}", line, this.textTrace.Text);
			this.textTrace.Text = newLine;
		}

		/// <summary>
		/// Colorizes the given line.
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		private string ColorizeLine(string line)
		{
			return line;
		}

		private void menuItem1_Click(object sender, System.EventArgs e)
		{
			this.textTrace.Clear();
		}

		private void menuLogTestMessage_Click(object sender, System.EventArgs e)
		{
			ILog log = LogSet.GetLog(this.GetType());
			log.Debug("I am a debug message");
			log.Info("I am a info message");
			log.Warn("I am a warning message");
			log.Error("I am a error message");
			log.Fatal("I am a fatal message");

			// tell...
			Alert.ShowInformation(this, "Messages have been written to all categories.");
		}
	}
}
