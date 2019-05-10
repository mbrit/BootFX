// BootFX - Application framework for .NET applications
// 
// File: IDataControlBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.ComponentModel;

namespace BootFX.Common.UI.DataBinding
{
	/// <summary>
	/// Represents a control that holds data.
	/// </summary>
	public interface IDataControlBase
	{
		/// <summary>
		/// Raised when the <c>Position</c> property has changed.
		/// </summary>
		event EventHandler PositionChanged;
		
		/// <summary>
		/// Raised when the <c>Current</c> property has changed.
		/// </summary>
		event EventHandler CurrentChanged;
		
		/// <summary>
		/// Raised when the <c>MetaData</c> property has changed.
		/// </summary>
		event EventHandler MetaDataChanged;

		/// <summary>
		/// Gets the data source.
		/// </summary>
		object DataSource
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the current item.
		/// </summary>
		object Current
		{
			get;
		}

		/// <summary>
		/// Gets the data source.
		/// </summary>
		IList DataSourceAsIList
		{
			get;
		}

		/// <summary>
		/// Gets the data source.
		/// </summary>
		IBindingList DataSourceAsIBindingList
		{
			get;
		}

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		void MoveFirst();

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		void MoveLast();

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		void MovePrevious();

		/// <summary>
		/// Moves to the first item in the list.
		/// </summary>
		void MoveNext();

		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		int Position
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the count.
		/// </summary>
		int Count
		{
			get;
		}

		/// <summary>
		/// Returns true if the control has a datasource defined.
		/// </summary>
		bool HasDataSource
		{
			get;
		}

		/// <summary>
		/// Returns true if the datasource is a list, false if the object is null, or the object is non-null and does not implement <see cref="IEnumerable"></see>.
		/// </summary>
		bool IsBindingToList
		{
			get;
		}
	}
}
