// BootFX - Application framework for .NET applications
// 
// File: ImageListEx.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Defines an instance of <c>ImageListEx</c>.
	/// </summary>
	public class ImageListEx : IDisposable
	{
		/// <summary>
		/// Private field to support <c>AssemblyLookup</c> property.
		/// </summary>
		private Lookup _assemblyLookup;
		
		/// <summary>
		/// Private field to support <see cref="ImageList"/> property.
		/// </summary>
		private ImageList _innerImageList;
		
		public ImageListEx()
		{
			// create the inner list...
			_innerImageList = new ImageList();
			this.InnerImageList.ColorDepth = ColorDepth.Depth32Bit;

			// create the top-level lookup...
			_assemblyLookup = new Lookup();
			_assemblyLookup.CreateItemValue += new CreateLookupItemEventHandler(_assemblyLookup_CreateItemValue);
		}

		public ImageListEx(Size size) : this()
		{		
			this.InnerImageList.ImageSize = size;
		}

		public ImageListEx(IconSize size) : this(IconSizeToSize(size))
		{
		}

		/// <summary>
		/// Gets the imagelist.
		/// </summary>
		public ImageList InnerImageList
		{
			get
			{
				return _innerImageList;
			}
		}

		internal static Size IconSizeToSize(IconSize size)
		{
			return new Size((int)size, (int)size);
		}

		/// <summary>
		/// Gets an image from the given resource.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		public int GetImageIndex(Assembly asm, string resourceName)
		{
			if(resourceName == null)
				throw new ArgumentNullException("resourceName");
			if(resourceName.Length == 0)
				throw new ArgumentOutOfRangeException("'resourceName' is zero-length.");

			// check...
			if(asm == null)
			{
				asm = DefaultAssembly;
				if(asm == null)
					throw new InvalidOperationException("asm is null.");
			}

			// do we have it?
			ImagesForAssemblyLookup lookup = (ImagesForAssemblyLookup)AssemblyLookup[asm];
			if(lookup == null)
				throw new InvalidOperationException("lookup is null.");

			// get...
			return (int)lookup[resourceName];
	}

		/// <summary>
		/// Gets the assemblylookup.
		/// </summary>
		private Lookup AssemblyLookup
		{
			get
			{
				// returns the value...
				return _assemblyLookup;
			}
		}

		private void _assemblyLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			ImagesForAssemblyLookup newLookup = new ImagesForAssemblyLookup((Assembly)e.Key);
			newLookup.CreateItemValue += new CreateLookupItemEventHandler(newLookup_CreateItemValue);

			// return...
			e.NewValue = newLookup;
		}

		/// <summary>
		/// Defines an instance of <c>ImagesForAssemblyLookup</c>.
		/// </summary>
		private class ImagesForAssemblyLookup : Lookup
		{
			/// <summary>
			/// Private field to support <see cref="Assembly"/> property.
			/// </summary>
			private Assembly _assembly;
			
			internal ImagesForAssemblyLookup(Assembly asm) : base(false)
			{
				if(asm == null)
					throw new ArgumentNullException("asm");
				_assembly = asm;
			}

			/// <summary>
			/// Gets the assembly.
			/// </summary>
			internal Assembly Assembly
			{
				get
				{
					return _assembly;
				}
			}
		}

		private Assembly DefaultAssembly
		{
			get
			{
				return Assembly.GetEntryAssembly();
			}
		}

		private void newLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// get...
			Assembly asm = ((ImagesForAssemblyLookup)sender).Assembly;
			if(asm == null)
				throw new InvalidOperationException("asm is null.");

			// get...
			Image image = DesktopResourceHelper.GetImage(asm, (string)e.Key);
			if(image == null)
				throw new InvalidOperationException("image is null.");
			e.NewValue = this.InnerImageList.Images.Add(image, this.InnerImageList.TransparentColor);
		}

		public void Dispose()
		{
			if(_innerImageList != null)
			{
				_innerImageList.Dispose();
				_innerImageList = null;
			}

			// sup...
			GC.SuppressFinalize(this);
		}
	}
}
