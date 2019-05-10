// BootFX - Application framework for .NET applications
// 
// File: CommandClickContextBase.cs
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

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>CommandFireContextBase</c>.
	/// </summary>
	public abstract class CommandClickContextBase : ICommandClickContext
	{
		protected CommandClickContextBase()
		{
		}

		/// <summary>
		/// Gets the focused entity.
		/// </summary>
		public object FocusedEntity
		{		
			get
			{
				IEntityView view = this.GetAssociatedView();
				if(view != null)
					return view.FocusedEntity;
				else
					return null;
			}
		}

		/// <summary>
		/// Gets the selected entities.
		/// </summary>
		public IList SelectedEntities
		{
			get
			{
				IEntityView view = this.GetAssociatedView();
				if(view != null)
					return view.SelectedEntities;
				else
					return null;
			}
		}

		public int SelectedEntitiesCount
		{
			get
			{
				IList selected = this.SelectedEntities;
				if(selected != null)
					return selected.Count;
				else
					return 0;
			}
		}

		/// <summary>
		/// Gets the associated view.
		/// </summary>
		/// <returns></returns>
		protected abstract IEntityView GetAssociatedView();
	}
}
