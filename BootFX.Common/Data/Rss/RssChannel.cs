// BootFX - Application framework for .NET applications
// 
// File: RssChannel.cs
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
    /// Defines an instance of <c>RssChannel</c>.
    /// </summary>
    public class RssChannel : RssElement
    {
        /// <summary>
        /// Private field to support <c>Items</c> property.
        /// </summary>
        private RssItemCollection _items = new RssItemCollection();

        public string ImageUrl { get; set; }

        public const string BfxPrefix = "bfx";
        public const string BfxNamespace = "https://bootfx.mbrit.com/rss";

        public RssChannel()
        {
        }

        public string Copyright
        {
            get
            {
                return this.GetStringValue("copyright", null, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this["copyright"] = value;
            }
        }

        public string Language
        {
            get
            {
                return this.GetStringValue("language", null, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this["language"] = value;
            }
        }

        public string Generator
        {
            get
            {
                return this.GetStringValue("generator", null, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this["generator"] = value;
            }
        }

        public int TtlMinutes
        {
            get
            {
                return this.GetInt32Value("ttl", 60, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this["ttl"] = value;
            }
        }

        /// <summary>
        /// Gets a collection of RssItem objects.
        /// </summary>
        public RssItemCollection Items
        {
            get
            {
                return _items;
            }
        }

        internal override void Populate(System.Xml.XmlElement element, BootFX.Common.Xml.XmlNamespaceManagerEx manager)
        {
            // do the values...
            base.Populate(element, manager);

            if (this.HasImageUrl)
            {
                var image = element.OwnerDocument.CreateElement("image");
                element.AppendChild(image);
                image.AddElement("url", this.ImageUrl);
                image.AddElement("title", this.Title);
            }

            // no do the items...
            foreach (RssItem item in this.Items)
            {
                // create...
                XmlElement itemElement = element.OwnerDocument.CreateElement("item");
                element.AppendChild(itemElement);
                item.Populate(itemElement, manager);
            }
        }

        public bool HasImageUrl
        {
            get
            {
                return !(string.IsNullOrEmpty(this.ImageUrl));
            }
        }
	}
}
