// BootFX - Application framework for .NET applications
// 
// File: DesktopRuntime.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using BootFX.Common.Management;
using System.Security;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Provides access to common runtime data for desktop ("Windows Forms") applications.
	/// </summary>
    [SecurityCritical]
	public class DesktopRuntime
	{
		/// <summary>
		/// Private field to support <c>DefaultFormIcon</c> property.
		/// </summary>
		private Icon _defaultFormIcon;
		
		/// <summary>
		/// Private field to hold the singleton instance.
		/// </summary>
		private static DesktopRuntime _current;
		
		/// <summary>
		/// Private field to support <see cref="MainUIThreadId"/> property.
		/// </summary>
		private int _mainUIThreadId = Runtime.GetCurrentThreadId();

		/// <summary>
		/// Defines the XP-style 'dot' password character.
		/// </summary>
		public const char XpPasswordChar = '\u25CF';
		
		/// <summary>
		/// Private constructor.
		/// </summary>
        [SecurityCritical]
		private DesktopRuntime(bool enableVisibleStyles)
		{
			// set the name of the thread...
			try
			{
				Thread.CurrentThread.Name = "Main UI Thread";
			}
			catch
			{
				// ignore exceptions here...
			}

			// initialize application style...
			if(enableVisibleStyles)
			{
				Application.EnableVisualStyles();

				// a lot of community chat re EnableVisualStyles says you have to do this...
				Application.DoEvents();
			}

			// initialize exception handling...
			Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

//			// create a thread for operation dialog...
//			this.StartSharedOperationDialogThread();

			// setup the default icon...
			_defaultFormIcon = ImageLibrary.GetDefaultIcon();
		}

		/// <summary>
		/// Returns true if this is the main UI thread.
		/// </summary>
		public bool IsMainUIThread
		{
			get
			{
				if(this.MainUIThreadId == Runtime.GetCurrentThreadId())
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the mainuithreadid.
		/// </summary>
		public int MainUIThreadId
		{
			get
			{
				return _mainUIThreadId;
			}
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static DesktopRuntime Current
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
		private static void AssertIsStarted()
		{
			if(IsStarted == false)
				throw new DesktopRuntimeNotStartedException();
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
		/// Starts the runtime.
		/// </summary>
		public static void Start()
		{
			Start(true);
		}

		/// <summary>
		/// Starts the runtime.
		/// </summary>
		public static void Start(bool enableVisualStyles)
		{
			// setup...
			_current = new DesktopRuntime(enableVisualStyles);
		}

		/// <summary>
		/// Disposes the runtime.
		/// </summary>
		public void Dispose()
		{
			// reset...
			_current = null;
		}

		private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			this.HandleException(null, "Unhandled UI exception.", e.Exception);
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			this.HandleException(null, "Unhandled AppDomain exception.", (Exception)e.ExceptionObject);
		}

		/// <summary>
		/// Handles the given exception.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="ex"></param>
		private DialogResult HandleException(IWin32Window owner, string message, Exception ex)
		{
			if(ex == null)
				throw new ArgumentNullException("ex");

            //// show it...
            //ErrorDialog dialog = new ErrorDialog();
            //dialog.Exception = ex;
            //try
            //{
            //    return dialog.ShowDialog(owner);
            //}
            //catch(Exception showEx)
            //{
            //    // mbr - 27-11-2006 - added - not sure why this happens.				
            //    const string sep = "\r\n\r\n------------------------\r\n";
            //    Alert.ShowWarning(owner, string.Format("An error occurred when displaying an error message.{0}Original error:\r\n\t{1}{0}Display error:\r\n\t{2}", 
            //        ex, sep, showEx));
            //    return DialogResult.OK;
            //}

            return Alert.ShowError(owner, message, ex);
		}

		/// <summary>
		/// Shows Microsoft System Information.
		/// </summary>
		public void LaunchMicrosoftSystemInformation(IWin32Window owner)
		{
			try
			{
				System.Diagnostics.Process.Start("msinfo32.exe");
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("The Microsoft System Information utility could not be shown.  It may not be installed on your computer.", ex);
			}
		}

		/// <summary>
		/// Shows Microsoft System Information.
		/// </summary>
		public string SaveMicrosoftSystemInformation(IWin32Window owner)
		{
			string filename = Alert.ShowSaveTextFileDialog(owner, "System Information.txt");
			if(filename != null)
				this.SaveMicrosoftSystemInformation(owner, filename);

			// nope...
			return filename;
		}

		/// <summary>
		/// Shows Microsoft System Information.
		/// </summary>
		public void SaveMicrosoftSystemInformation(IWin32Window owner, string filename)
		{
            if (filename == null)
                throw new ArgumentNullException("filename");
            if (filename.Length == 0)
                throw new ArgumentException("'filename' is zero-length.");

			try
			{
				// alert...
				if(Alert.ShowInformation(owner, "The system information will now be saved.  This process may take a few minutes.", true) != DialogResult.OK)
					return;

				// run it...
				Process process = Process.Start("msinfo32.exe", string.Format(Cultures.System, "/report \"{0}\"", filename));
				if(process.WaitForExit(5 * 60 * 1000) == true)		// 5 minutes...
					Alert.ShowInformation(owner, string.Format(Cultures.User, "The system information has been saved to '{0}'.", filename));
				else
					Alert.ShowWarning(owner, string.Format(Cultures.User, "The system information has been saved to '{0}'.", filename));
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException("System information could not be written.", ex);
			}
		}

		/// <summary>
		/// Displays the system information dialog.
		/// </summary>
		public void ShowAbout(IWin32Window owner)
		{
			AboutDialog dialog = new AboutDialog();
			dialog.ShowDialog(owner);
		}

		/// <summary>
		/// Runs the <c>GC.Collect</c> method and reports the results.  (Debug use only.)
		/// </summary>
		/// <param name="owner"></param>
		/// <remarks>This method is useful for chasing down memory leaks in cominbation with something like CLR Profiler or DevPartner Studio.  
		/// Do not use this in production code.</remarks>
		public void RunGCCollectAndReport(IWin32Window owner)
		{
			// before...
			long before = GC.GetTotalMemory(false);
			GC.Collect();
			long after = GC.GetTotalMemory(true);

			// report...
			Alert.ShowInformation(owner, string.Format(Cultures.User, string.Format("GC complete.\r\n\r\nBefore: {0}\r\nAfter: {1}\r\nDifference: {2} ({3:f2}%)", 
				before, after, before - after, (1 - ((double)after / (double)before)) * 100)));
		}

		/// <summary>
		/// Shows the given URL.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="url"></param>
		public void ShowUrl(Control owner, string url)
		{
			if(url == null)
				throw new ArgumentNullException("url");
			if(url.Length == 0)
				throw new ArgumentOutOfRangeException("'url' is zero-length.");

			System.Diagnostics.Process.Start(url);
		}

		/// <summary>
		/// Gets or sets the defaultformicon
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

		public Form GetOwnerForm(Control owner, bool throwIfNotFound)
		{
			if(owner == null)
				throw new ArgumentNullException("owner");
			
			// while...
			while(owner != null)
			{
				if(owner is Form)
					return (Form)owner;
				owner = owner.Parent;
			}
			
			// nope...
			if(throwIfNotFound)
				throw new InvalidOperationException("Owner form not found.");
			else
				return null;
		}

		/// <summary>
		/// Brings the form to the foreground, handling minimized and non-handle-created states.
		/// </summary>
		/// <param name="form"></param>
		public void BringToFront(Form form)
		{
			if(form == null)
				throw new ArgumentNullException("form");
			
			// are we visible?
			if(!(form.IsHandleCreated) || !(form.Visible))	
				form.Show();

			// can we see it?
			if(form.WindowState == FormWindowState.Minimized)
				form.WindowState = FormWindowState.Normal;

			// bring it to the front and activate it...
			form.BringToFront();
			form.Activate();
		}

		/// <summary>
		/// Gets or sets the errorreportingurl
		/// </summary>
		[Obsolete("Use 'PhoneHomeUrl' on Runtime instead.")]
		public string ErrorReportingUrl
		{
			get
			{
				return Runtime.Current.PhoneHomeUrl;
			}
			set
			{
				Runtime.Current.PhoneHomeUrl = value;
			}
		}

		[Obsolete("Use 'HasPhoneHomeUrl' on Runtime instead.")]
		public bool HasErrorReportingUrl
		{
			get
			{
				return Runtime.Current.HasPhoneHomeUrl;
			}
		}

		/// <summary>
		/// Measures a string.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="font"></param>
		/// <returns></returns>
		/// <remarks>This method is the same as Graphics.MeasureString, but will create and manage a DC for you.</remarks>
		public SizeF MeasureString(string text, Font font)
		{
			using(Bitmap bitmap = new Bitmap(1, 1, PixelFormat.Format32bppPArgb))
			{
				using(Graphics graphics = Graphics.FromImage(bitmap))
				{
					return graphics.MeasureString(text, font);
				}
			}
		}
	}
}
