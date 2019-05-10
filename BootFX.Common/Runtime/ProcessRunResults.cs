// BootFX - Application framework for .NET applications
// 
// File: ProcessRunResults.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Returns the results of a process that is run until it quits.
	/// </summary>
	public class ProcessRunResults
	{
		/// <summary>
		/// Private field to support <see cref="ExitCode"/> property.
		/// </summary>
		private int _exitCode;

		/// <summary>
		/// Private field to support <see cref="OutMessage"/> property.
		/// </summary>
		private string _outMessage;

		/// <summary>
		/// Private field to support <see cref="ErrorMessage"/> property.
		/// </summary>
		private string _errorMessage;
		
		internal ProcessRunResults(int exitCode, string outMessage, string errorMessage)
		{
			_exitCode = exitCode;
			_outMessage = outMessage;
			_errorMessage = errorMessage;
		}

		/// <summary>
		/// Gets the errormessage.
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return _errorMessage;
			}
		}

        public bool HasErrorMessage
        {
            get
            {
                return !(string.IsNullOrEmpty(this.ErrorMessage));
            }
        }

        /// <summary>
        /// Gets the outmessage.
        /// </summary>
        public string OutMessage
		{
			get
			{
				return _outMessage;
			}
		}

        public bool HasOutMessage
        {
            get
            {
                return !(string.IsNullOrEmpty(this.OutMessage));
            }
        }
		
		/// <summary>
		/// Gets the exitcode.
		/// </summary>
		public int ExitCode
		{
			get
			{
				return _exitCode;
			}
		}

        public void AssertZeroExitCode()
        {
            if (this.ExitCode != 0)
                throw new InvalidOperationException(string.Format("Invalid exit code '{0}'\r\nOut: {1}\r\nError: {2}", this.ExitCode, this.OutMessage, this.ErrorMessage));
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Run results:\r\nExit code: ");
            builder.Append(this.ExitCode);
            builder.Append("\r\nOut: ");
            if (this.HasOutMessage)
                builder.Append(this.OutMessage);
            else
                builder.Append("(null)");
            builder.Append("\r\nError: ");
            if (this.HasErrorMessage)
                builder.Append(this.ErrorMessage);
            else
                builder.Append("(null)");

            return builder.ToString();
        }

        public void DumpToLog(ILog log = null)
        {
            if (log == null)
                log = LogSet.GetLog<ProcessRunResults>();

            if (log.IsInfoEnabled)
                log.Info(this.ToString());
        }
	}
}
