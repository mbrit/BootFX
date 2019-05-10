// BootFX - Application framework for .NET applications
// 
// File: IConstraintControl.cs
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
using BootFX.Common.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.UI
{
	/// <summary>
	/// Defines an instance of <c>IConstraintControl</c>.
	/// </summary>
	// mbr - 19-02-2008 - Removed ISqlFilterMangler.
	public interface IConstraintControl //: ISqlFilterMangler
	{
		/// <summary>
		/// Returns true if the value has been set.  
		/// </summary>
		/// <remarks>This has different meaning depending on the control.  On a text box, it means that it has to be filled in.  On a list box, an item must be selected.</remarks>
		bool IsValueSet
		{
			get;
		}

		object Value
		{
			get;
			set;
		}

		string EntityTypeId
		{
			get;
			set;
		}

		string FieldName
		{
			get;
			set;
		}

		// mbr - 19-02-2008 - removed.
//		SqlOperator Operator
//		{
//			get;
//			set;
//		}

		/// <summary>
		/// Gets or sets whether the control can understand DBNull.  
		/// </summary>
		/// <remarks>If this method returns null, BootFX will convert DBNullvalues into the closest CLR-legal equivalent.</remarks>
		ConstraintControlSupportFlags SupportFlags
		{
			get;
		}
	}
}
