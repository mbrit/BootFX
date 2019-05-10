// BootFX - Application framework for .NET applications
// 
// File: Runtime.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Threading;
using System.Net;
using System.Web;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using Microsoft.Win32;
using System.Security.Cryptography;
using BootFX.Common.Crypto;
using System.Threading.Tasks;

namespace BootFX.Common
{
	/// <summary>
	/// Provides UI-agnostic runtime functionality to the application.
	/// </summary>
	public class Runtime : Loggable, IDisposable
	{
		private Random _userPasswordRandom = new Random();
		private object _userPasswordLock = new object();

		/// <summary>
		/// Private field to support <see cref="PasswordLetters"/> property.
		/// </summary>
		private static char[] _passwordLetters = new char[] { 'a', 'b', 'c', 'd', 'f', 'g', 'h', 'j', 'l', 'm', 'n', 'p', 'q', 'r', 's', 'v', 'w', 'x', 'y', 'z' };
		
		/// <summary>
		/// Private field to support <see cref="InstallationSettingsFileName"/> property.
		/// </summary>
		private string _installationSettingsFileName;
		
		/// <summary>
		/// Private field to support <c>ErrorReportingUrl</c> property.
		/// </summary>
		// mbr - 15-05-2008 - changed this to default...
		// jrn - 29-09-2008 - changed this to use installationsettings...
//		private string _phoneHomeUrl = "https://www.errorreport.info/webservices/receiveph.aspx";
		
		/// <summary>
		/// Private field to support <see cref="ConfigurationMode"/> property.
		/// </summary>
		private static RuntimeConfigMode _configurationMode;
		
		/// <summary>
		/// Raised when a thread-affinity database connection is required by the application.
		/// </summary>
		public event IConnectionEventHandler AffinityConnectionNeeded;
		
		/// <summary>
		/// Private field to support <see cref="TempFiles"/> property.
		/// </summary>
		private ArrayList _tempFiles = new ArrayList();
		private object _tempFilesLock = new object();
		
		/// <summary>
		/// Private field to support <c>IsWebProxyExplictlySet</c> property.
		/// </summary>
		private bool _isWebProxyExplictlySet;
		
		/// <summary>
		/// Private field to support <see cref="WebProxy"/> property.
		/// </summary>
		private IWebProxy _webProxy;

		/// <summary>
		/// Private field to support <c>OnlineMode</c> property.
		/// </summary>
		private OnlineMode _onlineMode = OnlineMode.Online;
		
		/// <summary>
		/// Private field to support <c>SharedSettings</c> property.
		/// </summary>
		private SharedSettings _sharedSettings = new SharedSettings();

		/// <summary>
		/// Private field to support <c>InternalManagementService</c> property.
		/// </summary>
        //private InternalManagementService _internalManagementService;
		
		/// <summary>
		/// Defines the filename for installation settings.
		/// </summary>
		// mbr - 03-11-2005 - this is a .dat file because ultimately it will be encrypted.		
		// mbr - 28-07-2007 - changed to a property...		
		//		private const string InstallationSettingsFileName = "InstallationSettings.dat";

		/// <summary>
		/// Private field to support <see cref="InstallationSettings"/> property.
		/// </summary>
		private InstallationSettings _installationSettings;

        public static bool IsUnix { get; private set; }

        private List<string> NodeNamesFirst { get; set; }
        private List<string> NodeNamesLast { get; set; }
        private object _nodeNamesLock = new object();

		/// <summary>
		/// Raised when a log is created.
		/// </summary>
        private static ILogEventHandler _logCreatedStorage;
        public event ILogEventHandler LogCreated
        {
            add
            {
                _logCreatedStorage += value;
                LogManager.Reset();
            }
            remove
            {
                _logCreatedStorage -= value;
                LogManager.Reset();
            }
        }
		
        /// <summary>
        /// Private field to support <see cref="DoPerformanceCounters"/> property.
        /// </summary>
        private bool _doPerformanceCounters = false;
		
		/// <summary>
		/// Private field to support <c>Application</c> property.
		/// </summary>
		private MbrApplication _application;

        internal IEntityPersistenceFactory EntityPersistenceFactory { get; private set; }

        private BfxLookup<Type, object, string> EnumDescriptionsLookup { get; set; }
        private BfxLookup<Type, object, string> EnumDescriptionsLookupSpaceWords { get; set; }

        public static event EventHandler Disposing;

        private PropertyBag _userSettings;
        private object _userSettingsLock = new object();

        private List<IDisposableThread> DisposableThreads { get; set; }

        /// <summary>
        /// Private field to hold the singleton instance.
        /// </summary>
        private static Runtime _current = null;

        /// <summary>
        /// Private constructor.
        /// </summary>
        private Runtime(MbrApplication application)
		{
			if(application == null)
				throw new ArgumentNullException("application");

			// setup...
			this.Initialize(application);
		}

		/// <summary>
		/// Destructor.
		/// </summary>
		~Runtime()
		{
            // mbr - 2016-06-17 - removed this, because who cares...
			//try
			//{
			//	// set...
			//	this.Dispose(DisposeType.FromFinalizer);
			//}
			//catch
			//{
			//}
		}

        static Runtime()
        {
            IsUnix = Environment.OSVersion.Platform == PlatformID.Unix;
        }

