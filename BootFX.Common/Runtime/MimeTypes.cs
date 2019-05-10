// BootFX - Application framework for .NET applications
// 
// File: MimeTypes.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for MimeTypes.
	/// </summary>
	public sealed class MimeTypes
	{
		private MimeTypes()
		{
		}

		public const string Html = "text/html";
		public const string Xml = "text/xml";
		public const string Csv = "text/csv";
		public const string Text = "text/plain";

		public const string Jpeg = "image/jpeg";
		public const string Png = "image/png";
		public const string Gif = "image/gif";

		public const string MsOfficeWord = "application/msword";
		public const string MsOfficeExcel = "application/excel";
		public const string MsOfficePowerPoint = "application/x-mspowerpoint";

		public const string Pdf = "application/pdf";
	}
}
