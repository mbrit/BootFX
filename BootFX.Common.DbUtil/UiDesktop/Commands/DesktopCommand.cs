// BootFX - Application framework for .NET applications
// 
// File: DesktopCommand.cs
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
using System.Drawing;
using System.Reflection;
using System.Collections;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>DesktopCommand</c>.
	/// </summary>
	public class DesktopCommand : Command
	{
		/// <summary>
		/// Private field to support <see cref="Image"/> property.
		/// </summary>
		private Image _image;
		
		/// <summary>
		/// Private field to support <c>ImageResourceAssembly</c> property.
		/// </summary>
		private Assembly _imageResourceAssembly;

		/// <summary>
		/// Private field to support <c>ImageResourceName</c> property.
		/// </summary>
		private string _imageResourceName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public DesktopCommand(string fullName) : base(fullName)
		{			
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DesktopCommand(string fullName, CommandFlags flags) : base(fullName, flags)
		{
		}

		/// <summary>
		/// Gets or sets the imageresourcename
		/// </summary>
		public string ImageResourceName
		{
			get
			{
				return _imageResourceName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageResourceName)
				{
					// set the value...
					_imageResourceName = value;
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the imageresourceassembly
		/// </summary>
		public Assembly ImageResourceAssembly
		{
			get
			{
				return _imageResourceAssembly;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _imageResourceAssembly)
				{
					// set the value...
					_imageResourceAssembly = value;
				}
			}
		}

		internal bool HasImage
		{
			get
			{
				if(this.ImageResourceName != null && this.ImageResourceName.Length > 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the image.
		/// </summary>
		internal Image Image
		{
			get
			{
				// gets the image...
				if(_image == null && this.HasImage)
				{
					_image = DesktopResourceHelper.GetImage(this.ImageResourceAssembly, this.ImageResourceName);
					if(_image == null)
						throw new InvalidOperationException("_image is null.");
				}
				return _image;
			}
		}
	}
}
