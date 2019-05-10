// BootFX - Application framework for .NET applications
// 
// File: HttpResult.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Net;
using System.Xml;

namespace BootFX.Common
{
	/// <summary>
	/// Defines the class returned from an HTTP POST call.
	/// </summary>
	public class HttpResult
	{
        /// <summary>
        /// Private value to support the <see cref="RespondingUrl">RespondingUrl</see> property.
        /// </summary>
        private Uri _respondingUrl;

        /// <summary>
		/// Private field to support <see cref="exception"/> property.
		/// </summary>
		private WebException _exception;
		
		/// <summary>
		/// Private field to support <see cref="responseStatus"/> property.
		/// </summary>
		private HttpResponseStatus _responseStatus;
		
		/// <summary>
		/// Private field to support <see cref="Status"/> property.
		/// </summary>
		private HttpStatusCode _status;
		
		/// <summary>
		/// Private field to support <see cref="RawResult"/> property.
		/// </summary>
		private object _rawResult;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="result"></param>
        // mbr - 2009-08-26 - added RespondingUrl...
		internal HttpResult(object rawResult, HttpResponseStatus responseStatus, HttpStatusCode status, Uri respondingUrl,
			WebException exception)
		{
			_rawResult = rawResult;
			_responseStatus = responseStatus;
			_status = status;
            _respondingUrl = respondingUrl;
			_exception = exception;
		}

		/// <summary>
		/// Gets the rawresult.
		/// </summary>
		internal object RawResult
		{
			get
			{
				return _rawResult;
			}
		}

		public bool IsNull
		{
			get
			{
				if(_rawResult == null)
					return true;
				else
					return false;
			}
		}

		public XmlDocument ResultAsXml
		{
			get
			{
				return this.RawResult as XmlDocument;
			}
		}

		public string ResultAsString
		{
			get
			{
				return this.RawResult as string;
			}
		}

		/// <summary>
		/// Gets the status.
		/// </summary>
		public HttpStatusCode Status
		{
			get
			{
				return _status;
			}
		}

		/// <summary>
		/// Gets the responsestatus.
		/// </summary>
		public HttpResponseStatus ResponseStatus
		{
			get
			{
				return _responseStatus;
			}
		}

		/// <summary>
		/// Gets the exception.
		/// </summary>
		public WebException Exception
		{
			get
			{
				return _exception;
			}
		}

        /// <summary>
        /// Gets the RespondingUrl value.
        /// </summary>
        // mbr - 2009-08-26 - added RespondingUrl...
        public Uri RespondingUrl
        {
            get
            {
                return _respondingUrl;
            }
        }

        public bool HasError
        {
            get
            {
                if(this.Exception != null)
                    return true;
                else
                    return false;
            }
        }
	}
}
