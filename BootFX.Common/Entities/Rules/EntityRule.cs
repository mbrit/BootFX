// BootFX - Application framework for .NET applications
// 
// File: EntityRule.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines a base class for entity rules.
	/// </summary>
	public abstract class EntityRule
	{
		/// <summary>
		/// Private field to support <see cref="ViolationMessage"/> property.
		/// </summary>
		private string _violationMessage;
		
		/// <summary>
		/// Private field to support <see cref="Applies"/> property.
		/// </summary>
		private EntityRuleApplies _applies;
		
		/// <summary>
		/// Raised when the rule needs to be validated.
		/// </summary>
		public event EventHandler Validate;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		protected EntityRule(EntityRuleApplies applies, string violationMessage)
		{
			if(violationMessage == null)
				throw new ArgumentNullException("violationMessage");
			if(violationMessage.Length == 0)
				throw new ArgumentOutOfRangeException("'violationMessage' is zero-length.");

			_applies = applies;
			_violationMessage = violationMessage;
		}

		/// <summary>
		/// Gets the violationmessage.
		/// </summary>
		private string ViolationMessage
		{
			get
			{
				return _violationMessage;
			}
		}

		/// <summary>
		/// Gets the applies.
		/// </summary>
		private EntityRuleApplies Applies
		{
			get
			{
				return _applies;
			}
		}

		/// <summary>
		/// Raises the <c>Validate</c> event.
		/// </summary>
		private void OnValidate()
		{
			OnValidate(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>Validate</c> event.
		/// </summary>
		protected virtual void OnValidate(EventArgs e)
		{
			// raise...
			if(Validate != null)
				Validate(this, e);
		}

		internal bool ValidateHasSubscribers
		{
			get
			{
				if(this.Validate == null)
					return false;
				else
					return true;
			}
		}
	}
}
