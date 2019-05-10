// BootFX - Application framework for .NET applications
// 
// File: SsrsReportId.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Web;
using System.Text;
using BootFX.Common;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines a class that encapsulates the ID of an SSRS report.
	/// </summary>
	public class SsrsReportId
	{
		/// <summary>
		/// Private field to support <see cref="IsHttps"/> property.
		/// </summary>
		private bool _isHttps;
		
		/// <summary>
		/// Private field to support <see cref="HostName"/> property.
		/// </summary>
		private string _hostName;

		/// <summary>
		/// Private field to support <see cref="SetName"/> property.
		/// </summary>
		private string _setName;

		/// <summary>
		/// Private field to support <see cref="ReportName"/> property.
		/// </summary>
		private string _reportName;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public SsrsReportId(bool isHttps, string hostName, string setName, string reportName)
		{
			if(hostName == null)
				throw new ArgumentNullException("hostName");
			if(hostName.Length == 0)
				throw new ArgumentOutOfRangeException("'hostName' is zero-length.");
			if(setName == null)
				throw new ArgumentNullException("setName");
			if(setName.Length == 0)
				throw new ArgumentOutOfRangeException("'setName' is zero-length.");
			if(reportName == null)
				throw new ArgumentNullException("reportName");
			if(reportName.Length == 0)
				throw new ArgumentOutOfRangeException("'reportName' is zero-length.");
			
			// set...
			_isHttps = isHttps;
			_hostName = hostName;
			_setName = setName;
			_reportName = reportName;
		}

		/// <summary>
		/// Gets the reportname.
		/// </summary>
		internal string ReportName
		{
			get
			{
				return _reportName;
			}
		}
		
		/// <summary>
		/// Gets the setname.
		/// </summary>
		internal string SetName
		{
			get
			{
				return _setName;
			}
		}
		
		/// <summary>
		/// Gets the hostname.
		/// </summary>
		internal string HostName
		{
			get
			{
				return _hostName;
			}
		}

		/// <summary>
		/// Gets the ishttps.
		/// </summary>
		internal bool IsHttps
		{
			get
			{
				return _isHttps;
			}
		}

		/// <summary>
		/// Gets the ID as an HTTP GET.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			// http://localhost/ReportServer/?/Forest.Symphony.Reports/DeliveryNotes&rs:Format=PDF

			// build it...
			StringBuilder builder = new StringBuilder();
			builder.Append("http");
			if(this.IsHttps)
				builder.Append("s");
			builder.Append("://");
			builder.Append(this.HostName);
			builder.Append("/ReportServer/?/");

			// set...
			string setName = this.SetName;
			if(setName == null)
				throw new InvalidOperationException("setName is null.");
			while(setName.StartsWith("/"))
				setName = setName.Substring(1);
			builder.Append(HttpUtility.UrlPathEncode(setName));

			// report...
			builder.Append("/");
			builder.Append(HttpUtility.UrlPathEncode(this.ReportName));

			// return...
			return builder.ToString();
		}
	}
}
