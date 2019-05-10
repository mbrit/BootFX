// BootFX - Application framework for .NET applications
// 
// File: ImageLibrary.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Reflection;
using BootFX.Common.Management;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Provides a library of images.
	/// </summary>
	public sealed class ImageLibrary
	{
		/// <summary>
		/// Defines the resource name for the icon.
		/// </summary>
		private const string MbrIconResourceName = "BootFX.Common.UI.Resources.Mbr.ico";

		/// <summary>
		/// Private field to support <c>Log</c> property.
		/// </summary>
		private static ILog _log = LogSet.GetLog(typeof(ImageLibrary));
		
		/// <summary>
		/// Constructor.
		/// </summary>
		private ImageLibrary()
		{
		}

		/// <summary>
		/// Gets the default icon for the application.
		/// </summary>
		/// <returns></returns>
		public static Icon GetDefaultIcon()
		{
			return SystemIcons.Application;
		}

		/// <summary>
		/// Gets the Iridium icon.
		/// </summary>
		/// <returns></returns>
		public static Icon GetMbrIcon()
		{
			Assembly asm = typeof(ImageLibrary).Assembly;
			Icon icon = GetIcon(asm, MbrIconResourceName, OnNotFound.ReturnNull);
			if(icon == null)
			{
				if(Log.IsWarnEnabled)
					Log.Warn(string.Format("Mbr icon not found in '{0}' at '{1}'.", asm, MbrIconResourceName));

				// set...
				icon = SystemIcons.Application;
			}

			// return...
			return icon;
		}

		/// <summary>
		/// Loads an icon from the given assembly.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="resourceName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static Icon GetIcon(Assembly asm, string resourceName, OnNotFound onNotFound)
		{
			if(asm == null)
				throw new ArgumentNullException("asm");
			if(resourceName == null)
				throw new ArgumentNullException("resourceName");
			if(resourceName.Length == 0)
				throw new ArgumentOutOfRangeException("'resourceName' is zero-length.");

			// open it...
			Stream stream = asm.GetManifestResourceStream(resourceName);
			if(stream != null)
				return new Icon(stream);
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format("Failed to find resource '{0}' in assembly '{1}'.", resourceName, asm));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}

		/// <summary>
		/// Loads an icon from the given assembly.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="resourceName"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static Image GetImage(Assembly asm, string resourceName, OnNotFound onNotFound)
		{
			if(asm == null)
				throw new ArgumentNullException("asm");
			if(resourceName == null)
				throw new ArgumentNullException("resourceName");
			if(resourceName.Length == 0)
				throw new ArgumentOutOfRangeException("'resourceName' is zero-length.");

			// open it...
			Stream stream = asm.GetManifestResourceStream(resourceName);
			if(stream != null)
				return Image.FromStream(stream);
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format("Failed to find resource '{0}' in assembly '{1}'.", resourceName, asm));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}

		/// <summary>
		/// Gets the log.
		/// </summary>
		private static ILog Log
		{
			get
			{
				// returns the value...
				return _log;
			}
		}
	}
}