		/// <summary>
		/// Initializes the runtime.
		/// </summary>
		private void Initialize(MbrApplication application)
		{
			if(application == null)
				throw new ArgumentNullException("application");
			_application = application;

            this.DisposableThreads = new List<IDisposableThread>();

			// principal...
			AppDomain.CurrentDomain.SetPrincipalPolicy(System.Security.Principal.PrincipalPolicy.WindowsPrincipal);

            this.EnumDescriptionsLookup = new BfxLookup<Type, object, string>();
            this.EnumDescriptionsLookup.CreateItemValue += EnumDescriptionsLookup_CreateItemValue;

            this.EnumDescriptionsLookupSpaceWords = new BfxLookup<Type, object, string>();
            this.EnumDescriptionsLookupSpaceWords.CreateItemValue += (sender, e) =>
            {
                this.EnumDescriptionsLookup_CreateItemValue(sender, e);
                e.NewValue = this.SplitWords(e.NewValue);
            };

            // mbr - 07-11-2005 - redo logging...			
            ConsoleAppender.Mode = application.StartArgs.ConsoleLogMode;
            LogSet.IncrementContextId();

            // set...
            if (application.StartArgs.EntityPersistenceFactory == null)
                this.EntityPersistenceFactory = new EntityPersistenceFactory();
            else
                this.EntityPersistenceFactory = application.StartArgs.EntityPersistenceFactory;

            // mbr - 04-10-2007 - case 851 - pass in the boot assembly...			
            EntityType.Initialize(application.StartArgs.EntityTypeFactory);

            //// management...
            //_internalManagementService = new InternalManagementService();

            //// mbr - 13-12-2007 - add others that we were given...
            //this.InternalManagementService.Engines.AddRange(application.StartArgs.ManagementServiceEngines);

            //// start it...
            //InternalManagementService.Start();

			// mbr - 03-11-2005 - setup installation settings...		
			string settingsFilePath = this.InstallationSettingsFilePath;
			if(settingsFilePath == null)
				throw new InvalidOperationException("'settingsFilePath' is null.");
			if(settingsFilePath.Length == 0)
				throw new InvalidOperationException("'settingsFilePath' is zero-length.");

			// load...
			if(File.Exists(settingsFilePath))
			{
				_installationSettings = (InstallationSettings)InstallationSettings.Load(settingsFilePath, typeof(InstallationSettings));
				if(_installationSettings == null)
					throw new InvalidOperationException("_installationSettings is null.");
			}
			else
				_installationSettings = new InstallationSettings();

			// mbr - 28-07-2007 - added name...			
			_installationSettings.SetName(this.InstallationName);
			_installationSettings.SetFilePath(settingsFilePath);

			// mbr - 03-11-2005 - after we've started, reset the existing logs so that all logs go through the event log and e-mail 
			// reporting that's defined as standard in the installation...
			LogSet.IncrementContextId();

			// mbr - 25-09-2007 - tell the database what extensibility provider we're using...
			Database.SetExtensibilityProvider(application.StartArgs.DatabaseExtensibilityProviderFactory);

			// mbr - 18-03-2007 - configuration mode...
			Database.SetDefaultDatabaseFromInstallationSettings(this.InstallationSettings);
			Database.DefaultDatabaseChanged += new EventHandler(Database_DefaultDatabaseChanged);

            // mbr - 2010-04-05 - added...
            if (application.StartArgs.HasConfigSettings)
                Database.SetConfigDatabase(application.StartArgs.ConfigDatabaseType, application.StartArgs.ConfigDatabaseString);

			// mbr - 10-11-2007 - load entity types from the calling assembly...
			if(Application.StartArgs.BootAssembly == null)
				throw new InvalidOperationException("Application.StartArgs.BootAssembly is null.");

            // mbr - 2009-09-22 - there is a problem with load order on entity loads... the context
            // should help determine fixup problems...
            EntityTypeLoadContext context = EntityTypeLoadContext.Initialize();
            if (context == null)
                throw new InvalidOperationException("'context' is null.");
            using (context)
                EntityType.LoadFromAttributes(Application.StartArgs.BootAssembly);
		}

