// BootFX - Application framework for .NET applications
// 
// File: NetSmtpMailProvider.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using Nm = System.Net.Mail;

namespace BootFX.Common.Email
{
    /// <summary>
    /// Summary description for WebSmtpMailWrapper.
    /// </summary>
    public class NetSmtpMailProvider : ISmtpProvider
    {
        /// <summary>
        /// Private field to support <c>Settings</c> property.
        /// </summary>
        private SmtpConnectionSettings _settings;

        /// <summary>
        /// Constructor.
        /// </summary>
        public NetSmtpMailProvider(SmtpConnectionSettings settings)
        {
            this.Settings = settings;
        }

        /// <summary>
        /// Sends the message.
        /// </summary>
        /// <param name="message"></param>
        public void Send(IMailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            // mbr - 11-11-2005 - check...
            if (Settings == null)
                throw new InvalidOperationException("Settings is null.");

            // create one...
            Nm.MailMessage mailMessage = new Nm.MailMessage();
            mailMessage.From = message.From.ToMailAddress();
            this.CopyAddresses(mailMessage.To, message.To);
            this.CopyAddresses(mailMessage.CC, message.Cc);
            this.CopyAddresses(mailMessage.Bcc, message.Bcc);
            mailMessage.Subject = message.Subject;

            if (message.ReplyTo != null)
                mailMessage.ReplyToList.Add(message.ReplyTo.ToMailAddress());

            // mbr - 2007-03-14 - attachments
            foreach (MailAttachment attachment in message.Attachments)
            {
                Nm.Attachment toAdd = new Nm.Attachment(attachment.FilePath);
                toAdd.ContentDisposition.FileName = attachment.FileName;

                // add...
                mailMessage.Attachments.Add(toAdd);
            }

            // do we have any embeds?
            var embedsToDispose = new List<MailEmbeddedImage>();
            if (!(message.EmbeddedImages.Any()))
            {
                mailMessage.Body = message.Body;

                // format...
                switch (message.MailFormat)
                {
                    case MailFormat.Html:
                        mailMessage.IsBodyHtml = true;
                        break;

                    case MailFormat.PlainText:
                        mailMessage.IsBodyHtml = false;
                        break;

                    default:
                        throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", message.MailFormat, message.MailFormat.GetType()));
                }
            }
            else
            {
                var av = AlternateView.CreateAlternateViewFromString(message.Body, null, MimeTypes.Html);
                mailMessage.AlternateViews.Add(av);

                foreach (var embed in message.EmbeddedImages)
                {
                    var ct = new ContentType()
                    {
                        MediaType = embed.ContentType,
                        Name = embed.Name
                    };
                    var inline = new LinkedResource(embed.Path, ct)
                    {
                        ContentId = embed.ContentId
                    };
                    av.LinkedResources.Add(inline);

                    if (!(embed.IsPersistent))
                        embedsToDispose.Add(embed);
                }
            }

            try
            {
                // create a client...
                Nm.SmtpClient client = new Nm.SmtpClient();
                client.Host = this.Settings.HostName;

                // mbr - 2009-02-28 - jm discovered a problem...
                if (this.Settings.UseIisDeliveryMethod)
                    client.DeliveryMethod = Nm.SmtpDeliveryMethod.PickupDirectoryFromIis;

                // auth...
                bool didAuth = false;
                if (this.Settings.Username != null && this.Settings.Username.Length > 0)
                {
                    client.Credentials = new NetworkCredential(this.Settings.Username, this.Settings.Password);
                    didAuth = true;
                }

                // port?
                if (this.Settings.Port != 25)
                    client.Port = this.Settings.Port;

                if (this.Settings.EnableTls)
                    client.EnableSsl = true;

                // mbr - 11-11-2005 - check...
                if (Settings.HostName == null)
                    throw new InvalidOperationException("'Settings.HostName' is null.");
                if (Settings.HostName.Length == 0) 
                    throw new InvalidOperationException("'Settings.HostName' is zero-length.");

                // send it...
                try
                {
                    //Nm.SmtpMail.Send(mailMessage);
                    client.Send(mailMessage);
                }
                catch (Exception ex)
                {
                    string auth = null;
                    if (didAuth)
                        auth = string.Format("(Authenticating as '{0}'.)", this.Settings.Username);
                    else
                        auth = "(No authentication.)";
                    throw new InvalidOperationException(string.Format("Failed to send mail to server '{0}'.  {1}", client.Host, auth), ex);
                }
            }
            finally
            {
                foreach (var walk in embedsToDispose)
                    walk.DisposeImageIfNotPersistent();
            }
        }

        private void CopyAddresses(Nm.MailAddressCollection addresses, MailMessageRecipientCollection recipients)
        {
            if (addresses == null)
                throw new ArgumentNullException("addresses");
            if (recipients == null)
                throw new ArgumentNullException("recipients");

            // copy...
            addresses.Clear();
            foreach (MailMessageRecipient recipient in recipients)
                addresses.Add(recipient.ToMailAddress());
        }

        /// <summary>
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <param name="message"></param>
        private void AddAttachments(Nm.MailMessage mailMessage, IMailMessage message)
        {
            if (mailMessage == null)
                throw new ArgumentNullException("mailMessage");
            if (message == null)
                throw new ArgumentNullException("message");

            // attach...
            foreach (MailAttachment attachment in message.Attachments)
            {
                Nm.Attachment mailAttachment = this.ToNmMailAttachment(attachment);
                if (mailAttachment == null)
                    throw new InvalidOperationException("mailAttachment is null.");

                // add...
                mailMessage.Attachments.Add(mailAttachment);
            }
        }

        private Nm.Attachment ToNmMailAttachment(MailAttachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException("attachment");

            // create...
            Nm.Attachment mailAttachment = new Nm.Attachment(attachment.FilePath);

            // return...
            return mailAttachment;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public SmtpConnectionSettings Settings
        {
            get
            {
                // returns the value...
                return _settings;
            }
            set
            {
                if (value == null)
                    value = SmtpConnectionSettings.GetDefaultSettings();
                _settings = value;
            }
        }
    }
}
