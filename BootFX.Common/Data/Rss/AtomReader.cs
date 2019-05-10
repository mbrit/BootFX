// BootFX - Application framework for .NET applications
// 
// File: AtomReader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BootFX.Common.Data.Rss
{
    internal class AtomReader
    {
        internal const string AtomNamespace = "http://www.w3.org/2005/Atom";

        internal RssReadResults Read(XmlElement element, XmlNamespaceManager manager)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            // channel...
            RssChannel channel = new RssChannel();

            // walk items...
            RssReadResults results = new RssReadResults(channel);
            foreach (XmlElement itemElement in element.SelectNodes("a:entry", manager))
            {
                // create...
                var item = new RssItem()
                {
                    Title = itemElement.SelectSingleNode("a:title", manager).InnerText,
                    Guid = itemElement.SelectSingleNode("a:link", manager).Attributes["href"].InnerText,
                    Description = itemElement.SelectSingleNode("a:content", manager).InnerText,
                    PubDate = DateTime.Parse(itemElement.SelectSingleNode("a:published", manager).InnerText).ToLocalTime(),
                    GuidIsPermalink = true
                };

                // add...
                channel.Items.Add(item);
            }

            // return...
            return results;
        }
    }
}
