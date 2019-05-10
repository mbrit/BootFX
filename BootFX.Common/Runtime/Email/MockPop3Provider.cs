// BootFX - Application framework for .NET applications
// 
// File: MockPop3Provider.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Email;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Summary description for MockPop3Provider.
	/// </summary>
	public class MockPop3Provider : IPop3Provider
	{
		public MockPop3Provider()
		{
		}

		public void Connect(Pop3ConnectionSettings settings)
		{
		}

		public void Dispose()
		{
			// TODO:  Add MockPop3Provider.Dispose implementation
		}

		public MailMessage GetMessage(int index)
		{	
			MailMessage message = new MailMessage();
			message.FromAsString = "russell@mbrit.com";
			message.ToAsString = "enquiries.foo@mbrit.com";
			message.Subject = "Hello everyone!";
			message.Body = "This is not spam.  I would like to spend �500,000 with you please.";

			// return...
			return message;
		}

		public void DeleteMessage(int index)
		{
		}

		public int MessageCount
		{
			get
			{
				return 1;
			}
		}
	}
}
