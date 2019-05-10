// BootFX - Application framework for .NET applications
// 
// File: EntityLinkBinding.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Windows.Forms;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.UI.Desktop.DataBinding
{
	/// <summary>
	/// Extends Windows Forms data binding for use with links.
	/// </summary>
	/// <remarks>Specifically, this handles formatting and parsing the values to and from strings in accordance
	/// with Windows Forms rules and the rules of the link itself.</remarks>
	public class EntityLinkBinding : EntityMemberBinding
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityLinkBinding(EntityLink link, object dataSource, string propertyName) : base(link, propertyName, dataSource, link.Name)
		{
		}

		/// <summary>
		/// Gets the Link.
		/// </summary>
		public EntityLink Link
		{
			get
			{
				return (EntityLink)this.Member;
			}
		}

		protected override void OnFormat(ConvertEventArgs e)
		{
			// this doesn't 't call the base implementation as we're overriding behavior, not extending...

			// log...
			if(this.Log.IsInfoEnabled)
				this.Log.Info(string.Format(Cultures.Log, "Requested format of '{0}' to '{1}'", LogSet.ToString(e.Value), e.DesiredType));

			// convert...
			if(typeof(string).IsAssignableFrom(e.DesiredType))
				e.Value = ConversionHelper.ToString(e.Value, Cultures.User);
			else
				e.Value = ConversionHelper.ChangeType(e.Value, e.DesiredType, Cultures.User, ConversionFlags.Safe);

			// log...
			if(this.Log.IsInfoEnabled)
				this.Log.Info(string.Format(Cultures.Log, "\tResult: {0}", LogSet.ToString(e.Value)));
		}

		protected override void OnParse(ConvertEventArgs e)
		{
			// this doesn't 't call the base implementation as we're overriding behavior, not extending...

			// log...
			if(this.Log.IsInfoEnabled)
				this.Log.Info(string.Format(Cultures.Log, "Requested parse of '{0}' to '{1}'", LogSet.ToString(e.Value), e.DesiredType));

			// unwrap an entity view...
			object value = EntityView.Unwrap(e.Value);

			// get the entity type...
			if(e.DesiredType.IsAssignableFrom(value.GetType()) == true)
				e.Value = value;
			else
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Cannot convert '{0}' to '{1}'.", value.GetType(), e.DesiredType));

			// log...
			if(this.Log.IsInfoEnabled)
				this.Log.Info(string.Format(Cultures.Log, "\tResult: {0}", LogSet.ToString(e.Value)));
		}
	}
}
