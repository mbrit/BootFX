// BootFX - Application framework for .NET applications
// 
// File: EntityPersistence.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Linq;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Management;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Base class for getting entity data in to and out of the underlying store.
	/// </summary>
	public abstract class EntityPersistence : EntityService, IEntityPersistence
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityType"></param>
		protected EntityPersistence(EntityType entityType) : base(entityType)
		{
		}

		/// <summary>
		/// Gets storage for the entity.
		/// </summary>
		protected IEntityStorage Storage
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				IEntityStorage storage = this.EntityType.Storage;
				if(storage == null)
					throw new ArgumentNullException("storage");
				return storage;
			}
		}

		/// <summary>
		/// Saves changes to the given entity.
		/// </summary>
		/// <param name="entity"></param>
        public void SaveChanges(object entity, ITimingBucket timings = null)
        {
            this.SaveChangesInternal(entity, false, timings);
        }

        public void SaveChangesOutsideTransaction(object entity, ITimingBucket timings = null)
        {
            this.SaveChangesInternal(entity, true, timings);
        }

        private void SaveChangesInternal(object entity, bool outsideTransaction, ITimingBucket timings)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

            if (timings == null)
                timings = NullTimingBucket.Instance;
			
			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);

			// gets the work units...
            WorkUnitCollection units = null;
            using (timings.GetTimer("GetWorkUnits"))
            {
                units = this.GetWorkUnits(entity);
                if (units == null)
                    throw new ArgumentNullException("units");
                if (units.Count == 0)
                    return;
            }

			// execute...
			WorkUnitProcessor processor = new WorkUnitProcessor();
            using (var child = timings.GetChildBucket("Process"))
                processor.Process(units);

            //// mbr - 26-01-2006 - invoke the change register...
            //using (timings.GetTimer("Signal"))
            //{
            //    foreach (WorkUnit unit in units)
            //        ChangeRegister.Current.ChangesCommitted(this.EntityType, entity, unit);
            //}
		}

		/// <summary>
		/// Reconciles the results of a work unit process.
		/// </summary>
		/// <param name="units"></param>
		public virtual void ReconcileWorkUnitProcessorResults(WorkUnitCollection units)
		{
			if(units == null)
				throw new ArgumentNullException("units");
			
			// walk...
			foreach(IWorkUnit unit in units)
			{
				// We check that the unit is for our entity type otherwise we ignore
				if(!unit.EntityType.Equals(this.EntityType))
					continue;
				
				// get it...
				switch(unit.WorkUnitType)
				{
					case WorkUnitType.Insert:
						this.ReconcileAfterInsert(unit);
						break;

					case WorkUnitType.Update:
						this.ReconcileAfterUpdate(unit);
						break;

					case WorkUnitType.Delete:
						this.ReconcileAfterDelete(unit);
						break;

						// mbr - 10-10-2007 - added, no-op...						
					case WorkUnitType.Schema:
						break;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", unit.WorkUnitType, unit.WorkUnitType.GetType()));
				}
			}
		}

		/// <summary>
		/// Reconciles an insert.
		/// </summary>
		/// <param name="unit"></param>
		private void ReconcileAfterDelete(IWorkUnit unit)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");
			
			object entity = unit.Entity;
			if(entity == null)
				throw new ArgumentNullException("entity");

			// clear...
			Storage.MarkAsDeleted(entity);
		}

		/// <summary>
		/// Reconciles an insert.
		/// </summary>
		/// <param name="unit"></param>
		private void ReconcileAfterUpdate(IWorkUnit unit)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");
			
			object entity = unit.Entity;
			if(entity == null)
				throw new ArgumentNullException("entity");

			// clear...
			Storage.ResetModifiedFlags(entity);
		}

		/// <summary>
		/// Reconciles an insert.
		/// </summary>
		/// <param name="unit"></param>
		private void ReconcileAfterInsert(IWorkUnit unit)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");

			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			object entity = unit.Entity;
			if(entity == null)
				throw new ArgumentNullException("entity");

			// get the last inserted it...
			object newIdAsObject = unit.ResultsBag.LastCreatedId;

			// get the key fields...
			EntityField[] keyFields = this.EntityType.GetKeyFields();
			if(keyFields == null)
				throw new ArgumentNullException("keyFields");

			// check...
			if(newIdAsObject == null && this.EntityType.GetAutoIncrementFields().Length == 1)
				throw ExceptionHelper.CreateLengthMismatchException("newId", "keyFields", 0, 1);

			// mjr - 07-07-2005 - this wasn't checking that AI fields were set...
			EntityField[] aiFields = EntityType.GetAutoIncrementFields();
			if(aiFields == null)
				throw new InvalidOperationException("aiFields is null.");
			if(aiFields.Length > 0)
			{
				// initialize the entity...
				Storage.BeginInitialize(entity);
				try
				{
					if(newIdAsObject != null)
						Storage.SetValue(entity, aiFields[0], newIdAsObject, SetValueReason.DemandLoad);
				}
				finally
				{
					Storage.EndInitialize(entity);
				}
			}

			// reset...
			Storage.ResetIsNew(entity);

			// mbr - 10-10-2007 - this was actually missing!			
			Storage.ResetModifiedFlags(entity);
		}

		/// <summary>
		/// Gets the work units for the given entity.
		/// </summary>
		/// <param name="?"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		public WorkUnitCollection GetWorkUnits(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// new?
			bool isNew = Storage.IsNew(entity);
			bool isModified = Storage.IsModified(entity);
			
			WorkUnitCollection units = new WorkUnitCollection();

			// what's happened to the entity?
			if(Storage.IsDeleted(entity, true) == true)
			{
				// have we already been deleted?
				// ok, we're modified and we've been deleted... therefore create a delete work unit...
				if(Storage.IsDeleted(entity, false) == false && !isNew)
				{
					// mbr - 20-07-2007 - cascade deletion now disabled.					
//					if(Storage.IsMarkedForCascadeDeletion(entity))
//					{
//						// Walk each entity and create work units to delete the child entities
//						foreach(Entity childEntity in ((Entity)entity).GetAllChildrenEntities())
//						{
//							// Mark the entity for deletion
//							childEntity.MarkForDeletion();
//							
//							// Get the persistence and add the units to the collection
//							IEntityPersistence persistence = childEntity.EntityType.Persistence;
//							units.AddRange(persistence.GetWorkUnits(childEntity));
//						}
//					}

					// mbr - 25-09-2007 - renamed.
//					units.AddRange(CreateDeleteAllExtendedPropertyWorkUnits(entity));
					units.AddRange(CreateDeleteExtendedPropertyWorkUnits(entity));
					units.Add(CreateDeleteWorkUnit(entity));
				}
			}

			if(isNew == true)
			{
				// creates an insert work unit...
				units.Insert(0,CreateInsertWorkUnit(entity));
				units.AddRange(CreateInsertExtendedPropertyWorkUnits(entity));
			}
			else if(isModified == true)
			{
				// creates an update work unit...
				if(GetModifiedFieldsExcludingKeys(entity).Length > 0)
					units.Add(CreateUpdateWorkUnit(entity));

				units.AddRange(CreateUpdateExtendedPropertyWorkUnits(entity));
			}

			return units;
		}

		/// <summary>
		/// Creates an insert work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected internal abstract IWorkUnit CreateInsertWorkUnit(object entity);

		/// <summary>
		/// Creates an update work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected internal abstract IWorkUnit CreateUpdateWorkUnit(object entity);
		/// <summary>
		/// Creates an update work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected internal abstract IWorkUnit CreateDeleteWorkUnit(object entity);

		/// <summary>
		/// Creates an insert work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected abstract IWorkUnit[] CreateInsertExtendedPropertyWorkUnits(object entity);

		/// <summary>
		/// Creates an update work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected abstract IWorkUnit[] CreateUpdateExtendedPropertyWorkUnits(object entity);
		/// <summary>
		/// Creates an update work unit for the entity.
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		// mbr - 25-09-2007 - renamed.
//		protected abstract IWorkUnit[] CreateDeleteAllExtendedPropertyWorkUnits(object entity);
		protected abstract IWorkUnit[] CreateDeleteExtendedPropertyWorkUnits(object entity);

		/// <summary>
		/// Gets the fields modified in the entity including key fields
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected EntityField[] GetModifiedFields(object entity)
		{
			return GetFields(entity,true,true,false);
		}

		/// <summary>
		/// Gets the extended property fields modified in the entity excluding keys
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected EntityField[] GetModifiedFieldsExcludingKeys(object entity)
		{
			return GetFields(entity,false,true,false);
		}

		/// <summary>
		/// Gets the extended property fields modified in the entity including key fields
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected EntityField[] GetModifiedExtendedFields(object entity)
		{
			return GetFields(entity,true,true,true);
		}

		/// <summary>
		/// Gets the extended property fields modified in the entity excluding keys
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected internal EntityField[] GetModifiedExtendedFieldsExcludingKeys(object entity)
		{
			return GetFields(entity,false,true,true);
		}

		/// <summary>
		/// Gets the fields modified in the entity
		/// </summary>
		/// <param name="entity"></param>
		/// <returns></returns>
		protected EntityField[] GetFields(object entity, bool isKey, bool isModified, bool isExtended)
		{
			// get the fields...
			ArrayList fields = new ArrayList();
			foreach(EntityField field in this.EntityType.Fields)
			{
				if(isKey == true && field.IsKey())
					fields.Add(field);
				 else if(Storage.IsModified(entity, field) == isModified && field.IsExtendedProperty == isExtended)
					fields.Add(field);
			}
			return (EntityField[])fields.ToArray(typeof(EntityField));
		}

		// mbr - 12-12-2005 - deprecated.		
