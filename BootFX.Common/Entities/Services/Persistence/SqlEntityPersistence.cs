// BootFX - Application framework for .NET applications
// 
// File: SqlEntityPersistence.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Crypto;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Defines a specific entity persistence implementation for use with relational databases.
	/// </summary>
	public class SqlEntityPersistence : EntityPersistence
	{
		/// <summary>
		/// Private field to support <c>InsertWorkUnitType</c> property.
		/// </summary>
		private Type _insertWorkUnitType = typeof(SqlInsertWorkUnit);
		
		/// <summary>
		/// Private field to support <c>UpdateWorkUnitType</c> property.
		/// </summary>
        private Type _updateWorkUnitType = typeof(SqlUpdateWorkUnit);
		
		/// <summary>
		/// Private field to support <c>DeleteWorkUnitType</c> property.
		/// </summary>
		private Type _deleteWorkUnitType = typeof(SqlDeleteWorkUnit);

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Private field to support <c>InsertExtendedPropertyWorkUnitType</c> property.
//		/// </summary>
//		private Type _insertExtendedPropertyWorkUnitType = typeof(SqlExtendedPropertyInsertWorkUnit);
//		
//		/// <summary>
//		/// Private field to support <c>UpdateExtendedPropertyWorkUnitType</c> property.
//		/// </summary>
//		private Type _updateExtendedPropertyWorkUnitType = typeof(SqlExtendedPropertyUpdateWorkUnit);
//		
//		/// <summary>
//		/// Private field to support <c>DeleteExtendedPropertyWorkUnitType</c> property.
//		/// </summary>
//		private Type _deleteAllExtendedPropertyWorkUnitType = typeof(SqlExtendedPropertyDeleteAllWorkUnit);

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlEntityPersistence(EntityType entityType) : base(entityType)
		{
		}

		/// <summary>
		/// Gets or sets the deleteworkunittype
		/// </summary>
		public Type DeleteWorkUnitType
		{
			get
			{
				return _deleteWorkUnitType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _deleteWorkUnitType)
				{
					// set the value...
					_deleteWorkUnitType = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the updateworkunittype
		/// </summary>
		public Type UpdateWorkUnitType
		{
			get
			{
				return _updateWorkUnitType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _updateWorkUnitType)
				{
					// set the value...
					_updateWorkUnitType = value;
				}
			}
		}

		/// <summary>
		/// Gets the insertworkunittype.
		/// </summary>
		public Type InsertWorkUnitType
		{
			get
			{
				return _insertWorkUnitType;
			}
			set
			{
				if(_insertWorkUnitType != value)
					_insertWorkUnitType = value;
			}
		}

		// mbr - 25-09-2007 - moved to provider.		
//		/// <summary>
//		/// Gets or sets the deleteExtendedPropertyworkunittype
//		/// </summary>
//		public Type DeleteAllExtendedPropertyWorkUnitType
//		{
//			get
//			{
//				return _deleteAllExtendedPropertyWorkUnitType;
//			}
//			set
//			{
//				// check to see if the value has changed...
//				if(value != _deleteAllExtendedPropertyWorkUnitType)
//				{
//					// set the value...
//					_deleteAllExtendedPropertyWorkUnitType = value;
//				}
//			}
//		}
//
//		/// <summary>
//		/// Gets or sets the updateExtendedPropertyworkunittype
//		/// </summary>
//		public Type UpdateExtendedPropertyWorkUnitType
//		{
//			get
//			{
//				return _updateExtendedPropertyWorkUnitType;
//			}
//			set
//			{
//				// check to see if the value has changed...
//				if(value != _updateExtendedPropertyWorkUnitType)
//				{
//					// set the value...
//					_updateExtendedPropertyWorkUnitType = value;
//				}
//			}
//		}
//
//		/// <summary>
//		/// Gets the insertExtendedPropertyworkunittype.
//		/// </summary>
//		public Type InsertExtendedPropertyWorkUnitType
//		{
//			get
//			{
//				return _insertExtendedPropertyWorkUnitType;
//			}
//			set
//			{
//				if(_insertExtendedPropertyWorkUnitType != value)
//					_insertExtendedPropertyWorkUnitType = value;
//			}
//		}

		/// <summary>
		/// Creates an insert work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="storage"></param>
		/// <returns></returns>
		protected internal override IWorkUnit CreateInsertWorkUnit(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// check...
			if(InsertWorkUnitType == null)
				throw new ArgumentNullException("InsertWorkUnitType");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(InsertWorkUnitType == null)
				throw new ArgumentNullException("InsertWorkUnitType");
			if(Storage.IsNew(entity) == false)
				throw new InvalidOperationException("Entity is not new.");

			// mbr - 13-10-2005 - if the entity type is using partitioning, give the partition an opportunity to set the partition ID prior to insert...
            //if(this.EntityType.SupportsPartitioning)
            //{
            //    // get the strategy...
            //    PartitioningStrategy strategy = this.EntityType.PartitioningStrategy;
            //    if(strategy == null)
            //        throw new InvalidOperationException("strategy is null.");

            //    // apply...
            //    strategy.BeforeInsert(this.EntityType, entity);
            //}

			// get all of the fields...
			EntityField[] fields = GetModifiedFields(entity);
			object[] values = this.Storage.GetValues(entity, fields, FetchFlags.KeepEncrypted);

			// create...
			try
			{
				// mbr - 13-10-2005 - added null check...				
				WorkUnit unit = (WorkUnit)Activator.CreateInstance(this.InsertWorkUnitType, new object[] { this.EntityType, entity, fields, values });
				if(unit == null)
					throw new InvalidOperationException("unit is null.");
				return unit;
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to instantiate an instance of '{0}'.", this.InsertWorkUnitType), ex);
			}
		}

		/// <summary>
		/// Creates an update work unit.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected internal override IWorkUnit CreateUpdateWorkUnit(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// check...
			if(UpdateWorkUnitType == null)
				throw new ArgumentNullException("UpdateWorkUnitType");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(InsertWorkUnitType == null)
				throw new ArgumentNullException("InsertWorkUnitType");
			if(Storage.IsNew(entity) == true)
				throw new InvalidOperationException("Entity is new.");

			// mbr - 04-09-2007 - for c7 - needs to work on update too.			
            //if(this.EntityType.SupportsPartitioning)
            //{
            //    // get the strategy...
            //    PartitioningStrategy strategy = this.EntityType.PartitioningStrategy;
            //    if(strategy == null)
            //        throw new InvalidOperationException("strategy is null.");

            //    // apply...
            //    strategy.BeforeUpdate(this.EntityType, entity);
            //}
			
			// get the values...
			EntityField[] allFields = GetModifiedFields(entity);
			if(allFields.Length == 0)
				throw new InvalidOperationException("Modified fields array is zero-length.");
			object[] values = this.Storage.GetValues(entity, allFields, FetchFlags.KeepEncrypted);

			// create...
			try
			{
				return (WorkUnit)Activator.CreateInstance(this.UpdateWorkUnitType, new object[] { this.EntityType, entity, allFields, values });
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to instantiate an instance of '{0}'.", this.UpdateWorkUnitType), ex);
			}
		}

		/// <summary>
		/// Creates a work unit to delete the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected internal override IWorkUnit CreateDeleteWorkUnit(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// check...
			if(DeleteWorkUnitType == null)
				throw new ArgumentNullException("DeleteWorkUnitType");
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			if(InsertWorkUnitType == null)
				throw new ArgumentNullException("InsertWorkUnitType");
			if(Storage.IsDeleted(entity) == false)
				throw new InvalidOperationException("Entity is not new.");
			if(Storage.IsDeleted(entity, false) == true)
				throw new InvalidOperationException("Entity has already been physically deleted.");

			// get all of the fields...
			EntityField[] fields = this.EntityType.GetKeyFields();
			object[] values = this.Storage.GetValues(entity, fields);

			// create...
			try
			{
				return (WorkUnit)Activator.CreateInstance(this.DeleteWorkUnitType, new object[] { this.EntityType, entity, fields, values });
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to instantiate an instance of '{0}'.", this.DeleteWorkUnitType), ex);
			}
		}

		protected override IWorkUnit[] CreateInsertExtendedPropertyWorkUnits(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// do we?
			if(!(this.EntityType.HasExtendedProperties))
				return new IWorkUnit[] {};

			// get all of the fields...
			EntityField[] fields = GetModifiedExtendedFieldsExcludingKeys(entity);
			if(fields == null)
				throw new InvalidOperationException("fields is null.");

			// mbr - 10-10-2007 - if we don't have anything to do, return nothing...
			if(fields.Length == 0)
				return new IWorkUnit[] {};

			// mbr - 25-09-2007 - defer...
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
			IWorkUnit[] results = Database.ExtensibilityProvider.GetInsertWorkUnits(entity, this.EntityType, fields);
			if(results == null)
				throw new InvalidOperationException("results is null.");

			// return...
			return results;
		}

		protected override IWorkUnit[] CreateUpdateExtendedPropertyWorkUnits(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// do we?
			if(!(this.EntityType.HasExtendedProperties))
				return new IWorkUnit[] {};

			// get all of the fields...
			EntityField[] fields = GetModifiedExtendedFieldsExcludingKeys(entity);

			// mbr - 25-09-2007 - changed to defer...
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
			IWorkUnit[] results = Database.ExtensibilityProvider.GetUpdateWorkUnits(entity, this.EntityType, fields);
			if(results == null)
				throw new InvalidOperationException("results is null.");

			// return...
			return results;
		}

		protected override IWorkUnit[] CreateDeleteExtendedPropertyWorkUnits(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// do we?
			if(!(this.EntityType.HasExtendedProperties))
				return new IWorkUnit[] {};

			// mbr - 25-09-2007 - changed to deferral...
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
			IWorkUnit[] results = Database.ExtensibilityProvider.GetDeleteWorkUnits(entity, this.EntityType, new EntityField[] {});
			if(results == null)
				throw new InvalidOperationException("results is null.");

			// return...
			return results;
		}

		public override void DemandLoad(object entity, EntityField[] fields)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(fields.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fields");
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			EntityType.AssertIsOfType(entity);

			// new?
			if(this.Storage.IsNew(entity) == true)
				throw new InvalidOperationException("Entity is new.");
			if(this.Storage.IsDeleted(entity, false) == true)
				throw new InvalidOperationException("Entity has been physically deleted.");

			// create a filter...
			object[] keyValues = this.Storage.GetKeyValues(entity);
			SqlFilter filter = SqlFilter.CreateGetByIdFilter(this.EntityType, keyValues);
			// jm - 14-07-2006 - Hacked - Matt to sort out
			if(filter == null)
				throw new InvalidOperationException("filter is null.");
			filter.IgnorePartitioning = true;

			// clear the fields...
			filter.Fields.Clear();
			filter.Fields.AddRange(fields);

			// get values...
			object[] values = filter.ExecuteValues();
			if(values == null)
				throw new ArgumentNullException("values");

			// check...
			if(values.Length != fields.Length)
				throw ExceptionHelper.CreateLengthMismatchException("values", "fields", values.Length, fields.Length);

			// init and set...
			this.Storage.BeginInitialize(entity);
			try
			{
				for(int index = 0; index < fields.Length; index++)
				{
					// value...
					EntityField field = fields[index];
					if(field == null)
						throw new InvalidOperationException("field is null.");
					object value = values[index];

                    // encrypted?
                    //if(field.IsEncrypted)
                    //{
                    //	if(value == null)
                    //		value = new EncryptedValue(null, (byte[])null);
                    //	else if(value is DBNull)
                    //		value = new EncryptedValue(typeof(DBNull), (byte[])null);
                    //	else
                    //	{
                    //		// switch...
                    //		switch(field.DBType)
                    //		{
                    //			case DbType.String:
                    //			case DbType.StringFixedLength:
                    //			case DbType.AnsiString:
                    //			case DbType.AnsiStringFixedLength:
                    //				value = new EncryptedValue(field.Type, (string)value);
                    //				break;

                    //			default:
                    //				throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", field.DBType, field.DBType.GetType()));
                    //		}
                    //	}
                    //}

                    if ((field.DBType == DbType.AnsiStringFixedLength || field.DBType == DbType.StringFixedLength) && value is string)
                        value = ((string)value).TrimEnd();

                    // set...
                    this.Storage.SetValue(entity, field, value, SetValueReason.DemandLoad);
				}
			}
			finally
			{
				this.Storage.EndInitialize(entity);
			}
		}

		/// <summary>
		/// Gets the parent for the given entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		protected override object LoadParent(object entity, ChildToParentEntityLink link)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(link == null)
				throw new ArgumentNullException("link");

			// defer...
			return SqlFilter.CreateGetParentFilter(entity, link).ExecuteEntity();
		}

		/// <summary>
		/// Gets the entity with the given ID.
		/// </summary>
		/// <param name="keyValues"></param>
		/// <returns></returns>
		public override object GetById(object[] keyValues, OnNotFound onNotFound)
		{
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");

			// return...
			return SqlFilter.GetById(this.EntityType, keyValues, onNotFound);
		}

		/// <summary>
		/// Gets all entities.
		/// </summary>
		/// <returns></returns>
		public override IList GetAll()
		{
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			return SqlFilter.GetAll(this.EntityType);
		}
	}
}
