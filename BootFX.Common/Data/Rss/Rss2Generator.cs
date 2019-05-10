// BootFX - Application framework for .NET applications
// 
// File: Rss2Generator.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using BootFX.Common.Xml;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines a generator that can generate RSS 2.0 documents.
	/// </summary>
	public class Rss2Generator : RssGenerator
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		[Obsolete("Use the constructor that takes no arguments.")]
		public Rss2Generator(string title, string mainUrl) : base(title, mainUrl)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public Rss2Generator()
		{
		}

		/// <summary>
		/// Generates an RSS document.
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public override System.Xml.XmlDocument Generate(RssChannel channel)
		{
			if(channel == null)
				throw new ArgumentNullException("channel");

			// create a document...
			XmlDocument document = new XmlDocument();

			// doc...
			XmlNamespaceManagerEx manager = XmlHelper.GetNamespaceManagerEx(document);
			if(manager == null)
				throw new InvalidOperationException("manager is null.");

            manager.AddNamespace(RssChannel.BfxPrefix, RssChannel.BfxNamespace, true);
			
			// doc...
			XmlElement rssElement = document.CreateElement("rss");
			document.AppendChild(rssElement);
			XmlHelper.SetAttributeValue(rssElement, "version", "2.0");

			// channel...
			XmlElement channelElement = document.CreateElement("channel");
			rssElement.AppendChild(channelElement);
			channel.Populate(channelElement, manager);

			// return...
			return document;
		}
	}
}
