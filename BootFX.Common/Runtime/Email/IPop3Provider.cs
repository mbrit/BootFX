// BootFX - Application framework for .NET applications
// 
// File: IPop3Provider.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Email
{
	/// <summary>
	/// Summary description for <see cref="IPop3Provider"/>.
	/// </summary>
	public interface IPop3Provider : IDisposable
	{
		/// <summary>
		/// Opens the connection.
		/// </summary>
		/// <param name="hostName"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="port"></param>
		void Connect(Pop3ConnectionSettings settings);

		/// <summary>
		/// Gets the message count.
		/// </summary>
		int MessageCount
		{
			get;
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <param name="index"></param>
		MailMessage GetMessage(int index);

		/// <summary>
		/// Deletes a message.
		/// </summary>
		/// <param name="index"></param>
		void DeleteMessage(int index);
	}
}
