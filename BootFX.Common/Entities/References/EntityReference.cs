// BootFX - Application framework for .NET applications
// 
// File: EntityReference.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Collections;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>EntityReference</c>.
	/// </summary>
	[Serializable()]
	public class EntityReference : ISerializable
	{
		/// <summary>
		/// Private field to support <c>Entity</c> property.
		/// </summary>
		private object _entity;

		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Private field to support <c>Reference</c> property.
		/// </summary>
		private string _reference;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entity"></param>
		public EntityReference(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// set...
			_entity = entity;
			_entityType = EntityType.GetEntityType(entity, OnNotFound.ThrowException);
		}

		protected EntityReference(SerializationInfo info, StreamingContext context)
		{
			// get...
			_reference = info.GetString("_ref");
			if(_reference == null)
				throw new InvalidOperationException("'_reference' is null.");
			if(_reference.Length == 0)
				throw new InvalidOperationException("'_reference' is zero-length.");
		}

		private void CheckHydration()
		{
			// returns the value...
			if(_entity == null || _entityType == null)
			{
				// get...
				string reference = _reference;
				if(reference == null)
					throw new InvalidOperationException("'reference' is null.");
				if(reference.Length == 0)
					throw new InvalidOperationException("'reference' is zero-length.");

				// split...
				string[] parts = reference.Split('|');
				if(parts == null)
					throw new InvalidOperationException("'parts' is null.");
				if(parts.Length == 1)
					throw new InvalidOperationException("'parts' is an invalid length.");

				// get...
				Type type = Type.GetType(parts[0]);
				if(type == null)
					throw new InvalidOperationException(string.Format("'{0}' is an invalid type.", type));

				// keys...
				_entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
				if(_entityType == null)
					throw new InvalidOperationException("_entityType is null.");

				// get...
				EntityField[] keyFields = _entityType.GetKeyFields();
				if(keyFields.Length != parts.Length - 1)
					throw new InvalidOperationException(string.Format("Key length mismatch: {0} cf {1}.", keyFields.Length, parts.Length - 1));
				object[] keys = new object[keyFields.Length];
				for(int index = 0; index < keyFields.Length; index++)
				{
					// get...
					string asString = parts[index + 1];

					// convert...
					keys[index] = ConversionHelper.ChangeType(asString, keyFields[index].Type, Cultures.System);
				}

				// get...
				_entity = _entityType.Persistence.GetById(keys, OnNotFound.ThrowException);
				if(_entity == null)
					throw new InvalidOperationException("_entity is null.");
			}
		}

		/// <summary>
		/// Gets the entity.
		/// </summary>
		public object Entity
		{
			get
			{
				this.CheckHydration();
				return _entity;
			}
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		private EntityType EntityType
		{
			get
			{
				this.CheckHydration();
				return _entityType;
			}
		}

		public override string ToString()
		{
			return this.Reference;
		}

		/// <summary>
		/// Gets the entity representation.
		/// </summary>
		private string Reference
		{
			get
			{
				if(_reference == null || _reference.Length == 0)
				{
					// check...
					if(EntityType == null)
						throw new InvalidOperationException("EntityType is null.");
					if(Entity == null)
						throw new InvalidOperationException("Entity is null.");

					// set...
					StringBuilder builder = new StringBuilder();
					builder.Append(this.EntityType.TypePartiallyQualifiedName);

					// get...
					foreach(EntityField field in this.EntityType.GetKeyFields())
					{
						builder.Append("|");
						builder.Append(ConversionHelper.ToString(field.GetValue(this.Entity), Cultures.System));
					}

					// return...
					_reference = builder.ToString();
				}

				// return...
				return _reference;
			}
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			// get...
			string reference = this.Reference;
			if(reference == null)
				throw new InvalidOperationException("'reference' is null.");
			if(reference.Length == 0)
				throw new InvalidOperationException("'reference' is zero-length.");

			// return...
			info.AddValue("_ref", reference);
		}
	}
}
