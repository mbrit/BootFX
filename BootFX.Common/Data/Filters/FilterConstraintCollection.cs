// BootFX - Application framework for .NET applications
// 
// File: FilterConstraintCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using BootFX.Common.Entities;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Holds a collection of <c ref="FilterConstraint">FilterConstraint</c> instances.
    /// </summary>
    [Serializable]
    public class FilterConstraintCollection : CollectionBase
	{
		/// <summary>
		/// Private field to support <c>Creator</c> property.
		/// </summary>
		private SqlStatementCreator _creator;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal FilterConstraintCollection(SqlStatementCreator creator)
		{
			if(creator == null)
				throw new ArgumentNullException("creator");
			_creator = creator;
		}

		/// <summary>
		/// Gets the filter.
		/// </summary>
		public SqlStatementCreator Creator
		{
			get
			{
				return _creator;
			}
		}
		
		/// <summary>
		/// Adds a FilterConstraint instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public int Add(FilterConstraint item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			return List.Add(item);
		}  

		/// <summary>
		/// Adds a FilterConstraint instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void AddRange(FilterConstraint[] items)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			for(int index = 0; index < items.Length; index++)
				this.Add(items[index]);
		}  

		/// <summary>
		/// Adds constraints for the given link.
		/// </summary>
		/// <param name="linkName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="relatedEntity"></param>
		/// <returns></returns>
		public void AddConstraintsForLink(string linkName, SqlOperator filterOperator, object relatedEntity)
		{
			this.AddConstraintsForLink(linkName, filterOperator, new object[] { relatedEntity });
		}

		/// <summary>
		/// Adds constraints for the given link.
		/// </summary>
		/// <param name="linkName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="relatedEntity"></param>
		public void AddConstraintsForLink(string linkName, SqlOperator filterOperator, IList relatedEntities)
		{
			if(linkName == null)
				throw new ArgumentNullException("linkName");
			if(linkName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("linkName");
			if(Creator == null)
				throw new ArgumentNullException("Creator");
			if(relatedEntities == null)
				throw new ArgumentNullException("relatedEntities");

			// get the link...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// get the link...
			EntityLink link = this.EntityType.Links.GetLink(linkName, OnNotFound.ThrowException);
			if(link == null)
				throw new ArgumentNullException("link");

			// add...
			this.AddConstraintsForLink(link, filterOperator, relatedEntities);
		}

		/// <summary>
		/// Adds constraints for the given link.
		/// </summary>
		/// <param name="linkName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="relatedEntity"></param>
		public void AddConstraintsForLink(EntityLink link, SqlOperator filterOperator, object relatedEntity)
		{
			this.AddConstraintsForLink(link, filterOperator, new object[] { relatedEntity });
		}

		/// <summary>
		/// Adds constraints for the given link.
		/// </summary>
		/// <param name="linkName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="relatedEntity"></param>
		public void AddConstraintsForLink(EntityLink link, SqlOperator filterOperator, IList relatedEntities)
		{
			if(link == null)
				throw new ArgumentNullException("link");
			if(Creator == null)
				throw new ArgumentNullException("Creator");
			if(relatedEntities == null)
				throw new ArgumentNullException("relatedEntities");
			
			// check...
			if(link is ChildToParentEntityLink)
				this.AddConstraintsForLink((ChildToParentEntityLink)link, filterOperator, relatedEntities);
			else
				throw new NotSupportedException(string.Format(Cultures.Exceptions, "Cannot handle '{0}'.", link.GetType()));
		}

		/// <summary>
		/// Creates constraints for the given link.
		/// </summary>
		/// <param name="linkName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="relatedEntity"></param>
		/// <returns></returns>
		private void AddConstraintsForLink(ChildToParentEntityLink link, SqlOperator filterOperator, IList relatedEntities)
		{
			if(link == null)
				throw new ArgumentNullException("link");

			// check..
			if(filterOperator != SqlOperator.EqualTo && filterOperator != SqlOperator.NotEqualTo)
				throw new NotSupportedException(string.Format(Cultures.System, "'{0}' is an invalid operator.  Only 'EqualTo' and 'NotEqualTo' are supported.", filterOperator));

			// check...
			if(link.ParentEntityType == null)
				throw new ArgumentNullException("link.ParentEntityType");

			// create...
			IEntityStorage storage = link.ParentEntityType.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");

			// get the key fields of the related type...
			EntityField[] foreignKeyFields = link.ParentEntityType.GetKeyFields();
			if(foreignKeyFields.Length == 0)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Key fields on '{0}' was zero-length.", link.ParentEntityType));

			// get teh fields in this...
			EntityField[] relatedFields = link.GetLinkFields();

			// compare...
			if(foreignKeyFields.Length != relatedFields.Length)
				throw ExceptionHelper.CreateLengthMismatchException("foreignKeyFields", "relatedFields", foreignKeyFields.Length, relatedFields.Length);

			// now loop...
			FilterConstraint lastTopConstraint = null;
			for(int entityIndex = 0; entityIndex < relatedEntities.Count; entityIndex++)
			{
				// get and check...
				object relatedEntity = relatedEntities[entityIndex];
				link.ParentEntityType.AssertIsOfType(relatedEntity);

				// create a top level constraint...
				FilterConstraint topConstraint = new FilterConstraint(this.Creator);
				Add(topConstraint);

				// set...
				if(lastTopConstraint != null)
					lastTopConstraint.CombineWithNext = SqlCombine.Or;

				// new?
				if(storage.IsNew(relatedEntity) == false && storage.IsDeleted(relatedEntity, false) == false)
				{
					// loop...
					for(int index = 0; index < foreignKeyFields.Length; index++)
					{
						// get...
						object foreignValue = storage.GetValue(relatedEntity, foreignKeyFields[index]);

						// add a constraint...
						topConstraint.ChildConstraints.Add(new EntityFieldFilterConstraint(this.Creator, relatedFields[index], filterOperator, foreignValue));
					}
				}

				// next...
				lastTopConstraint = topConstraint;
			}
		}

		/// <summary>
		/// Adds a null constraint.
		/// </summary>
		/// <returns></returns>
		public FilterConstraint AddNullConstraint()
		{
			if(Creator == null)
				throw new ArgumentNullException("Creator");
			FilterConstraint constraint = new FilterConstraint(this.Creator);

			// add...
			this.Add(constraint);

			// return...
			return constraint;
		}

        public ValueListConstraint AddValueListConstraint(string name, IEnumerable values, bool useFullyQualifiedNames = true)
        {
            return this.AddValueListConstraint(this.EntityType.Fields.GetField(name, OnNotFound.ThrowException), values, useFullyQualifiedNames);
        }
        
        public ValueListConstraint AddValueListConstraint(EntityField field, IEnumerable values, bool useFullyQualifiedNames = true)
        {
            var constraint = new ValueListConstraint(this.Creator, field, values, useFullyQualifiedNames);
            this.Add(constraint);
            return constraint;
        }

        public ArrayConstraint AddArrayConstraint(string name, IEnumerable values, bool useFullyQualifiedNames = true)
        {
            return this.AddArrayConstraint(this.EntityType.Fields.GetField(name, OnNotFound.ThrowException), values, useFullyQualifiedNames);
        }

        public ArrayConstraint AddArrayConstraint(EntityField field, IEnumerable values, bool useFullyQualifiedNames = true)
        {
            var constraint = new ArrayConstraint(this.Creator, field, values, useFullyQualifiedNames, SqlOperator.EqualTo);
            this.Add(constraint);
            return constraint;
        }

        public ArrayConstraint AddArrayExclusionConstraint(string name, IEnumerable values, bool useFullyQualifiedNames = true)
        {
            return this.AddArrayExclusionConstraint(this.EntityType.Fields.GetField(name, OnNotFound.ThrowException), values, useFullyQualifiedNames);
        }

        public ArrayConstraint AddArrayExclusionConstraint(EntityField field, IEnumerable values, bool useFullyQualifiedNames = true)
        {
            var constraint = new ArrayConstraint(this.Creator, field, values, useFullyQualifiedNames, SqlOperator.NotEqualTo);
            this.Add(constraint);
            return constraint;
        }

        public FreeFilterConstraint AddFreeConstraint(string sql)
        {
            return this.AddFreeConstraint(sql, null);
        }

        public FreeFilterConstraint AddFreeConstraint(string sql, params string[] referencedFields)
		{
			if(sql == null)
				throw new ArgumentNullException("sql");
			if(sql.Length == 0)
				throw new ArgumentOutOfRangeException("'sql' is zero-length.");

            var fields = new List<EntityField>();
            if (referencedFields != null)
            {
                foreach (var name in referencedFields)
                {
                    var field = this.EntityType.Fields[name];
                    if (field == null)
                        throw new InvalidOperationException(string.Format("Field '{0}' was not found on '{1}'.", name, this.EntityType));
                    fields.Add(field);
                }
            }

			// add...
			if(Creator == null)
				throw new InvalidOperationException("Creator is null.");
			FreeFilterConstraint constraint = new FreeFilterConstraint(this.Creator, sql, fields);
			this.Add(constraint);

			// return...
			return constraint;
		}

		/// <summary>
		/// Adds a constraint.
		/// </summary>
		/// <param name="item"></param>
		public EntityFieldFilterConstraint Add(EntityField field, object value)
		{
			return Add(field, SqlOperator.EqualTo, value);
		}

		/// <summary>
		/// Adds a constraint.
		/// </summary>
		/// <param name="item"></param>
		public EntityFieldFilterConstraint Add(EntityField field, SqlOperator filterOperator, object value)
		{
			if(field == null)
				throw new ArgumentNullException("field");

			// create...
			if(Creator == null)
				throw new ArgumentNullException("Creator");
			EntityFieldFilterConstraint constraint = new EntityFieldFilterConstraint(this.Creator, field, filterOperator, value);

			// add...
			this.Add(constraint);

			// return...
			return constraint;
		}

		/// <summary>
		/// Adds a constraint.
		/// </summary>
		/// <param name="item"></param>
		public EntityFieldFilterConstraint Add(string fieldName, object value)
		{
			return this.Add(fieldName, SqlOperator.EqualTo, value);
		}

		/// <summary>
		/// Adds a constraint.
		/// </summary>
		/// <param name="item"></param>
		public EntityFieldFilterConstraint Add(string fieldName, SqlOperator filterOperator, object value)
		{
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("fieldName");

			// check...
			if(Creator == null)
				throw new ArgumentNullException("Creator");

			// get the field...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");
			EntityField field = this.EntityType.Fields.GetField(fieldName, OnNotFound.ThrowException);
			if(field == null)
				throw new ArgumentNullException("field");

			// create...
			EntityFieldFilterConstraint constraint = new EntityFieldFilterConstraint(this.Creator, field, filterOperator, value);

			// add and return...
			this.Add(constraint);
			return constraint;
		}

		/// <summary>
		/// Removes a FilterConstraint item to the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		public void Remove(FilterConstraint item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Remove(item);
		}  
		
		/// <summary>
		/// Gets or sets an item.
		/// </summary>
		/// <param name="index">The index in the collection.</param>
		public FilterConstraint this[int index]
		{
			get
			{
				return (FilterConstraint)List[index];
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value");
				List[index] = value;
			}
		}
		
		/// <summary>
		/// Returns the index of the item in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>The index of the item, or -1 if it is not found.</returns>
		public int IndexOf(FilterConstraint item)
		{
			return List.IndexOf(item);
		}
		
		/// <summary>
		/// Discovers if the given item is in the collection.
		/// </summary>
		/// <param name="item">The item to find.</param>
		/// <returns>Returns true if the given item is in the collection.</returns>
		public bool Contains(FilterConstraint item)
		{
			if(IndexOf(item) == -1)
				return false;
			else
				return true;
		}
		
		/// <summary>
		/// Copies the entire collection to an array.
		/// </summary>
		/// <returns>Returns the array of items.</returns>
		public FilterConstraint[] ToArray()
		{
			return (FilterConstraint[])InnerList.ToArray(typeof(FilterConstraint));
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		private EntityType EntityType
		{
			get
			{
				if(Creator == null)
					throw new ArgumentNullException("Creator");
				return this.Creator.EntityType;
			}
		}

		/// <summary>
		/// Adds a FilterConstraint instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Insert(int index, FilterConstraint item)
		{
			if(item == null)
				throw new ArgumentNullException("item");
			List.Insert(index, item);
		}  

		/// <summary>
		/// Adds a FilterConstraint instance to the collection.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void CopyTo(FilterConstraint[] items, int index)
		{
			if(items == null)
				throw new ArgumentNullException("items");
			List.CopyTo(items, index);
		}  
	}
}
