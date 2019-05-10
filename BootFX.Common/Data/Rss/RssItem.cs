// BootFX - Application framework for .NET applications
// 
// File: RssItem.cs
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
using System.Collections.Specialized;
using System.Globalization;
using BootFX.Common;
using BootFX.Common.Xml;
using BootFX.Common.Data;
using System.Collections.Generic;
using System.Linq;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Describes an RSS item.
	/// </summary>
	public class RssItem : RssElement
	{
        public List<string> Tags { get; private set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		public RssItem()
		{
            this.Tags = new List<string>();
		}

		/// <summary>
		/// Gets or sets the url.
		/// </summary>
		public string Guid
		{
			get
			{
				return this.GetStringValue("guid", string.Empty, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("guid", value);

                if(string.IsNullOrEmpty(this.Link))
                    this.SetValue("link", value);
			}
		}
		
		/// <summary>
		/// Gets or sets the url.
		/// </summary>
		public string Category
		{
			get
			{
				return this.GetStringValue("category", string.Empty, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("category", value);
			}
		}

        //public string Link
        //{
        //    get
        //    {
        //        return this.GetStringValue("link", string.Empty, null, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        this.SetValue("link", value);
        //    }
        //}

        public string ResolvedLink
        {
            get
            {
                if (this.GuidIsPermalink || string.IsNullOrEmpty(this.Link))
                    return this.Guid;
                else
                    return this.Link;
            }
        }

		/// <summary>
		/// Gets or sets the url.
		/// </summary>
		public bool GuidIsPermalink
		{
			get
			{
				return this.GetBooleanValue("$GuidIsPermalink", false, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("$GuidIsPermalink", value);
			}
		}

        private DateTime RawPubDate
        {
            get
            {
                var dt = this.GetDateTimeValue("pubDate", DateTime.MinValue, null, OnNotFound.ReturnNull);
                if (dt == DateTime.MinValue)
                    dt = this.GetDateTimeValue("pubdate", DateTime.MinValue, null, OnNotFound.ReturnNull);

                return dt;
            }
        }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public DateTime PubDate
		{
			get
			{
                var dt = this.RawPubDate;
                if (dt != DateTime.MinValue)
                    return dt;
                else
                    return DateTime.Now;
			}
			set
			{
				this.SetValue("pubDate", value);
			}
		}

        public bool HasPubDate
        {
            get
            {
                return this.RawPubDate != DateTime.MinValue;
            }
        }

		public string PubDateAsString
		{
			get
			{
				return FormatDateTime(this.PubDate);
			}
			set
			{
				// Fri, 07 Jul 2006 06:54:13 GMT
				if(value != null && value.Length > 0)
				{
					DateTime date = ParseDateTime(value);
					this.PubDate = date;
				}
				else
					this.PubDate = DateTime.MinValue;
			}
		}

		/// <summary>
		/// Gets or sets the url.
		/// </summary>
		public int PubDateOffsetMinutes
		{
			get
			{
				return this.GetInt32Value("$PubDateOffsetMinutes", 0, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("$PubDateOffsetMinutes", value);
			}
		}

		/// <summary>
		/// Creates an item from the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static RssItem CreateItem(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");

			// check...
			if(obj is IRssItem)
				return ((IRssItem)obj).ToRssItem();
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "'{0}' does not support IRssItem.", obj.GetType()));
		}

		/// <summary>
		/// Creates an element for RSS 2.
		/// </summary>
		/// <param name="document"></param>
		/// <returns></returns>
		internal XmlElement GetRss2Element(XmlDocument document)
		{
			if(document == null)
				throw new ArgumentNullException("document");
			
			// create...
			XmlElement element = document.CreateElement("item");

			//			// walk the values...
			//			foreach(DictionaryEntry entry in this)
			//				XmlHelper.AddElement(element, Convert.ToString(entry.Key), ConversionHelper.ToString(entry.Value, Cultures.System));
			foreach(DictionaryEntry entry in this)
			{
				object currentValue = entry.Value;
				string currentValueAsString = FormatValue(currentValue);

				// add...
				XmlHelper.AddElement(element, Convert.ToString(entry.Key), currentValueAsString);
			}

			// return...
			return element;
		}

		internal override void Populate(XmlElement element, XmlNamespaceManagerEx manager)
		{
			base.Populate (element, manager);

			// find the guid...
			XmlElement guidElement = (XmlElement)element.SelectSingleNode("guid");
			if(guidElement != null)
				XmlHelper.SetAttributeValue(guidElement, "isPermaLink", this.GuidIsPermalink.ToString().ToLower());

            if (this.Tags.Any())
            {
                StringBuilder builder = new StringBuilder();
                var first = true;
                foreach (var tag in this.Tags)
                {
                    if (first)
                        first = false;
                    else
                        builder.Append(" ");
                    builder.Append(tag);
                }

                manager.AddNamespace(RssChannel.BfxPrefix, RssChannel.BfxNamespace, true);
                element.AddElement("tags", builder.ToString(), RssChannel.BfxPrefix, RssChannel.BfxNamespace);
            }
		}

        /// <summary>
        /// Gets the description as a plain-text value.
        /// </summary>
        public string DescriptionAsPlainText
        {
            get
            {
                return this.StripHtml(this.Description);
            }
        }

        private string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            // regex...
            Regex regex = new Regex(@"<(.|\n)*?>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            return regex.Replace(html, string.Empty);
        }

        internal override void PopulateValue(string name, string value)
        {
            if (name == "category" && this.Contains("category"))
                this.Tags.Add(value);
            else
                base.PopulateValue(name, value);
        }
	}
}
