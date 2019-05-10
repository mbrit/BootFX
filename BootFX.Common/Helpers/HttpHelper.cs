// BootFX - Application framework for .NET applications
// 
// File: HttpHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Net;
using BootFX.Common.Data;
using System.Reflection;
using System.Collections.Generic;
using System.IO.Compression;

namespace BootFX.Common
{
    public delegate string QueryStringDataFormatCallback(object value);

	/// <summary>
	/// Defines a class that can help download resources over HTTP.
	/// </summary>
	public static class HttpHelper
	{
		public static XmlDocument DownloadToXmlDocument(string url, HttpDownloadSettings settings)
		{
			string path = DownloadToTempFile(url, settings);
			if(path == null)
				throw new InvalidOperationException("'path' is null.");
			if(path.Length == 0)
				throw new InvalidOperationException("'path' is zero-length.");

			try
			{
				XmlDocument doc = new XmlDocument();
				doc.Load(path);

				// return...
				return doc;
			}
			finally
			{
				Runtime.Current.SafeDelete(path);
			}
		}

		/// <summary>
		/// Downloads the given URL to a temporary file.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string DownloadToTempFile(string url, HttpDownloadSettings settings)
		{
			return DownloadToTempFile(url, (byte[])null, settings);
		}

		/// <summary>
		/// Downloads the given URL to a temporary file.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string DownloadToTempFile(string url, string base64Md5Hash, HttpDownloadSettings settings)
		{
			return DownloadToTempFile(url, HashHelper.ConvertMd5Hash(base64Md5Hash), settings);
		}

		/// <summary>
		/// Downloads the given URL to a temporary file.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string DownloadToTempFile(string url, byte[] md5Hash, HttpDownloadSettings settings)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			
			// get...
			string path = Runtime.Current.GetTempFilePath();
			if(path == null)
				throw new InvalidOperationException("'path' is null.");
			if(path.Length == 0)
				throw new InvalidOperationException("'path' is zero-length.");

			// download...
			Download(url, path, md5Hash, settings);

			// return...
			return path;
		}

		/// <summary>
		/// Downloads the given URL to the given file.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void Download(string url, string path, HttpDownloadSettings settings)
		{
			Download(url, path, (byte[])null, settings);
		}

		/// <summary>
		/// Downlaods the given URL to a string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static string Download(string url, HttpDownloadSettings settings = null)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");

            if (settings == null)
                settings = HttpDownloadSettings.Default;
            
