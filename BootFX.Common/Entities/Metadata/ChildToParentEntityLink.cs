// BootFX - Application framework for .NET applications
// 
// File: ChildToParentEntityLink.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Globalization;
using BootFX.Common.Data.Comparers;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a link between a child and parent, e.g. <c>InvoiceItems</c> to <c>Invoice</c>, <c>Invoice</c> to <c>Customer</c>.
	/// </summary>
	public class ChildToParentEntityLink : EntityLink
	{
		/// <summary>
		/// Private field to support <c>ParentEntityTypeAsType</c> property.
		/// </summary>
		private Type _parentEntityTypeAsType;
		
		/// <summary>
		/// Private field to support <c>ParentEntityType</c> property.
		/// </summary>
		private EntityType _parentEntityType;

		/// <summary>
		/// Private field to support <c>LinkFieldNames</c> property.
		/// </summary>
		private string[] _linkFieldNames;
		private object _linkFieldNamesLock = new object();
		
		/// <summary>
		/// Private field to support <c>LinkFields</c> property.
		/// </summary>
		private EntityField[] _linkFields;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		public ChildToParentEntityLink(string name, string nativeName, Type parentEntityType, string[] linkFieldNames) : base(name, nativeName)
		{
			if(parentEntityType == null)
				throw new ArgumentNullException("parentEntityType");
			if(linkFieldNames == null)
				throw new ArgumentNullException("linkFieldNames");
			if(linkFieldNames.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("linkFieldNames");
			
			// set...
			_parentEntityTypeAsType = parentEntityType;
			_linkFieldNames = linkFieldNames;
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="name"></param>
		public ChildToParentEntityLink(string name, string nativeName, EntityType parentEntityType) : base(name, nativeName)
		{
			if(parentEntityType == null)
				throw new ArgumentNullException("parentEntityType");
			
			// set...
			_parentEntityType = parentEntityType;
		}

		/// <summary>
		/// Gets the parententitytype.
		/// </summary>
		public EntityType ParentEntityType
		{
			get
			{
				if(_parentEntityType == null)
				{
					// we use deferred loading here because the entity type we're linking to might not have been loaded...
					if(ParentEntityTypeAsType == null)
						throw new ArgumentNullException("ParentEntityTypeAsType");

					// demand load...
					_parentEntityType = EntityType.GetEntityType(this.ParentEntityTypeAsType, OnNotFound.ThrowException);

					// reset...
					_parentEntityTypeAsType = null;
				}
				return _parentEntityType;
			}
		}

		/// <summary>
		/// Gets the parententitytypeastype.
		/// </summary>
		internal Type ParentEntityTypeAsType
		{
			get
			{
				// returns the value...
				return _parentEntityTypeAsType;
			}
		}

		/// <summary>
		/// Gets the link fields.
		/// </summary>
		/// <returns></returns>
		public EntityField[] GetLinkFields()
		{
			// init?
			if(this._linkFields == null)
			{
				// mbr - 19/5/06 - locking...
				lock(_linkFieldNamesLock)
				{
					if(_linkFieldNames != null)
					{
						// check...
						if(EntityType == null)
							throw new ArgumentNullException("EntityType");

						// create...
						EntityField[] fields = new EntityField[_linkFieldNames.Length];
						for(int index = 0; index < _linkFieldNames.Length; index++)
							fields[index] = this.EntityType.Fields.GetField(_linkFieldNames[index], OnNotFound.ThrowException);

						// set...
						_linkFields = fields;

						// reset...
						_linkFieldNames = null;
					}
					else
						throw new InvalidOperationException("_linkFieldNames and _linkFields are both null.");
				}
			}

			// return...
			EntityField[] newFields = new EntityField[this._linkFields.Length];
			_linkFields.CopyTo(newFields, 0);
			return newFields;
		}

		public override System.ComponentModel.PropertyDescriptor GetPropertyDescriptor()
		{
			return new ChildToParentEntityLinkPropertyDescriptor(this);
		}

		// jmm - 8/10/2007 - added
		/// <summary>
		/// Returns whether the link is nullable
		/// </summary>
		/// <returns>Returns true if any of the fields that make up this link are nullable</returns>
		public bool IsNullable()
		{
			// get the fields
			EntityField[] fields = this.GetLinkFields();
			if(fields == null)
				throw new ArgumentNullException("fields");
			
			foreach(EntityField field in fields)
			{
				if(field.IsNullable())
					return true;
			}

			return false;
		}

		public Type Type
		{
			get
			{
				EntityField[] fields = this.GetLinkFields();
				if(fields == null)
					throw new InvalidOperationException("fields is null.");
				if(fields.Length != 1)
					throw new NotSupportedException("Link flex field operations can only be done with non-compostite foreign keys.");

				// return...
				return fields[0].Type;
			}
		}

		public System.Data.DbType DBType
		{
			get
			{
				EntityField[] fields = this.GetLinkFields();
				if(fields == null)
					throw new InvalidOperationException("fields is null.");
				if(fields.Length != 1)
					throw new NotSupportedException("Link flex field operations can only be done with non-compostite foreign keys.");

				// return...
				return fields[0].DBType;
			}
		}

		public long Size
		{
			get
			{
				EntityField[] fields = this.GetLinkFields();
				if(fields == null)
					throw new InvalidOperationException("fields is null.");
				if(fields.Length != 1)
					throw new NotSupportedException("Link flex field operations can only be done with non-compostite foreign keys.");

				// return...
				return fields[0].Size;
			}
		}

		public bool IsLookupProperty
		{
			get
			{
				return false;
			}
		}

		public bool IsMultiValue
		{
			get
			{
				return false;
			}
		}

		public bool IsLarge
		{
			get
			{
				return false;
			}
		}

		public override object GetValue(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// get...
			return this.EntityType.Storage.GetParent(entity, this);
		}

		public override void SetValue(object entity, object value, BootFX.Common.Entities.SetValueReason reason)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// get...
			this.EntityType.Storage.SetParent(entity, this, value);
		}

		public bool IsRequired
		{
			get
			{
				return false;
			}
		}

        /// <summary>
        /// Gets the comparer for the field.
        /// </summary>
        /// <returns></returns>
        public override IComparer GetComparer(CultureInfo culture)
        {
            return new EntityMemberComparer(new ToStringComparer(culture), this);
        }
	}
}
