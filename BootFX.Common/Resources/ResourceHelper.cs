// BootFX - Application framework for .NET applications
// 
// File: ResourceHelper.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Reflection;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for Images.
	/// </summary>
	public class ResourceHelper
	{
		/// <summary>
		/// Private constructor.
		/// </summary>
		private ResourceHelper()
		{
		}

		/// <summary>
		/// Opens a stream from a resource.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Stream GetResourceStream(Assembly assembly, string name)
		{
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			Stream stream = assembly.GetManifestResourceStream(name);
			if(stream != null)
				return stream;
			else
				throw CreateMissingResourceException(assembly, name);
		}

		private static Exception CreateMissingResourceException(Assembly asm, string name)
		{
			return new InvalidOperationException(string.Format("A resource with name '{0}' was not found in '{1}'.", name, asm));
		}

		// mbr - 06-07-2007 - added.
		/// <summary>
		/// Loads an XML document from the given stream.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static XmlDocument GetXmlDocument(string name)
		{
			return GetXmlDocument(Assembly.GetCallingAssembly(),name);
		}

		// mbr - 06-07-2007 - added.
		/// <summary>
		/// Loads an XML document from the given stream.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static XmlDocument GetXmlDocument(Assembly asm, string name)
		{
			// stream...
			Stream stream = GetResourceStream(asm, name);
			if(stream == null)
				throw new InvalidOperationException("stream is null.");

			// using...
			// mbr - 06-07-2007 - added using.			
			using(stream)
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(stream);

				// return...
				return doc;
			}
		}

		/// <summary>
		/// Gets a string from a resource.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetString(string name)
		{
			return GetString(Assembly.GetCallingAssembly(), name);
		}

		/// <summary>
		/// Gets a string from a resource.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetString(Assembly asm, string name)
		{
			if(asm == null)
				throw new ArgumentNullException("asm");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			Stream stream = GetResourceStream(asm, name);
			if(stream == null)
				throw new InvalidOperationException("stream is null.");
			using(stream)
			{
				StreamReader reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Copies the given resource to a temporary file.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>This method is designed to be used from unit tests.</remarks>
		public static string CopyResourceToTempFile(string name)
		{
			return CopyResourceToTempFile(Assembly.GetCallingAssembly(), name);
		} 

		/// <summary>
		/// Copies the given resource to a temporary file.
		/// </summary>
		/// <param name="asm"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		/// <remarks>This method is designed to be used from unit tests.</remarks>
		public static string CopyResourceToTempFile(Assembly asm, string name)
		{
			if(asm == null)
				throw new ArgumentNullException("asm");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// create...
			string extension = Path.GetExtension(name);
			if(extension == null || extension.Length == 0)
				extension = ".tmp";

			// get...
			Stream inStream = GetResourceStream(asm, name);
			if(inStream == null)
				throw new InvalidOperationException("'inStream' is null.");
			using(inStream)
			{
				// copy...
				string tempPath = Runtime.Current.GetTempFilePath(extension);
				StreamHelper.CopyStreamToFile(inStream, tempPath);
				
				// return...
				return tempPath;
			}
		}

        public static byte[] GetBytes(Assembly asm, string name)
        {
            using (var inStream = GetResourceStream(asm, name))
            {
                using (var outStream = new MemoryStream())
                {
                    inStream.CopyTo(outStream);
                    return outStream.ToArray();
                }
            }
        }

        public static IEnumerable<string> GetStringsFromCommentedFile(Assembly asm, string name)
        {
            var asString = GetString(asm, name);
            using (var reader = new StringReader(asString))
                return Runtime.Current.ReadCommentedFile(reader);
        }
	}
}