            // create...
			using(MemoryStream stream = new MemoryStream())
			{
				Download(url, stream, settings);

				// return...
				if(settings.Encoding == null)
					throw new InvalidOperationException("settings.Encoding is null.");
				return settings.Encoding.GetString(stream.ToArray());
			}
		}

		/// <summary>
		/// Downloads the given URL to the given file.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void Download(string url, string path, string base64Md5Hash, HttpDownloadSettings settings)
		{
			Download(url, path, HashHelper.ConvertMd5Hash(base64Md5Hash), settings);
		}
		
		/// <summary>
		/// Downloads the given URL to the given file.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void Download(string url, string path, byte[] md5Hash, HttpDownloadSettings settings)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");

			// download...
			string temp = Runtime.Current.GetTempFilePath();
			try
			{
				// open...
				using(FileStream outStream = new FileStream(temp, FileMode.Create, FileAccess.Write))
					Download(url, outStream, settings);

				// compare...
				if(md5Hash != null && md5Hash.Length > 0)
					HashHelper.AssertMd5HashForFile(temp, md5Hash);

				// move...
				if(File.Exists(path))
					File.Delete(path);
				File.Move(temp, path);
			}
			finally
			{
				Runtime.Current.SafeDelete(temp);
			}
		}

        public static void DownloadToStream(string url, Stream outStream, HttpDownloadSettings settings = null)
        {
            Download(url, outStream, settings);
        }

		/// <summary>
		/// Downloads the given URL to the given file.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void Download(string url, Stream outStream, HttpDownloadSettings settings)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			if(outStream == null)
				throw new ArgumentNullException("outStream");
			
			// download to path...
			HttpWebRequest request = CreateRequest(url, settings);
			if(request == null)
				throw new InvalidOperationException("request is null.");

			// walk...
			try
			{
				using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
					using(Stream inStream = response.GetResponseStream())	
					    StreamHelper.CopyStream(inStream, outStream);
				}
			}
			catch(Exception ex) 
			{
				throw CreateDownloadException("Failed to download resource.", request, ex);
			}
		}

        /// <summary>
        /// Downloads the given URL to the given file.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        // mbr - 2009-08-26 - added...
        public static HttpResult Download(string url, HttpDownloadSettings settings, HttpResultType expected)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            if (url.Length == 0)
                throw new ArgumentOutOfRangeException("'url' is zero-length.");

            // download to path...
            HttpWebRequest request = CreateRequest(url, settings);
            if (request == null)
                throw new InvalidOperationException("request is null.");

            // walk...
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    return HandleHttpResult(response, expected);
            }
            catch (WebException ex)
            {
                return HandleHttpResultException(ex);
            }
            catch (Exception ex)
            {
                throw CreateDownloadException("Failed to download resource.", request, ex);
            }
        }

		public static Exception CreateDownloadException(string message, HttpWebRequest request, Exception ex)
		{
			if(request == null)
				throw new ArgumentNullException("request");

            // mbr - 2009-05-28 - reworked overloads...
			//return CreateDownloadExceptionInternal(message, request.RequestUri.ToString(), request.Credentials, request.Proxy, ex);
            return CreateDownloadExceptionInternal(message, request, ex);
		}

        // mbr - 2009-05-28 - this was weird... there was a method that dumped loads of information from the request 
        // directly, which was more informative, but not being used...
        //private static Exception CreateDownloadExceptionInternal(string message, string url, ICredentials credentials,
        //    IWebProxy proxy, Exception ex)
        private static Exception CreateDownloadExceptionForWebServiceCall(string message, string url, ICredentials credentials,
            IWebProxy proxy, Exception ex)
        {
            // builder...
            StringBuilder builder = new StringBuilder();
            builder.Append(message);
            builder.Append("\r\n");

            // dump...
            DumpRequestInfo(builder, url, credentials, proxy, ex);

            // return...
            return new HttpDownloadException(message, builder.ToString(), ex);
        }

        // mbr - 2009-05-28 - this was weird... there was a method that dumped loads of information from the request 
        // directly, which was more informative, but not being used...
        //private static Exception CreateDownloadExceptionInternal(string message, string url, ICredentials credentials, 
        //    IWebProxy proxy, Exception ex)
        private static Exception CreateDownloadExceptionInternal(string message, HttpWebRequest request, Exception ex)
		{
			// builder...
			StringBuilder builder = new StringBuilder();
			builder.Append(message);
			builder.Append("\r\n");

			// mbr - 2009-05-28 - changed this...
			//DumpRequestInfo(builder, url, credentials, proxy, ex);
            DumpRequestInfo(builder, request);

            // return...
			return new HttpDownloadException(message, builder.ToString(), ex);
		}

		// mbr - 04-06-2007 - modified this to return the content in a separate payload builder.		
		// mbr - 11-06-2008 - moved it back again to support user message exceptions!
		private static void DumpRequestInfo(StringBuilder builder, string url, ICredentials credentials, 
			IWebProxy proxy, Exception ex)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			// add...
			builder.Append("\tURL: ");
			builder.Append(url);
			builder.Append("\r\n\tCredentials: ");
			DumpCredentials(credentials, builder);
			builder.Append("\r\n\tProxy: ");
			DumpProxy(proxy, builder);

			// do we have ex?
			if(ex is WebException && ((WebException)ex).Response is HttpWebResponse)
			{
				// get...
				HttpWebResponse response = (HttpWebResponse)((WebException)ex).Response;

				try
				{
					// try it...
					builder.Append("\r\n\tSERVER RESPONSE:\r\n");
					builder.Append("\t\tCode: ");
					builder.Append((int)response.StatusCode);
					builder.Append(" (");
					builder.Append(response.StatusDescription);
					builder.Append(")\r\n");

					string html = null;
					using (Stream stream = response.GetResponseStream())
					{
						StreamReader reader = new StreamReader(stream);
						html = reader.ReadToEnd();
					}

					// mbr - 15-03-2007 - changed to 10k.		
//					const int maxLen = 2048;
					const int maxLen = 10240;
					if (html.Length < maxLen)
					{
						builder.Append("\t\tThe complete return message is specified within the builder.");
						builder.Append(html);
					}
					else
					{
						builder.Append("\t\tThe first ");
						builder.Append(maxLen);
			
			builder.Append(" byte(s) returned from the server are specified within the builder.");
						builder.Append(html.Substring(0, maxLen));
					}
				}
				catch(Exception debugEx)
				{
					builder.Append("\tAn error occurred when producing exception information: ");
					builder.Append(debugEx);
				}
			}
		}

        public static void DumpRequestInfo(StringBuilder builder, HttpWebRequest request)
        {
            if (builder == null)
                throw new ArgumentNullException("builder");
            if (request == null)
                throw new ArgumentNullException("request");

            // dump...
            DumpRequestInfo(builder, request.RequestUri.ToString(), request.Credentials, request.Proxy, null);

            // mbr - 18-09-2006 - headers...
            builder.Append("\tHeaders:\t\n");
            foreach (string name in request.Headers.AllKeys)
            {
                builder.Append("\t\t");
                builder.Append(name);
                builder.Append(": ");
                builder.Append(request.Headers[name]);
                builder.Append("\r\n");
            }
        }

		/// <summary>
		/// Creates a request to the given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		// mbr - 28-12-2006 - made public.		
		public static HttpWebRequest CreateRequest(string url, HttpDownloadSettings settings)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");

			// default?
			if(settings == null)
				settings = HttpDownloadSettings.Default;
			
			// create...
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			if(request == null)
				throw new InvalidOperationException("request is null.");

			// set...
			request.Proxy = settings.Proxy;
			request.Credentials = settings.Credentials;
            request.KeepAlive = settings.KeepAlive;

            // mbr - 2009-01-16 - agent...
            if(!(string.IsNullOrEmpty(settings.UserAgent)))
                request.UserAgent = settings.UserAgent;

			// mbr - 02-08-2007 - only use this if we have a timeout > 0...
			if(settings.Timeout > 0)
				request.Timeout = settings.Timeout; // EG Added 2007-08-01

            // mbr - 2010-04-19 - headings...
            if (settings.HasExtraHeaders)
            {
                foreach (string key in settings.ExtraHeaders.Keys)
                    request.Headers.Add(key, settings.ExtraHeaders[key]);
            }

			// return...
			return request;
		}

		internal static void DumpCredentials(ICredentials credentials, StringBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			if(credentials == CredentialCache.DefaultCredentials)
			{
				builder.Append("{Default credentials - ");
				builder.Append(Environment.UserDomainName);
				builder.Append("\\");
				builder.Append(Environment.UserName);
				builder.Append("}");
			}
			else if(credentials != null)
			{
				if(credentials is NetworkCredential)
				{
					NetworkCredential net = (NetworkCredential)credentials;
					builder.Append("Network credentials [domain: ");
					builder.Append(net.Domain);
					builder.Append(", username: ");
					builder.Append(net.UserName);
					builder.Append(", password: ");
					if(net.Password != null && net.Password.Length > 0)
						builder.Append("*****");
					else
						builder.Append("(null)");
					builder.Append("]");
				}
				else
					builder.Append(credentials.ToString());
			}
			else
				builder.Append("(null)");
		}

		internal static void DumpProxy(IWebProxy proxy, StringBuilder builder)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");

			if(proxy != null)
			{
				bool isSystemProxy = false;
				bool isEmptyProxy = false;

				isSystemProxy = false;
				isEmptyProxy = (proxy == WebRequest.DefaultWebProxy);

				if(isSystemProxy)
				{
					builder.Append("{System proxy - ");
					builder.Append(Environment.UserDomainName);
					builder.Append("\\");
					builder.Append(Environment.UserName);
					builder.Append("}");
				}
				else if(isEmptyProxy || proxy.GetType().Name == "EmptyWebProxy")
					builder.Append("{Empty proxy}");
				else
				{
					if(proxy is WebProxy)
					{
						WebProxy web = (WebProxy)proxy;
						builder.Append(web.Address);
						builder.Append(" [");
						builder.Append("bypass on local: ");
						builder.Append(web.BypassProxyOnLocal);
						builder.Append(", credentials: ");
						DumpCredentials(proxy.Credentials, builder);
						builder.Append("]");
					}
					else
					{
						builder.Append(proxy.GetType());
						builder.Append(" [");
						builder.Append("credentials: ");
						DumpCredentials(proxy.Credentials, builder);
						builder.Append("]");
					}
				}
			}
			else
				builder.Append(" (null)");
		}

		public static string MakePostRequestAndDownloadToTempFile(string url, IDictionary values, HttpDownloadSettings settings)
		{
			string path = Runtime.Current.GetTempFilePath();
			using(StreamWriter writer = new StreamWriter(path))
				MakePostRequestInternal(url, values, settings, writer, null);

			// return...
			return path;
		}

        public static string MakePostRequestAndDownloadBinaryToTempFile(string url, IDictionary values, HttpDownloadSettings settings)
        {
            string path = Runtime.Current.GetTempFilePath();
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                MakePostRequestInternal(url, values, settings, null, stream);

            // return...
            return path;
        }

        public static string MakePostRequestAndDownload(string url, IDictionary values, HttpDownloadSettings settings)
		{
			using(StringWriter writer = new StringWriter())
			{
				// defer...
				MakePostRequestInternal(url, values, settings, writer, null);
				return writer.GetStringBuilder().ToString();
			}
		}

		public static void MakePostRequest(string url, IDictionary values, HttpDownloadSettings settings)
		{
			MakePostRequestInternal(url, values, settings, null, null);
		}

        public static void MakePostRequest(string url, string formUrlEncoded, HttpDownloadSettings settings = null)
        {
            MakePostRequestInternal(url, formUrlEncoded, settings, null, null);
        }

        private static void MakePostRequestInternal(string url, IDictionary values, HttpDownloadSettings settings, TextWriter writer, Stream outStream)
        {
            // formulate...
            string asString = PostRequestValuesToString(values);
            if (asString == null)
                asString = string.Empty;

            MakePostRequestInternal(url, asString, settings, writer, outStream);
        }

		private static void MakePostRequestInternal(string url, string formUrlEncoded, HttpDownloadSettings settings, TextWriter writer, Stream outStream)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			
			// create...
			HttpWebRequest request = CreateRequest(url, settings);
			
			if(request == null)
				throw new InvalidOperationException("request is null.");

			// set the basics...
			request.Method = "POST";

			// bytes...
            byte[] bs = Encoding.ASCII.GetBytes(formUrlEncoded);
			
			// set...
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = bs.Length;

			// mbr - 12-04-2007 - moved this up to before the getrequeststream called as
			// proxy auth failure here would prevent a rich exception.
			try
			{
				// get...
				using(Stream stream = request.GetRequestStream())
					stream.Write(bs, 0, bs.Length);
	
				// get the response...
				using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
				{
                    using (Stream stream = response.GetResponseStream())
                    {
    					// now what?
                        if (writer != null)
                        {
                            if (settings == null)
                            {
                                settings = HttpDownloadSettings.Default;
                                if (settings == null)
                                    throw new InvalidOperationException("'settings' is null.");
                            }
                            if (settings.Encoding == null)
                                throw new InvalidOperationException("settings.Encoding is null.");

                            // get...
                            StreamHelper.CopyStreamToTextWriter(stream, writer, settings.Encoding);
                        }
                        else if (outStream != null)
                            StreamHelper.CopyStream(stream, outStream);
                    }
				}
			}
			catch(Exception ex)
			{
				throw CreateDownloadException("Failed to make a POST request.", request, ex);
			}
		}

		private static string PostRequestValuesToString(IDictionary values)
		{
			if(values == null)
				throw new ArgumentNullException("values");
			
			StringBuilder builder = new StringBuilder();
			bool first = true;
			foreach(DictionaryEntry entry in values)
			{
				if(first)
					first = false;
				else
					builder.Append("&");

				// name...
				builder.Append(ConversionHelper.ToString(entry.Key, Cultures.System));
				builder.Append("=");
				builder.Append(HttpUtility.UrlEncode(ConversionHelper.ToString(entry.Value, Cultures.System)));
			}

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Takes a URL and replaces the protocol, host and application name portions.
		/// </summary>
		public static string PatchUrl(string url, bool isSecure, string host, string appName)
		{
			// return...
			return PatchUrl(url, isSecure, host, 0, appName);
		}

		/// <summary>
		/// Takes a URL and replaces the protocol, host and application name portions.
		/// </summary>
		public static string PatchUrl(string url, bool isSecure, string host, int port, string appName)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			if(port < 0)
				throw new  ArgumentOutOfRangeException("port", port, "Port must be >= 0.");
			
			// create...
			Uri uri = new Uri(url);

			// do the basics...
			StringBuilder builder = new StringBuilder();
			if(isSecure)
				builder.Append("https://");
			else
				builder.Append("http://");
			builder.Append(host);
			if(port != 0)
			{
				builder.Append(":");
				builder.Append(port);
			}
			builder.Append("/");

			// do we have an application name?
			if(appName != null && appName.Length > 0)
			{
				if(appName.StartsWith("/"))
					builder.Append(appName.Substring(1));
				else
					builder.Append(appName);
			}

			// ok, now get the local path...
			string localPath = uri.LocalPath;
			while(localPath.StartsWith("/"))
				localPath = localPath.Substring(1);

			// find the next slash - this gives you the app name to omit...
			int index = localPath.IndexOfAny(new char[] { '/', '\\' });
			if(index == -1)
				throw new InvalidOperationException(string.Format("A url of '{0}' is invalid because it has a local path of '{1}'.", url, uri.LocalPath));

			// trim...
			localPath = localPath.Substring(index + 1);
			builder.Append("/");
			builder.Append(localPath);

			// return...
			return builder.ToString();
		}

        public static string CombineUrlParts(string a, params string[] ps)
        {
            for (var index = 0; index < ps.Length; index++)
                a = CombineUrlParts(a, ps[index]);
            return a;
        }

        /// <summary>
        /// Combines two URL parts, e.g. "http://foo/" and "/widget.aspx" = "http://foo/widget.aspx".
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string CombineUrlParts(string a, string b)
		{
			if(b == null || b.Length == 0)
				return a;
			if(a == null || a.Length == 0)
				return b;

            // get...
            a = ForwardizeSlashes(a);
            b = ForwardizeSlashes(b);

            // remove leading slashes from b...
            while (b.StartsWith("/"))
                b = b.Substring(1);

            // if a is just a forward slash, this is a special case...
            if (a == "/" || b.Length == 0)
            {
                // ok, if a is a slash and b was a slash and we have made it empty, return a...
                if (a == "/" && b.Length == 0)
                    return a;

                // if a and b are both zero length, a was blank and b was a slash, so return a slash...
                if (a.Length == 0 && b.Length == 0)
                    return "/";

                // otherwise, return a plus b as a can only be a slash and b must be longer...
                return a + b;
            }
            else
            {
                while (a.EndsWith("/"))
                    a = a.Substring(0, a.Length - 1);

                // return...
                string result = string.Concat(a, "/", b);
                return result;
            }
		}

        private static string ForwardizeSlashes(string buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");
            if (buf.Length == 0)
                throw new ArgumentException("'buf' is zero-length.");

            // builder...
            StringBuilder builder = new StringBuilder(buf.Length);
            for (int index = 0; index < buf.Length; index++)
            {
                if (buf[index] == '\\')
                    builder.Append('/');
                else
                    builder.Append(buf[index]);
            }

            // return...
            return builder.ToString();
        }

        /// <summary>
        /// Gets the folder for given given URL.
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static string GetFolder(Uri uri)
		{
			if(uri == null)
				throw new ArgumentNullException("uri");
			
			// return...
			return GetFolder(uri.ToString());
		}

		/// <summary>
		/// Gets the folder for given given URL.
		/// </summary>
		/// <param name="a"></param>
		/// <returns></returns>
		public static string GetFolder(string url)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			
			// ?
			url = RemoveQueryStringFromUrl(url);
			if(url == null)
				throw new InvalidOperationException("url is null.");

			// find...
			int index = url.LastIndexOfAny(new char[] { '/', '\\' });
			if(index != -1)
				return url.Substring(0, index);
			else
				throw new InvalidOperationException(string.Format("'{0}' does not have a containing folder.", url));
		}

		/// <summary>
		/// Removes the query string from a URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static string RemoveQueryStringFromUrl(string url)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			
			// defer...
			string qs = null;
			return RemoveQueryStringFromUrl(url, ref qs);
		}

		/// <summary>
		/// Removes the query string from a URL, returning the discovered query string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="queryString"></param>
		/// <returns></returns>
		public static string RemoveQueryStringFromUrl(string url, ref string queryString)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			
			// set...
			queryString = null;

			// index...
			int index = url.IndexOf("?");
			if(index != -1)
			{
				queryString = url.Substring(index + 1);
				return url.Substring(0, index);
			}
			else
				return url;
		}

		/// <summary>
		/// Parses cookies from a response.
		/// </summary>
		/// <remarks>Included because we have had some problems getting this to work properly using the regular objects.</remarks>
		/// <param name="response"></param>
		/// <returns></returns>
		public static CookieCollection ParseCookies(HttpWebResponse response)
		{
			if(response == null)
				throw new ArgumentNullException("response");
			
			// get...
			CookieCollection results = new CookieCollection();
			string buf = response.Headers["Set-cookie"];
			if(buf != null && buf.Length > 0)
			{
				// get...
				Regex regex = new Regex(@"(?<name>[^=]+)=(?<value>[^;]*);\s*path=(?<path>[^;]*);\s*(domain=(?<domain>[^;]+);\s*)?expires=(?<expires>[^;]*),?", 
					RegexOptions.Singleline | RegexOptions.IgnoreCase);
				foreach(Match match in regex.Matches(buf))
				{
					// get...
					string name = match.Groups["name"].Value;
					string value = match.Groups["value"].Value;
					string path = match.Groups["path"].Value;
					string domain = match.Groups["domain"].Value;

					// create...
					Cookie cookie = new Cookie(name, value, path, domain);
					results.Add(cookie);
				}
			}

			// return...
			return results;
		}

		/// <summary>
		/// Transmits a document over HTTP POST.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="doc"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		[Obsolete("Use the overload that takes 'HttpResultType' instead.")]
		public static XmlDocument PostXmlDocument(string url, XmlDocument doc, HttpDownloadSettings settings)
		{
			HttpResult result = PostXmlDocument(url, doc, settings, HttpResultType.Xml);
			if(result == null)
				throw new InvalidOperationException("result is null.");

			// return...
			return result.ResultAsXml;
		}

		/// <summary>
		/// Transmits a document over HTTP POST.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="doc"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static HttpResult PostXmlDocument(string url, XmlDocument doc, HttpDownloadSettings settings, 
			HttpResultType expectedReturnType)
		{
			// create...
			HttpDownloadContext context = new HttpDownloadContext();
			context.Settings = settings;
			context.ExpectedReturnType = expectedReturnType;

			// defer...
			return PostXmlDocument(url, doc, context);
		}

		/// <summary>
		/// Transmits a document over HTTP POST.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="doc"></param>
		/// <param name="settings"></param>
		/// <returns></returns>
		public static HttpResult PostXmlDocument(string url, XmlDocument doc, HttpDownloadContext context)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");
			if(doc == null)
				throw new ArgumentNullException("doc");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// create...
			HttpWebRequest request = context.CreateRequest(url);
			if(request == null)
				throw new InvalidOperationException("request is null.");

			// post...
			request.Method = "POST";
			request.ContentType = "text/xml";

			// bytes...
			using(Stream outStream = request.GetRequestStream())
				doc.Save(outStream);

			// before...
			context.OnBeforeRequestSent(EventArgs.Empty);

			// request...
			try
			{
				using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					return HandleHttpResult(response, context.ExpectedReturnType);
			}
			catch(Exception ex)
			{
				throw CreateDownloadException("Failed to post XML document.", request, ex);
			}
		}

		public static HttpResult HandleHttpResult(HttpWebResponse response, HttpResultType type)
		{
			if(response == null)
				throw new ArgumentNullException("response");
			
			// get the encoding...
			Encoding encoding = Encoding.UTF8;
			if(response.ContentEncoding != null && response.ContentEncoding.Length > 0)
				encoding = Encoding.GetEncoding(response.ContentEncoding);

            // what URL responded?
            Uri respondingUrl = response.ResponseUri;

			// stream...
			Stream inStream = response.GetResponseStream();
			if(inStream == null)
				throw new InvalidOperationException("inStream is null.");
			using(inStream)
			{
				// create a reader...
				StreamReader reader = new StreamReader(inStream, encoding);

				// switch...
				switch(type)
				{
					case HttpResultType.Xml:

						// read it...
						string xml = reader.ReadToEnd();

						// anything?
						if(xml != null && xml.Length > 0)
						{
							XmlDocument doc = new XmlDocument();
							try
							{
								doc.LoadXml(xml);
							}
							catch(Exception ex)
							{
								throw new InvalidOperationException(string.Format("The XML returned from the server was invalid and cannot be processed.\r\nData: {0}", xml), ex);
							}

							// return...
							return new HttpResult(doc, HttpResponseStatus.ResponseOk, response.StatusCode, respondingUrl, null);
						}
						else
                            return new HttpResult(null, HttpResponseStatus.ResponseOk, response.StatusCode, respondingUrl, null);

					case HttpResultType.String:
                        return new HttpResult(reader.ReadToEnd(), HttpResponseStatus.ResponseOk, response.StatusCode, respondingUrl, null);

					case HttpResultType.None:
                        return new HttpResult(null, HttpResponseStatus.ResponseOk, response.StatusCode, respondingUrl, null);

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
				}
			}
		}

		/// <summary>
		/// Gets the HTTP status code for the given URL.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public static HttpResult GetHttpStatus(string url, HttpDownloadSettings settings)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			if (url.Length == 0)
				throw new ArgumentException("'url' is zero-length.");

			// get...
			HttpWebRequest request = null;
			try
			{
				request = HttpHelper.CreateRequest(url, settings);
				if (request == null)
					throw new InvalidOperationException("'request' is null.");

				using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
					return new HttpResult(null, HttpResponseStatus.ResponseOk, response.StatusCode, response.ResponseUri, null);
			}
			catch (WebException ex)
			{
                // mbr - 2009-08-26 - moved to method...
                //// do we have a HttpWebResponse...
                //if(ex.Response != null)
                //{
                //    if(ex.Response is HttpWebResponse)
                //        return new HttpResult(null, HttpResponseStatus.ResponseOk, ((HttpWebResponse)ex.Response).StatusCode, ex);
                //    else
                //        return new HttpResult(null, HttpResponseStatus.ResponseIsNotHttp, HttpStatusCode.InternalServerError, ex);
                //}
                //else
                //    return new HttpResult(null, HttpResponseStatus.NoResponse, HttpStatusCode.InternalServerError, ex);
                return HandleHttpResultException(ex);
			}
		}

        private static HttpResult HandleHttpResultException(WebException ex)
        {
            if (ex.Response != null)
            {
                if (ex.Response is HttpWebResponse)
                    return new HttpResult(null, HttpResponseStatus.ResponseOk, ((HttpWebResponse)ex.Response).StatusCode, ((HttpWebResponse)ex.Response).ResponseUri, ex);
                else
                    return new HttpResult(null, HttpResponseStatus.ResponseIsNotHttp, HttpStatusCode.InternalServerError, null, ex);
            }
            else
                return new HttpResult(null, HttpResponseStatus.NoResponse, HttpStatusCode.InternalServerError, null, ex);
        }

        public static string AppendVariableToQueryString(string url, string name, object value)
        {
            var values = new Dictionary<string, object>();
            values[name] = value;
            return AppendVariablesToQueryString(url, values);
        }

    	/// <summary>
		/// Appends a collection of variables to the given string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="values"></param>
		// mbr - 07-12-2007 - added.
        public static string AppendVariablesToQueryString(string url, IDictionary values)
        {
            return AppendVariablesToQueryString(url, values, null);
        }

		/// <summary>
		/// Appends a collection of variables to the given string.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="values"></param>
		// mbr - 2009-06-30 - added callback...
		public static string AppendVariablesToQueryString(string url, IDictionary values, QueryStringDataFormatCallback callback)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(values == null)
				throw new ArgumentNullException("values");
			
			// builder...
			StringBuilder builder = new StringBuilder();
			builder.Append(url);

			// get...
			string qs = BuildQueryString(values, callback);
			if(qs == null)
				throw new ArgumentNullException("qs");
			if(qs.Length > 0)
			{
                if (url.Length > 0)
                {
                    if (url.IndexOf("?") == -1)
                        builder.Append("?");
                    else
                        builder.Append("&");
                }

				// add...
				builder.Append(qs);
			}

			// return...
			string result = builder.ToString();
			return result;
		}

		/// <summary>
		/// Builds a query string from the given values.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		// mbr - 24-03-2008 - added.
        public static string BuildQueryString(IDictionary values)
        {
            return BuildQueryString(values, null);
        }

		/// <summary>
		/// Builds a query string from the given values.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
        // mbr - 2009-06-30 - added callback...
		public static string BuildQueryString(IDictionary values, QueryStringDataFormatCallback callback)
		{
			if(values == null)
				throw new ArgumentNullException("values");
			
			// build...
			StringBuilder builder = new StringBuilder();
			BuildQueryString(builder, values, callback);

			// return...
			return builder.ToString();
		}

        // mbr - 2009-06-30 - added callback...
