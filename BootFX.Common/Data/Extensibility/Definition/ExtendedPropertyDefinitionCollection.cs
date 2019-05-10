// BootFX - Application framework for .NET applications
// 
// File: ExtendedPropertyDefinitionCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for ExtendedPropertyDefinitionCollection.
	/// </summary>
	public class ExtendedPropertyDefinitionCollection : CollectionBase
	{
		public ExtendedPropertyDefinitionCollection()
		{
		}

		public int Add(ExtendedPropertyDefinition property)
		{
			if(InnerList.Contains(property))
				return InnerList.IndexOf(property);

			if(Contains(property))
				throw new InvalidOperationException(string.Format("A property with name '{0}' for '{1}' already exists.", property.Name, property.EntityTypeId));

			return InnerList.Add(property);
		}

		public void Remove(ExtendedPropertyDefinition property)
		{
			if(!Contains(property))
				return;

			InnerList.RemoveAt(IndexOf(property));
		}

		public int IndexOf(ExtendedPropertyDefinition property)
		{
			return IndexOf(property.EntityTypeId, property.NativeName);
		}

		public int IndexOf(string entityTypeId, string nativeName)
		{
			if(entityTypeId == null)
				throw new ArgumentNullException("entityTypeId");
			if(entityTypeId.Length == 0)
				throw new ArgumentOutOfRangeException("'entityTypeId' is zero-length.");
			if(nativeName == null)
				throw new ArgumentNullException("name");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			for(int i = 0; i < InnerList.Count; i++)
			{
				ExtendedPropertyDefinition existingProperty = (ExtendedPropertyDefinition) InnerList[i];
				if(existingProperty.EntityTypeId == entityTypeId && string.Compare(existingProperty.NativeName, nativeName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					return i;
			}

			// nope...
			return -1;
		}

		public bool Contains(string entityTypeId, string nativeName)
		{
			if(entityTypeId == null)
				throw new ArgumentNullException("entityTypeId");
			if(entityTypeId.Length == 0)
				throw new ArgumentOutOfRangeException("'entityTypeId' is zero-length.");
			if(nativeName == null)
				throw new ArgumentNullException("name");
			if(nativeName.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// find...
			if(IndexOf(entityTypeId, nativeName) != -1)
				return true;
			else
				return false;
		}

		public bool Contains(ExtendedPropertyDefinition property)
		{
			return Contains(property.EntityTypeId, property.NativeName);
		}

		public ExtendedPropertyDefinition this[int index]
		{
			get
			{
				return (ExtendedPropertyDefinition) InnerList[index];
			}
		}

		public ExtendedPropertyDefinition this[string entityTypeId, string nativeName]
		{
			get
			{
				if(entityTypeId == null)
					throw new ArgumentNullException("entityTypeId");
				if(entityTypeId.Length == 0)
					throw new ArgumentOutOfRangeException("'entityTypeId' is zero-length.");
				if(nativeName == null)
					throw new ArgumentNullException("name");
				if(nativeName.Length == 0)
					throw new ArgumentOutOfRangeException("'name' is zero-length.");

				int index = IndexOf(entityTypeId, nativeName);
				if(index != -1)
					return this[index];
				else
					return null;
			}
		}

		public ExtendedPropertyDefinition[] GetPropertiesForEntityType(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			string id = entityType.Id;
			if(id == null)
				throw new InvalidOperationException("'id' is null.");
			if(id.Length == 0)
				throw new InvalidOperationException("'id' is zero-length.");

			// walk...
			ArrayList entityTypes = new ArrayList();
			foreach(ExtendedPropertyDefinition property in InnerList)
			{
				if(property.EntityTypeId == id)
					entityTypes.Add(property);
			}

			// return...
			return (ExtendedPropertyDefinition[])entityTypes.ToArray(typeof(ExtendedPropertyDefinition));
		}

		
		public EntityType[] GetExtendedEntityTypes()
		{
			ArrayList results = new ArrayList();
			foreach(ExtendedPropertyDefinition property in InnerList)
			{
				if(!(results.Contains(property.EntityTypeId)))
					results.Add(EntityType.GetEntityTypeForId(property.EntityTypeId, OnNotFound.ThrowException));
			}

			// return...
			return (EntityType[])results.ToArray(typeof(EntityType));
		}

		public ExtendedPropertyDefinition GetByName(string entityTypeId, string name, bool throwIfNotFound)
		{
			if(entityTypeId == null)
				throw new ArgumentNullException("entityTypeId");
			if(entityTypeId.Length == 0)
				throw new ArgumentOutOfRangeException("'entityTypeId' is zero-length.");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// walk...
			foreach(ExtendedPropertyDefinition prop in this.InnerList)
			{
				if(string.Compare(prop.EntityTypeId, entityTypeId, true, System.Globalization.CultureInfo.InvariantCulture) == 0 && 
					string.Compare(prop.Name, name, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				{
					return prop;
				}
			}

			// nope...
			if(throwIfNotFound)
				throw new InvalidOperationException(string.Format("A property with name '{0}' was not found on '{1}'.", name, entityTypeId));
			else
				return null;
		}
	}
}