//		/// <summary>
//		/// Demand loads data for the given fields.
//		/// </summary>
//		/// <param name="entity"></param>
//		/// <param name="fields"></param>
//		void IEntityPersistence.DemandLoad(object entity, string[] fieldNames)
//		{
//			if(entity == null)
//				throw new ArgumentNullException("entity");
//			if(fieldNames == null)
//				throw new ArgumentNullException("fieldNames");
//			
//			// get...
//			if(EntityType == null)
//				throw new InvalidOperationException("EntityType is null.");
//			EntityField[] fields = this.EntityType.Fields.GetFields(fieldNames, OnNotFound.ThrowException);
//			if(fields == null)
//				throw new InvalidOperationException("fields is null.");
//			if(fields.Length != fieldNames.Length)
//				throw new InvalidOperationException(string.Format("Length mismatch for 'fields' and 'fieldNames': {0} cf {1}.", fields.Length, fieldNames.Length));
//
//			// defer...
//			this.DemandLoad(entity, fields);
//		}

		/// <summary>
		/// Demand loads data for the given field.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="fields"></param>
		public abstract void DemandLoad(object entity, EntityField[] fields);

		/// <summary>
		/// Gets the parent for the given entity and link.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public object GetParent(object entity, ChildToParentEntityLink link)
		{
			return this.GetParent(entity, link, new EntityField[] {});
		}

		/// <summary>
		/// Gets the parent for the given entity and link.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public object GetParent(object entity, ChildToParentEntityLink link, EntityField[] fields)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(link == null)
				throw new ArgumentNullException("link");
			if(fields == null)
				throw new ArgumentNullException("fields");

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			this.EntityType.AssertIsOfType(entity);
			
			// get the link type...
			if(link.ParentEntityType == null)
				throw new ArgumentNullException("link.ParentEntityType");

			// do we cache?
            //if (link.ParentEntityType.HasCache)
            //    return this.GetParentFromCache(entity, link, link.ParentEntityType, link.ParentEntityType.Cache);
            //else
                return this.LoadParent(entity, link);
		}

        ///// <summary>
        ///// Gets the parent object from the cache.
        ///// </summary>
        ///// <param name="entity"></param>
        ///// <param name="link"></param>
        ///// <returns></returns>
        //private object GetParentFromCache(object entity, ChildToParentEntityLink link, EntityType parentEntityType, EntityCache parentCache)
        //{
        //    if(entity == null)
        //        throw new ArgumentNullException("entity");
        //    if(link == null)
        //        throw new ArgumentNullException("link");
        //    if(parentEntityType == null)
        //        throw new ArgumentNullException("parentEntityType");
        //    if(parentCache == null)
        //        throw new ArgumentNullException("parentCache");

        //    // what are we trying to get?
        //    EntityField[] linkFields = link.GetLinkFields();
        //    if (linkFields == null)
        //        throw new InvalidOperationException("'linkFields' is null.");
        //    if (linkFields.Length != 1)
        //        throw new InvalidOperationException("'linkFields' is an invalid length.");

        //    // get the values...
        //    if(EntityType.Storage == null)
        //        throw new ArgumentNullException("EntityType.Storage");
        //    object[] foreignKey = this.EntityType.Storage.GetValues(entity, linkFields);

        //    // contains?
        //    object parent = parentCache[foreignKey[0]];
        //    if(parent == null)
        //        parent = this.LoadParent(entity, link);

        //    // return...
        //    return parent;
        //}

		/// <summary>
		/// Loads the parent entity from the underlying store.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		protected abstract object LoadParent(object entity, ChildToParentEntityLink link);

		/// <summary>
		/// Gets the entity with the given ID.
		/// </summary>
		/// <param name="keyValues"></param>
		/// <returns></returns>
		public abstract object GetById(object[] keyValues, OnNotFound onNotFound);

		/// <summary>
		/// Gets all entities.
		/// </summary>
		/// <returns></returns>
		public abstract IList GetAll();

		/// <summary>
		/// Gets the parent for a given item.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		object IEntityPersistence.GetParent(object entity, string linkName, string[] parentFieldNames)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(linkName == null)
				throw new ArgumentNullException("linkName");
			if(linkName.Length == 0)
				throw new ArgumentOutOfRangeException("'linkName' is zero-length.");
			if(parentFieldNames == null)
				throw new ArgumentNullException("parentFieldNames");
			
			// get the link...
			if(EntityType == null)
				throw new InvalidOperationException("EntityType is null.");
			EntityLink link = this.EntityType.Links.GetLink(linkName, OnNotFound.ThrowException);
			if(link == null)
				throw new InvalidOperationException("link is null.");
			if(!(link is ChildToParentEntityLink))
				throw new InvalidOperationException(string.Format("Link '{0}' is a '{1}', not child-to-parent.", linkName, link.GetType()));

			// get the field names...
			EntityField[] parentFields = this.EntityType.Fields.GetFields(parentFieldNames, OnNotFound.ThrowException);
			if(parentFields == null)
				throw new ArgumentNullException("parentFields");
			if(parentFieldNames.Length != parentFields.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'parentFieldNames' and 'parentFields': {0} cf {1}.", parentFieldNames.Length, parentFields.Length));

			// defer...
			return this.GetParent(entity, (ChildToParentEntityLink)link, parentFields);
		}

        public virtual IEnumerable<T> GetByIds<T>(IEnumerable<long> ids)
            where T : Entity
        {
            if (ids.Any())
            {
                // split into pages...
                var pages = ids.SplitIntoPages(500);
                var unordered = (IList)this.EntityType.CreateCollectionInstance();
                foreach (var page in pages)
                {
                    var filter = new SqlFilter<T>();
                    filter.Constraints.AddValueListConstraint(this.EntityType.GetKeyFields()[0], page);

                    // put the retrieved reuslts back in the order we requested...
                    var toAdds = (IList)filter.ExecuteEnumerable();
                    foreach (var toAdd in toAdds)
                        unordered.Add(toAdd); 
                }

                unordered.ReorderEntities(ids);
                return (IEnumerable<T>)unordered;
            }
            else
                return this.EntityType.CreateCollectionInstance<T>();
        }

        public PagedDataResult<T> GetByIdsPaged<T>(IEnumerable<long> ids, IPagedDataRequestSource source)
            where T : Entity
        {
            return this.GetByIdsPaged<T>(ids, source.Request);
        }

        public PagedDataResult<T> GetByIdsPaged<T>(IEnumerable<long> ids, IPagedDataRequest request)
            where T : Entity
        {
            if (request.PageSize <= 0)
            {
                var items = this.GetByIds<T>(ids);
//                return new PagedDataResult<T>(items, new PageData(0, items.Count(), 1, items.Count(), false));
                return new PagedDataResult<T>(items, PageData.GetUnpagedData(items.Count()));
            }
            else
            {
                // get the details...
                var pages = Runtime.Current.GetPages(ids, request.PageSize);
                IEnumerable<T> items = null;
                if (request.PageNumber < pages.Count)
                    items = (IEnumerable<T>)this.EntityType.Persistence.GetByIds<T>(pages[request.PageNumber]);
                else
                    items = this.EntityType.CreateCollectionInstance<T>();

                // setup...
                var data = new PageData(request.PageNumber, request.PageSize, pages.Count, ids.Count(), false);
                return new PagedDataResult<T>(items, data);
            }
        }

        public void Delete(IEnumerable<long> ids, bool inTransaction = true)
        {
            TransactionState txn = null;
            if (inTransaction)
                txn = Database.StartTransaction();

            try
            {
                var pages = ids.SplitIntoPages(500);
                foreach (var page in pages)
                {
                    var sql = new SqlStatement();
                    var builder = new StringBuilder();

                    builder.Append("delete from ");
                    builder.Append(sql.Dialect.FormatTableName(this.EntityType.NativeName));
                    builder.Append(" where ");
                    builder.Append(sql.Dialect.FormatColumnName(this.EntityType.GetKeyFields()[0].NativeName));
                    builder.Append(" in (");
                    sql.AddValueList<long>(builder, page);
                    builder.Append(")");

                    sql.CommandText  = builder.ToString();
                    Database.ExecuteNonQuery(sql);
                }

                // ok...
                if (txn != null)
                    txn.Commit();
            }
            catch (Exception ex)
            {
                if(txn != null)
                    txn.Rollback(ex);

                throw new InvalidOperationException("The operation failed", ex);
            }
            finally
            {
                if (txn != null)
                    txn.Dispose();
            }
        }
    }
}
