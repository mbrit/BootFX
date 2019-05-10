// BootFX - Application framework for .NET applications
// 
// File: InstallationSettings.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using BootFX.Common.Data;
using BootFX.Common.Email;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Defines a class that holds installation settings.
	/// </summary>
	/// <remarks>This class defines a standard way of holding connection strings and exception reporting configuration for an application.
	/// This allows for a standard UI to control such things.</remarks>
	public class InstallationSettings : SimpleXmlPropertyBag
	{
        private const string ConnectionStringKey = "ConnectionString";

		/// <summary>
		/// Private field to support <see cref="Name"/> property.
		/// </summary>
		private string _name;
		
		private const string DefaultTopElementName = "InstallationSettings";

		/// <summary>
		/// Private field to support <see cref="UseEventLogSourceName"/> property.
		/// </summary>
		private string _useEventLogSourceName;
		
		/// <summary>
		/// Private field to support <see cref="FilePath"/> property.
		/// </summary>
		private string _filePath;

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 28-07-2007 - made internal, added name...		
		public InstallationSettings()
		{
		}

		/// <summary>
		/// Gets or sets the connection type.
		/// </summary>
		public Type ConnectionType
		{
			get
			{
				return this.GetTypeValue("ConnectionType", typeof(SqlServerConnection), Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				// check...
				if(value != null && !(typeof(IConnection).IsAssignableFrom(value)))
					throw new InvalidOperationException(string.Format("The type '{0}' does not implement IConnection.", value));

				// set...
				this.SetValue("ConnectionType", value);
			}
		}

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		public string ConnectionString
		{
			get
			{
                // mbr - 2009-06-05 - changed this to handle encryption...
                if (this.IsConnectionStringEncrypted)
                    return this.GetEncryptedStringValue(ConnectionStringKey, null, OnNotFound.ReturnNull);
                else
                    return this.RawConnectionString;
			}
			set
			{
                // mbr - 2009-06-05 - are we allowed to do encrypted?
                if (Runtime.Current.Application.StartArgs.AllowEncryptedConnectionStrings)
                {
                    this.SetEncryptedStringValue(ConnectionStringKey, value);
                    this.IsConnectionStringEncrypted = true;
                }
                else
                {
                    this.RawConnectionString = value;
                    this.IsConnectionStringEncrypted = false;
                }
			}
		}

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        // mbr - 2009-06-05 - added...
        private string RawConnectionString
        {
            get
            {
                return this.GetStringValue(ConnectionStringKey, null, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this.SetValue(ConnectionStringKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        // mbr - 2009-06-05 - added...
        private bool IsConnectionStringEncrypted
        {
            get
            {
                return this.GetBooleanValue("ConnectionStringEncrypted", false, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this.SetValue("ConnectionStringEncrypted", value);
            }
        }

        /// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		[Obsolete("E-mail log transmission is now deprecated.")]
		public string EmailReportingHostName
		{
			get
			{
				return this.GetStringValue("EmailReportingHostName", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("EmailReportingHostName", value);
			}
		}

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		[Obsolete("E-mail log transmission is now deprecated.")]
		public string EmailReportingUsername
		{
			get
			{
				return this.GetStringValue("EmailReportingUsername", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("EmailReportingUsername", value);
			}
		}

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		[Obsolete("E-mail log transmission is now deprecated.")]
		public string EmailReportingPassword
		{
			get
			{
				return this.GetStringValue("EmailReportingPassword", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("EmailReportingPassword", value);
			}
		}
		
		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		[Obsolete("E-mail log transmission is now deprecated.")]
		public string EmailReportingFrom
		{
			get
			{
				return this.GetStringValue("EmailReportingFrom", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("EmailReportingFrom", value);
			}
		}

		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		[Obsolete("E-mail log transmission is now deprecated.")]
		public string EmailReportingTo
		{
			get
			{
				return this.GetStringValue("EmailReportingTo", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("EmailReportingTo", value);
			}
		}

		/// <summary>
		/// Returns true if e-mail reporting is properly configured.
		/// </summary>
		/// <remarks>This means, is hostname, from and to all set.</remarks>
		[Obsolete("E-mail log transmission is now deprecated.")]
		public bool IsEmailReportingProperlyConfigured
		{
			get
			{
				string hostName = this.GetStringValue("EmailReportingHostName", null, Cultures.System, OnNotFound.ReturnNull);
				string from = this.GetStringValue("EmailReportingFrom", null, Cultures.System, OnNotFound.ReturnNull);
				string to = this.GetStringValue("EmailReportingTo", null, Cultures.System, OnNotFound.ReturnNull);

				// all set...
				if(hostName != null && hostName.Length > 0 && from != null && from.Length > 0 && to != null && to.Length > 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
		public bool UseSmartErrorHandlingOnWeb
		{
			get
			{
				return this.GetBooleanValue("UseSmartErrorHandlingOnWeb", true, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("UseSmartErrorHandlingOnWeb", value);
			}
		}

		/// <summary>
		/// Gets or sets whether exception information should be shown to the user.
		/// </summary>
		public bool ShowExceptionDetailOnUi
		{
			get
			{
				return this.GetBooleanValue("ShowExceptionDetailOnUi", true, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("ShowExceptionDetailOnUi", value);
			}
		}

        ///// <summary>
        ///// Gets or sets the level of messages that should be written to the event log.
        ///// </summary>
        //[Obsolete("E-mail log transmission is now deprecated.")]
        //public bool DoEmailReporting
        //{
        //    get
        //    {
        //        return this.GetBooleanValue("DoEmailReporting", false, Cultures.System, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        this.SetValue("DoEmailReporting", value);
        //    }
        //}

        // mbr - 2009-12-18 - deprecated.
        ///// <summary>
        ///// Gets or sets the level of messages that should be written to the event log.
        ///// </summary>
        //public string DatabaseLoggingTableNativeName
        //{
        //    get
        //    {
        //        string result = this.GetStringValue("DatabaseLoggingTableNativeName", null, Cultures.System, OnNotFound.ReturnNull);
        //        if(result == null || result.Length == 0)
        //            return DatabaseAppender.DefaultTableName;
        //        else
        //            return result;
        //    }
        //    set
        //    {
        //        this.SetValue("DatabaseLoggingTableNativeName", value);
        //    }
        //}

        ///// <summary>
        ///// Gets the maximum number of days to keep database log entries.
        ///// </summary>
        ///// <remarks>This now only considers non-audit items.</remarks>
        //public int MaxDatabaseLogHistoryDays
        //{
        //    get
        //    {
        //        return this.GetInt32Value("MaxDatabaseLogHistoryDays", 90, Cultures.System, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        if(value < 0)
        //            throw new ArgumentOutOfRangeException("value", value, "Value must be zero or a positive integer.");
        //        this.SetValue("MaxDatabaseLogHistoryDays", value);
        //    }
        //}

        ///// <summary>
        ///// Gets the maximum number of days to keep database log entries.
        ///// </summary>
        //// mbr - 20-08-2007 - added.		
        //public int MaxDatabaseAuditLogHistoryDays
        //{
        //    get
        //    {
        //        return this.GetInt32Value("MaxDatabaseAuditLogHistoryDays", 0, Cultures.System, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        if(value < 0)
        //            throw new ArgumentOutOfRangeException("value", value, "Value must be zero or a positive integer.");
        //        this.SetValue("MaxDatabaseAuditLogHistoryDays", value);
        //    }
        //}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
		public bool DoPhoneHomeLogging
		{
			get
			{
				return this.GetBooleanValue("DoPhoneHomeLogging", false, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("DoPhoneHomeLogging", value);
			}
		}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
		public string PhoneHomeUrl
		{
			get
			{
                // mbr - 2009-10-01 - problem here where the option could get clear in installation settings...
                const string def = "https://www.errorreport.info/webservices/receiveph.aspx";
				string url = this.GetStringValue("PhoneHomeUrl", def, Cultures.System, OnNotFound.ReturnNull);
                if (string.IsNullOrEmpty(url))
                    url = def;

                // return...
                return url;
			}
			set
			{
				this.SetValue("PhoneHomeUrl", value);
			}
		}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
		public string PhoneHomeKey
		{
			get
			{
				return this.GetStringValue("PhoneHomeKey", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("PhoneHomeKey", value);
			}
		}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
        // mbr - 2009-12-18 - deprecated...
        //public bool DoDatabaseLogging
        //{
        //    get
        //    {
        //        return this.GetBooleanValue("DoDatabaseLogging", false, Cultures.System, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        this.SetValue("DoDatabaseLogging", value);
        //    }
        //}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
		public bool DoEventLogReporting
		{
			get
			{
				return this.GetBooleanValue("DoEventLogReporting", true, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("DoEventLogReporting", value);
			}
		}

        ///// <summary>
        ///// Gets or sets the level of messages that should be written to the event log.
        ///// </summary>
        //public LogLevel DatabaseLoggingLevel
        //{
        //    get
        //    {
        //        return (LogLevel)this.GetEnumerationValue("DatabaseLoggingLevel", LogLevel.Warn, typeof(LogLevel), Cultures.System, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        this.SetValue("DatabaseLoggingLevel", value);
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the level of messages that should be written to the event log.
        ///// </summary>
        //[Obsolete("E-mail log transmission is now deprecated.")]
        //public LogLevel EmailReportingLevel
        //{
        //    get
        //    {
        //        return (LogLevel)this.GetEnumerationValue("EmailReportingLevel", LogLevel.Warn, typeof(LogLevel), Cultures.System, OnNotFound.ReturnNull);
        //    }
        //    set
        //    {
        //        this.SetValue("EmailReportingLevel", value);
        //    }
        //}

		/// <summary>
		/// Gets or sets the level of messages that should be written to the event log.
		/// </summary>
		public LogLevel EventLogLevel
		{
			get
			{
				return (LogLevel)this.GetEnumerationValue("EventLogLevel", LogLevel.Warn, typeof(LogLevel), Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("EventLogLevel", value);
			}
		}

		/// <summary>
		/// Gets the useeventlogsourcename.
		/// </summary>
		private string UseEventLogSourceName
		{
			get
			{
				return _useEventLogSourceName;
			}
			set
			{
				_useEventLogSourceName = value;
			}
		}

		/// <summary>
		/// Gets or sets the event log source.
		/// </summary>
		public string EventLogSourceName
		{
			get
			{
				string name = this.GetStringValue("EventLogSourceName", null, Cultures.System, OnNotFound.ReturnNull);
				if(name == null || name.Length == 0)
				{
					name = this.UseEventLogSourceName;
					if(name == null || name.Length == 0)
					{
                        // mbr - 2011-01-19 - changed this to allow override through args...
                        name = Runtime.Current.Application.StartArgs.EventLogSourceName;
                        if (!(string.IsNullOrEmpty(name)))
                            _useEventLogSourceName = name;
                        else
                        {
                            name = Runtime.Current.Application.ProductName;
                            if (name != null && name.Length > 0)
                            {
                                // only the first eight chars of the name are supported by Windows, so remove spaces and return just the first eight...
                                StringBuilder builder = new StringBuilder();
                                foreach (char c in name)
                                {
                                    if (!(char.IsWhiteSpace(c)))
                                    {
                                        builder.Append(c);
                                        if (builder.Length == 8)
                                            break;
                                    }
                                }

                                // get...
                                name = builder.ToString();
                            }

                            // set...
                            _useEventLogSourceName = name;
                        }
					}
				}

				// return...
				return name;
			}
			set
			{
				this.SetValue("EventLogSourceName", value);
			}
		}

		/// <summary>
		/// Gets the filepath.
		/// </summary>
		private string FilePath
		{
			get
			{
				return _filePath;
			}
		}

		/// <summary>
		/// Sets the file path.
		/// </summary>
		/// <param name="filePath"></param>
		internal void SetFilePath(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			_filePath = filePath;

			// TODO: start monitoring this file for changes.
		}

		/// <summary>
		/// Saves the installation settings.
		/// </summary>
		public void Save()
		{
			if(FilePath == null)
				throw new InvalidOperationException("'FilePath' is null.");
			if(FilePath.Length == 0)
				throw new InvalidOperationException("'FilePath' is zero-length.");

			// directory?
			string folderPath = Path.GetDirectoryName(this.FilePath);
			if(!(Directory.Exists(folderPath)))
				Directory.CreateDirectory(folderPath);

			// save...
			this.Save(this.FilePath, DefaultTopElementName, true);
		}

		protected override void OnSaved(EventArgs e)
		{
			base.OnSaved (e);

			// if the file has been saved, redo the logs...  (we can't redo connection strings as this would probably break the app, 
			// but logs should be fairly innocuous.)
			LogSet.IncrementContextId();
		}

		/// <summary>
		/// Creates the given event log source.
		/// </summary>
		public void EnsureEventLogSourceExists()
		{
			string name = this.EventLogSourceName;
			if(name == null)
				throw new InvalidOperationException("'name' is null.");
			if(name.Length == 0)
				throw new InvalidOperationException("'name' is zero-length.");

			// check...
			if(!(EventLog.SourceExists(name)))
				EventLog.CreateEventSource(name, name);
		}

		[Obsolete("E-mail log transmission is now deprecated.")]
		public void SendTestEmailReport()
		{
			// check...
			if(!(this.IsEmailReportingProperlyConfigured))
				throw new InvalidOperationException("E-mail reporting is not properly configured.");

			// get...
			ISmtpProvider provider = MailProviderFactory.GetSmtpProvider();
			provider.Settings = new SmtpConnectionSettings(this.EmailReportingHostName, this.EmailReportingUsername, this.EmailReportingPassword);

			// send...
			MailMessage message = new MailMessage();
			message.FromAsString = this.EmailReportingFrom;
			message.ToAsString = this.EmailReportingTo;
			message.Subject = string.Format("E-mail Error Reporting Test - {0} - {1}", Runtime.Current.Application.ProductCompany, Runtime.Current.Application.ProductName);
			message.Body = "This is a test message from E-mail Error Reporting.";

			// send...
			provider.Send(message);
		}

        //public void WriteTestDatabaseLogEntry()
        //{
        //    if(DatabaseLoggingTableNativeName == null)
        //        throw new InvalidOperationException("'DatabaseLoggingTableNativeName' is null.");
        //    if(DatabaseLoggingTableNativeName.Length == 0)
        //        throw new InvalidOperationException("'DatabaseLoggingTableNativeName' is zero-length.");

        //    // write...
        //    const string message = "This is a test message.";
        //    LogData data = new LogData(LogLevel.Info, message, null, message);
        //    DatabaseAppender.WriteEntry(this.GetConnectionSettingsForLogging(), this.DatabaseLoggingTableNativeName, data);
        //}

		public void WriteTestEventLogEntry()
		{
			if(EventLogSourceName == null)
				throw new InvalidOperationException("'EventLogSourceName' is null.");
			if(EventLogSourceName.Length == 0)
				throw new InvalidOperationException("'EventLogSourceName' is zero-length.");

			// ensure...
			try
			{
				this.EnsureEventLogSourceExists();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException("Failed to check (or create) the event source.", ex);
			}

			// write...
			EventLog.WriteEntry(this.EventLogSourceName, "This is a test message.", EventLogEntryType.Information);
		}
			 
		/// <summary>
		/// Returns true if the database type has been properly configured.
		/// </summary>
		internal bool IsDatabaseProperlyConfigured
		{
			get
			{
				if(this.ConnectionType != null && this.ConnectionString != null && this.ConnectionString.Length > 0)
					return true;
				else
					return false;
			}
		}
		
		/// <summary>
		/// Gets connection settings for the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ConnectionSettings GetConnectionSettings(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		/// <summary>
		/// Gets the connection settings used for logging.
		/// </summary>
		/// <returns></returns>
		public ConnectionSettings GetConnectionSettingsForLogging()
		{
			return Database.DefaultConnectionSettings;
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
		public WebProxyMode WebProxyMode
		{
			get
			{
				return (WebProxyMode)this.GetEnumerationValue("WebProxyMode", WebProxyMode.None, typeof(WebProxyMode), 
					Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyMode", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public WebAuthenticationType WebProxyAuthenticationType
		{
			get
			{
				return (WebAuthenticationType)this.GetEnumerationValue("WebProxyAuthenticationType", WebAuthenticationType.None, typeof(WebAuthenticationType), 
					Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyAuthenticationType", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public string WebProxyHostName
		{
			get
			{
				return this.GetStringValue("WebProxyHostName", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyHostName", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public string WebProxyUsername
		{
			get
			{
				return this.GetStringValue("WebProxyUsername", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyUsername", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public string WebProxyPassword
		{
			get
			{
				return this.GetStringValue("WebProxyPassword", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyPassword", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public string WebProxyDomainName
		{
			get
			{
				return this.GetStringValue("WebProxyDomainName", null, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyDomainName", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public int WebProxyPort
		{
			get
			{
				return this.GetInt32Value("WebProxyPort", 0, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyPort", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        public bool WebProxyBypassLocal
		{
			get
			{
				return this.GetBooleanValue("WebProxyBypassLocal", true, Cultures.System, OnNotFound.ReturnNull);
			}
			set
			{
				this.SetValue("WebProxyBypassLocal", value);
			}
		}

        // mbr - 2009-10-01 - removed...
        [Obsolete("Deprecated.")]
        internal IWebProxy GetWebProxy()
		{
			switch(WebProxyMode)
			{
				case WebProxyMode.None:
					return null;

				case WebProxyMode.Default:
					return WebRequest.DefaultWebProxy;

                case WebProxyMode.Specific:

					// host...
					string host = this.WebProxyHostName;
					if(host == null || host.Length == 0)
						throw new InvalidOperationException("Proxy host name was not supplied.");
					if(this.WebProxyPort == 0)
						throw new InvalidOperationException("Proxy port is invalid.");
					// return...
					WebProxy proxy = new WebProxy(string.Format("{0}:{1}", host, this.WebProxyPort), this.WebProxyBypassLocal);

					// auth...
					switch(WebProxyAuthenticationType)
					{
						case WebAuthenticationType.None:
							break;

						case WebAuthenticationType.Integrated:
							proxy.Credentials = CredentialCache.DefaultCredentials;
							break;

						case WebAuthenticationType.Specific:
							proxy.Credentials = new NetworkCredential(WebProxyUsername, WebProxyPassword, WebProxyDomainName);
							break;

						default:
							throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", WebProxyAuthenticationType, WebProxyAuthenticationType.GetType()));
					}

					// return...
					return proxy;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", WebProxyMode, WebProxyMode.GetType()));
			}
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		internal void SetName(string name)
		{
			_name = name;
		}

        public bool IsTraceEnabled
        {
            get
            {
                return this.GetBooleanValue("IsTraceEnabled", false, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this.SetValue("IsTraceEnabled", value);
            }
        }

        // AJ 2014-06-11
		// Used to return from the installation settings if we want snapshot isolation in this farm.
        public bool IsSnapshotIsolation
        {
            get
            {
                return this.GetBooleanValue("IsSnapshotIsolation", false, Cultures.System, OnNotFound.ReturnNull);
            }
            set
            {
                this.SetValue("IsSnapshotIsolation", value);
            }
        }
	}
}
