// BootFX - Application framework for .NET applications
// 
// File: SqlFilter.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Crypto;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using System.Linq;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Defines a class that allows for filtering of data.
    /// </summary>
    /// <remarks>This works on the premise that you are attempting to generate <c>SELECT</c> statements for use with a relational databases.
    /// By default, the entire table is selected and then constrained using the <see cref="Constraints"></see> collection.</remarks>
    /// 
    /// <remarks>The order can be defined using the <see cref="SqlStatementCreator.SortOrder"></see> collection.  By default, if no sort order is defined, the 
    /// default order of the entity will be used.  Likewise with fields - the <see cref="SqlStatementCreator.Fields"></see> property defines the fields to be returned on 
    /// each entity and will be the default set of fields for the entity unless specifically indicated.</remarks>
    ///
    /// <example>
    /// <code>
    /// // Gets all customers...
    /// public static CustomerCollection GetCustomers()
    /// {
    ///		// create a filter...
    ///		SqlFilter filter = new SqlFilter(typeof(Customer));
    ///		
    ///		// by default, the common field set and sort order will be used...
    ///		
    ///		// constrain by last name...
    ///		filter.Contraints.Add("FirstName", SqlOperator.EqualTo, "Kelly");
    ///		
    ///		// run...
    ///		return (CustomerCollection)filter.ExecuteEntityCollection();
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class SqlFilter : SqlStatementCreator
	{
		/// <summary>
		/// Private field to support <c>CombineWithNext</c> property.
		/// </summary>
		private SqlCombine _combineWithNext = SqlCombine.And;
		
		/// <summary>
		/// Private field to support <c>Constraints</c> property.
		/// </summary>
		private FilterConstraintCollection _constraints;

        private ISqlFilterSink Sink { get; set; }

        public int CacheTtlMinutes { get; set; }

        private static Dictionary<string, bool> ForcedIndexCheckList { get; set; }
        private static object _forcedIndexCheckList = new object();
        private bool ForceSignalRecompile { get; set; }
        private bool ForceSignalRecompileSet { get; set; }

        /// <summary>
		/// Constructor.
		/// </summary>
		public SqlFilter(Type type) 
            : base(type)
		{
            this.Initialize();
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlFilter(Type type, EntityField[] fields) 
            : base(type, fields)
		{
            this.Initialize();
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlFilter(EntityType entityType) 
            : base(entityType)
		{
            this.Initialize();
        }

		/// <summary>
		/// Constructor.
		/// </summary>
		public SqlFilter(EntityType entityType, EntityField[] fields) 
            : base(entityType, fields)
		{
            this.Initialize();
        }

        static SqlFilter()
        {
            ForcedIndexCheckList = new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
        }

        private void Initialize()
        {
            this.Sink = this.EntityType.SqlFilterSink;

            // if...
            if (this.Sink != null)
                this.Sink.FilterCreated(this);
        }

		/// <summary>
		/// Appends constraints to the filter.
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		protected internal override void AppendConstraints(SqlStatement statement, StringBuilder builder)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(builder == null)
				throw new ArgumentNullException("builder");
			
			// check...
			if(this.Constraints.Count == 0)
				return;

			// mbr - 13-10-2005 - the creator now does this...			
			//			// where...
			//			builder.Append(" ");
			//			builder.Append(statement.Dialect.WhereKeyword);
			//			builder.Append(" ");

			// walk...
			int constraintNumberSeed = 0;
			string[] sql = WalkConstraints(statement, this.Constraints, ref constraintNumberSeed);

			// stitch...
			string finalSql = this.StitchSql(statement, this.Constraints, sql);

			// append...
			builder.Append(finalSql);
		}

		/// <summary>
		/// Walk constraints and add them.
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		/// <param name="constraints"></param>
		private string[] WalkConstraints(SqlStatement statement, FilterConstraintCollection constraints, ref int constraintNumberSeed)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(constraints == null)
				throw new ArgumentNullException("constraints");
			if(constraints.Count == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("constraints");

			// create a context...
			FilterConstraintAppendContext context = new FilterConstraintAppendContext(this, statement, constraintNumberSeed);
			constraintNumberSeed++;

			// add...
			FilterConstraint lastConstraint = null;
			string[] sql = new string[constraints.Count];
			for(int index = 0; index < constraints.Count; index++)
			{
				FilterConstraint constraint = constraints[index];

				// configure the context...
				context.SetLastConstraint(lastConstraint);
				context.ResetBuilder();

				// add...
				constraint.Append(context);

				// set...
				sql[index] = context.Sql.ToString();

				// child?
				if(constraint.ChildConstraints.Count > 0)
				{
					// walk...
					string[] childSql = this.WalkConstraints(statement, constraint.ChildConstraints, ref constraintNumberSeed);

					// stitch...
					string combinedChildSql = this.StitchSql(statement, constraint.ChildConstraints, childSql);
					if(combinedChildSql.Length > 0)
					{
						// anything?
						string useSql = sql[index].Trim();
						if(useSql.Length > 0)
						{
                            throw new NotImplementedException("This operation has not been implemented.");
						}
						else
							sql[index] = string.Format(Cultures.System, "({0})", combinedChildSql);
					}
				}

				// next...
				lastConstraint = constraint;
			}

			// return...
			return sql;
		}

		/// <summary>
		/// Stitches the given SQL statements together.
		/// </summary>
		/// <param name="constraints"></param>
		/// <param name="sql"></param>
		/// <returns></returns>
		private string StitchSql(SqlStatement statement, FilterConstraintCollection constraints, string[] sql)
		{
			if(statement == null)
				throw new ArgumentNullException("statement");
			if(constraints == null)
				throw new ArgumentNullException("constraints");
			if(sql == null)
				throw new ArgumentNullException("sql");
			
			// check...
			if(constraints.Count != sql.Length)
				throw ExceptionHelper.CreateLengthMismatchException("constraints", "sql", constraints.Count, sql.Length);

			// anything to do?
			if(constraints.Count == 0)
				return string.Empty;

			// walk and add...
			StringBuilder builder = new StringBuilder();
			int lastNonEmptyString = -1;
			for(int index = 0; index < constraints.Count; index++)
			{
				// add...
				string useSql = sql[index].Trim();
				if(useSql.Length > 0)
				{
					// combine?
					if(lastNonEmptyString != -1)
					{
						// find...
						SqlCombine combine = constraints[lastNonEmptyString].CombineWithNext;
                        this.AppendIndent(builder);
                        builder.Append(statement.Dialect.GetCombineKeyword(combine));
						builder.Append(" ");
					}

					// add...
					builder.Append(useSql);

					// set...
					lastNonEmptyString = index;
				}
			}

			// return...
			return builder.ToString().Trim();
		}

		/// <summary>
		/// Returns true if the given operator can handle DB null.
		/// </summary>
		/// <param name="sqlOperator"></param>
		/// <returns></returns>
		public static bool IsDBNullValidForOperator(SqlOperator sqlOperator)
		{
			switch(sqlOperator)
			{
				case SqlOperator.EqualTo:
				case SqlOperator.NotEqualTo:
					return true;

				case SqlOperator.StartsWith:
				case SqlOperator.NotStartsWith:
				case SqlOperator.GreaterThan:
				case SqlOperator.GreaterThanOrEqualTo:
				case SqlOperator.LessThan:
				case SqlOperator.LessThanOrEqualTo:
				case SqlOperator.Between:
				case SqlOperator.NotBetween:
					return false;

				default:
					throw ExceptionHelper.CreateCannotHandleException(sqlOperator);
			}
		}

		/// <summary>
		/// Gets a collection of FilterConstraint objects.
		/// </summary>
		public FilterConstraintCollection Constraints
		{
			get
			{
				if(_constraints == null)
					_constraints = new FilterConstraintCollection(this);
				return _constraints;
			}
		}

		/// <summary>
		/// Gets or sets the combinewithnext
		/// </summary>
		public SqlCombine CombineWithNext
		{
			get
			{
				return _combineWithNext;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _combineWithNext)
				{
					// set the value...
					_combineWithNext = value;
				}
			}
		}

		/// <summary>
		/// Creates a filter that selected entities with the given ID.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(Type entityType, object keyValue)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValue == null)
				throw new ArgumentNullException("keyValue");

			// defer...
			return CreateGetByIdFilter(entityType, keyValue, new EntityField[] {});
		}

		/// <summary>
		/// Creates a filter that selected entities with the given keyValue.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(Type entityType, object[] keyValues)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(keyValues.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("keyValues");
			
			// defer...
			return CreateGetByIdFilter(entityType, keyValues, new EntityField[] {});
		}

		/// <summary>
		/// Creates a filter that selected entities with the given keyValue.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(EntityType entityType, object keyValue)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValue == null)
				throw new ArgumentNullException("keyValue");

			// defer...
			return CreateGetByIdFilter(entityType, keyValue, new EntityField[] {});
		}

		/// <summary>
		/// Creates a filter that selected entities with the given keyValue.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(EntityType entityType, object[] keyValues)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(keyValues.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("keyValues");
			
			// defer...
			return CreateGetByIdFilter(entityType, keyValues, new EntityField[] {});
		}

		/// <summary>
		/// Creates a filter that selected entities with the given ID.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(Type entityType, object keyValue, EntityField[] fields)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValue == null)
				throw new ArgumentNullException("keyValue");
			if(fields == null)
				throw new ArgumentNullException("fields");

			// defer...
			return CreateGetByIdFilter(entityType, new object[] { keyValue }, fields);
		}

		/// <summary>
		/// Creates a filter that selected entities with the given keyValue.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(Type entityType, object[] keyValues, EntityField[] fields)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(keyValues.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("keyValues");
			if(fields == null)
				throw new ArgumentNullException("fields");
			
			// defer...
			return CreateGetByIdFilter(EntityType.GetEntityType(entityType, OnNotFound.ThrowException), keyValues);
		}

		/// <summary>
		/// Creates a filter that selected entities with the given keyValue.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(EntityType entityType, object keyValue, EntityField[] fields)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValue == null)
				throw new ArgumentNullException("keyValue");
			if(fields == null)
				throw new ArgumentNullException("fields");

			// defer...
			return CreateGetByIdFilter(entityType, new object[] { keyValue });
		}

		/// <summary>
		/// Creates a filter that selected entities with the given keyValue.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="keyValue"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetByIdFilter(EntityType entityType, object[] keyValues, EntityField[] fields)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(keyValues.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("keyValues");
			if(fields == null)
				throw new ArgumentNullException("fields");

			// create...
			SqlFilter filter = new SqlFilter(entityType);

			// clear the order - we don't need it as we're only getting one...
			filter.SortOrder.Clear();

			// select the fields...
			if(fields.Length > 0)
			{
				filter.Fields.Clear();
				filter.Fields.AddRange(fields);
			}

			// key fields...
			EntityField[] keyFields = entityType.GetKeyFields();
			if(keyValues.Length != keyFields.Length)
				throw ExceptionHelper.CreateLengthMismatchException("keyValues", "keyFields", keyValues.Length, keyFields.Length);

			// create...
			for(int index = 0; index < keyFields.Length; index++)
				filter.Constraints.Add(keyFields[index], SqlOperator.EqualTo, keyValues[index]);

			// return...
			return filter;
		}

		/// <summary>
		/// Creates a filter to get all items.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetAllFilter(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			return CreateGetAllFilter(EntityType.GetEntityType(entityType, OnNotFound.ThrowException));
		}

		/// <summary>
		/// Creates a filter to get all items.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetAllFilter(EntityType entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			return new SqlFilter(entityType);
		}

		/// <summary>
		/// Creates a "get parent" filter for the given entity and link.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetParentFilter(object entity, ChildToParentEntityLink link)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(link == null)
				throw new ArgumentNullException("link");
			
			// defer...
			return CreateGetParentFilter(entity.GetType(), entity, link, new EntityField[] {});
		}

		/// <summary>
		/// Creates a "get parent" filter for the given entity and link.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetParentFilter(Type entityType, object entity, ChildToParentEntityLink link)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(link == null)
				throw new ArgumentNullException("link");
			
			// defer...
			return CreateGetParentFilter(EntityType.GetEntityType(entityType, OnNotFound.ThrowException), entity, link, new EntityField[] {});
		}

		/// <summary>
		/// Creates a "get parent" filter for the given entity and link.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetParentFilter(EntityType entityType, object entity, ChildToParentEntityLink link)
		{
			return CreateGetParentFilter(entityType, entity, link, new EntityField[] {});
		}

		/// <summary>
		/// Creates a "get parent" filter for the given entity and link.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetParentFilter(Type entityType, object entity, ChildToParentEntityLink link, EntityField[] parentFields)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(link == null)
				throw new ArgumentNullException("link");
			if(parentFields == null)
				throw new ArgumentNullException("parentFields");
			
			// defer...
			return CreateGetParentFilter(EntityType.GetEntityType(entityType, OnNotFound.ThrowException), entity, link, parentFields);
		}

		/// <summary>
		/// Creates a "get parent" filter for the given entity and link.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="entity"></param>
		/// <param name="link"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetParentFilter(EntityType entityType, object entity, ChildToParentEntityLink link, EntityField[] parentFields)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(entity == null)
				throw new ArgumentNullException("entity");
			if(link == null)
				throw new ArgumentNullException("link");
			if(parentFields == null)
				throw new ArgumentNullException("parentFields");
			
			// check...
			entityType.AssertIsOfType(entity);
			if(link.ParentEntityType == null)
				throw new ArgumentNullException("link.ParentEntityType");

			// create...
			SqlFilter filter = new SqlFilter(link.ParentEntityType);

			// no need to sort - we're only after one...
			filter.SortOrder.Clear();

			// custom fields?
			if(parentFields.Length > 0)
			{
				filter.Fields.Clear();
				filter.Fields.AddRange(parentFields);
			}

			// get the key fields...
			EntityField[] parentKeyFields = link.ParentEntityType.GetKeyFields();
            if (parentKeyFields == null)
                throw new InvalidOperationException("'parentKeyFields' is null.");
            if (parentKeyFields.Length == 0)
                throw new InvalidOperationException("'parentKeyFields' is zero-length.");

			// find the matching fields...
			EntityField[] foreignFields = link.GetLinkFields();
            if (foreignFields == null)
                throw new InvalidOperationException("'foreignFields' is null.");
            if (foreignFields.Length == 0)
                throw new InvalidOperationException("'foreignFields' is zero-length.");

			// same length?
			if(parentKeyFields.Length != foreignFields.Length)
				throw ExceptionHelper.CreateLengthMismatchException("parentKeyFields", "foreignFields", parentKeyFields.Length, foreignFields.Length);

			// create...
			IEntityStorage storage = entityType.Storage;
			if(storage == null)
				throw new ArgumentNullException("storage");
			for(int index = 0; index < foreignFields.Length; index++)
			{
				// get the value...
				object value = storage.GetValue(entity, foreignFields[index]);

				// constraint...
				filter.Constraints.Add(parentKeyFields[index], SqlOperator.EqualTo, value);
			}

			// return...
			return filter;
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(EntityType entityType, object keyValue, OnNotFound onNotFound)
		{
			return GetById(entityType, new object[] { keyValue }, new EntityField[] {}, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(EntityType entityType, object keyValue, EntityField[] fields, OnNotFound onNotFound)
		{
			return GetById(entityType, new object[] { keyValue }, new EntityField[] {}, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(Type type, object keyValue, OnNotFound onNotFound)
		{
			return GetById(type, new object[] { keyValue }, new EntityField[] {}, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(Type type, object keyValue, EntityField[] fields, OnNotFound onNotFound)
		{
			return GetById(type, new object[] { keyValue }, new EntityField[] {}, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(EntityType entityType, object[] keyValues, OnNotFound onNotFound)
		{
			return GetById(entityType, keyValues, new EntityField[] {}, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(EntityType entityType, object[] keyValues, EntityField[] fields, OnNotFound onNotFound)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(keyValues == null)
				throw new ArgumentNullException("keyValues");
			if(keyValues.Length == 0)
				throw new ArgumentOutOfRangeException("'keyValues' is zero-length.");
			
			// create...
			SqlFilter filter = CreateGetByIdFilter(entityType, keyValues, fields);
			if(filter == null)
				throw new InvalidOperationException("filter is null.");
			return filter.ExecuteEntity(onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(Type type, object[] keyValues, OnNotFound onNotFound)
		{
			return GetById(type, keyValues, new EntityField[] {}, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object GetById(Type type, object[] keyValues, EntityField[] fields, OnNotFound onNotFound)
		{
			return GetById(EntityType.GetEntityType(type, OnNotFound.ThrowException), keyValues, fields, onNotFound);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IList GetAll(EntityType entityType)
		{
			return GetAll(entityType, new EntityField[] {});
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IList GetAll(EntityType entityType, EntityField[] fields)
		{
			return GetAll(entityType, new EntityField[] {}, null);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IList GetAll(EntityType entityType, EntityField[] fields, SortSpecificationCollection sortOrder)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(fields == null)
				throw new ArgumentNullException("fields");
			
			// create...
			SqlFilter filter = new SqlFilter(entityType, fields);
			if(sortOrder != null)
			{
				filter.SortOrder.Clear();
				filter.SortOrder.AddRange(sortOrder);
			}

			// run...
			return filter.ExecuteEntityCollection();
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IList GetAll(Type type)
		{
			return GetAll(type, new EntityField[] {});
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IList GetAll(Type type, EntityField[] fields)
		{
			return GetAll(type, new EntityField[] {}, null);
		}

		/// <summary>
		/// Gets all entities of the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static IList GetAll(Type type, EntityField[] fields, SortSpecificationCollection sortOrder)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(fields == null)
				throw new ArgumentNullException("fields");
			
			// get...
			EntityType entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// return...
			return GetAll(entityType, fields, sortOrder);
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fieldName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(Type type, string fieldName, SqlOperator filterOperator, object value)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
			
			// get...
			EntityType entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// return...
			return CreateGetFilter(entityType, fieldName, filterOperator, value);
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="fieldName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(EntityType entityType, string fieldName, SqlOperator filterOperator, object value)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(fieldName == null)
				throw new ArgumentNullException("fieldName");
			if(fieldName.Length == 0)
				throw new ArgumentOutOfRangeException("'fieldName' is zero-length.");
			
			// defer...
			return CreateGetFilter(entityType, new string[] { fieldName }, new SqlOperator[] { filterOperator }, new object[] { value });
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="fieldName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(Type type, string[] fieldNames, SqlOperator[] filterOperators, object[] values)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(fieldNames == null)
				throw new ArgumentNullException("fieldNames");
			if(filterOperators == null)
				throw new ArgumentNullException("filterOperators");
			if(values == null)
				throw new ArgumentNullException("values");
			if(fieldNames.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fieldNames' and 'values': {0} cf {1}.", fieldNames.Length, values.Length));
			if(fieldNames.Length != filterOperators.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fieldNames' and 'filterOperators': {0} cf {1}.", fieldNames.Length, filterOperators.Length));
			
			// get...
			EntityType entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// return...
			return CreateGetFilter(entityType, fieldNames, filterOperators, values);
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="fieldName"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(EntityType entityType, string[] fieldNames, SqlOperator[] filterOperators, object[] values)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(fieldNames == null)
				throw new ArgumentNullException("fieldNames");
			if(filterOperators == null)
				throw new ArgumentNullException("filterOperators");
			if(values == null)
				throw new ArgumentNullException("values");
			if(fieldNames.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fieldNames' and 'values': {0} cf {1}.", fieldNames.Length, values.Length));
			if(fieldNames.Length != filterOperators.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fieldNames' and 'filterOperators': {0} cf {1}.", fieldNames.Length, filterOperators.Length));
			
			// get...
			SqlFilter filter = new SqlFilter(entityType);
			for(int index = 0; index < fieldNames.Length; index++)
				filter.Constraints.Add(fieldNames[index], filterOperators[index], values[index]);

			// return...
			return filter;
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="field"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(Type type, EntityField field, SqlOperator filterOperator, object value)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(field == null)
				throw new ArgumentNullException("field");
			
			// get...
			EntityType entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// return...
			return CreateGetFilter(entityType, field, filterOperator, value);
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="field"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(EntityType entityType, EntityField field, SqlOperator filterOperator, object value)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(field == null)
				throw new ArgumentNullException("field");
			
			// defer...
			return CreateGetFilter(entityType, new EntityField[] { field }, new SqlOperator[] { filterOperator }, new object[] { value });
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="field"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(Type type, EntityField[] fields, SqlOperator[] filterOperators, object[] values)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(filterOperators == null)
				throw new ArgumentNullException("filterOperators");
			if(values == null)
				throw new ArgumentNullException("values");
			if(fields.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fields' and 'values': {0} cf {1}.", fields.Length, values.Length));
			if(fields.Length != filterOperators.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fields' and 'filterOperators': {0} cf {1}.", fields.Length, filterOperators.Length));
			
			// get...
			EntityType entityType = EntityType.GetEntityType(type, OnNotFound.ThrowException);
			if(entityType == null)
				throw new ArgumentNullException("entityType");

			// return...
			return CreateGetFilter(entityType, fields, filterOperators, values);
		}

		/// <summary>
		/// Creates a simple get filter.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="field"></param>
		/// <param name="filterOperator"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static SqlFilter CreateGetFilter(EntityType entityType, EntityField[] fields, SqlOperator[] filterOperators, object[] values)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(fields == null)
				throw new ArgumentNullException("fields");
			if(filterOperators == null)
				throw new ArgumentNullException("filterOperators");
			if(values == null)
				throw new ArgumentNullException("values");
			if(fields.Length != values.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fields' and 'values': {0} cf {1}.", fields.Length, values.Length));
			if(fields.Length != filterOperators.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'fields' and 'filterOperators': {0} cf {1}.", fields.Length, filterOperators.Length));
			
			// get...
			SqlFilter filter = new SqlFilter(entityType);
			for(int index = 0; index < fields.Length; index++)
				filter.Constraints.Add(fields[index], filterOperators[index], values[index]);

			// return...
			return filter;
		}

		/// <summary>
		/// Returns true if DataTable filtering is supported.
		/// </summary>
		/// <returns></returns>
		public bool SupportsDataTableFilterExpression
		{
			get
			{
				foreach(FilterConstraint constraint in this.Constraints)
				{
					if(constraint.ChildConstraints.Count > 0)
						return false;
				}

				// yep...
				return true;
			}
		}

		/// <summary>
		/// Gets a filter expression for use with DataTable instances.
		/// </summary>
		/// <returns></returns>
		public string GetDataTableFilterExpression()
		{
			// supports?
			if(this.SupportsDataTableFilterExpression == false)
				throw new NotSupportedException("DataTable filter expression generation not supported.");

			// walk...
			StringBuilder builder = new StringBuilder();
			for(int index = 0; index < this.Constraints.Count; index++)
			{
				FilterConstraint constraint = this.Constraints[index];

				// operator...
				if(index > 0)
				{
					// operator...
					SqlCombine combine = this.Constraints[index - 1].CombineWithNext;
					switch(combine)
					{
						case SqlCombine.And:
							builder.Append(" AND ");
							break;

						case SqlCombine.Or:
							builder.Append(" OR ");
							break;

						default:
							throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", combine, combine.GetType()));
					}
				}

				// add...
				if(constraint is EntityFieldFilterConstraint)
				{
					EntityFieldFilterConstraint fieldConstraint = (EntityFieldFilterConstraint)constraint;
					builder.Append(fieldConstraint.Field.NativeName.ToString());

					// operator...
					switch(fieldConstraint.FilterOperator)
					{
						case SqlOperator.EqualTo:
							builder.Append("=");
							break;

						default:
							throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", fieldConstraint.FilterOperator, fieldConstraint.FilterOperator.GetType()));
					}

					// add the value...
					object value = fieldConstraint.Value;
					string valueAsString = ConversionHelper.FormatValueForSql(value, fieldConstraint.Field.DBType);

					// add...
					builder.Append(valueAsString);
				}
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", constraint.GetType()));
			}

			// log...
			if(this.Log.IsInfoEnabled)
				this.Log.Info("Filter expression: " + builder.ToString());

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Gets a sort expression for use with DataTable instances.
		/// </summary>
		/// <returns></returns>
		public string GetDataTableSortExpression()
		{
			StringBuilder builder = new StringBuilder();
			foreach(SortSpecification sortSpecification in this.SortOrder)
			{
				if(builder.Length > 0)
					builder.Append(", ");

				// add...
				builder.Append(sortSpecification.Field.NativeName.ToString());
				builder.Append(" ");
				switch(sortSpecification.Direction)
				{
					case SortDirection.Ascending:
						builder.Append("ASC");
						break;

					case SortDirection.Descending:
						builder.Append("DESC");
						break;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", sortSpecification.Direction, sortSpecification.Direction.GetType()));
				}
			}

			// log...
			if(this.Log.IsInfoEnabled)
				this.Log.Info("Sort expression: " + builder.ToString());

			// return...
			return builder.ToString();
		}

        public override IList ExecuteEntityCollection()
        {
            IList results = null;
            var sink = this.Sink;
            object cacheTag = null;
            if(sink != null)
            {
                results = sink.BeforeExecuteEntityCollection(this, ref cacheTag);
                if (results != null)
                    return results;
            }

            // call the database...
            results = base.ExecuteEntityCollection();

            // give it back to the sink...
            if (sink != null)
                sink.AfterExecuteEntityCollection(this, cacheTag, results);

            // return...
            return results;
        }

        public override object ExecuteEntity(OnNotFound onNotFound)
        {
            object result = null;
            var sink = this.Sink;
            object cacheTag = null;
            if (sink != null)
            {
                result = sink.BeforeExecuteEntity(this, ref cacheTag);
                if (result != null)
                    return result;
            }

            // run...
            var oldTop = this.Top;
            this.Top = 1;
            try
            {
                result = base.ExecuteEntity(onNotFound);
            }
            finally
            {
                this.Top = oldTop;
            }

            // give it back to the sink...
            if (sink != null)
                sink.AfterExecuteEntity(this, cacheTag, result);

            // return...
            return result;
        }

        public IEnumerable<T> ExecuteKeyValues<T>()
        {
            var donor = this.Fields;
            this.Fields.Clear();
            this.Fields.Add(this.EntityType.GetKeyFields()[0]);

            // if we have a distinct, we need everything in the sort...
            if (this.Distinct && this.SortOrder.Count > 0)
            {
                foreach (SortSpecification sort in this.SortOrder)
                    this.Fields.Add(sort.Field);
            }

            try
            {
                return this.ExecuteValuesVertical<T>();
            }
            finally
            {
                this.Fields.Clear();
                this.Fields.AddRange(donor);
            }
        }

        public void SetSortOrder(string field, SortDirection direction = SortDirection.Ascending)
        {
            this.SortOrder.Clear();
            this.SortOrder.Add(new SortSpecification(this.EntityType.Fields[field], direction));
        }

        public void ForceSuggestedIndex()
        {
            this.LogTrace(() => string.Format("Forcing suggested index for '{0}'...", this.EntityType));

            var builder = this.SuggestIndex();
            var indexName = builder.IndexName;

            this.LogTrace(() => string.Format("Index name: {0}", indexName));

            var didCheck = false;
            var exists = false;
            try
            {
                // does it exist?
                lock (_forcedIndexCheckList)
                {
                    if (!(ForcedIndexCheckList.ContainsKey(indexName)))
                    {
                        this.LogTrace(() => "Checking for index...");
                        ForcedIndexCheckList[indexName] = Database.DoesIndexExist(this.EntityType.NativeName.Name, indexName);
                        didCheck = true;
                    }
                    else
                        this.LogTrace(() => "Index check already done.");

                    exists = ForcedIndexCheckList[indexName];
                    if (!(exists))
                    {
                        this.LogTrace(() => "Creating index...");

                        var sql = builder.ToString();
                        Database.ExecuteNonQuery(sql);

                        ForcedIndexCheckList[indexName] = true;
                    }
                }

                // force...
                this.LogTrace(() => "Finished forcing suggested index.");
                this.ForcedIndex = builder.IndexName;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Forced index suggestion failed.\r\nIndex name: {0}\r\nDid check: {1}\r\nExists: {2}", 
                    indexName, didCheck, exists), ex);
            }
        }

        public IndexBuilder SuggestIndex()
        {
            if (this.Fields.Count == 0)
                throw new InvalidOperationException("Index suggestion cannot be used with queries where output columns are not explicitly specified.");

            var joins = this.GetJoins();
            if (!(joins.Any()) && this.Constraints.Count == 0)
                throw new InvalidOperationException("The filter has no constraints or joins.");

            var builder = IndexBuilder.GetIndexBuilder(this.EntityType, this.Dialect);

            // include the output fields...
            foreach (EntityField field in this.Fields)
                builder.AddIncludeColumn(field);

            // include anything referenced in a join...
            if (joins.Any())
            {
                foreach (SqlJoin join in joins)
                {
                    foreach (EntityField field in join.SourceFields)
                    {
                        if (field.EntityType == this.EntityType)
                            builder.AddIncludeColumn(field);
                    }

                    foreach (EntityField field in join.TargetKeyFields)
                    {
                        if (field.EntityType == this.EntityType)
                            builder.AddIncludeColumn(field);
                    }
                }
            }

            // now the constraints...
            foreach (FilterConstraint constraint in this.Constraints)
            {
                if (constraint is EntityFieldFilterConstraint)
                {
                    var fieldConstraint = (EntityFieldFilterConstraint)constraint;
                    if(fieldConstraint.Field.EntityType == this.EntityType)
                        builder.AddIndexColumn(fieldConstraint.Field);
                }
                else if (constraint is ArrayConstraint)
                {
                    // foo in (select id from @q0) --> these should be done as includes...
                    var arrayConstraint = (ArrayConstraint)constraint;
                    if (arrayConstraint.Field.EntityType == this.EntityType && !(builder.IsInIndexColumns(arrayConstraint.Field)))
                        builder.AddIncludeColumn(arrayConstraint.Field);
                }
                else if(constraint is FreeFilterConstraint)
                {
                    var freeConstraint = (FreeFilterConstraint)constraint;
                    if (!(freeConstraint.ReferencedFields.Any()))
                        throw new InvalidOperationException(string.Format("Free constraint '{0}' has no referenced fields."));

                    foreach (var field in freeConstraint.ReferencedFields)
                        builder.AddIndexColumn(field);
                }
                else
                    throw new NotSupportedException(string.Format("Cannot handle '{0}'.", constraint.GetType()));
            }

            return builder;
        }

        public override bool SignalRecompile
        {
            get
            {
                if (this.ForceSignalRecompileSet)
                    return this.ForceSignalRecompile;
                else
                {
                    foreach (FilterConstraint constraint in this.Constraints)
                    {
                        if (constraint is ArrayConstraint)
                            return true;
                    }
                    return false;
                }
            }
            set
            {
                this.ForceSignalRecompile = value;
                this.ForceSignalRecompileSet = true;
            }
        }
    }
}

