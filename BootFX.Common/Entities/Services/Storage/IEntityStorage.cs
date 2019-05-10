// BootFX - Application framework for .NET applications
// 
// File: IEntityStorage.cs
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
	///	 Defines a base interface for entity storage.
	/// </summary>
	/// <seealso cref="EntityManagedStorage"></seealso>
	public interface IEntityStorage : IEntityType
	{
		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>By default, <see cref="ConversionFlags.Safe"></see> is implied</remarks>
		object GetValue(object entity, EntityField field);
		
		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		object GetValue(object entity, EntityField field, ConversionFlags flags);

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>By default, <see cref="ConversionFlags.Safe"></see> is implied</remarks>
		object GetValue(object entity, EntityField field, FetchFlags fetchFlags);
		
		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		object GetValue(object entity, EntityField field, ConversionFlags flags, FetchFlags fetchFlags);

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>By default, <see cref="ConversionFlags.Safe"></see> is implied</remarks>
		object[] GetValues(object entity, EntityField[] fields);

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		object[] GetValues(object entity, EntityField[] fields, ConversionFlags flags);

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>By default, <see cref="ConversionFlags.Safe"></see> is implied</remarks>
		object[] GetValues(object entity, EntityField[] fields, FetchFlags fetchFlags);

		/// <summary>
		/// Gets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		object[] GetValues(object entity, EntityField[] fields, ConversionFlags flags, FetchFlags fetchFlags);

		/// <summary>
		/// Gets key values for an entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		/// <remarks>By default, <see cref="ConversionFlags.Safe"></see> is implied</remarks>
		object[] GetKeyValues(object entity);

		/// <summary>
		/// Gets key values for an entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		object[] GetKeyValues(object entity, ConversionFlags flags);

		/// <summary>
		/// Sets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <param name="value"></param>
		void SetValue(object entity, EntityField field, object value, SetValueReason reason);

		/// <summary>
		/// Sets a value on the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <param name="value"></param>
		void SetValues(object entity, EntityField[] fields, object[] values, SetValueReason reason);

		/// <summary>
		/// Begins initialization.
		/// </summary>
		/// <param name="entity"></param>
		void BeginInitialize(object entity);

		/// <summary>
		/// Begins initialization.
		/// </summary>
		/// <param name="entity"></param>
		void EndInitialize(object entity);

		/// <summary>
		/// Begins initialization.
		/// </summary>
		/// <param name="entity"></param>
		bool IsInitializing(object entity);

		/// <summary>
		/// Returns true if the given entity is new.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsNew(object entity);

		/// <summary>
		/// Returns true if the given entity is modified.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsModified(object entity);

		/// <summary>
		/// Returns true if the given field is modified.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		bool IsModified(object entity, EntityField field);

		/// <summary>
		/// Returns true if the entity has been deleted or is marked for deletion.
		/// </summary>
		/// <returns></returns>
		bool IsDeleted(object entity);

		/// <summary>
		/// Returns true if the entity has been deleted or (optionally) is marked for deletion.
		/// </summary>
		/// <param name="treatMarkedAsDeleted">True if <c>IsMarkedForDeletion</c> should be treated as 'deleted'.</param>
		/// <returns></returns>
		bool IsDeleted(object entity, bool treatMarkedAsDeleted);
				
		/// <summary>
		/// Returns true if an entity is marked for deletion.
		/// </summary>
		/// <returns></returns>
		bool IsMarkedForDeletion(object entity);

		/// <summary>
		/// Sets the object up to be deleted.
		/// </summary>
		void MarkForDeletion(object entity);

		/// <summary>
		/// Sets the object up to be deleted.
		/// </summary>
		void UnmarkForDeletion(object entity);

		/// <summary>
		/// Marks the entity as physically removed from the database.
		/// </summary>
		void MarkAsDeleted(object entity);

		/// <summary>
		/// Called when the entity should no longer be marked as modified.
		/// </summary>
		void ResetModifiedFlags(object entity);

		/// <summary>
		/// Called when the entity should no longer be marked as new.
		/// </summary>
		void ResetIsNew(object entity);

		/// <summary>
		/// Called when the entity should no be marked as new.
		/// </summary>
		void SetIsNew(object entity);

		/// <summary>
		/// Sets the parent for the given entity and link.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		void SetParent(object entity, ChildToParentEntityLink link, object newParent);

		/// <summary>
		/// Gets the parent for the given entity and link.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		object GetParent(object entity, ChildToParentEntityLink link);

		/// <summary>
		/// Returns true if the given field is holding a DB null value.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		bool IsDBNull(object entity, EntityField field);

		/// <summary>
		/// Sets the given field to be DB null.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="field"></param>
		/// <returns></returns>
		void SetDBNull(object entity, EntityField field);

		/// <summary>
		/// Gets whether the entity is read-only.
		/// </summary>
		bool GetIsReadOnly(object entity);

		/// <summary>
		/// Sets whether an entity is read-only.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="isReadOnly"></param>
		void SetReadOnly(object entity, bool isReadOnly);
	}
}
