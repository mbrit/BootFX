// BootFX - Application framework for .NET applications
// 
// File: EntityIndex.cs
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
using System.Collections;
using System.Globalization;
using BootFX.Common.Entities.Attributes;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines an instance of <c>EntityIndex</c>.
	/// </summary>
	public class EntityIndex : EntityMember
	{
		/// <summary>
		/// Private field to support <c>HasUniqueValues</c> property.
		/// </summary>
		private bool _hasUniqueValues;
		
		/// <summary>
		/// Private field to support <c>Fields</c> property.
		/// </summary>
		private EntityFieldCollection _fields;

        public EntityFieldCollection IncludedFields { get; private set; }
        public EntityFieldCollection ComputedFields { get; private set; }
		
		public EntityIndex(EntityType entityType, string name, string nativeName) 
            : base(name, nativeName)
		{
			this.Initialize(entityType);
		}

		internal EntityIndex(EntityType entityType, IndexAttribute attr) 
            : base(attr.Name, attr.NativeName)
		{
			this.Initialize(entityType);
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// set...
			_hasUniqueValues = attr.HasUniqueValues;

			// setup the fields...
			if(attr.ColumnNativeNames == null)
				throw new InvalidOperationException("'attr.ColumnNativeNames' is null.");
			if(attr.ColumnNativeNames.Length == 0)
				throw new InvalidOperationException("'attr.ColumnNativeNames' is zero-length.");
            
            // add...
            AddColumns(entityType, attr.ColumnNativeNames, this.Fields);
            this.IncludedFields = new EntityFieldCollection(entityType);
            AddColumns(entityType, attr.IncludedColumns, this.IncludedFields);
            this.ComputedFields = new EntityFieldCollection(entityType);
            AddColumns(entityType, attr.ComputedColumns, this.ComputedFields);
        }

        private void AddColumns(EntityType et, string names, EntityFieldCollection fields)
        {
            if (names == null)
                return;

            //  walk...
			string[] parts = names.Split(',');
			foreach(string name in parts)
			{
				string useName = name.Trim();

				// find...
				EntityField field = et.Fields.GetField(useName, OnNotFound.ThrowException);
				if(field == null)
					throw new InvalidOperationException("field is null.");

				// add...
				fields.Add(field);
			}
		}

		private void Initialize(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			this.SetEntityType(entityType);

			// create...
			_fields = new EntityFieldCollection(this.EntityType);
		}

		/// <summary>
		/// Gets a collection of EntityField objects.
		/// </summary>
		public EntityFieldCollection Fields
		{
			get
			{
				return _fields;
			}
		}

		/// <summary>
		/// Gets or sets the hasuniquevalues
		/// </summary>
		public bool HasUniqueValues
		{
			get
			{
				return _hasUniqueValues;
			}
		}

		public override System.ComponentModel.PropertyDescriptor GetPropertyDescriptor()
		{
			// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

        public override IComparer GetComparer(CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override object GetValue(object entity)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetValue(object entity, object value, SetValueReason reason)
        {
            throw new Exception("The method or operation is not implemented.");
        }
	}
}
