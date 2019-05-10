// BootFX - Application framework for .NET applications
// 
// File: UacHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Diagnostics;
using BootFX.Common;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for UacHelper.
	/// </summary>
	public class UacHelper
	{
		public static bool Elevated
		{
			get
			{
				// os version
				OperatingSystem osInfo = Environment.OSVersion;

				// vista?
				if(osInfo.Version.Major == 6)
				{
					// get principle
					WindowsPrincipal priciple = new WindowsPrincipal(WindowsIdentity.GetCurrent());

					// return are we admin
					return priciple.IsInRole(WindowsBuiltInRole.Administrator);
				}

				// if we aren't running vista, we don't need to elevate at all - so treat as elevated
				return true;
			}
		}
		public static void Elevate(ILog log, string runFolderPath, string fileName)
		{
			if (log == null)
				throw new InvalidOperationException("'log' was null.");
			if (runFolderPath == null)
				throw new InvalidOperationException("'runFolderPath' was null.");
			if (runFolderPath.Length == 0)
				throw new InvalidOperationException("'runFolderPath' was zero-length");
			if (fileName == null)	
				throw new InvalidOperationException("'fileName' was null.");
			if (fileName.Length == 0)
				throw new InvalidOperationException("'fileName' was zero-length");

			if(Elevated)
				return;

			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.UseShellExecute = true;
			startInfo.WorkingDirectory = runFolderPath;
			startInfo.FileName = fileName;
			startInfo.Verb = "runas";

			if(log.IsInfoEnabled)
				log.InfoFormat("\tStarting {0}\\{1}", startInfo.WorkingDirectory, startInfo.FileName);

			try
			{
				Process p = Process.Start(startInfo);
				p.WaitForExit();
				if(log.IsInfoEnabled)
					log.Info("\tPermissions elevated succersfully.");

				Environment.Exit(0);
			}
			catch (Exception ex)
			{
				if(log.IsErrorEnabled)
					log.Error("\tError trying to elevate permissions", ex);
				return;
			}
		}
	}
}
