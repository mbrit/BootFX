// BootFX - Application framework for .NET applications
// 
// File: Startup.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using BootFX.Common;
using BootFX.Common.UI;
using BootFX.Common.UI.Desktop;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using BootFX.Common.Xml;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Boot class.
	/// </summary>
	public class Startup : Loggable
	{
		/// <summary>
		/// Private field to support <see cref="StartArgs"/> property.
		/// </summary>
		private RuntimeStartArgs _startArgs;
		
		/// <summary>
		/// Private field to support <c>CheckForOtherInstances</c> property.
		/// </summary>
		private bool _checkForOtherInstances = false;
		
		/// <summary>
		/// Raised when another process with the name of this one is running.
		/// </summary>
		public event CancelEventHandler OtherProcessRunning;
		
		// mbr - 14-04-2006 - added.				
		public const int DefaultFailureExitCode = 100;

		/// <summary>
		/// Private field to support <c>FailureExitCode</c> property.
		/// </summary>
		// mbr - 14-04-2006 - added.				
		private int _failureExitCode = DefaultFailureExitCode;
		
		/// <summary>
		/// Private field to support <see cref="ProductModule"/> property.
		/// </summary>
		private string _productModule;
		
		/// <summary>
		/// Private field to support <c>UseEventLogAppender</c> property.
		/// </summary>
		private bool _useEventLogAppender = false;
		
		/// <summary>
		/// Private field to support <c>DefaultIcon</c> property.
		/// </summary>
		private Icon _defaultFormIcon = SystemIcons.Application;
		
		/// <summary>
		/// Raised when entity types should be loaded.
		/// </summary>
		public event CancelEventHandler ConfigureRuntime;
		
		/// <summary>
		/// Raised when the application should be started.
		/// </summary>
		public event EventHandler StartApplication;
		
		/// <summary>
		/// Defines the filename of the last failure.
		/// </summary>
		private const string LastFailureFileName = "LastFailure.xml";

		/// <summary>
		/// Private field to support <see cref="CompanyName"/> property.
		/// </summary>
		private string _companyName;

		/// <summary>
		/// Private field to support <see cref="ProductName"/> property.
		/// </summary>
		private string _productName;

		/// <summary>
		/// Private field to support <see cref="Version"/> property.
		/// </summary>
		private Version _version;

//		/// <summary>
//		/// Private field to support <see cref="Copyright"/> property.
//		/// </summary>
//		private string _copyright;
//
//		/// <summary>
//		/// Private field to support <see cref="ProductSupportUrl"/> property.
//		/// </summary>
//		private string _productSupportUrl;

		/// <summary>
		/// Constructor.
		/// </summary>
		[Obsolete("You should specify the product module.", false)]
		public Startup(string companyName, string productName, Version version)
			: this(companyName, productName, "Client", version)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 06-09-2007 - added.		
		public Startup(string companyName, string productName, string productModule, Version version)
			: this(companyName, productName, productModule, version, new RuntimeStartArgs(Assembly.GetCallingAssembly()))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 06-09-2007 - added startargs.		
		public Startup(string companyName, string productName, string productModule, Version version, RuntimeStartArgs args)
		{
			if(companyName == null)
				throw new ArgumentNullException("companyName");
			if(companyName.Length == 0)
				throw new ArgumentOutOfRangeException("'companyName' is zero-length.");
			if(productName == null)
				throw new ArgumentNullException("productName");
			if(productName.Length == 0)
				throw new ArgumentOutOfRangeException("'productName' is zero-length.");
			if(productModule == null)
				throw new ArgumentNullException("productModule");
			if(productModule.Length == 0)
				throw new ArgumentOutOfRangeException("'productModule' is zero-length.");

			// mbr - 10-11-2007 - null args?			
			if(args == null)
				args = new RuntimeStartArgs(Assembly.GetCallingAssembly());
			
			// set...
			_companyName = companyName;
			_productName = productName;
			_productModule = productModule;
			_version = version;
//			_copyright = copyright;
//			_productSupportUrl = productSupportUrl;

			// mbr - 06-09-2007 - args...
			_startArgs = args;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		[Obsolete("Use the constructor that does not required product support URL or copyright.", false)]
		public Startup(string companyName, string productName, Version version, string productSupportUrl, string copyright) 
			: this(companyName, productName, version)
		{
		}

		/// <summary>
		/// Runs the application.
		/// </summary>
		public void Run()
		{
			try
			{
				// mbr - 21-07-2006 - added.				
				if(this.CheckForOtherInstances)
				{
					// mbr - 22-06-2006 - this can fail?
					string name = null;
					try
					{
						// mbr - 20-04-2006 - is a version of this application running?
						Process thisProcess = Process.GetCurrentProcess();
						if(thisProcess == null)
							throw new InvalidOperationException("thisProcess is null.");

						// find others with this name...
						name = thisProcess.ProcessName;
						Process[] otherProcesses = Process.GetProcessesByName(thisProcess.ProcessName);
						if(otherProcesses.Length > 1)
						{
							// raise an event...
							CancelEventArgs otherE = new CancelEventArgs(false);
							this.OnOtherProcessRunning(otherE);
							if(otherE.Cancel)
							{
								// TODO: activate the existing application...	
								Alert.ShowInformation(null, "The application is already running.");

								// quit...
								return;
							}
						}
					}
					catch(Exception ex)
					{
						// mbr - 22-06-2006 - a blank name in the warning is indicative that the name of the process 
						// could not be determined;
						if(this.Log.IsWarnEnabled)
							this.Log.Warn(string.Format("Failed to check for processes with name '{0}'.", name), ex);
					}
				}

				// get the last exception...
				Exception lastFailure = this.GetFailureException();

				// reset...
				this.SetFailureException(null);

				// check...
				bool enableVisualStyles = true;
				if(lastFailure is System.Runtime.InteropServices.SEHException)
				{
					// ask!
					if(Alert.AskYesNoQuestion(null, "This application failed last time it was executed.  The error may have been caused by enabling XP styles.  Do you want to enable XP styles?") == DialogResult.Yes)
						enableVisualStyles = true;
					else
						enableVisualStyles = false;
				}

				// mbr - 06-09-2007 - added args...
//				Runtime.Start(this.CompanyName, this.ProductName, this.ProductModule, this.Version);
				Runtime.Start(this.CompanyName, this.ProductName, this.ProductModule, this.Version, this.StartArgs);

				// configure...
				CancelEventArgs e = new CancelEventArgs();
				this.OnConfigureRuntime(e);

				// check...
				if(!(e.Cancel))
				{
					// desktop...
					DesktopRuntime.Start(enableVisualStyles);
					DesktopRuntime.Current.DefaultFormIcon = this.DefaultFormIcon;

					// login...
					this.OnStartApplication();
				}
				else
				{
					if(this.Log.IsWarnEnabled)
						this.Log.Warn("Runtime configuration was cancelled.");
				}
			}
			catch(Exception ex)
			{
				// log...
				if(Log.IsWarnEnabled)
					Log.Warn(string.Format("A critical exception occurred:\r\n{0}", ex));

				// write the last exception...
				this.SetFailureException(ex);

				// show...
				Alert.ShowError(null, string.Format("A critical exception has stopped this application.  The exception has been logged to: {0}", 
					this.LastFailureFilePath), ex);

				// mbr - 14-04-2006 - added.				
				Environment.ExitCode = this.FailureExitCode;
			}
			finally
			{
				try
				{
					if(Runtime.IsStarted)
						Runtime.Current.Dispose();
					if(DesktopRuntime.IsStarted)
						DesktopRuntime.Current.Dispose();
				}
				catch
				{
					// no-op...
					// TODO: log this somewhere...
				}
			}
		}

		/// <summary>
		/// Raises the <c>OtherProcessRunning</c> event.
		/// </summary>
		protected virtual void OnOtherProcessRunning(CancelEventArgs e)
		{
			// raise...
			if(OtherProcessRunning != null)
				OtherProcessRunning(this, e);
		}

		/// <summary>
		/// Sets the last failed exception.
		/// </summary>
		/// <param name="ex"></param>
		private void SetFailureException(Exception failure)
		{
			string path = string.Empty;
			try
			{
				// path...
				path = this.LastFailureFilePath;
				if(path == null)
					throw new InvalidOperationException("'path' is null.");
				if(path.Length == 0)
					throw new InvalidOperationException("'path' is zero-length.");

				// directory...
				string folderPath = Path.GetDirectoryName(path);
				if(!(Directory.Exists(folderPath)))
					Directory.CreateDirectory(folderPath);

				// what now?
				if(failure != null)
				{
					IFormatter formatter = this.GetFormatter();
					if(formatter == null)
						throw new InvalidOperationException("formatter is null.");

					// save...
					FileInfo info = new FileInfo(path);
					if(info.Directory.Exists == false)
						info.Directory.Create();
					string tempPath = path + "." + Guid.NewGuid().ToString();
					try
					{
						using(FileStream stream = new FileStream(tempPath, FileMode.Create, FileAccess.Write))
						{
							formatter.Serialize(stream, failure);
						}

						// move...
						if(File.Exists(path))
							File.Delete(path);
						File.Move(tempPath, path);
					}
					finally
					{
						if(File.Exists(tempPath))
						{
							try
							{
								File.Delete(tempPath);
							}
							catch
							{
							}
						}
					}
				}
				else
				{
					if(File.Exists(path))
					{
						try
						{
							File.Delete(path);
						}
						catch
						{
							// no-op...
						}
					}
				}
			}
			catch(Exception ex)
			{
				// no-op...
				if(Log.IsErrorEnabled)
					Log.Error(string.Format("Failed to save failure exception to {0}:\r\n{1}", path, ex));
			}
		}

		/// <summary>
		/// Gets the formatter.
		/// </summary>
		/// <returns></returns>
		private IFormatter GetFormatter()
		{
			// create...
			var formatter = new SoapFormatter();
			formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;

			// return...
			return formatter;
		}

		/// <summary>
		/// Gets the filepath of the last failure exception.
		/// </summary>
		private string LastFailureFilePath
		{
			get
			{
				return Runtime.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, this.CompanyName, this.ProductName) 
					+ "\\" + LastFailureFileName;
			}
		}

//		/// <summary>
//		/// Gets the productsupporturl.
//		/// </summary>
//		public string ProductSupportUrl
//		{
//			get
//			{
//				return _productSupportUrl;
//			}
//		}
//		
//		/// <summary>
//		/// Gets the copyright.
//		/// </summary>
//		public string Copyright
//		{
//			get
//			{
//				return _copyright;
//			}
//		}
		
		/// <summary>
		/// Gets the version.
		/// </summary>
		public Version Version
		{
			get
			{
				return _version;
			}
		}
		
		/// <summary>
		/// Gets the productname.
		/// </summary>
		public string ProductName
		{
			get
			{
				return _productName;
			}
		}
		
		/// <summary>
		/// Gets the companyname.
		/// </summary>
		public string CompanyName
		{
			get
			{
				return _companyName;
			}
		}

		/// <summary>
		/// Gets the last failure exception.
		/// </summary>
		/// <returns></returns>
		private Exception GetFailureException()
		{
			string path = string.Empty;
			try
			{
				path = this.LastFailureFilePath;
				if(path == null)
					throw new InvalidOperationException("'path' is null.");
				if(path.Length == 0)
					throw new InvalidOperationException("'path' is zero-length.");

				// check...
				if(File.Exists(path))
				{
					// load...
					IFormatter formatter = this.GetFormatter();
					if(formatter == null)
						throw new InvalidOperationException("formatter is null.");

					// return...
					using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
					{
						return (Exception)formatter.Deserialize(stream);
					}
				}
				else
					return null;
			}
			catch(Exception ex)
			{
				if(Log.IsErrorEnabled)
					Log.Error(string.Format("Failed to load last failed exception from '{0}'.\r\nException: {1}", path, ex));

				// no-op...
				return null;
			}
		}

		/// <summary>
		/// Raises the <c>StartApplication</c> event.
		/// </summary>
		private void OnStartApplication()
		{
			OnStartApplication(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>StartApplication</c> event.
		/// </summary>
		protected virtual void OnStartApplication(EventArgs e)
		{
			// raise...
			if(StartApplication != null)
				StartApplication(this, e);
		}

		/// <summary>
		/// Raises the <c>LoadEntityTypes</c> event.
		/// </summary>
		protected virtual void OnConfigureRuntime(CancelEventArgs e)
		{
			// raise...
			if(ConfigureRuntime != null)
				ConfigureRuntime(this, e);
		}

		/// <summary>
		/// Gets or sets the defaulticon
		/// </summary>
		public Icon DefaultFormIcon
		{
			get
			{
				return _defaultFormIcon;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _defaultFormIcon)
				{
					// set the value...
					_defaultFormIcon = value;
				}
			}
		}

		/// <summary>
		/// Checks the MDAC version.
		/// </summary>
		public bool CheckMdac()
		{
            return true;
		}

		/// <summary>
		/// Gets or sets the useeventlogappender
		/// </summary>
		[Obsolete("Deprected - please let MaBR know if anyone is actually using this.")]
        public bool UseEventLogAppender
        {
            get
            {
                return _useEventLogAppender;
            }
            set
            {
                // check to see if the value has changed...
                if (value != _useEventLogAppender)
                {
                    // set the value...
                    _useEventLogAppender = value;
                }
            }
        }

		/// <summary>
		/// Gets the productmodule.
		/// </summary>
		public string ProductModule
		{
			get
			{
				return _productModule;
			}
		}

		/// <summary>
		/// Gets or sets the failureexitcode
		/// </summary>
		// mbr - 14-04-2006 - added.				
		public int FailureExitCode
		{
			get
			{
				return _failureExitCode;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _failureExitCode)
				{
					// set the value...
					_failureExitCode = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the checkforotherinstances
		/// </summary>
		// mbr - 21-07-2006 - added.		
		public bool CheckForOtherInstances
		{
			get
			{
				return _checkForOtherInstances;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _checkForOtherInstances)
				{
					// set the value...
					_checkForOtherInstances = value;
				}
			}
		}

		/// <summary>
		/// Gets the startargs.
		/// </summary>
		private RuntimeStartArgs StartArgs
		{
			get
			{
				return _startArgs;
			}
		}
	}
}
