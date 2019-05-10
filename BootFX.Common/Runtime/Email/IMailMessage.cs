// BootFX - Application framework for .NET applications
// 
// File: IMailMessage.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Generic;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Summary description for IMailMessage.
	/// </summary>
	public interface IMailMessage
	{
		string Subject
		{
			get;
			set;
		}

		MailMessageRecipient From
		{
			get;
			set;
		}

        MailMessageRecipient ReplyTo
        {
            get;
            set;
        }

		string FromAsString
		{
			get;
			set;
		}

		string ToAsString
		{
			get;
			set;
		}

		MailMessageRecipientCollection To
		{
			get;
		}

		string CcAsString
		{
			get;
			set;
		}

		MailMessageRecipientCollection Cc
		{
			get;
		}

		string BccAsString
		{
			get;
			set;
		}

		MailMessageRecipientCollection Bcc
		{
			get;
		}

		string Body
		{
			get;
			set;
		}

		MailFormat MailFormat
		{
			get;
			set;
		}

		MailAttachmentCollection Attachments
		{
			get;
		}

        List<MailEmbeddedImage> EmbeddedImages { get; }
    }
}
