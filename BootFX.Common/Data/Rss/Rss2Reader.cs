// BootFX - Application framework for .NET applications
// 
// File: Rss2Reader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Xml;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines an instance of <c>Rss2Reader</c>.
	/// </summary>
	internal class Rss2Reader : RssReader
	{
		internal Rss2Reader()
		{
		}

		internal override RssReadResults Read(XmlElement element, XmlNamespaceManagerEx manager)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// channel element...
			XmlElement channelElement = (XmlElement)element.SelectSingleNode("channel");
			if(channelElement == null)
				throw new InvalidOperationException("channelElement is null.");

			// channel...
			RssChannel channel = new RssChannel();
			channel.ReadLeafValues(channelElement);

			// walk items...
			RssReadResults results = new RssReadResults(channel);
			foreach(XmlElement itemElement in channelElement.SelectNodes("item"))
			{
				// create...
				RssItem item = this.GetItem(itemElement, manager);
				if(item == null)
					throw new InvalidOperationException("item is null.");

				// add...
				channel.Items.Add(item);
			}

			// return...
			return results;
		}

		private RssItem GetItem(XmlElement element, XmlNamespaceManagerEx manager)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// create...
			RssItem item = new RssItem();

			// mbr - 17-08-2006 - read them in using the leaf reader...			
//			item.Title = XmlHelper.GetElementString(element, "title", OnNotFound.ThrowException);
//			item.Description = XmlHelper.GetElementString(element, "description", OnNotFound.ReturnNull);
//			item.Link = XmlHelper.GetElementString(element, "link", OnNotFound.ThrowException);
//			item.Category = XmlHelper.GetElementString(element, "category", OnNotFound.ReturnNull);
//			item.PubDateAsString = XmlHelper.GetElementString(element, "pubDate", OnNotFound.ReturnNull);
			item.ReadLeafValues(element);

			// guid...
			XmlElement guidElement = (XmlElement)element.SelectSingleNode("guid");
			if(guidElement != null)
			{
				item.Guid = guidElement.InnerText;
				item.GuidIsPermalink = XmlHelper.GetAttributeBoolean(guidElement, "isPermaLink", OnNotFound.ReturnNull);
			}

            var tags = (XmlElement)element.SelectSingleNode(RssChannel.BfxPrefix + ":tags", manager.Manager);
            if (tags != null)
            {
                var asString = tags.InnerText;
                var parts = asString.Split(' ');
                foreach (var part in parts)
                {
                    var usePart = part.Trim();
                    if (usePart.Length > 0)
                        item.Tags.Add(usePart);
                }
            }

			// return...
			return item;
		}
	}
}
