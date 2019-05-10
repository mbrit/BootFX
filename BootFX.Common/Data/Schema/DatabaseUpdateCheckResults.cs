// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdateCheckResults.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Defines an instance of <c>DatabaseUpdateCheckResults</c>.
	/// </summary>
	public class DatabaseUpdateCheckResults
	{
		/// <summary>
		/// Private field to support <see cref="Steps"/> property.
		/// </summary>
		private DatabaseUpdateStepCollection _steps = new DatabaseUpdateStepCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="steps"></param>
		internal DatabaseUpdateCheckResults(DatabaseUpdateStepCollection steps)
		{
			if(steps == null)
				throw new ArgumentNullException("steps");
			this.Steps.AddRange(steps);
		}

		/// <summary>
		/// Gets the steps.
		/// </summary>
		internal DatabaseUpdateStepCollection Steps
		{
			get
			{
				return _steps;
			}
		}

		public string[] GetFriendlyUnitWorkMessages()
		{
			StringCollection messages = new StringCollection();
			foreach(DatabaseUpdateStep step in this.Steps)
				step.GetFriendlyWorkMessages(messages);

			// return...
			string[] results = new string[messages.Count];
			for(int index = 0; index < messages.Count; index++)
				results[index] = messages[index];
			return results;
		}

		public bool IsUpToDate
		{
			get
			{
				foreach(DatabaseUpdateStep step in this.Steps)
				{
					if(!(step.IsUpToDate))
						return false;
				}

				// ok...
				return true;
			}
		}
	}
}
