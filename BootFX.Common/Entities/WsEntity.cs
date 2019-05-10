// BootFX - Application framework for .NET applications
// 
// File: WsEntity.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Reflection;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for WsEntity.
	/// </summary>
	public class WsEntity
	{
		public string[] ExtendedPropertyNames;
		public object[] ExtendedPropertyValues;

		/// <summary>
		/// Populates an entity from its equivalent web service entity, it will
		/// attempt to load the entity from the database using the keys
		/// </summary>
		public Entity PopulateEntity(Type entityType)
		{
			Entity entity = GetEntity(entityType);

			// Walk each field and set the values
			FieldInfo[] fields = this.GetType().GetFields(BindingFlags.Public |BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			foreach(FieldInfo field in fields)
			{
				try
				{
					entity.GetType().GetProperty(field.Name,BindingFlags.Public |BindingFlags.Instance | BindingFlags.FlattenHierarchy).SetValue(entity,field.GetValue(this),null);
				}
				catch{}
			}
			return entity;
		}


		private Entity GetEntity(Type entityType)
		{
			// Create the template entity
			Entity entity = (Entity) Activator.CreateInstance(entityType);

			// Walk each value from the entity that is a key field
			ArrayList keyValues = new ArrayList();
			ArrayList keyTypes = new ArrayList();
			EntityField[] keyFields = entity.EntityType.GetKeyFields();
			foreach(EntityField field in keyFields)
			{
				// Get the field and set the value
				FieldInfo fieldInfo = this.GetType().GetField(field.Name);
				if(fieldInfo != null)
				{
					keyValues.Add(fieldInfo.GetValue(this));
					keyTypes.Add(field.Type);
				}
			}

			// Now we load the entity up using the GetById method
			Entity loadedEntity = null;
			MethodInfo getById = entity.GetType().GetMethod("GetById",BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public,null,(Type[]) keyTypes.ToArray(typeof(Type)),null);
			if(getById != null)
				loadedEntity = (Entity) getById.Invoke(entityType,(object[]) keyValues.ToArray(typeof(object)));

			// If we have a loaded entity we return it, otherwise we return the newly created entity
			if(loadedEntity != null)
				return loadedEntity;

			return entity;
		}
	}
}
