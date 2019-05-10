// BootFX - Application framework for .NET applications
// 
// File: SelectMap.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Runtime.Serialization;
using BootFX.Common.Entities;
using System.Security;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Stores how fields in a SQL statement maps to fields on an entity.
	/// </summary>
	[Serializable()]
	public class SelectMap : ISerializable
	{
		/// <summary>
		/// Private field to support <c>MapFields</c> property.
		/// </summary>
		private SelectMapFieldCollection _mapFields = new SelectMapFieldCollection();
		
		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal SelectMap(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			_entityType = entityType;
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private SelectMap(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// get...
			_entityType = EntityType.Restore(info, OnNotFound.ThrowException);
			_mapFields = (SelectMapFieldCollection)info.GetValue("_mapFields", typeof(SelectMapFieldCollection));
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				return _entityType;
			}
		}

		/// <summary>
		/// Gets a collection of SelectMapFields objects.
		/// </summary>
		public SelectMapFieldCollection MapFields
		{
			get
			{
				return _mapFields;
			}
		}

		/// <summary>
		/// Gets the object data.
		/// </summary>
		/// <param name="?"></param>
		/// <param name="context"></param>
        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// set...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			this.EntityType.Store(info);

			// fields...
			info.AddValue("_mapFields", _mapFields);
		}
	}
}