        public string SplitWords(string buf)
        {
            StringBuilder builder = new StringBuilder();
            if (!(string.IsNullOrEmpty(buf)))
            {
                var first = true;
                foreach (var c in buf)
                {
                    if (first)
                        first = false;
                    else
                    {
                        if (char.IsUpper(c) && !(builder.EndsWith(" ")))
                            builder.Append(" ");
                    }
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public IEnumerable<string> ReadCommentedFile(string path)
        {
            using (var reader = new StreamReader(path))
                return this.ReadCommentedFile(reader);
        }

        public IEnumerable<string> ReadCommentedFile(TextReader reader)
        {
            var names = new List<string>();
            while (true)
            {
                var buf = reader.ReadLine();
                if (buf == null)
                    break;

                var index = buf.IndexOf('#');
                if (index != -1)
                {
                    if (index == -0)
                        buf = string.Empty;
                    else
                        buf = buf.Substring(0, index);
                }

                buf = buf.Trim();
                if (buf.Length > 0)
                    names.Add(buf);
            }

            return names;
        }

        private void EnumDescriptionsLookup_CreateItemValue(object sender, CreateLookupItemEventArgs<Type, object, string> e)
        {
            var fields = e.Key1.GetFields();
            foreach (var field in fields)
            {
                if ((int)(field.Attributes & FieldAttributes.SpecialName) == 0)
                {
                    var fieldValue = field.GetValue(null);
                    if (object.Equals(e.Key2, fieldValue))
                    {
                        var attr = field.GetCustomAttribute<DescriptionAttribute>();
                        if (attr != null)
                            e.NewValue = attr.Description;
                        else
                            e.NewValue = e.Key2.ToString();

                        return;
                    }
                }
            }

            throw new InvalidOperationException(string.Format("Value '{0}' not found on enumeation type '{1}'.", e.Key2, e.Key1));
        }

		/// <summary>
		/// Gets the installationsettings.
		/// </summary>
		public InstallationSettings InstallationSettings
		{
			get
			{
				return _installationSettings;
			}
		}

		/// <summary>
		/// Gets an instance of an object that can retrieve application settings from the BfxConfig table in the main system database.
		/// </summary>
		public SharedSettings SharedSettings
		{
			get
			{
				// returns the value...
				return _sharedSettings;
			}
		}

		/// <summary>
		/// Gets the path used for installation settings.
		/// </summary>
		/// <remarks>This is either the default location for the application, or it can be overridden by setting the <c>BfxInstallationSettingsFilePath</c> 
		/// value in 'app.config'.</remarks>
		public string InstallationSettingsFilePath
		{
			get
			{
                var path = this.Application.StartArgs.InstallationSettingsFilePath;
				return path;
			}
		}

		/// <summary>
		/// Gets the application.
		/// </summary>
		public MbrApplication Application
		{
			get
			{
				return _application;
			}
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static Runtime Current
		{
			get
			{
				AssertIsStarted();
				return _current;
			}
		}
	
		/// <summary>
		/// Throws an exception if the runtime is not started.
		/// </summary>
		internal static void AssertIsStarted()
		{
			if(IsStarted == false)
				throw new RuntimeNotStartedException();
		}

		/// <summary>
		/// Returns true if the runtime is started.
		/// </summary>
		/// <returns></returns> 
		public static bool IsStarted
		{
			get
			{
				if(_current == null)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Returns true if the runtime should support dynamic performance counters.
		/// </summary>
		public bool DoPerformanceCounters
		{
			get
			{
				return _doPerformanceCounters;
			}
		}

		///// <summary>
		///// Starts the runtime.
		///// </summary>
		//[Obsolete("This 'Start' method is not supported.  Call the version that takes company, name and version.", false)]
		//public static void Start()
		//{
		//	// setup...
		//	_current = new Runtime(new MbrApplication("MBR", "BootFX", "Test", new Version(1,0,0,0), 
		//		new RuntimeStartArgs(Assembly.GetCallingAssembly())));
		//}

		//[Obsolete("Use the 'Start' method that takes a module.", false)]
		//public static void Start(string productCompany, string productName, Version productVersion)
		//{
		//	Start(productCompany, productName, "Application", productVersion, new RuntimeStartArgs(Assembly.GetCallingAssembly()));
		//}

		/// <summary>
		/// Starts the BootFX Runtime.
		/// </summary>
		/// <param name="productCompany"></param>
		/// <param name="productName"></param>
		/// <param name="productModule"></param>
		/// <param name="productVersion"></param>
		public static IDisposable Start(string productCompany, string productName, string productModule, Version productVersion)
		{
			// mbr - 05-09-2007 - defer.
			return Start(productCompany, productName, productModule, productVersion, new RuntimeStartArgs(Assembly.GetCallingAssembly()));
		}

		/// <summary>
		/// Starts the BootFX Runtime.
		/// </summary>
		/// <param name="productCompany"></param>
		/// <param name="productName"></param>
		/// <param name="productModule"></param>
		/// <param name="productVersion"></param>
		// mbr - 05-09-2007 - added.		
		public static IDisposable Start(string productCompany, string productName, string productModule, Version productVersion,
			RuntimeStartArgs args)
		{
			_current = new Runtime(new MbrApplication(productCompany, productName, productModule, productVersion, 
				args));
            return _current;
		}

		///// <summary>
		///// Starts the runtime with the given default database and application parameters.
		///// </summary>
		//[Obsolete("This 'Start' method is not supported.  Call the version that takes company, name and version.", false)]
		//public static void Start(string productCompany, string productName, Version productVersion, string productSupportUrl, string copyright)
		//{
		//	_current = new Runtime(new MbrApplication(productCompany, productName, "Application", productVersion, 
		//		new RuntimeStartArgs(Assembly.GetCallingAssembly())));
		//}

		///// <summary>
		///// Starts the runtime with the given default database and application parameters.
		///// </summary>
		//[Obsolete("This 'Start' method is not supported.  Call the version that takes company, name and version.", false)]
		//public static void Start(string productCompany, string productName, Version productVersion, string productSupportUrl,
		//	string copyright, ConnectionSettings connectionSettings)
		//{
		//	Start(productCompany, productName, productVersion, productSupportUrl, copyright, connectionSettings.ConnectionType, connectionSettings.ConnectionString);
		//}

		///// <summary>
		///// Starts the runtime with the given default database and application parameters.
		///// </summary>
		///// <remarks>Normally, startup should be done with the overload of this method that takes no parameters.  This is used
		///// for unit testing where a) the application configuration cannot be inferred from the entry assembly (there is none), 
		///// and b) where a .config file is harder to implement</remarks>
		//[Obsolete("This 'Start' method is not supported.  Call the version that takes company, name and version.", false)]
		//public static void Start(string productCompany, string productName, Version productVersion, string productSupportUrl,
		//	string copyright, Type defaultDatabaseType, string defaultConnectionString)
		//{
		//	if(defaultDatabaseType == null)
		//		throw new ArgumentNullException("defaultDatabaseType");
		//	if(defaultConnectionString == null)
		//		throw new ArgumentNullException("defaultConnectionString");
		//	if(defaultConnectionString.Length == 0)
		//		throw new ArgumentOutOfRangeException("'defaultConnectionString' is zero-length.");

		//	// setup...
		//	//			_current = new Runtime(new MbrApplication(productCompany, productName, productVersion, productSupportUrl, copyright), defaultDatabaseType, defaultConnectionString);
		//	_current = new Runtime(new MbrApplication(productCompany, productName, "Application", productVersion, 
		//		new RuntimeStartArgs(Assembly.GetCallingAssembly())));
		//	Database.SetDefaultDatabase(defaultDatabaseType, defaultConnectionString);
		//}

		/// <summary>
		/// Disposes the runtime.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(DisposeType.FromDispose);
		}

		/// <summary>
		/// Disposes the runtime.
		/// </summary>
		private void Dispose(DisposeType disposeType)
		{
            OnDisposing();

			// mbr - 12-02-2007 - don't do this if we're a Web app...
            try
            {
                // mbr - 21-09-2007 - reset the property bag...
                ConnectionStringsPropertyBag.Deinitialize();

                this.DisposeDisposableThreads();

                // cleanup...
                this.CleanupTempFiles();
            }
            catch (Exception ex)
            {
                if (this.Log.IsErrorEnabled)
                    this.Log.Error("Failed to teardown runtime.", ex);
            }
            finally
            {
                // reset...
                _current = null;
            }

			// sup...
			GC.SuppressFinalize(this);
		}

        private void DisposeDisposableThreads()
        {
            try
            {
                foreach (var thread in this.DisposableThreads)
                {
                    Task.Run(() =>
                    {
                        if (thread.Thread.IsAlive)
                        {
                            var name = thread.Thread.Name;
                            this.LogInfo(() => string.Format("Disposing thread '{0}'...", name));
                            thread.Cancel();

                            if (!(thread.Thread.Join(TimeSpan.FromSeconds(30))))
                            {
                                this.LogInfo(() => string.Format("Aborting thread '{0}'...", name));
                                thread.Thread.Abort();
                            }
                            
                            this.LogInfo(() => string.Format("Thread '{0}' finished.", name));
                        }
                    });
                }
            }
            finally
            {
                this.DisposableThreads = new List<IDisposableThread>();
            }
        }

        private void OnDisposing()
        {
            if (Disposing != null)
                Disposing(typeof(Runtime), EventArgs.Empty);
        }

        /// <summary>
        /// Cleans up any created temp files.
        /// </summary>
        private void CleanupTempFiles()
		{
			try
			{
				// walk...
				foreach(string tempFile in this.TempFiles)
					SafeDelete(tempFile);
			}
			finally
			{
				_tempFiles = new ArrayList();
			}
		}

		/// <summary>
		/// Gets the user's for this application data path.
		/// </summary>
		public string ApplicationDataFolderPath
		{
			get
			{
				return GetFolderPath(Environment.SpecialFolder.ApplicationData);
			}
		}

		/// <summary>
		/// Gets the user's for this application data path.
		/// </summary>
		public string AllUsersApplicationDataFolderPath
		{
			get
			{
                var path = this.Application.StartArgs.AllUsersApplicationDataFolderPath;
                if (string.IsNullOrEmpty(path))
                    return GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                else
                    return path;
			}
		}

		/// <summary>
		/// Gets the user's for this application data path.
		/// </summary>
		public string LocalApplicationDataFolderPath
		{
			get
			{
				return GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			}
		}

		/// <summary>
		/// Gets the user's for this application data path.
		/// </summary>
		public string LogsFolderPath
		{
			get
			{
				string path = this.LocalApplicationDataFolderPath;
				if(path.EndsWith("\\") == false)
					path += "\\";

				// mbr - 28-07-2007 - if we have a default installation name...
				path = path + "Logs";
				if(this.InstallationName != null && this.InstallationName.Length > 0)
					path = path + "-" + this.InstallationName;

				// return...
				return path;
			}
		}

		/// <summary>
		/// Gets the user's for this application data path.
		/// </summary>
		public string DocumentsFolderPath
		{
			get
			{
				return GetFolderPath(Environment.SpecialFolder.Personal);
			}
		}

		/// <summary>
		/// Gets the given special folder.
		/// </summary>
		/// <param name="specialFolder"></param>
		/// <returns></returns>
		private string GetFolderPath(Environment.SpecialFolder specialFolder)
		{
			if(Application == null)
				throw new ArgumentNullException("Application");
			
			// defer...
			return GetFolderPath(specialFolder, this.Application.ProductCompany, this.Application.ProductName);
		}

		/// <summary>
		/// Gets the folder path.
		/// </summary>
		/// <param name="specialFolder"></param>
		/// <returns></returns>
		public static string GetFolderPath(Environment.SpecialFolder specialFolder, string companyName, string productName)
		{
			if(companyName == null)
				throw new ArgumentNullException("companyName");
			if(companyName.Length == 0)
				throw new ArgumentOutOfRangeException("'companyName' is zero-length.");
			if(productName == null)
				throw new ArgumentNullException("productName");
			if(productName.Length == 0)
				throw new ArgumentOutOfRangeException("'productName' is zero-length.");
			
			// return...
			string path = Environment.GetFolderPath(specialFolder);
			if(path.EndsWith("\\") == false && path.EndsWith("/") == false)
				path += "\\";

			// return...
			return string.Format(Cultures.System, @"{0}{1}\{2}", path, companyName, productName);
		}

		/// <summary>
		/// Gets the folder path for the given log set.
		/// </summary>
		/// <param name="logName"></param>
		/// <returns></returns>
		public string GetLogFolderPath(string logName)
		{
			if(logName == null)
				throw new ArgumentNullException("logName");
			if(logName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("logName");
			
			// get...
			string path = this.LogsFolderPath;
			if(path.EndsWith("\\") == false)
				path += "\\";
			path += logName;

			// return...
			return path;
		}

		/// <summary>
		/// Gets a temporary text file.
		/// </summary>
		/// <returns></returns>
		public string GetTempHtmlFilePath()
		{
			return this.GetTempFilePath(".html");
		}

		/// <summary>
		/// Gets a temporary text file.
		/// </summary>
		/// <returns></returns>
		public string GetTempXmlFilePath()
		{
			return this.GetTempFilePath(".xml");
		}

		/// <summary>
		/// Gets a temporary text file.
		/// </summary>
		/// <returns></returns>
		public string GetTempTextFilePath()
		{
			return this.GetTempFilePath(".txt");
		}

		/// <summary>
		/// Gets a temporary file.
		/// </summary>
		/// <returns></returns>
		public string GetTempFilePath()
		{
			return GetTempFilePath(".tmp");
		}

		/// <summary>
		/// Gets a temporary file.
		/// </summary>
		/// <param name="extension"></param>
		/// <returns></returns>
		public string GetTempFilePath(string extension)
		{
			if(extension == null || extension.Length == 0)
				extension = ".tmp";
			else if(extension.Length > 0 && !(extension.StartsWith(".")))
				extension = "." + extension;

			// stop other threads doing this.
			lock(_tempFilesLock)
			{
				string root = Path.GetTempPath();
				if(!(root.EndsWith("\\")) && !(root.EndsWith("/")))
					root += "\\";
				root += this.Application.TempFilenamePrefix;

				// walk...
				while(true)
				{
					// get one...
					string tempFilePath = root + Guid.NewGuid().ToString() + extension;

					// got it?
					if(!(File.Exists(tempFilePath)))
					{
						// register...
						this.RegisterTempFile(tempFilePath);

						// stop...
						return tempFilePath;
					}
				}
			}
		}

		private void RegisterTempFile(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			// add...
			lock(_tempFilesLock)
			{
				if(this.TempFiles.Count < 10000)
					this.TempFiles.Add(filePath);
			}
		}

		/// <summary>
		/// Gets the tempfiles.
		/// </summary>
		private ArrayList TempFiles
		{
			get
			{
				return _tempFiles;
			}
		}

		/// <summary>
		/// Gets a friendly version of the given filename.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		// mbr - 19-01-2006 - added.
		public string GetFriendlyTempFilePath(string fileName)
		{
			if(fileName == null)
				throw new ArgumentNullException("fileName");
			if(fileName.Length == 0)
				throw new ArgumentOutOfRangeException("'fileName' is zero-length.");
			
			// lock...
			lock(_tempFilesLock)
			{
				// root...
				string root = Path.GetTempPath();
				if(root == null)
					throw new InvalidOperationException("'root' is null.");
				if(root.Length == 0)
					throw new InvalidOperationException("'root' is zero-length.");

				// check...
				string path = this.GetUniqueFilePathInFolder(root, fileName);
				if(path == null)
					throw new InvalidOperationException("'path' is null.");
				if(path.Length == 0)
					throw new InvalidOperationException("'path' is zero-length.");

				// record...
				this.RegisterTempFile(path);

				// create a dummy file to prevent anyone else getting it...
				using(FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					// no-op...	
				}

				// return...
				return path;
			}
		}

		/// <summary>
		/// Gets the path to store the given unique file.
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		/// <remarks>e.g. if you are trying to store <c>Foo.doc</c> in <c>c:\temp</c>, and Foo.doc already exists, this will return <c>c:\temp\Foo (2).doc</c>.</remarks>
		public string GetUniqueFilePathInFolder(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// defer...
			return this.GetUniqueFilePathInFolder(Path.GetDirectoryName(path), Path.GetFileName(path));
		}

		/// <summary>
		/// Gets the path to store the given unique file.
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		/// <remarks>e.g. if you are trying to store <c>Foo.doc</c> in <c>c:\temp</c>, and Foo.doc already exists, this will return <c>c:\temp\Foo (2).doc</c>.</remarks>
		public string GetUniqueFilePathInFolder(string folderPath, string fileName)
		{
			if(folderPath == null)
				throw new ArgumentNullException("folderPath");
			if(folderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'folderPath' is zero-length.");
			if(fileName == null)
				throw new ArgumentNullException("fileName");
			if(fileName.Length == 0)
				throw new ArgumentOutOfRangeException("'fileName' is zero-length.");
			
			// get...
			string name = Path.GetFileNameWithoutExtension(fileName);
			string extension = Path.GetExtension(fileName);

			// check...
			if(!(folderPath.EndsWith("\\")) && !(folderPath.EndsWith("/")))
				folderPath += "\\";

			// for...
			string path = null;
			int index = 1;
			while(true)
			{
				// make...
				path = null;
				if(index == 1)
					path = folderPath + name + extension;
				else
					path = string.Format("{0}{1} ({2}){3}", folderPath, name, index, extension);

				// anything?
				if(!(File.Exists(path)))
					break;
				
				// next...
				index++;
			}

			// return...
			return path;
		}

		/// <summary>
		/// Gets the usersettings.
		/// </summary>
		public PropertyBag UserSettings
		{
			get
			{
                if (_userSettings == null)
                {
                    lock (_userSettingsLock)
                    {
                        var userSettings = new XmlFilePropertyBag(this.ApplicationDataFolderPath + @"\" + UserSettingsFileName, true);
                        userSettings.ValueSet += _userSettings_ValueSet;
                        _userSettings = userSettings;
                    }
                }
                return _userSettings;
			}
		}

        void _userSettings_ValueSet(object sender, ValueSetEventArgs e)
        {
            lock (_userSettingsLock)
            {
                // save...
                string path = Path.Combine(this.ApplicationDataFolderPath, this.UserSettingsFileName);
                string tempPath = path + ".tmp";

                // save...
                ((SimpleXmlPropertyBag)_userSettings).Save(tempPath, "Settings", true);

                // ok...
                SafeDelete(path);
                File.Move(tempPath, path);
            }
        }

        /// <summary>
        /// Raises the <c>LogCreated</c> event.
        /// </summary>
        protected internal virtual void OnLogCreated(ILogEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException("e");
			if(e.Log == null)
				throw new InvalidOperationException("e.Log is null.");

			// raise...
			if(_logCreatedStorage != null)
                _logCreatedStorage(this, e);
		}

		/// <summary>
		/// Gets the current thread ID.  
		/// </summary>
		/// <returns></returns>
		/// <remarks>This method is supplied to handle a breaking change from .NET 1.1 to 2.0.</remarks>
		public static int GetCurrentThreadId()
		{
			return Thread.CurrentThread.ManagedThreadId;
		}

        ///// <summary>
        ///// Gets the path containing global config files - i.e. c:\documents and settings\all users\application data\foo\bootfx.
        ///// </summary>
        //public static string GlobalConfigFolderPath
        //{
        //    get
        //    {
        //        string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        //        path = Path.Combine(path, MbrCompanyName + @"\MBR BootFX");
        //        return path;
        //    }
        //}

		/// <summary>
		/// Gets the internalmanagementservice.
		/// </summary>
        //internal InternalManagementService InternalManagementService
        //{
        //    get
        //    {
        //        // returns the value...
        //        return _internalManagementService;
        //    }
        //}

		private void Database_DefaultDatabaseChanged(object sender, EventArgs e)
		{
			_sharedSettings = new SharedSettings();
		}

		/// <summary>
		/// Returns true if the app is online.
		/// </summary>
		public bool IsOnline
		{
			get
			{
				switch(OnlineMode)
				{
					case OnlineMode.Online:
						return true;

					case OnlineMode.Offline:
						return false;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", OnlineMode, OnlineMode.GetType()));
				}
			}
		}

		/// <summary>
		/// Gets the onlineMode.
		/// </summary>
		private OnlineMode OnlineMode
		{
			get
			{
				// returns the value...
				return _onlineMode;
			}
		}

		/// <summary>
		/// Gets the version of the BootFX runtime.
		/// </summary>
		// mbr - 13-04-2006 - added.		
		public static Version Version
		{
			get
			{
				return typeof(Runtime).Assembly.GetName().Version;
			}
		}

		public ProcessRunResults RunProcessUntilQuit(string path, string args, TimeSpan waitUntil)
		{
			return this.RunProcessUntilQuit(path, args, waitUntil, new int[] {});
		}

        /// <summary>
        /// Runs a process until it quits.  
        /// </summary>
        /// <remarks>If the exit code is *not* one of the ones given, an exception will be thrown.</remarks>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <param name="waitUntil">The time to wait, or TimeSpan.MaxValue to wait infinitely.</param>
        /// <param name="throwIfNotExitCode"></param>
        /// <returns></returns>
        public ProcessRunResults RunProcessUntilQuit(string path, string args, TimeSpan waitUntil, int[] throwIfNotExitCode)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            if (path.Length == 0)
                throw new ArgumentOutOfRangeException("'path' is zero-length.");

            // create...
            ProcessStartInfo info = new ProcessStartInfo(path, args);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            return RunProcessUntilQuit(info, waitUntil, throwIfNotExitCode);
        }

        public ProcessRunResults RunProcessUntilQuit(ProcessStartInfo info, TimeSpan waitUntil)
        {
            return RunProcessUntilQuit(info, waitUntil, new int[] { });
        }

        public ProcessRunResults RunProcessUntilQuit(ProcessStartInfo info, TimeSpan waitUntil, int[] throwIfNotExitCode)
        {
            // run...
            var process = new Process()
            {
                StartInfo = info
            };

			try
			{
				process.Start();
			}
			catch(Exception ex)
			{
				throw this.GetProcessRunException("A general error occurred when running the process.", info.FileName, info.Arguments, null, ex);
			}

            // until...
            DateTime until = DateTime.MinValue;
            if (waitUntil == TimeSpan.MaxValue)
                until = DateTime.MaxValue;
            else
                until = DateTime.UtcNow.Add(waitUntil);

            var output = new StringBuilder();
            var error = new StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                var theOutput = e.Data;

                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(theOutput);
                Console.ForegroundColor = old;
                output.Append(theOutput);
                output.Append("\r\n");

            };
            process.ErrorDataReceived += (sender, e) =>
            {
                var theError = e.Data;

                var old = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(theError);
                Console.ForegroundColor = old;
                error.Append(theError);
                error.Append("\r\n");
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            while (!(process.HasExited) && DateTime.UtcNow <= until)
            {
                Thread.Sleep(1000);
                process.Refresh();
                Console.Write(".");
            }

            // running?
            if (!(process.HasExited))
                throw new InvalidOperationException("Process timed out.");

			// what's the exit code?
			if(throwIfNotExitCode != null && throwIfNotExitCode.Length > 0)
			{
				// check...
				bool valid = false;
				foreach(int check in throwIfNotExitCode)
				{
					if(process.ExitCode == check)
					{
						valid = true;
						break;
					}
				}

				// valid?
				if(!(valid))
					throw this.GetProcessRunException(string.Format("An invalid exit code of '{0}' was returned.", process.ExitCode), info.FileName, info.Arguments, process, null);
			}

			// return...
			return new ProcessRunResults(process.ExitCode, output.ToString(), error.ToString());
		}

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private Exception GetProcessRunException(string message, string path, string args, Process process, Exception innerException)
		{
			if(message == null)
				throw new ArgumentNullException("message");
			if(message.Length == 0)
				throw new ArgumentOutOfRangeException("'message' is zero-length.");
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// create...
			StringBuilder builder = new StringBuilder();
			builder.Append(message);
			builder.Append("\r\nPath: ");
			builder.Append(path);
			builder.Append("\r\nArgs: ");
			builder.Append(args);
			if(process != null)
			{
				builder.Append("\r\nExit code: ");
				builder.Append(process.ExitCode);

				const string div = "\r\n---------------------------------\r\n";
				string outMessage = process.StandardOutput.ReadToEnd();
				if(outMessage != null && outMessage.Length > 0)
				{
					builder.Append(div);
					builder.Append("Out:\r\n");
					builder.Append(outMessage);
				}
				string errorMessage = process.StandardError.ReadToEnd();
				if(errorMessage != null && errorMessage.Length > 0)
				{
					builder.Append(div);
					builder.Append("Error:\r\n");
					builder.Append(errorMessage);
				}
			}

			// throw...
			throw new ProcessRunException(builder.ToString(), innerException);
		}

		public void EnsureFolderCreated(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// create...
			if(!(Directory.Exists(path)))
				Directory.CreateDirectory(path);
		}

		public void EnsureFolderForFileCreated(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// get...
			string folder = Path.GetDirectoryName(path);
			if(folder == null)
				throw new InvalidOperationException("'folder' is null.");
			if(folder.Length == 0)
				throw new InvalidOperationException("'folder' is zero-length.");

			// defer...
			this.EnsureFolderCreated(folder);
		}

		public void SafeDelete(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			try
			{
				if(File.Exists(path))
					File.Delete(path);
			}
			catch
			{
				// no-op...
			}
		}

		/// <summary>
		/// Gets the webproxy.
		/// </summary>
        public IWebProxy WebProxy
		{
			get
			{
				// return...
				return _webProxy;
			}
			set
			{
				_webProxy = value;
				_isWebProxyExplictlySet = true;
			}
		}

		/// <summary>
		/// Resets the Web proxy used by the application.
		/// </summary>
		public void ResetWebProxy()
		{
			_webProxy = null;
			_isWebProxyExplictlySet = false;
		}

		/// <summary>
		/// Gets the iswebproxyexplictlyset.
		/// </summary>
		private bool IsWebProxyExplictlySet
		{
			get
			{
				// returns the value...
				return _isWebProxyExplictlySet;
			}
		}

		/// <summary>
		/// Transforms property bags into a data table.
		/// </summary>
		/// <param name="bags"></param>
		/// <returns></returns>
		public static DataTable DictionariesToDataTable(IEnumerable dicts)
		{
			if(dicts == null)
				throw new ArgumentNullException("dicts");
			
			// walk...
			ArrayList keys = new ArrayList();
			foreach(IDictionary dict in dicts)
			{
				foreach(object key in dict.Keys)
				{
					if(!(keys.Contains(key)))
						keys.Add(key);
				}
			}

			// sort...
			keys.Sort();

			// create...
			DataTable table = new DataTable();
			foreach(object key in keys)
			{
				string asString = ConversionHelper.ToString(key, Cultures.System);
				table.Columns.Add(asString);
			}

			// walk...
			foreach(IDictionary dict in dicts)
			{
				// add...
				object[] values = new object[keys.Count];
				for(int index = 0; index < keys.Count; index++)
					values[index] = dict[keys[index]];

				// add...
				table.Rows.Add(values);
			}

			// return...
			return table;
		}

		/// <summary>
		/// Gets a unique folder for temporary files.
		/// </summary>
		/// <returns></returns>
		public string GetNewTempFolderPath()
		{
			if(Application == null)
				throw new InvalidOperationException("Application is null.");
			
			// get...
			string path = Path.Combine(Path.Combine(Path.GetTempPath(), this.Application.TempFilenamePrefix), Guid.NewGuid().ToString());
			if(!(Directory.Exists(path)))
				Directory.CreateDirectory(path);

			// return...
			return path;
		}

		/// <summary>
		/// Calls an event to create an thread affinity connection.
		/// </summary>
		/// <param name="affinityState"></param>
		/// <returns></returns>
		internal IConnection SetupAffinity(object affinityState)
		{
			if(affinityState == null)
				throw new ArgumentNullException("affinityState");
			
			// get...
			IConnectionEventArgs e = new IConnectionEventArgs(affinityState);
			this.OnAffinityConnectionNeeded(e);

			// check...
			if(e.Connection == null)
				throw new InvalidOperationException("e.Connection is null.");

			// return...
			return e.Connection;
		}

		/// <summary>
		/// Raises the <c>AffinityConnectionNeeded</c> event.
		/// </summary>
		protected virtual void OnAffinityConnectionNeeded(IConnectionEventArgs e)
		{
			// raise...
			if(AffinityConnectionNeeded != null)
				AffinityConnectionNeeded(this, e);
			else
				throw new InvalidOperationException("An affinity connection was required but no subscribers are available.  Subscribe to Runtime.AffinityConnectionNeeded.");
		}

		/// <summary>
		/// Gets the configurationmode.
		/// </summary>
		public static RuntimeConfigMode ConfigurationMode
		{
			get
			{
				return _configurationMode;
			}
		}

		public static void SetConfigurationMode(RuntimeConfigMode mode)
		{
			if(IsStarted)
				throw new InvalidOperationException("Configuration mode cannot be set once the application has been started.");

			// set...
			_configurationMode = mode;
		}

		/// <summary>
		/// Gets or sets the URL used for 'Phone Home' error reporting.
		/// </summary>
		public string PhoneHomeUrl
		{
			get
			{
				return this.InstallationSettings.PhoneHomeUrl;
			}
			set
			{
				// check to see if the value has changed...
				if(value != this.InstallationSettings.PhoneHomeUrl)
				{
					// set the value...
					this.InstallationSettings.PhoneHomeUrl = value;
				}
			}
		}

		/// <summary>
		/// Returns true if the phone home URL is defined.
		/// </summary>
		public bool HasPhoneHomeUrl
		{
			get
			{
				if(this.PhoneHomeUrl != null && this.PhoneHomeUrl.Length > 0)
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the installationsettingsfilename.
		/// </summary>
		private string InstallationSettingsFileName
		{
			get
			{
				// mbr - 28-07-2007 - added this to handle one app installation, many configurations...				
				if(_installationSettingsFileName == null)
				{
					string name = this.InstallationName;
					if(name == null)
						throw new InvalidOperationException("name is null.");

					// if...
					const string defaultPrefix = "InstallationSettings";
					const string extension = ".dat";
					if(name.Length == 0)
						_installationSettingsFileName = defaultPrefix + extension;
					else
						_installationSettingsFileName = string.Format("{0}-{1}{2}", defaultPrefix, name, extension);
				}
				return _installationSettingsFileName;
			}
		}

		/// <summary>
		/// Gets the installationname.
		/// </summary>
		public string InstallationName
		{
			get
			{
				// mbr - 05-09-2007 - changed to defer to args...
				//				if(_installationName == null)
				//				{
				//					// get the config section...(string.Empty is the default name).
				//					string asString = (string)ConfigurationSettings.GetConfig("appInstallation");
				//					if(asString != null)
				//						_installationName = asString;
				//					else
				//						_installationName = string.Empty;
				//				}
				//				return _installationName;

				// get...
				if(Application == null)
					throw new InvalidOperationException("Application is null.");
				if(Application.StartArgs == null)
					throw new InvalidOperationException("Application.StartArgs is null.");
				return this.Application.StartArgs.InstallationName;
			}
		}

		/// <summary>
		/// Given a document ID, this method will return a fully qualified folder path.
		/// </summary>
		/// <param name="baseFolderPath"></param>
		/// <param name="id"></param>
		/// <param name="extension"></param>
		/// <returns></returns>
		public string GetFilePathForDocument(string baseFolderPath, int id, string extension)
		{
			if(baseFolderPath == null)
				throw new ArgumentNullException("baseFolderPath");
			if(baseFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'baseFolderPath' is zero-length.");
			
			// get...
			string partial = this.GetPartialFilePathForDocument(id, extension);
			if(partial == null)
				throw new InvalidOperationException("'partial' is null.");
			if(partial.Length == 0)
				throw new InvalidOperationException("'partial' is zero-length.");

			// combine...
			string result = Path.Combine(baseFolderPath, partial);
			return result;
		}

		/// <summary>
		/// Given a document ID, this method will return a partial folder path.
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public string GetPartialFilePathForDocument(int id, string extension)
		{
			int masterFolderId = id / (10000 * 10000);
			int folderId = id / 10000;

			// return...
			return string.Format(@"{0:d2}\{1:d6}\{2:d10}{3}", masterFolderId, folderId, id, extension);
		}

		/// <summary>
		/// Returns true if the application has been started in "low trust" mode.
		/// </summary>
		// mbr - 08-02-2008 - added.
		public bool IsLowTrust
		{
			get
			{
				return this.Application.StartArgs.IsLowTrust;
			}
		}

		/// <summary>
		/// Suggests a password to use with a user.
		/// </summary>
		/// <returns></returns>
		public string SuggestUserPassword()
		{
			return SuggestUserPassword(6, true, true);
		}

		/// <summary>
		/// Suggests a password to use with a user.
		/// </summary>
		/// <param name="length"></param>
		/// <param name="numbers"></param>
		/// <param name="upperCase"></param>
		/// <returns></returns>
		public string SuggestUserPassword(int length, bool numbers, bool upperCase)
		{
			// num...
			int numNumbers = 0;
			int numLetters = 0;
			if(numbers)
			{
				numNumbers = length / 2;
				numLetters = length - numNumbers;
			}
			else
				numLetters = length;

			// builder...
			StringBuilder builder = new StringBuilder();

			// lock...
			lock(_userPasswordLock)
			{
				// letters...
				for(int index = 0; index < numLetters; index++)
				{
					// letters...
					char c = PasswordLetters[_userPasswordRandom.Next(PasswordLetters.Length)];
					if(_userPasswordRandom.Next(10) >= 5)
						c = char.ToUpper(c);

					// add...
					builder.Append(c);
				}

				// numbers...
				for(int index = 0; index < numNumbers; index++)
					builder.Append((char)_userPasswordRandom.Next(48, 57));

				// return...
				return builder.ToString();
			}
		}

		/// <summary>
		/// Gets the passwordletters.
		/// </summary>
		private static char[] PasswordLetters
		{
			get
			{
				return _passwordLetters;
			}
		}

		/// <summary>
		/// Gets the description against the given object.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public string GetDescription(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// get...
			DescriptionAttribute[] attrs = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute) ,true);
			if(attrs.Length > 0)
				return attrs[0].Description;
			else
				return type.FullName;
		}

		// mbr - 2008-11-26 - added, because if you have a desktop module and a console app, the last one to close wins...
		private string UserSettingsFileName
		{
			get
			{
                return string.Format("UserSettings2-{0}.xml", this.Application.ProductModule);
			}
		}

        public string GetPartiallyQualifiedName<T>()
        {
            return GetPartiallyQualifiedName(typeof(T));
        }

        /// <summary>
        /// Gets a partially qualified name for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetPartiallyQualifiedName(Type type)
        {
            return GetPartiallyQualifiedNameInternal(type);
        }

        /// <summary>
        /// Gets a partially qualified name for a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static string GetPartiallyQualifiedNameInternal(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            // return...
            return string.Format("{0}, {1}", type.FullName, type.Assembly.GetName().Name);
        }

        public string InstallationSettingsFolderPath
        {
            get
            {
                var path = this.Application.StartArgs.InstallationSettingsFilePath;
                if (!(string.IsNullOrEmpty(path)))
                    return Path.GetDirectoryName(path);
                else
                    return GetFolderPath(Environment.SpecialFolder.CommonApplicationData, this.Application.ProductCompany, this.Application.ProductName);
            }
        }

        // mbr - 2010-04-05 - added...
        public static string GetInstallationSettingsFilePath(string company, string product)
        {
            if (company == null)
                throw new ArgumentNullException("company");
            if (company.Length == 0)
                throw new ArgumentException("'company' is zero-length.");
            if (product == null)
                throw new ArgumentNullException("product");
            if (product.Length == 0)
                throw new ArgumentException("'product' is zero-length.");

            // get...
            string path = GetFolderPath(Environment.SpecialFolder.CommonApplicationData, company, product);
            path = Path.Combine(path, "InstallationSettings.dat");
            return path;
        }

        public List<List<long>> GetPages(IEnumerable<long> ids, int pageSize)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");

            // create...
            List<List<long>> pages = new List<List<long>>();
            List<long> page = null;
            foreach (long id in ids)
            {
                if (page == null)
                {
                    page = new List<long>();
                    pages.Add(page);
                }

                // add...
                page.Add(id);

                // if...
                if (page.Count == pageSize)
                    page = null;
            }

            // return...
            return pages;
        }

        public List<List<int>> GetPages(IEnumerable<int> ids, int pageSize)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");

            // create...
            List<List<int>> pages = new List<List<int>>();
            List<int> page = null;
            foreach (int id in ids)
            {
                if (page == null)
                {
                    page = new List<int>();
                    pages.Add(page);
                }

                // add...
                page.Add(id);

                // if...
                if (page.Count == pageSize)
                    page = null;
            }

            // return...
            return pages;
        }

        public List<List<T>> GetPages<T>(IEnumerable<T> ids, int pageSize)
        {
            if (ids == null)
                throw new ArgumentNullException("ids");

            // create...
            List<List<T>> pages = new List<List<T>>();
            List<T> page = null;
            foreach (T id in ids)
            {
                if (page == null)
                {
                    page = new List<T>();
                    pages.Add(page);
                }

                // add...
                page.Add(id);

                // if...
                if (page.Count == pageSize)
                    page = null;
            }

            // return...
            return pages;
        }

        public string GetEnumerationValueDescription<T>(T value, bool splitWords = false)
        {
            return this.GetEnumerationValueDescription(typeof(T), value, splitWords);
        }

        private string GetEnumerationValueDescription(Type type, object value, bool spaceWords = false)
        {
            if (!(spaceWords))
                return this.EnumDescriptionsLookup[type, value];
            else
                return this.EnumDescriptionsLookupSpaceWords[type, value];
        }

        public string GetNodeName(Random rand = null)
        {
            return GetNodeName("_", rand);
        }

        public string GetNodeName(string separator, Random rand = null)
        {
            lock(_nodeNamesLock)
            {
                if (rand == null)
                    rand = new Random();

                if (this.NodeNamesFirst == null)
                {
                    this.NodeNamesFirst = new List<string>(ResourceHelper.GetStringsFromCommentedFile(this.GetType().Assembly, "BootFX.Common.Resources.NodeNamesFirst.txt"));
                    this.NodeNamesLast = new List<string>(ResourceHelper.GetStringsFromCommentedFile(this.GetType().Assembly, "BootFX.Common.Resources.NodeNamesSecond.txt"));
                }

                // create...
                return this.NodeNamesFirst[rand.Next(0, this.NodeNamesFirst.Count - 1)] + separator +
                    this.NodeNamesLast[rand.Next(0, this.NodeNamesLast.Count - 1)];
            }
        }

        public string GetStrongPassword(int length = 24)
        {
            var allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            StringBuilder builder = new StringBuilder();
            for (var index = 0; index < length; index++)
            {
                var rng = new RNGCryptoServiceProvider();
                var bs = new byte[4];
                rng.GetBytes(bs);

                var seed = BitConverter.ToInt32(bs, 0);
                var random = new Random(seed);
                var c = random.Next(0, allowed.Length - 1);

                builder.Append(allowed[c]);
            }

            return builder.ToString();
        }

        public string GetFullName(string first, string last, string defaultName = "?")
        {
            var hasFirst = !(string.IsNullOrEmpty(first));
            var hasLast = !(string.IsNullOrEmpty(last));

            if (hasFirst && hasLast)
                return first + " " + last;
            else if (hasFirst)
                return first;
            else if (hasLast)
                return last;
            else
                return defaultName;
        }

        public bool HasFullName(string first, string last)
        {
            return !(string.IsNullOrEmpty(first)) || !(string.IsNullOrEmpty(last));
        }

        internal static string GetToken()
        {
            return EncryptionHelper.GetRandomBytesAsHexString(24);
        }

        public void RegisterThread(IDisposableThread thread)
        {
            this.DisposableThreads.Add(thread);
        }

        public string Slugify(string name, Func<string> defaultValue = null)
        {
            return Slugify(name, -1, defaultValue);
        }

        public string Slugify(string name, int maxLength, Func<string> defaultValue = null)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var c in name)
            {
                if (char.IsLetterOrDigit(c))
                    builder.Append(char.ToLower(c));
                else if (builder.Length > 0 && !(builder.EndsWith("-")))
                    builder.Append("-");

                if (builder.Length == maxLength)
                    break;
            }

            var tag = builder.ToString();
            while (tag.EndsWith("-"))
                tag = tag.Substring(0, tag.Length - 1);

            if (tag.Length > 0)
                return tag;
            else
            {
                if (defaultValue != null)
                    return defaultValue();
                else
                    return null;
            }
        }
    }
}

