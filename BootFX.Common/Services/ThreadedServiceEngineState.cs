// BootFX - Application framework for .NET applications
// 
// File: ThreadedServiceEngineState.cs
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

namespace BootFX.Common.Services
{
	/// <summary>
	/// Defines an instance of <c>ThreadedServiceEngineState</c>.
	/// </summary>
	public class ThreadedServiceEngineState
	{
		/// <summary>
		/// Private field to support <see cref="IsWorking"/> property.
		/// </summary>
		private bool _isWorking;

		/// <summary>
		/// Private field to support <see cref="WorkTime"/> property.
		/// </summary>
		private TimeSpan _workTime;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="isWorking"></param>
		/// <param name="workTime"></param>
		internal ThreadedServiceEngineState(bool isWorking, TimeSpan workTime)
		{
			_isWorking = isWorking;
			_workTime = workTime;
		}

		/// <summary>
		/// Gets the worktime.
		/// </summary>
		public TimeSpan WorkTime
		{
			get
			{
				return _workTime;
			}
		}
		
		/// <summary>
		/// Gets the isworking.
		/// </summary>
		public bool IsWorking
		{
			get
			{
				return _isWorking;
			}
		}
	}
}
