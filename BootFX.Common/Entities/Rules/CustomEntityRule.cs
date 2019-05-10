// BootFX - Application framework for .NET applications
// 
// File: CustomEntityRule.cs
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
	/// Summary description for CustomRule.
	/// </summary>
	public class CustomEntityRule : EntityRule
	{
		public CustomEntityRule(EntityRuleApplies applies, string violationMessage)
			 : base(applies, violationMessage)
		{
		}

		protected override void OnValidate(EventArgs e)
		{
			if(!(this.ValidateHasSubscribers))
				throw new InvalidOperationException("Custom rule does not have any subscribers.");

			// base...
			base.OnValidate(e);
		}
	}
}
