// BootFX - Application framework for .NET applications
// 
// File: EntityMember.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Globalization;
using BootFX.Common.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a member on an entity.
	/// </summary>
	[Serializable()]
	public abstract class EntityMember : IEntityType
	{
		/// <summary>
		/// Defines the default stored name.
		/// </summary>
		private const string DefaultStoredName = "$_entityMember";

		/// <summary>
		/// Private field to support <c>EntityType</c> property.
		/// </summary>
		private EntityType _entityType;
		
		/// <summary>
		/// Private field to support <c>Ordinal</c> property.
		/// </summary>
		private int _ordinal;
		
		/// <summary>
		/// Private field to support <c>Name</c> property.
		/// </summary>
		private string _name;
		
		/// <summary>
		/// Private field to support <c>NativeName</c> property.
		/// </summary>
		private NativeName _nativeName;

		/// <summary>
		/// Private field to support <c>NativeName</c> property.
		/// </summary>
		private bool _ordinalSet = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		protected EntityMember(string name) : this(name, name)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		protected EntityMember(string name, string nativeName) : this(name, NativeName.GetNativeName(nativeName))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		protected EntityMember(string name, NativeName nativeName)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("name");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");
			
			_name = name;
			_nativeName = nativeName;
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Gets the nativename.
		/// </summary>
		public NativeName NativeName
		{
			get
			{
				return _nativeName;
			}
		}

		/// <summary>
		/// Gets the ordinal.
		/// </summary>
		public int Ordinal
		{
			get
			{
				return _ordinal;
			}
		}

		/// <summary>
		/// Sets the ordinal.
		/// </summary>
		/// <param name="ordinal"></param>
		internal void SetOrdinal(int ordinal)
		{
			if(_ordinalSet == true)
				throw new InvalidOperationException("Ordinal has already been set.");
			_ordinal = ordinal;
			_ordinalSet = true;
		}

		/// <summary>
		/// Gets the string represention of the field.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format(Cultures.System, "{0}: {1}, {2}, #{3}", base.ToString(), this.Name, this.NativeName, this.Ordinal);
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		public EntityType EntityType
		{
			get
			{
				// returns the value...
				return _entityType;
			}
		}

		/// <summary>
		/// Sets the entity type.
		/// </summary>
		/// <param name="entityType"></param>
		internal void SetEntityType(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			
			// already set?
			if(this.EntityType != null)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Entity type for '{0}' ({1}) already set to '{2}'.", this.Name, this.GetType(), entityType));

			// set...
			_entityType = entityType;
		}

		/// <summary>
		/// Gets the view property for the member.
		/// </summary>
		/// <returns></returns>
		public virtual EntityViewProperty GetViewProperty()
		{
			return new EntityMemberViewProperty(this);
		}

		/// <summary>
		/// Gets the property descriptor for the member.
		/// </summary>
		/// <returns></returns>
		public abstract PropertyDescriptor GetPropertyDescriptor();

		/// <summary>
		/// Gets the converter for this field.
		/// </summary>
		/// <returns></returns>
		public virtual TypeConverter GetConverter()
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		/// <summary>
		/// Gets the member name.
		/// </summary>
		internal EntityMemberName MemberName
		{
			get
			{
				return new EntityMemberName(this);
			}
		}

		/// <summary>
		/// Stores the object for serialization.
		/// </summary>
		/// <param name="info"></param>
		public void Store(SerializationInfo info)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// defer...
			this.Store(info, DefaultStoredName);
		}
	
		/// <summary>
		/// Stores the object for serialization.
		/// </summary>
		/// <param name="info"></param>
		public void Store(SerializationInfo info, string name)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// save...
			info.AddValue(name, this.MemberName);
		}

		/// <summary>
		/// Restores the object from serialization.
		/// </summary>
		/// <param name="info"></param>
		public static EntityMember Restore(SerializationInfo info, OnNotFound onNotFound)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			return Restore(info, DefaultStoredName, onNotFound);
		}

		/// <summary>
		/// Restores the object from serialization.
		/// </summary>
		/// <param name="info"></param>
		public static EntityMember Restore(SerializationInfo info, string name, OnNotFound onNotFound)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			EntityMemberName memberName = (EntityMemberName)info.GetValue(name, typeof(EntityMemberName));

			// find the type...
			EntityType entityType = EntityType.GetEntityType(memberName.EntityTypeName, onNotFound);
			if(entityType != null)
			{
				// find the member...
				return entityType.GetMember(memberName, onNotFound);
			}
			else
				return null;
		}

        /// <summary>
        /// Gets a comparer for this member.
        /// </summary>
        /// <returns></returns>
        // mbr - 2010-04-05 - added...
        public abstract IComparer GetComparer(CultureInfo culture);

        /// <summary>
        /// Gets a value for a member.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        // mbr - 2010-04-05 - added...
        public abstract object GetValue(object entity);

        /// <summary>
        /// Sets a value for a member.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="value"></param>
        // mbr - 2010-04-05 - added...
        public abstract void SetValue(object entity, object value, SetValueReason reason);
    }
}
