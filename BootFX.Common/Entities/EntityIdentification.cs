// BootFX - Application framework for .NET applications
// 
// File: EntityIdentification.cs
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
	///	 Provides functionality for comparing entities or sets of keys.
	/// </summary>
	public sealed class EntityIdentification
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private EntityIdentification()
		{
		}

		/// <summary>
		/// Compares two entities.
		/// </summary>
		/// <param name="entityA"></param>
		/// <param name="entityB"></param>
		/// <returns></returns>
		public new static bool Equals(object entityA, object entityB)
		{
			// convert...
			if(entityA is EntityView)
				entityA = ((EntityView)entityA).Entity;
			if(entityB is EntityView)
				entityB = ((EntityView)entityB).Entity;

			// both null?
			if(entityA == null && entityB == null)
				return true;

			// either null?
			// TODO: can this be collapsed down to a single test?
			if(entityA == null && entityB != null)
				return false;
			if(entityA != null && entityB == null)
				return false;

			// type?
			Type typeA = entityA.GetType();
			if(typeA != entityB.GetType())
				return false;

			// get the entity type...
			EntityType entityType = EntityType.GetEntityType(typeA, OnNotFound.ReturnNull);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// storage...
			IEntityStorage storage = entityType.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// get the keys from both...
			EntityField[] keyFields = entityType.GetKeyFields();
			object[] keyA = storage.GetValues(entityA, keyFields);
			object[] keyB = storage.GetValues(entityB, keyFields);

			// compare...
			return CompareKeys(keyA, keyB);
		}

		/// <summary>
		/// Compares two sets of keys.
		/// </summary>
		/// <param name="keyA"></param>
		/// <param name="keyB"></param>
		/// <returns></returns>
		public static bool CompareKeys(object[] keyA, object[] keyB)
		{
			if(keyA == null)
				throw new ArgumentNullException("keyA");
			if(keyB == null)
				throw new ArgumentNullException("keyB");
			
			if(keyA.Length != keyB.Length)
				throw ExceptionHelper.CreateLengthMismatchException("keyA", "keyB", keyA.Length, keyB.Length);

			// walk...
			for(int index = 0; index < keyA.Length; index++)
			{
				// compare...
				if(object.Equals(keyA[index], keyB[index]) == false)
					return false;
			}

			// yep...
			return true;
		}

		public static bool CompareEntityKeyValues(object entity1, object entity2)
		{
			if(entity1 == null)
				throw new ArgumentNullException("entity1");
			if(entity2 == null)
				throw new ArgumentNullException("entity2");
			
			// get...
			EntityType et = EntityType.GetEntityType(entity1, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");
			et.AssertIsOfType(entity2);

			// get...
			object[] key1 = et.Storage.GetKeyValues(entity1);
			if(key1 == null)
				throw new ArgumentNullException("key1");			
			object[] key2 = et.Storage.GetKeyValues(entity2);
			if(key2 == null)
				throw new ArgumentNullException("key2");	

			// compare...
			return CompareKeys(key1, key2);
		}
	}
}
