// BootFX - Application framework for .NET applications
// 
// File: IEntityDataControl.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.UI.Desktop.DataBinding;
using BootFX.Common.Entities;

namespace BootFX.Common.UI.Desktop.DataBinding
{
	/// <summary>
	/// Defines an interface that extends <see cref="IDataControl"/> to include an understanding of entities.
	/// </summary>
	public interface IEntityDataControl : IDataControl, IEntityType
	{
		/// <summary>
		/// Gets the entity binding manager.
		/// </summary>
		EntityBindingManager EntityBindingManager
		{
			get;
		}

		/// <summary>
		/// Gets the <c>Current</c> as an entity.
		/// </summary>
		/// <remarks>This will unwrap any <c>EntityView</c> instance passed in.  This property does not
		/// validate that the entity is a vaild entity.</remarks>
		object CurrentEntity
		{
			get;
		}

		/// <summary>
		/// Gets the entity type of <c>CurrentEntity</c>.  This will return null if the entity type is not found.
		/// </summary>
		EntityType CurrentEntityType
		{
			get;
		}

		/// <summary>
		/// Gets the entity type of <c>CurrentEntity</c>.
		/// </summary>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		EntityType GetCurrentEntityType(OnNotFound onNotFound);
	}
}
