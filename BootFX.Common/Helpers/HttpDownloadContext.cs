// BootFX - Application framework for .NET applications
// 
// File: HttpDownloadContext.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Net;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Context class for making HTTP requests.
	/// </summary>
	public class HttpDownloadContext : OperationContext
	{
		/// <summary>
		/// Private field to support <see cref="Request"/> property.
		/// </summary>
		private HttpWebRequest _request;
		
		/// <summary>
		/// Raised before the request is sent.
		/// </summary>
		public event EventHandler BeforeRequestSent;
		
		/// <summary>
		/// Private field to support <c>Settings</c> property.
		/// </summary>
		private HttpDownloadSettings _settings;
		
		/// <summary>
		/// Private field to support <c>ExpectedReturnType</c> property.
		/// </summary>
		private HttpResultType _expectedReturnType;
		
		public HttpDownloadContext()
			: base(null, null)
		{
		}

		public HttpDownloadContext(ILog log, IOperationItem innerOperation)
			: base(log, innerOperation)
		{
		}

		/// <summary>
		/// Gets or sets the expectedreturntype
		/// </summary>
		public HttpResultType ExpectedReturnType
		{
			get
			{
				return _expectedReturnType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _expectedReturnType)
				{
					// set the value...
					_expectedReturnType = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the settings
		/// </summary>
		public HttpDownloadSettings Settings
		{
			get
			{
				return _settings;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _settings)
				{
					// set the value...
					_settings = value;
				}
			}
		}

		/// <summary>
		/// Raises the <c>BeforeRequestSent</c> event.
		/// </summary>
		private void OnBeforeRequestSent()
		{
			OnBeforeRequestSent(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>BeforeRequestSent</c> event.
		/// </summary>
		protected internal virtual void OnBeforeRequestSent(EventArgs e)
		{
			// raise...
			if(BeforeRequestSent != null)
				BeforeRequestSent(this, e);
		}

		/// <summary>
		/// Creates the request.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		internal HttpWebRequest CreateRequest(string url)
		{
			HttpWebRequest request = HttpHelper.CreateRequest(url, this.Settings);
			if(request == null)
				throw new InvalidOperationException("request is null.");

			// set...
			_request = request;

			// return...
			return request;
		}

		/// <summary>
		/// Gets the request.
		/// </summary>
		public HttpWebRequest Request
		{
			get
			{
				return _request;
			}
		}
	}
}
