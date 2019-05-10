// BootFX - Application framework for .NET applications
// 
// File: Sound.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;	

namespace BootFX.Common.UI.Desktop
{
	/// <summary>
	/// Provides access to sounds.
	/// </summary>
	public sealed class Sound
	{
		// imports...
		[DllImport("winmm.dll")]
		private static extern int sndPlaySoundA(string lpszSoundName, int uFlags);

		/// <summary>
		/// Constructor.
		/// </summary>
		private Sound()
		{
		}

		/// <summary>
		/// Plays the given sound file or system sound.
		/// </summary>
		/// <returns></returns>
		public static void PlaySound(string path) 
		{
			// play...
			try
			{
				sndPlaySoundA(path, 1);
			}
			catch
			{
				// eat exceptions here - do we care if the sound could not be played?
			}
		}

		/// <summary>
		/// Plays the given system sound.
		/// </summary>
		/// <param name="sound"></param>
		public static void PlaySound(SystemSound sound)
		{
			switch(sound)
			{
				case SystemSound.Error:
					PlaySound("SystemHand");
					break;
				case SystemSound.SystemExit:
					PlaySound("SystemExit");
					break;
				case SystemSound.SystemStart:
					PlaySound("SystemStart");
					break;
				case SystemSound.Beep:
					PlaySound("SystemDefault");
					break;
				case SystemSound.Warning:
					PlaySound("SystemExclamation");
					break;
				case SystemSound.Question:
					PlaySound("SystemQuestion");
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", sound, sound.GetType()));
			}
		}

		/// <summary>
		/// Plays the given system sound.
		/// </summary>
		/// <param name="iconType"></param>
		public static void PlaySound(IconType iconType)
		{
			switch(iconType)
			{
				case IconType.Warning:
					PlaySound(SystemSound.Warning);
					break;

				case IconType.Error:
					PlaySound(SystemSound.Error);
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", iconType, iconType.GetType()));
			}
		}
	}
}
