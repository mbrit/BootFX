// BootFX - Application framework for .NET applications
// 
// File: HttpDownloadException.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Net;
using System.Security;

namespace BootFX.Common
{
	/// <summary>
	/// Raised when there is a problem downloading.
	/// </summary>
	// mbr - 11-06-2008 - changed to user message...
	[Serializable()]
//	public class HttpDownloadException : ApplicationExceptionWithPayload
	public class HttpDownloadException : ExceptionWithUserMessages
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal HttpDownloadException(string message, string payload, Exception innerException) 
			// meaning is reversed for usermessages...
//			: base(message, payload, innerException)
			: base(payload, message, innerException)
		{
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected HttpDownloadException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary> 
		/// Provides data to serialization.
		/// </summary> 
        [SecurityCritical]
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// base...
			base.GetObjectData(info, context);

			// add code here to stream extra state into 'info'.
			// remember to update the deserializtion constructor.
		}

		/// <summary>
		/// Returns the Web exception.
		/// </summary>
		public WebException InnerWebException
		{
			get
			{
				Exception scan = this.InnerException;
				while(scan != null)
				{
					if(scan is WebException)
						return (WebException)scan;
				}

				// nope...
				return null;
			}
		}

		/// <summary>
		/// Gets the response.
		/// </summary>
		public HttpWebResponse Response
		{
			get
			{
				WebException webEx = this.InnerWebException;
				if(webEx != null)
					return webEx.Response as HttpWebResponse;
				else
					return null;
			}
		}

		// mbr - 11-06-2008 - wasn't used...	
//		/// <summary>
//		/// Gets the payload as an HTTP result.
//		/// </summary>
//		/// <returns></returns>
//		public HttpResult PayloadToHttpResult()
//		{
//			HttpResponseStatus status = HttpResponseStatus.NoResponse;
//			HttpStatusCode httpStatus = HttpStatusCode.OK;
//
//			// get...
//			HttpWebResponse response = this.Response;
//			if(response != null)
//			{
//				status = HttpResponseStatus.ResponseOk;
//				httpStatus = response.StatusCode;
//			}
//
//			// return...
//			return new HttpResult(this.Payload, status, httpStatus, this.InnerWebException);
//		}
	}
}
