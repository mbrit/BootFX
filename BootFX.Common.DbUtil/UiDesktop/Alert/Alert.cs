// BootFX - Application framework for .NET applications
// 
// File: Alert.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Provides access to message boxes and related dialogs.
	/// </summary>
	public static class Alert
	{
		private static string _caption = null;

		/// <summary>
		/// Defines the filter used for saving and opening XML files.
		/// </summary>
		private const string XmlFileFilter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*||";

		/// <summary>
		/// Defines the filter used for saving and opening HTML files.
		/// </summary>
		private const string HtmlFileFilter = "HTML Files (*.html,*.htm)|*.html,*.htm|All Files (*.*)|*.*||";

		/// <summary>
		/// Defines the filter used for saving and opening text files.
		/// </summary>
		private const string TextFileFilter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*||";

		/// <summary>
		/// Shows the given exception.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		public static DialogResult ShowError(IWin32Window owner, string message, Exception ex)
		{
			return ShowError(owner, message, ex, null);
		}

		/// <summary>
		/// Shows the given exception.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="ex"></param>
		/// <param name="state"></param>
		public static DialogResult ShowError(IWin32Window owner, string message, Exception ex, object state)
		{
			if(ex == null)
				throw new ArgumentNullException("ex");

			// mbr - 2014-09-01 - turne off error dialog...

//            // show...
//            if(DesktopRuntime.IsStarted == true)
//            {
////				// publish...
////				ExceptionPublisher.Current.PublishExceptionDetails(message, ex);

//                // show...
//                ErrorDialog dialog = new ErrorDialog();
//                dialog.Exception = ex;
//                dialog.State = state;
//                // 2007 12 27 - lpm - added to display message.
//                if (message != null && message.Length > 0)
//                    dialog.Message = message;
//                return dialog.ShowDialog(owner);
//            }
//            else
//                return MessageBox.Show(owner, ex.ToString());

            return ShowErrorInternal(owner, message, ex, MessageBoxIcon.Error);
		}

        private static DialogResult ShowErrorInternal(IWin32Window owner, string message, Exception ex, MessageBoxIcon icon)
        {
            if (ex != null)
                message = string.Format("{0}\r\n\r\n-----\r\n{1}", message, ex);

            return MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, icon);
        }

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		public static DialogResult ShowInformation(IWin32Window owner, string message)
		{
			return ShowInformation(owner, message, false);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="allowCancel"></param>
		public static DialogResult ShowInformation(IWin32Window owner, string message, bool allowCancel)
		{
			MessageBoxButtons buttons = MessageBoxButtons.OK;
			if(allowCancel == true)
				buttons = MessageBoxButtons.OKCancel;
			return MessageBox.Show(owner, message, Caption, buttons, MessageBoxIcon.Information);
		}

		/// <summary>
		/// Asks a 'yes/no' question.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="allowCancel"></param>
		public static DialogResult AskYesNoQuestion(IWin32Window owner, string message)
		{
			return AskYesNoQuestion(owner, message, false);
		}

		/// <summary>
		/// Asks a 'yes/no' question.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="allowCancel"></param>
		public static DialogResult AskYesNoQuestion(IWin32Window owner, string message, bool allowCancel)
		{
			MessageBoxButtons buttons = MessageBoxButtons.YesNo;
			if(allowCancel == true)
				buttons = MessageBoxButtons.YesNoCancel;
			return MessageBox.Show(owner, message, Caption, buttons, MessageBoxIcon.Question);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		public static void ShowWarning(IWin32Window owner, string message)
		{
			MessageBox.Show(owner, message, Caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		public static void ShowWarning(IWin32Window owner, string message, Exception exception)
		{
			// mbr - 15-05-2007 - changed.			
//			ShowWarning(owner, message, exception, null);
			ShowWarning(owner, message, exception, null, AlertFlags.Normal);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="state"></param>
		[Obsolete("Do not use this method.")]
		public static void ShowWarning(IWin32Window owner, string message, Exception exception, object state)
		{
			if(exception == null)
				throw new ArgumentNullException("exception");

			// defer...
			ShowWarning(owner, message, exception, state, PublishExceptionMode.PublishException);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="publishMode"></param>
		[Obsolete("Do not use this method.")]
		public static void ShowWarning(IWin32Window owner, string message, Exception exception, 
			PublishExceptionMode publishMode)
		{
			if(exception == null)
				throw new ArgumentNullException("exception");
		
			// defer...
			ShowWarning(owner, message, exception, null, publishMode);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="state"></param>
		/// <param name="publishMode"></param>
		[Obsolete("Do not use this method.")]
		public static void ShowWarning(IWin32Window owner, string message, Exception exception, 
			object state, PublishExceptionMode publishMode)
		{
			AlertFlags flags = AlertFlags.Normal;
			if(publishMode == PublishExceptionMode.DoNotPublishException)
				flags |= AlertFlags.SuppressErrorReport;

			// defer...
			ShowWarning(owner, message, exception, state, flags);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="state"></param>
		/// <param name="publishMode"></param>
		public static DialogResult ShowWarning(IWin32Window owner, string message, Exception exception, 
			AlertFlags flags)
		{
			return ShowWarning(owner, message, exception, null, flags);
		}

		/// <summary>
		/// Shows the given information message.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		/// <param name="state"></param>
		/// <param name="publishMode"></param>
		public static DialogResult ShowWarning(IWin32Window owner, string message, Exception ex, 
			object state, AlertFlags flags)
		{
            //if(exception == null)
            //    throw new ArgumentNullException("exception");

			// mbr - 10-05-2007 - back in the day, this uses to log error messages.  then for some
			// reason we stopped doing it.  I have re-instated the logging mechanism, but only if
			// we're running...

			// this is the old one...
//			// publish...
//			if(Runtime.IsStarted == true && publishMode == PublishExceptionMode.PublishException)
//				ExceptionPublisher.Current.PublishExceptionDetails(message, exception);

            // mbr - 2014-09-01 - turned off the error dialog....

            //// suppress...
            //bool suppressErrorReport = false;
            //if((int)(flags & AlertFlags.SuppressErrorReport) != 0)
            //    suppressErrorReport = true;

            //// this is the new one...
            //if(Runtime.IsStarted && !(suppressErrorReport))
            //{
            //    ILog log = LogSet.GetLog(typeof(Alert));
            //    if(log.IsErrorEnabled)
            //        log.Error(message, exception);
            //}

            //// show...
            //using(ErrorDialog dialog = new ErrorDialog())
            //{
            //    dialog.IconType = IconType.Warning;

            //    // show...
            //    dialog.Exception = exception;
            //    dialog.Message = message;

            //    // set...
            //    dialog.SuppressErrorReport = suppressErrorReport;
            //    dialog.State = state;

            //    // show...
            //    return dialog.ShowDialog(owner);
            //}

            return ShowErrorInternal(owner, message, ex, MessageBoxIcon.Warning);
		}

		/// <summary>
		/// Gets the caption.
		/// </summary>
		// mbr - 13-04-2006 - added setter and ability to change caption.		
		public static string Caption
		{
			get
			{
				if(_caption == null || _caption.Length == 0)
				{
					if(Runtime.IsStarted == true)
						return Runtime.Current.Application.ProductName;
					else
						return "Message";
				}
				else
					return _caption;
			}
			set
			{
				_caption = value;
			}
		}

		/// <summary>
		/// Shows the save file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowSaveHtmlFileDialog(IWin32Window owner, string filename)
		{
			return ShowSaveFileDialog(owner, HtmlFileFilter, filename);
		}

		/// <summary>
		/// Shows the save file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowSaveXmlFileDialog(IWin32Window owner, string filename)
		{
			return ShowSaveFileDialog(owner, XmlFileFilter, filename);
		}

		/// <summary>
		/// Shows the save file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowSaveTextFileDialog(IWin32Window owner, string filename)
		{
			return ShowSaveFileDialog(owner, TextFileFilter, filename);
		}

		/// <summary>
		/// Shows the save file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filter"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowSaveFileDialog(IWin32Window owner, string filter, string filename)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.Filter = filter;
			dialog.FileName = filename;
			if(Runtime.IsStarted == true)
			{
				dialog.InitialDirectory = GetInitialFolderForOpenSaveDialog(owner, filter);
				if(dialog.InitialDirectory != null && dialog.InitialDirectory.Length > 0 && Directory.Exists(dialog.InitialDirectory) == false)
					Directory.CreateDirectory(dialog.InitialDirectory);
			}
			if(dialog.ShowDialog(owner) == DialogResult.OK)
			{
				SetInitialFolderForOpenSaveDialog(owner, filter, new FileInfo(dialog.FileName).DirectoryName);
				return dialog.FileName;
			}
			else
				return null;
		}

		/// <summary>
		/// Shows the save file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowOpenXmlFileDialog(IWin32Window owner, string filename)
		{
			return ShowOpenFileDialog(owner, XmlFileFilter, filename);
		}

		/// <summary>
		/// Shows the save file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowOpenHtmlFileDialog(IWin32Window owner, string filename)
		{
			return ShowOpenFileDialog(owner, HtmlFileFilter, filename);
		}

		/// <summary>
		/// Shows the Open file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowOpenTextFileDialog(IWin32Window owner, string filename)
		{
			return ShowOpenFileDialog(owner, TextFileFilter, filename);
		}

		/// <summary>
		/// Shows the Open file dialog.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="filter"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string ShowOpenFileDialog(IWin32Window owner, string filter, string filename)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = filter;
			dialog.FileName = filename;
			if(Runtime.IsStarted == true)
			{
				dialog.InitialDirectory = GetInitialFolderForOpenSaveDialog(owner, filter);
				if(dialog.InitialDirectory != null && dialog.InitialDirectory.Length > 0 && Directory.Exists(dialog.InitialDirectory) == false)
					Directory.CreateDirectory(dialog.InitialDirectory);
			}
			if(dialog.ShowDialog(owner) == DialogResult.OK)
			{
				SetInitialFolderForOpenSaveDialog(owner, filter, new FileInfo(dialog.FileName).DirectoryName);
				return dialog.FileName;
			}
			else
				return null;
		}	

		/// <summary>
		/// Gets the initial folder for the open/save dialogs.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private static void SetInitialFolderForOpenSaveDialog(IWin32Window owner, string filter, string path)
		{
			try
			{
				if(filter != null && filter.Length > 0)
					Runtime.Current.UserSettings[@"SaveOpenDialog\" + filter] = path;
			}
			catch(Exception ex)
			{
				Alert.ShowWarning(owner, "The path for this type of document could not be set.", ex);
			}
		}

		/// <summary>
		/// Gets the initial folder for the open/save dialogs.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private static string GetInitialFolderForOpenSaveDialog(IWin32Window owner, string filter)
		{
			try
			{
				string path = null;
				if(filter != null && filter.Length > 0)
					path = Runtime.Current.UserSettings.GetStringValue(@"SaveOpenDialog\" + filter, null, null, OnNotFound.ReturnNull);

				// default?
				if(path != null && path.Length > 0)
					return path;
				else
					return Runtime.Current.DocumentsFolderPath;
			}
			catch(Exception ex)
			{
				// tell teh user...
				Alert.ShowWarning(owner, "The default path for this type of document could not be retrieved.", ex);

				// return null - we don't know if the retrieval of the default failed...
				return string.Empty;
			}
		}

        public static void ReportException(this IWin32Window owner, Action callback)
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                Alert.ShowWarning(owner, "An error occurred.", ex);
            }
        }
	}
}
