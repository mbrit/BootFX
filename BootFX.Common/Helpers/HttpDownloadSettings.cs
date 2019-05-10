// BootFX - Application framework for .NET applications
// 
// File: HttpDownloadSettings.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>HttpDownloadSettings</c>.
	/// </summary>
	public sealed class HttpDownloadSettings
	{
        /// <summary>
        /// Private value to support the <see cref="ExtraHeaders">ExtraHeaders</see> property.
        /// </summary>
        private WebHeaderCollection _extraHeaders;

        /// <summary>
        /// Private value to support the <see cref="UserAgent">UserAgent</see> property.
        /// </summary>
        private string _userAgent;

        /// <summary>
		/// Private field to support <see cref="PhoneHomeDefault"/> property.
		/// </summary>
		private static HttpDownloadSettings _phoneHomeDefault;
		
		/// <summary>
		/// Private field to support <c>Encoding</c> property.
		/// </summary>
		private Encoding _encoding = Encoding.ASCII;
		
		/// <summary>
		/// Private field to support <c>Credentials</c> property.
		/// </summary>
		private ICredentials _credentials = CredentialCache.DefaultCredentials;
		
		/// <summary>
		/// Private field to support <c>Proxy</c> property.
		/// </summary>
		private IWebProxy _proxy = null;

		/// <summary>
		/// Private field to support <c>Timeout</c> property.
		/// </summary>
		/// <remarks>
		/// EG Added 2007-08-01
		/// </remarks>
		private int _timeout = 0; 
		
		private static HttpDownloadSettings _default = new HttpDownloadSettings();

        /// <summary>
        /// Private value to support the <see cref="KeepAlive">KeepAlive</see> property.
        /// </summary>
        private bool _keepAlive = true;

        /// <summary>
        /// Gets the KeepAlive value.
        /// </summary>
        public bool KeepAlive
        {
            get
            {
                return _keepAlive;
            }
            set
            {
                _keepAlive = value;
            }
        }

		public HttpDownloadSettings()
		{
		}

		public static HttpDownloadSettings Default
		{
			get
			{
				return _default;
			}
		}

		/// <summary>
		/// Gets the phonehomedefault.
		/// </summary>
		public static HttpDownloadSettings PhoneHomeDefault
		{
			get
			{
				if(_phoneHomeDefault == null)
				{
					// setup...
					_phoneHomeDefault = new HttpDownloadSettings();
					_phoneHomeDefault.Credentials = CredentialCache.DefaultCredentials;
				}

				// return...
				return _phoneHomeDefault;
			}
			set
			{
				_phoneHomeDefault = value;
			}
		}

		/// <summary>
		/// Gets or sets the proxy
		/// </summary>
		public IWebProxy Proxy
		{
			get
			{
				if(_proxy == null)
				{
					// mbr - 03-08-2006 - changed.					
					//					return GlobalProxySelection.GetEmptyWebProxy();
                    // mbr - 2009-10-01 - changed back!
					//return Runtime.Current.InstallationSettings.GetWebProxy();
                    return null;
				}
				else
					return _proxy;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _proxy)
				{
					// set the value...
					_proxy = value;
				}
			}
		}
		/// <summary>
		/// Gets or sets the timeout
		/// </summary>
		/// <remarks>
		/// Added by EG 2007-08-01
		/// </remarks>
		public int Timeout
		{
			get
			{
				return _timeout;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _timeout)
				{
					// set the value...
					_timeout = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the credentials
		/// </summary>
		public ICredentials Credentials
		{
			get
			{
				return _credentials;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _credentials)
				{
					// set the value...
					_credentials = value;
				}
			}
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("Credentials: ");
			HttpHelper.DumpCredentials(this.Credentials, builder);
			builder.Append(", proxy: ");
			HttpHelper.DumpProxy(this.Proxy, builder);

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Gets or sets the encoding
		/// </summary>
		public Encoding Encoding
		{
			get
			{
				return _encoding;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _encoding)
				{
					// set the value...
					_encoding = value;
				}
			}
		}

        /// <summary>
        /// Gets the UserAgent value.
        /// </summary>
        public string UserAgent
        {
            get
            {
                return _userAgent;
            }
            set
            {
                _userAgent = value;
            }
        }

        public void SetUserAgentToIe()
        {
            this.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; .NET CLR 2.0.50727; .NET CLR 1.1.4322; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; InfoPath.1)";
        }

        /// <summary>
        /// Gets the ExtraHeaders value.
        /// </summary>
        public WebHeaderCollection ExtraHeaders
        {
            get
            {
                if (_extraHeaders == null)
                    _extraHeaders = new WebHeaderCollection();
                return _extraHeaders;
            }
        }

        internal bool HasExtraHeaders
        {
            get
            {
                if (_extraHeaders == null || this.ExtraHeaders.Count == 0)
                    return false;
                else
                    return true;
            }
        }
	}
}
