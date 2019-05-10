// BootFX - Application framework for .NET applications
// 
// File: ControlReference.cs
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
using WF = System.Windows.Forms;
using System.Collections;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>WebControlReference</c>.
	/// </summary>
	public class ControlReference
	{
		/// <summary>
		/// Private field to support <c>Control</c> property.
		/// </summary>
		private object _control;
		
		/// <summary>
		/// Private field to support <c>InnerId</c> property.
		/// </summary>
		private string _innerId;

		/// <summary>
		/// Private field to support <c>Caption</c> property.
		/// </summary>
		private string _caption;

		public ControlReference(WF.Control control, string caption) : this(control, null, caption)
		{
		}

		private ControlReference(object control, string innerId, string caption)
		{
			if(control == null)
				throw new ArgumentNullException("control");
			
			// set...
			_control = control;
			_innerId = innerId;
			_caption = caption;
		}

        private WF.Control WfControl
		{
			get
			{
				return this.Control as WF.Control;
			}
		}
        
		private void AssertWf()
		{
			if(Control == null)
				throw new InvalidOperationException("Control is null.");
			if(!(this.Control is WF.Control))
				throw new InvalidOperationException(string.Format("Control '{0}' is not a Windows Forms control."));
		}

		internal object ResolvedAnyControl
		{
			get
			{
				if(this.IsWebControl)
                    throw new NotImplementedException("This operation has not been implemented.");
				else
					return this.ResolvedWfControl;
			}
		}

		public WF.Control ResolvedWfControl
		{
			get
			{
				this.AssertWf();

				// return...
				return (WF.Control)this.Control;
			}
		}

		internal bool IsWebControl
		{
			get
			{
                return false;
			}
		}

		/// <summary>
		/// Gets the caption.
		/// </summary>
		public string Caption
		{
			get
			{
				// returns the value...
				if(_caption == null || _caption.Length == 0)
				{
					if(this.IsWebControl)
                        throw new NotImplementedException("This operation has not been implemented.");
					else
					{
						if(ResolvedWfControl == null)
							throw new InvalidOperationException("ResolvedWfControl is null.");
						return this.WfControl.Name;
					}
				}
				else
				{
					// we have a fixed caption!
					return _caption;
				}
			}
		}
		
		/// <summary>
		/// Gets the innerid.
		/// </summary>
		private string InnerId
		{
			get
			{
				// returns the value...
				return _innerId;
			}
		}

		/// <summary>
		/// Gets the control.
		/// </summary>
		private object Control
		{
			get
			{
				// returns the value...
				return _control;
			}
		}
	}
}
