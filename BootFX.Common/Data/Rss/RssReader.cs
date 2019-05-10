// BootFX - Application framework for .NET applications
// 
// File: RssReader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Xml;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines an instance of <c>RssReader</c>.
	/// </summary>
	public abstract class RssReader
	{
        protected RssReader()
		{
		}

		/// <summary>
		/// Gets a reader for the given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static RssReadResults ReadRss(string url, HttpDownloadSettings settings = null)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			
			// get...
			XmlDocument doc = HttpHelper.DownloadToXmlDocument(url, settings);
			if(doc == null)
				throw new InvalidOperationException("doc is null.");

			// defer...
			return ReadRss(doc);
		}

		/// <summary>
		/// Gets a reader for the given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static RssReadResults ReadRssFromFile(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				return ReadRss(stream);
		}

		/// <summary>
		/// Gets a reader for the given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static RssReadResults ReadRss(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
		
			// get the XML...
			XmlDocument doc = new XmlDocument();
			doc.Load(stream);

			// retrun...
			return ReadRss(doc);
		}

		/// <summary>
		/// Gets RSS items from the given document.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static RssReadResults ReadRss(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			
			// get....
			XmlElement element = (XmlElement)doc.SelectSingleNode("rss");
            if (element != null)
            {
                // get the version...
                RssReader reader = null;
                string version = XmlHelper.GetAttributeString(element, "version", OnNotFound.ThrowException);
                if (version == "2.0")
                    reader = new Rss2Reader();
                else
                    throw new NotSupportedException(string.Format("A version number of '{0}' is not supported.", version));

                // get it...
                if (reader == null)
                    throw new InvalidOperationException("reader is null.");

                var manager = new XmlNamespaceManagerEx(doc);
                manager.AddNamespace(RssChannel.BfxPrefix, RssChannel.BfxNamespace, true);
                return reader.Read(element, manager);
            }
            else
            {
                // mbr - 2009-01-07 - see if it is an atom feed...
                var manager = new XmlNamespaceManager(doc.NameTable);
                manager.AddNamespace("a", AtomReader.AtomNamespace);
                element = (XmlElement)doc.SelectSingleNode("a:feed", manager);
                if (element != null)
                {
                    var reader = new AtomReader();
                    return reader.Read(element, manager);
                }
                else
                    throw new NotImplementedException("This operation has not been implemented.");
            }
		}

		/// <summary>
		/// Reads the RSS from teh document.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		internal abstract RssReadResults Read(XmlElement element, XmlNamespaceManagerEx manager);
	}
}
