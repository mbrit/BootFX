// BootFX - Application framework for .NET applications
// 
// File: SelectMapField.cs
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
	///	 Describes how a specific column on a SQL statement maps to a specific <see cref="EntityField"></see>.
	/// </summary>
	[Serializable()]
	public class SelectMapField : ISerializable
	{
		/// <summary>
		/// Private field to support <see cref="Join"/> property.
		/// </summary>
		private SqlJoin _join;
		
		/// <summary>
		/// Private field to support <c>ResultOrdinal</c> property.
		/// </summary>
		private int _resultOrdinal;
		
		/// <summary>
		/// Private field to support <c>Field</c> property.
		/// </summary>
		private EntityField _field;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="field"></param>
		public SelectMapField(EntityField field, int resultOrdinal)
		{
			if(field == null)
				throw new ArgumentNullException("field");
			
			_field = field;
			_resultOrdinal = resultOrdinal;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="field"></param>
		// mbr - 10-10-2007 - case 875 - added.		
		internal SelectMapField(EntityField field, int resultOrdinal, SqlJoin join)
			: this(field, resultOrdinal)
		{
			if(join == null)
				throw new ArgumentNullException("join");
			
			// set...
			_join = join;
		}

		/// <summary>
		/// Deserialization constructor.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SelectMapField(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			
			// get...
			_resultOrdinal = info.GetInt32("_resultOrdinal");
			_field = (EntityField)EntityMember.Restore(info, OnNotFound.ThrowException);
			if(_field == null)
				throw new InvalidOperationException("_field is null.");
		}

		/// <summary>
		/// Gets the resultordinal.
		/// </summary>
		public int ResultOrdinal
		{
			get
			{
				return _resultOrdinal;
			}
		}

		/// <summary>
		/// Gets the field.
		/// </summary>
		public EntityField Field
		{
			get
			{
				return _field;
			}
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		// mbr - 10-10-2007 - case 875 - added.
		internal EntityType EntityType
		{
			get
			{
				if(Field == null)
					throw new InvalidOperationException("Field is null.");
				return this.Field.EntityType;
			}
		}

        [SecurityCritical]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		/// <summary>
		/// Gets serialized data.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if(info == null)
				throw new ArgumentNullException("info");

			// add...
			info.AddValue("_resultOrdinal", _resultOrdinal);
			if(_field == null)
				throw new InvalidOperationException("_field is null.");
			_field.Store(info);
		}

		/// <summary>
		/// Gets the join.
		/// </summary>
		internal SqlJoin Join
		{
			get
			{
				return _join;
			}
		}
	}
}
