// BootFX - Application framework for .NET applications
// 
// File: MailMessage.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Xml;
using BootFX.Common.Xml;
using System.Collections.Generic;
using System.IO;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Summary description for MailMessage.
	/// </summary>
	// mbr - 16-11-2007 - added XML serialization...	
	public class MailMessage : IMailMessage
	{
		/// <summary>
		/// Private field to support <c>Attachments</c> property.
		/// </summary>
		private MailAttachmentCollection _attachments = new MailAttachmentCollection();
		
		/// <summary>
		/// Private field to support <c>Date</c> property.
		/// </summary>
		private DateTime _date;
		
		/// <summary>
		/// Private field to support <c>Headers</c> property.
		/// </summary>
		private string _headers;
		
		/// <summary>
		/// Private field to support <c>From</c> property.
		/// </summary>
		private MailMessageRecipient _from;

        private MailMessageRecipient _replyTo;

        /// <summary>
        /// Private field to support <c>To</c> property.
        /// </summary>
        private MailMessageRecipientCollection _to = new MailMessageRecipientCollection();
		
		/// <summary>
		/// Private field to support <c>Cc</c> property.
		/// </summary>
		private MailMessageRecipientCollection _cc = new MailMessageRecipientCollection();

		/// <summary>
		/// Private field to support <c>Bcc</c> property.
		/// </summary>
		private MailMessageRecipientCollection _bcc = new MailMessageRecipientCollection();
		
		/// <summary>
		/// Private field to support <c>MailFormat</c> property.
		/// </summary>
		private MailFormat _mailFormat = MailFormat.PlainText;
		
		/// <summary>
		/// Private field to support <c>Body</c> property.
		/// </summary>
		private string _body;
		
		/// <summary>
		/// Private field to support <c>Subject</c> property.
		/// </summary>
		private string _subject;

        public List<MailEmbeddedImage> EmbeddedImages { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MailMessage()
		{
            this.EmbeddedImages = new List<MailEmbeddedImage>();
        }

		/// <summary>
		/// Gets or sets the subject
		/// </summary>
		public string Subject
		{
			get
			{
				return _subject;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _subject)
				{
					// set the value...
					_subject = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the bcc
		/// </summary>
		public string BccAsString
		{
			get
			{
				return this.Bcc.ToString();
			}
			set
			{
				this.Bcc.Clear();
				this.Bcc.AddRange(value);
			}
		}
		
		/// <summary>
		/// Gets or sets the Cc
		/// </summary>
		public string CcAsString
		{
			get
			{
				return this.Cc.ToString();
			}
			set
			{
				this.Cc.Clear();
				this.Cc.AddRange(value);
			}
		}
		
		/// <summary>
		/// Gets or sets the To
		/// </summary>
		public string ToAsString
		{
			get
			{
				return this.To.ToString();
			}
			set
			{
				this.To.Clear();
				this.To.AddRange(value);
			}
		}
		
		/// <summary>
		/// Gets or sets the from
		/// </summary>
		public string FromAsString
		{
			get
			{
				if(this.From != null)
					return this.From.ToString();
				else
					return null;
			}
			set
			{
				_from = MailMessageRecipient.Parse(value);
			}
		}

        public string ReplyToAsString
        {
            get
            {
                if (this.ReplyTo != null)
                    return this.ReplyTo.ToString();
                else
                    return null;
            }
            set
            {
                _replyTo = MailMessageRecipient.Parse(value);
            }
        }

        /// <summary>
        /// Gets or sets the body
        /// </summary>
        public string Body
		{
			get
			{
				return _body;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _body)
				{
					// set the value...
					_body = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the mailformat
		/// </summary>
		public MailFormat MailFormat
		{
			get
			{
				return _mailFormat;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _mailFormat)
				{
					// set the value...
					_mailFormat = value;
				}
			}
		}

		/// <summary>
		/// Gets a collection of MailMessageRecipient objects.
		/// </summary>
		public MailMessageRecipientCollection Bcc
		{
			get
			{
				return _bcc;
			}
		}

		/// <summary>
		/// Gets a collection of MailMessageRecipient objects.
		/// </summary>
		public MailMessageRecipientCollection Cc
		{
			get
			{
				return _cc;
			}
		}

		/// <summary>
		/// Gets a collection of MailMessageRecipient objects.
		/// </summary>
		public MailMessageRecipientCollection To
		{
			get
			{
				return _to;
			}
		}
		
		/// <summary>
		/// Gets or sets the from
		/// </summary>
		public MailMessageRecipient From
		{
			get
			{
				return _from;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _from)
				{
					// set the value...
					_from = value;
				}
			}
		}

        public MailMessageRecipient ReplyTo
        {
            get
            {
                return _replyTo;
            }
            set
            {
                // check to see if the value has changed...
                if (value != _replyTo)
                {
                    // set the value...
                    _replyTo = value;
                }
            }
        }

        /// <summary>
        /// Gets a list of all recipients.
        /// </summary>
        /// <returns></returns>
        public MailMessageRecipientCollection GetAllRecipients()
		{
			MailMessageRecipientCollection results = new MailMessageRecipientCollection();
			results.AddRange(this.To);
			results.AddRange(this.Cc);
			results.AddRange(this.Bcc);

			// return...
			return results;
		}

		/// <summary>
		/// Gets or sets the headers
		/// </summary>
		public string Headers
		{
			get
			{
				return _headers;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _headers)
				{
					// set the value...
					_headers = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the date
		/// </summary>
		public DateTime Date
		{
			get
			{
				return _date;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _date)
				{
					// set the value...
					_date = value;
				}
			}
		}

		/// <summary>
		/// Gets a collection of MailAttachment objects.
		/// </summary>
		public MailAttachmentCollection Attachments
		{
			get
			{
				return _attachments;
			}
		}

		/// <summary>
		/// Gets the mail message as an XML document.
		/// </summary>
		/// <returns></returns>
		public XmlDocument ToXmlDocument()
		{
			// create...
			XmlDocument doc = new XmlDocument();

			// root...
			XmlElement root = doc.CreateElement("MailMessage");
			doc.AppendChild(root);

			// from...
			if(From == null)
				throw new InvalidOperationException("From is null.");
			XmlElement fromElement = doc.CreateElement("From");
			root.AppendChild(fromElement);
			this.From.PopulateXmlElement(fromElement);

			// recipients...
			this.AddRecipientsToXml(root, "To", this.To);
			this.AddRecipientsToXml(root, "Cc", this.Cc);
			this.AddRecipientsToXml(root, "Bcc", this.Bcc);

            if (this.ReplyTo != null)
            {
                var replyToElement = doc.CreateElement("ReplyTo");
                root.AppendChild(replyToElement);
                this.ReplyTo.PopulateXmlElement(replyToElement);
            }

			// basic text stuff...
			XmlHelper.AddElement(root, "Subject", this.Subject);
			XmlHelper.AddElement(root, "Headers", this.Headers);
			XmlHelper.AddElement(root, "MailFormat", this.MailFormat);
			XmlHelper.AddElement(root, "Date", this.Date);

			// body...
			XmlElement bodyElement = doc.CreateElement("Body");
			root.AppendChild(bodyElement);
            bodyElement.AppendChild(doc.CreateCDataSection(this.Body));	

            // attachments...
            XmlElement attachmentsElement = doc.CreateElement("Attachments");
            root.AppendChild(attachmentsElement);
            foreach (MailAttachment attachment in this.Attachments)
            {
                XmlElement attachmentElement = doc.CreateElement("Attachment");
                attachmentsElement.AppendChild(attachmentElement);
                attachment.PopulateXmlElement(attachmentElement);
            }

            var embedsElement = root.AddElement("EmbeddedImages");
            foreach (MailEmbeddedImage embed in this.EmbeddedImages)
            {
                var embedElement = embedsElement.AddElement("EmbeddedImage");
                embedElement.AddElement("Name", embed.Name);
                embedElement.AddElement("Data", embed.GetBytesAsBase64String());
                embedElement.AddElement("ContentType", embed.ContentType);
                embedElement.AddElement("ContentId", embed.ContentId);
            }

            // return...
            return doc;
		}

		/// <summary>
		/// Adds the recipients to XML.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="name"></param>
		/// <param name="recipients"></param>
		// mbr - 16-11-2007 - added.		
		private void AddRecipientsToXml(XmlElement element, string name, MailMessageRecipientCollection recipients)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// create...
			XmlElement subElement = element.OwnerDocument.CreateElement(name);
			element.AppendChild(subElement);

			// walk...
			foreach(MailMessageRecipient recipient in recipients)
			{
				XmlElement recipientElement = element.OwnerDocument.CreateElement("Recipient");
				subElement.AppendChild(recipientElement);

				// populate....
				recipient.PopulateXmlElement(recipientElement);
			}
		}

		/// <summary>
		/// Gets a mail message from a document.
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		// mbr - 16-11-2007 - added.		
		public static MailMessage FromXmlDocument(XmlDocument doc)
		{
			if(doc == null)
				throw new ArgumentNullException("doc");
			
			// get...
			XmlElement element = (XmlElement)doc.SelectSingleNode("MailMessage");
			if(element == null)
				throw new InvalidOperationException("element is null.");

			// get...
			MailMessage message = new MailMessage();
			message.LoadFromXmlElement(element);

			// return...
			return message;
		}

		/// <summary>
		/// Loads data from an XML element.
		/// </summary>
		/// <param name="element"></param>
		// mbr - 16-11-2007 - added.		
		private void LoadFromXmlElement(XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			
			// from...
			XmlElement fromElement = (XmlElement)element.SelectSingleNode("From");
			if(fromElement == null)
				throw new InvalidOperationException("fromElement is null.");
			this.From = MailMessageRecipient.FromXmlElement(fromElement);

			// recipients...
			this.LoadRecipients(element, "To", this.To);
			this.LoadRecipients(element, "Cc", this.Cc);
			this.LoadRecipients(element, "Bcc", this.Bcc);

            var replyToElement = (XmlElement)element.SelectSingleNode("ReplyTo");
            if (replyToElement != null)
                this.ReplyTo = MailMessageRecipient.FromXmlElement(replyToElement);

			// basics...
			this.Subject = XmlHelper.GetElementString(element, "Subject", OnNotFound.ReturnNull);
			this.Headers = XmlHelper.GetElementString(element, "Headers", OnNotFound.ReturnNull);
			this.MailFormat = (MailFormat)XmlHelper.GetElementEnumerationValue(element, "MailFormat", typeof(MailFormat), OnNotFound.ReturnNull);
			this.Date = XmlHelper.GetElementDateTime(element, "Date", OnNotFound.ReturnNull);
			this.Body = XmlHelper.GetElementString(element, "Body", OnNotFound.ReturnNull);

            // attachments...
            foreach (XmlElement attachmentElement in element.SelectNodes("Attachments/Attachment"))
            {
                MailAttachment attachment = MailAttachment.FromXmlElement(attachmentElement);
                this.Attachments.Add(attachment);
            }

            // embeds...
            foreach (XmlElement embedElement in element.SelectNodes("EmbeddedImages/EmbeddedImage"))
            {
                var name = embedElement.GetElementString("Name");
                var asString = embedElement.GetElementString("Data");
                var contentType = embedElement.GetElementString("ContentType");
                var contentId = embedElement.GetElementString("ContentId");

                var ext = Path.GetExtension(name);
                var temp = Runtime.Current.GetTempFilePath(ext);
                var bs = Convert.FromBase64String(asString);
                File.WriteAllBytes(temp, bs);

                this.EmbeddedImages.Add(new MailEmbeddedImage(name, temp, contentType, contentId, true));
            }
		}

		/// <summary>
		/// Loads a list of recipients.
		/// </summary>
		/// <param name="element"></param>
		/// <param name="name"></param>
		private void LoadRecipients(XmlElement element, string name, MailMessageRecipientCollection recipients)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(recipients == null)
				throw new ArgumentNullException("recipients");
			
			// clear...
			recipients.Clear();

			// get...
			foreach(XmlElement recipientElement in element.SelectNodes(name + "/Recipient"))
				recipients.Add(MailMessageRecipient.FromXmlElement(recipientElement));
		}
	}
}
