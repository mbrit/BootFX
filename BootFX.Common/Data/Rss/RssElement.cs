// BootFX - Application framework for .NET applications
// 
// File: RssElement.cs
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
using System.Globalization;
using BootFX.Common.Xml;

namespace BootFX.Common.Data.Rss
{
	/// <summary>
	/// Defines an instance of <c>RssElement</c>.
	/// </summary>
	public abstract class RssElement : PropertyBag
	{
		/// <summary>
		/// Private field to support <c>DateTimeRegex</c> property.
		/// </summary>
		private static Regex _dateTimeRegex = new Regex(@"(?<weekday>[a-zA-Z]{3}),\s(?<day>[0-9]{2})\s(?<month>[a-zA-Z]{3})\s(?<year>[0-9]{4})\s(?<hour>[0-9]{2}):(?<minute>[0-9]{2}):(?<second>[0-9]{2})\s(?<timezone>[a-zA-Z]{3})", 
			RegexOptions.Singleline | RegexOptions.IgnoreCase);
		
		/// <summary>
		/// Defines the date format, used by BBC and Digg.  <c>Fri, 07 Jul 2006 06:54:13 GMT</c>.
		/// </summary>
		// ah - 19-07-2006 - the GMT is not added by default.		
		public const string DateFormat = "ddd, dd MMM yyyy HH:mm:ss"; 

		/// <summary>
		/// Constructor.
		/// </summary>
		internal RssElement()
		{
			this.CaseInsensitive = false;
		}

		/// <summary>
		/// Reads in leaf values from a reader.
		/// </summary>
		/// <param name="element"></param>
		internal void ReadLeafValues(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// walk...
			foreach(var walk in element.ChildNodes)
			{
                var child = walk as XmlElement;
                if (child != null)
                {
                    if (this is RssChannel && string.Compare(child.Name, "image", true) == 0)
                        ((RssChannel)this).ImageUrl = child.GetElementString("url");
                    else
                    {
                        // do we have just one child node, and is it some text?
                        if (child.ChildNodes.Count == 1 && (child.FirstChild.NodeType == XmlNodeType.Text || child.FirstChild.NodeType == XmlNodeType.CDATA))
                        {
                            if (child.NamespaceURI == null || child.NamespaceURI.Length == 0)
                            {
                                //this[child.Name] = child.InnerText;
                                this.PopulateValue(child.Name, child.InnerText);
                            }
                            else
                            {
                                //this[child.NamespaceURI + child.LocalName] = child.InnerText;
                                this.PopulateValue(child.NamespaceURI + child.LocalName, child.InnerText);
                            }
                        }
                    }
                }
			}
		}

        internal virtual void PopulateValue(string name, string value)
        {
            this[name] = value;
        }

		/// <summary>
		/// Populates an element.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="manager"></param>
		internal virtual void Populate(XmlElement element, XmlNamespaceManagerEx manager)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(manager == null)
				throw new ArgumentNullException("manager");
			
			// walk and add...
			foreach(string name in this.GetNames())
			{
                // if it starts with a dollar - it's special...
                if (string.Compare(name, "description", true, Cultures.System) == 0)
                {
                    XmlElement descElement = element.OwnerDocument.CreateElement("description");
                    element.AppendChild(descElement);

                    // text...
                    descElement.AppendChild(element.OwnerDocument.CreateCDataSection((string)this[name]));
                }
                else if (!(name.StartsWith("$")))
                {
                    // split it...
                    string namespaceUri = null;
                    string prefix = null;
                    string localName = null;
                    manager.SplitFullName(name, ref prefix, ref namespaceUri, ref localName);

                    // set...
                    XmlElement valueElement = element.OwnerDocument.CreateElement(prefix, localName, namespaceUri);
                    element.AppendChild(valueElement);

                    // set the element - don't use the dt namespace as RSS doesn't seem to use it.
                    this.SetElementValue(valueElement, this[name]);
                }
			}
		}

		protected string FormatValue(object value)
		{
			if(value == null || value is DBNull)
				return string.Empty;
			else if(value is DateTime)
				return FormatDateTime((DateTime)value);
			else
				return ConversionHelper.ToString(value, Cultures.System);
		}

		protected string FormatDateTime(DateTime value)
		{
			if(value != DateTime.MinValue)
				return value.ToString(DateFormat) + " GMT";
			else
				return null;
		}

		protected DateTime ParseDateTime(string value)
		{
			try
			{
				// regex - parse exact doesn't work.
				if(DateTimeRegex == null)
					throw new InvalidOperationException("DateTimeRegex is null.");
				Match match = DateTimeRegex.Match(value);
				if(match.Success)
				{
					// create...
					int day = ConversionHelper.ToInt32(match.Groups["day"].Value, Cultures.System);
					string monthAsString = match.Groups["month"].Value;
					int year = ConversionHelper.ToInt32(match.Groups["year"].Value, Cultures.System);
					int hour = ConversionHelper.ToInt32(match.Groups["hour"].Value, Cultures.System);
					int minute = ConversionHelper.ToInt32(match.Groups["minute"].Value, Cultures.System);
					int second = ConversionHelper.ToInt32(match.Groups["second"].Value, Cultures.System);

					// get...
					int month = 0;
					for(int index = 1; index <= 12; index++)
					{
						string name = DateTimeFormatInfo.InvariantInfo.GetAbbreviatedMonthName(index);
						if(string.Compare(name, monthAsString, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
						{
							month = index;
							break;
						}
					}

					// check...
					if(month == 0)
						throw new InvalidOperationException(string.Format("'{0}' is not a valid month name.", monthAsString));
					
					// create...
					return new DateTime(year, month, day, hour, minute, second);
				}
				else
					throw new InvalidOperationException("Regular expression matching failed when checking the date.");
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to convert '{0}' to a date/time.", value), ex);
			}
		}

		protected void SetElementValue(XmlElement element, object value)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// what is it?
			string asString = FormatValue(value);
			element.InnerText = asString;
		}

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		public string Title
		{
			get
			{
				return this.GetStringValue("title", string.Empty, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("title", value);
			}
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description
		{
			get
			{
				return this.GetStringValue("description", string.Empty, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("description", value);
			}
		}

		/// <summary>
		/// Gets or sets the url.
		/// </summary>
		public string Link
		{
			get
			{
				return this.GetStringValue("link", string.Empty, null, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("link", value);
			}
		}

		/// <summary>
		/// Gets the datetimeregex.
		/// </summary>
		private static Regex DateTimeRegex
		{
			get
			{
				// returns the value...
				return _dateTimeRegex;
			}
		}
	}
}