//		private static void BuildQueryString(StringBuilder builder, IDictionary values)
		private static void BuildQueryString(StringBuilder builder, IDictionary values, QueryStringDataFormatCallback callback)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(values == null)
				throw new ArgumentNullException("values");

			// sep...
			bool first = true;
			foreach(object key in values.Keys)
			{
				if(first)
					first = false;
				else
					builder.Append("&");

				// append...
				AppendValue(builder, key);
				builder.Append("=");

                // format...
                string asString = null;
                if(callback != null)
                    asString = callback(values[key]);
                else
                    asString = ConversionHelper.ToString(values[key], Cultures.System);

                // add...
				AppendValue(builder, asString);
			}
		}

		private static void AppendValue(StringBuilder builder, object value)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");

			// convert and append...
			string asString = ConversionHelper.ToString(value, Cultures.System);
			builder.Append(HttpUtility.UrlEncode(asString));
		}

        public static Dictionary<string, string> GetUrlParameters(string url)
        {
            var results = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var index = url.IndexOf('?');
            if(index != -1)
            {
                var qs = url.Substring(index + 1);

                var parsed = HttpUtility.ParseQueryString(qs);
                foreach (string key in parsed.Keys)
                {
                    if (key != null)
                        results[key] = parsed[key];
                }
            }

            return results;
        }

        public static string SetUrlParameters(string url, Dictionary<string, string> values, bool replace = true)
        {
            Dictionary<string, string> toSet = null;
            if(!(replace))
            {
                toSet = GetUrlParameters(url);
                foreach (var key in values.Keys)
                    toSet[key] = values[key];
            }
            else
                toSet = values;

            // get...
            var qs = BuildQueryString(toSet);
            var index = url.IndexOf('?');
            if (index != -1)
                url = url.Substring(0, index);

            // rebuild...
            if (qs.Length > 0)
            {
                url += "?";
                url += qs;
            }

            // return...
            return url;
        }

        public static string SetUrlParameter(string url, string name, string value)
        {
            var values = new Dictionary<string, string>();
            values[name] = value;
            return SetUrlParameters(url, values, false);
        }

        public static string RemoveUrlParameter(string url, string name)
        {
            var values = GetUrlParameters(url);
            if (values.ContainsKey(name))
            {
                values.Remove(name);
                return SetUrlParameters(url, values, true);
            }
            else
                return url;
        }

        public static string GetUrlSchemeAndHostnameAndPort(Uri uri)
        {
            var builder = new StringBuilder();

            var isSecure = string.Compare(uri.Scheme, "https", true) == 0;
            if (isSecure)
            {
                builder.Append("https://");
                builder.Append(uri.Host);
                if (uri.Port != 443)
                {
                    builder.Append(":");
                    builder.Append(uri.Port);
                }
            }
            else
            {
                builder.Append("http://");
                builder.Append(uri.Host);
                if (uri.Port != 80)
                {
                    builder.Append(":");
                    builder.Append(uri.Port);
                }
            }

            return builder.ToString();
        }

        public static HttpWebResponse GetResponseWithErrors(this HttpWebRequest request)
        {
            try
            {
                return (HttpWebResponse)request.GetResponse();
            }
            catch (Exception ex)
            {
                var error = GetErrorFromWebRequest(ex);
                throw new HttpHelperException(string.Format("The HTTP request failed.\r\nURI/method: {0} [{1}]\r\nError: {2}", 
                    request.RequestUri, request.Method, error), error, ex);
            }
        }

        private static string GetErrorFromWebRequest(Exception ex)
        {
            while (ex != null)
            {
                if (ex is WebException)
                {
                    var webEx = (WebException)ex;
                    if (webEx.Response != null)
                    {
                        using (var stream = webEx.Response.GetResponseStream())
                            return new StreamReader(stream).ReadToEnd();
                    }
                    else
                        return "(No response)";
                }

                ex = ex.InnerException;
            }

            return null;
        }

        public static string AddNonce(string url)
        {
            return AppendVariableToQueryString(url, "nonce", Environment.TickCount);
        }

        public static string GetResponseText(this HttpWebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                if (string.Compare(response.ContentEncoding, "gzip", true) == 0)
                {
                    using (var zip = new GZipStream(stream, CompressionMode.Decompress))
                        return new StreamReader(zip).ReadToEnd();
                }
                else
                    return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}
