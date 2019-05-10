// BootFX - Application framework for .NET applications
// 
// File: EntityManagedStorage.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a class that handle storage requests for <see cref="Entity"></see>.
	/// </summary>
	public class EntityManagedStorage : EntityService, IEntityStorage
	{
		/// <summary>
		/// Constructor..trim(
		/// </summary>
		/// <param name="entityType"></param>
		public EntityManagedStorage(EntityType entityType) : base(entityType)
		{
			if(typeof(Entity).IsAssignableFrom(entityType.Type) == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Entity type '{0}' cannot be used with managed storage.  Managed storage can only be used with the default Entity class.", entityType));
		}

		/// <summary>
		/// Gets a value from the instance.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public object GetValue(object entity, EntityField field)
		{
			// defer...
			return this.GetValue(entity, field, FetchFlags.None);
		}

		/// <summary>
		/// Gets a value from the instance.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public object GetValue(object entity, EntityField field, FetchFlags flags)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(field == null)
				throw new ArgumentNullException("field");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// cast...
			return this.GetValue(entity, field, ConversionFlags.Safe);
		}

		/// <summary>
		/// Gets a value from the instance.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public object GetValue(object entity, EntityField field, ConversionFlags flags)
		{
			return this.GetValue(entity, field, flags, FetchFlags.None);
		}

		/// <summary>
		/// Gets a value from the instance.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public object GetValue(object entity, EntityField field, ConversionFlags flags, FetchFlags fetchFlags)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(field == null)
				throw new ArgumentNullException("field");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// cast...
			return ((Entity)entity).GetValue(field, flags, fetchFlags);
		}

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <returns></returns>
		public object[] GetValues(object entity, EntityField[] fields)
		{
			// defer...
			return GetValues(entity, fields, FetchFlags.None);
		}

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <returns></returns>
		public object[] GetValues(object entity, EntityField[] fields, FetchFlags fetchFlags)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(fields.Length == 0)
				return new object[] {};

			// defer...
			return this.GetValues(entity, fields, ConversionFlags.Safe, fetchFlags);
		}

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <returns></returns>
		public object[] GetValues(object entity, EntityField[] fields, ConversionFlags flags)
		{
			// defer...
			return this.GetValues(entity, fields, flags, FetchFlags.None);
		}

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <returns></returns>
		public object[] GetValues(object entity, EntityField[] fields, ConversionFlags flags, FetchFlags fetchFlags)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(fields.Length == 0)
				return new object[] {};

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// create...
			object[] results = new object[fields.Length];
			for(int index = 0; index < fields.Length; index++)
				results[index] = this.GetValue(entity, fields[index], flags, fetchFlags);

			// return...
			return results;
		}

		/// <summary>
		/// Sets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		/// <param name="values"></param>
		public void SetValues(object entity, EntityField[] fields, object[] values, SetValueReason reason)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(fields.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fields");
			if(values == null)
				throw new ArgumentNullException("values");
			if(values.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("values");
			if(fields.Length != values.Length)
				throw ExceptionHelper.CreateLengthMismatchException("fields", "values", fields.Length, values.Length);
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// set...
			for(int index = 0; index < fields.Length; index++)
				this.SetValue(entity, fields[index], values[index], reason);
		}

		/// <summary>
		/// Gets a value from the instance.
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public void SetValue(object entity, EntityField field, object value, SetValueReason reason)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(field == null)
				throw new ArgumentNullException("field");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// cast...
			((Entity)entity).SetValue(field, value, reason);
		}

		public void BeginInitialize(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).BeginInitialize();
		}

		public void EndInitialize(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).EndInitialize();
		}

		public bool IsInitializing(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			return ((Entity)entity).IsInitializing();
		}

		public bool IsNew(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			return ((Entity)entity).IsNew();
		}

		public bool IsModified(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			return ((Entity)entity).IsModified();
		}

		/// <summary>
		/// Returns true if the given field is modified.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public bool IsModified(object entity, EntityField field)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(field == null)
				throw new ArgumentNullException("field");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			return ((Entity)entity).IsModified(field);
		}

		/// <summary>
		/// Returns true if the entity has been deleted or is marked for deletion.
		/// </summary>
		/// <returns></returns>
		public bool IsDeleted(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			return this.IsDeleted(entity, true);
		}

		/// <summary>
		/// Returns true if the entity has been deleted or (optionally) is marked for deletion.
		/// </summary>
		/// <param name="treatMarkedAsDeleted">True if <c>IsMarkedForDeletion</c> should be treated as 'deleted'.</param>
		/// <returns></returns>
		public bool IsDeleted(object entity, bool treatMarkedAsDeleted)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			return ((Entity)entity).IsDeleted(treatMarkedAsDeleted);
		}

		/// <summary>
		/// Returns true if an entity is marked for deletion.
		/// </summary>
		/// <returns></returns>
		public bool IsMarkedForDeletion(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			return ((Entity)entity).IsMarkedForDeletion();
		}

		/// <summary>
		/// Sets the object up to be deleted.
		/// </summary>
		public void MarkForDeletion(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).MarkForDeletion();
		}

		/// <summary>
		/// Sets the object up to be deleted.
		/// </summary>
		public void UnmarkForDeletion(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).UnmarkForDeletion();
		}
		
		/// <summary>
		/// Marks the entity as physically removed from the database.
		/// </summary>
		public void MarkAsDeleted(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).MarkAsDeleted();
		}

		/// <summary>
		/// Called when the entity should no longer be marked as modified.
		/// </summary>
		public void ResetModifiedFlags(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).ResetModifiedFlags();
		}

		/// <summary>
		/// Called when the entity should no longer be marked as new.
		/// </summary>
		public void ResetIsNew(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).ResetIsNew();
		}

		/// <summary>
		/// Called when the entity should no longer be marked as new.
		/// </summary>
		public void SetIsNew(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// init...
			((Entity)entity).SetIsNew();
		}

		/// <summary>
		/// Gets key values for an entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		public object[] GetKeyValues(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// return...
			return this.GetKeyValues(entity, ConversionFlags.Safe);
		}

		/// <summary>
		/// Gets key values for an entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		public object[] GetKeyValues(object entity, ConversionFlags flags)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// return...
			return this.GetValues(entity, this.EntityType.GetKeyFields(), flags);
		}

		/// <summary>
		/// Sets the parent for the given entity and link.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public void SetParent(object entity, ChildToParentEntityLink link, object newParent)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// set...
			((Entity)entity).SetParent(link, newParent);
		}

		/// <summary>
		/// Gets the parent for the given entity and link.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public object GetParent(object entity, ChildToParentEntityLink link)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// return...
			return ((Entity)entity).GetParent(link);
		}

		/// <summary>
		/// Returns true if the given field is holding a DB null value.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public bool IsDBNull(object entity, EntityField field)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// return...
			return ((Entity)entity).IsDBNull(field);
		}

		/// <summary>
		/// Sets the given field to be DB null.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		public void SetDBNull(object entity, EntityField field)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// return...
			((Entity)entity).SetDBNull(field);
		}

		/// <summary>
		/// Gets whether the entity is read-only.
		/// </summary>
		public bool GetIsReadOnly(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// get...
			return ((Entity)entity).IsReadOnly;
		}

		/// <summary>
		/// Sets whether an entity is read-only.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="isReadOnly"></param>
		public void SetReadOnly(object entity, bool isReadOnly)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// set...
			((Entity)entity).IsReadOnly = isReadOnly;
		}

		/// <summary>
		/// Raised when loading has finished.
		/// </summary>
		// mbr - 02-10-2007 - for c7 - added.		
		public void LoadFinished(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// defer...
			((Entity)entity).OnAfterLoad();
		}
	}
}
