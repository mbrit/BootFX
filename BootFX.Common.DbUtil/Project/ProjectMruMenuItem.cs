// BootFX - Application framework for .NET applications
// 
// File: ProjectMruMenuItem.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for ProjectMruMenuItem.
	/// </summary>
	internal class ProjectMruMenuItem : MenuItem
	{
		/// <summary>
		/// Private field to support <see cref="Path"/> property.
		/// </summary>
		private string _path;
		
		/// <summary>
		/// Private field to support <c>Main</c> property.
		/// </summary>
		private Generator _main;
		
		internal ProjectMruMenuItem(Generator main, int index, string path)
		{
			if(main == null)
				throw new ArgumentNullException("main");
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// main...
			_path = path;
			_main = main;
			this.Text = string.Format("&{0} {1}", index + 1, path);
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick (e);

			// open...
			if(Main == null)
				throw new InvalidOperationException("Main is null.");
			this.Main.OpenProject(this.Path);
		}

		/// <summary>
		/// Gets the main.
		/// </summary>
		private Generator Main
		{
			get
			{
				// returns the value...
				return _main;
			}
		}

		/// <summary>
		/// Gets the path.
		/// </summary>
		private string Path
		{
			get
			{
				return _path;
			}
		}
	}
}
