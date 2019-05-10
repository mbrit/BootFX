// BootFX - Application framework for .NET applications
// 
// File: MailMessageRecipient.cs
// Build: 5.0.61009.900
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
using BootFX.Common.Xml;
using Nm = System.Net.Mail;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Summary description for MailMessageRecipient.
	/// </summary>
	public class MailMessageRecipient
	{
		/// <summary>
		/// Private field to support <see cref="Email"/> property.
		/// </summary>
		private string _email;
		
		/// <summary>
		/// Private field to support <see cref="DisplayName"/> property.
		/// </summary>
		private string _displayName;

        /// <summary>
        /// Private value to support the <see cref="QuotedNameRegex">QuotedNameRegex</see> property.
        /// </summary>
        private static Regex _quotedNameRegex = new Regex(@"\""(?<name>.*)\""\s*\<(?<email>.*)\>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        /// <summary>
		/// Constructor for deserialization.
		/// </summary>
		// mbr - 16-11-2007 - added.		
		private MailMessageRecipient()
		{
		}
		
		public MailMessageRecipient(string email)
		{
			if(email == null)
				throw new ArgumentNullException("email");
			if(email.Length == 0)
				throw new ArgumentOutOfRangeException("'email' is zero-length.");
			_email = email;
		}

		public MailMessageRecipient(string displayName, string email) 
			: this(email)
		{
			if(displayName == null)
				throw new ArgumentNullException("displayName");
			if(displayName.Length == 0)
				throw new ArgumentOutOfRangeException("'displayName' is zero-length.");
			_displayName = displayName;
		}

		/// <summary>
		/// Gets the displayname.
		/// </summary>
		public string DisplayName
		{
			get
			{
				return _displayName;
			}
		}

		/// <summary>
		/// Gets the email.
		/// </summary>
		public string Email
		{
			get
			{
				return _email;
			}
		}

		public override string ToString()
		{
			if(this.HasDisplayName)
				return string.Format("\"{0}\" <{1}>", this.DisplayName, this.Email);
			else
				return this.Email;
		}

		public bool HasDisplayName
		{
			get
			{
				if(this.DisplayName != null && this.DisplayName.Length > 0)
				{
					if(this.DisplayName != this.Email)
						return true;
					else
						return false;
				}
				else
					return false;
			}
		}

		public static MailMessageRecipient Parse(string value)
		{
			if(value == null)
				return null;
			value = value.Trim();
			if(value.Length == 0)
				return null;

            try
            {
                // return it...
                Match match = QuotedNameRegex.Match(value);
                if (match.Success)
                    return new MailMessageRecipient(match.Groups["name"].Value, match.Groups["email"].Value);
                else
                    return new MailMessageRecipient(value);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to parse '{0}'.", value), ex);
            }
		}

		/// <summary>
		/// Populates an XML element with this recipient.
		/// </summary>
		/// <param name="element"></param>
		// mbr - 16-11-2007 - added.		
		// mbr - 16-11-2007 - added.		
		internal void PopulateXmlElement(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");

			// set...
			XmlHelper.AddElement(element, "DisplayName", this.DisplayName);
			XmlHelper.AddElement(element, "Email", this.Email);
		}

		/// <summary>
		/// Loads a recipient from an XML element.
		/// </summary>
		/// <param name="element"></param>
		// mbr - 16-11-2007 - added.		
		internal static MailMessageRecipient FromXmlElement(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// create...
			MailMessageRecipient recipient = new MailMessageRecipient();

			// name...
			string displayName = XmlHelper.GetElementString(element, "DisplayName", OnNotFound.ReturnNull);
			if(displayName != null && displayName.Length == 0)
				displayName = null;
			recipient._displayName = displayName;

			// email...
			recipient._email = XmlHelper.GetElementString(element, "Email", OnNotFound.ReturnNull);

			// return...
			return recipient;
		}

		internal Nm.MailAddress ToMailAddress()
		{
            try
            {
                if (this.HasDisplayName)
                    return new Nm.MailAddress(this.Email, this.DisplayName);
                else
                    return new Nm.MailAddress(this.Email);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to parse email address.\r\nEmail: {0}\r\nName: {1}", this.Email, this.DisplayName), ex);
            }
		}

        /// <summary>
        /// Gets the QuotedNameRegex value.
        /// </summary>
        private static Regex QuotedNameRegex
        {
            get
            {
                return _quotedNameRegex;
            }
        }
	}
}
