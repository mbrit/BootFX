// BootFX - Application framework for .NET applications
// 
// File: EntityType.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Data;
using System.Linq;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Reflection;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Data.Formatters;
using BootFX.Common.Entities.Attributes;
using BootFX.Common.Dto;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a type of entity.
	/// </summary>
	/// <example>
	/// Potentially the best way to define attributes is to use attribute decoration on the classes.  All entities for a particularly assembly
	/// can then be loaded using the <see cref="LoadFromAttributes"></see> method.
	/// <code>
	/// [Entity(typeof(CustomerCollection), "Customers"), SortSpecification("Name", SortDirection.Ascending)]
	/// public class Customer : Entity
	/// {
	/// 	/// <summary>
	/// 	/// Constructor.
	/// 	/// </summary>
	/// 	public Customer()
	/// 	{
	/// 	}
	/// 
	/// 	[EntityField("CustomerID", DbType.Int32, EntityFieldFlags.Key)]
	/// 	public int ID
	/// 	{
	/// 		get
	/// 		{
	/// 			return (int)this["ID"];
	/// 		}
	/// 		set
	/// 		{
	/// 			this["ID"] = value;
	/// 		}
	/// 	}
	/// 
	/// 	[EntityField("Name", DbType.String, EntityFieldFlags.Common, 64)]
	/// 	public string Name
	/// 	{
	/// 		get
	/// 		{
	/// 			return (string)this["Name"];
	/// 		}
	/// 		set
	/// 		{
	/// 			this["Name"] = value;
	/// 		}
	/// 	}
	/// </code>
	/// </example>
	public class EntityType
	{
		/// <summary>
		/// Private field to support <c>RuleSet</c> property.
		/// </summary>
		private EntityRuleSet _ruleSet;

		/// <summary>
		/// Raised after the entity type has been loaded.
		/// </summary>
		public event EventHandler AfterLoad;
		
		/// <summary>
		/// Private field to support <see cref="EntityTypeFactory"/> property.
		/// </summary>
		private static IEntityTypeFactory _entityTypeFactory;
		
		private const string ExtendedPropertiesKey = "ExtendedProperties";
		
		/// <summary>
		/// Private field to support <c>ExtendedPropertiesLock</c> property.
		/// </summary>
		private static ReaderWriterLock _extendedPropertiesLock = new ReaderWriterLock();

		/// <summary>
		/// Private field to support <c>Indexes</c> property.
		/// </summary>
		private EntityIndexCollection _indexes = new EntityIndexCollection();
		
		/// <summary>
		/// Private field to support settings for extended properties
		/// </summary>
		private static ExtendedPropertySettings _extendedPropertySettings = null; 
		
		/// <summary>
		/// Private field to support <see cref="InitializingStack"/> property.
		/// </summary>
        private static IList _initializingStack = new ArrayList();
		
		/// <summary>
		/// Private field to support <see cref="PartitioningStrategy"/> property.
		/// </summary>
        //private PartitioningStrategy _partitioningStrategy;
        //private object _partitioningStrategyLock = new object();
		
		/// <summary>
		/// Private field to support <see cref="PartitioningStrategyField"/> property.
		/// </summary>
		// mbr - 04-09-2007 - for c7 - added.
		private EntityField _partitioningStrategyField = null;
		
		/// <summary>
		/// Private field to support <see cref="Dialect"/> property.
		/// </summary>
		private SqlDialect _dialect;
		
		/// <summary>
		/// Private field to support <c>PersistenceFactory</c> property.
		/// </summary>
		//private static IEntityPersistenceFactory _persistenceFactory;
		
		/// <summary>
		/// Private field to support <c>PersistenceObjects</c> property.
		/// </summary>
		private static Lookup _persistenceLookup;
		
		/// <summary>
		/// Defines the default stored name of the entity type.
		/// </summary>
		private const string DefaultStoredName = "$_entityType";

		/// <summary>
		/// Private field to support <see cref="DatabaseName"/> property.
		/// </summary>
		private string _databaseName = null;
		
		/// <summary>
		/// Private field to support <c>EntityTypes</c> property.
		/// </summary>
		private static Lookup _entityTypesLookup;
		
		/// <summary>
		/// Private field to support <c>CachePolicy</c> property.
		/// </summary>
        //private EntityCachePolicy _cachePolicy = null;
		
		/// <summary>
		/// Private field to support <c>Cache</c> property.
		/// </summary>
        //private EntityCache _cache;

		/// <summary>
		/// Lock object for <c>Cache</c>.
		/// </summary>
		private object cacheLock = new object();
		
		/// <summary>
		/// Private field to support <c>Links</c> property.
		/// </summary>
		private EntityLinkCollection _links;
		
		/// <summary>
		/// Private field to support <c>ChildLinks</c> property.
		/// </summary>
		private EntityLinkCollection _childLinks = null;
		
		/// <summary>
		/// Private field to support <c>DefaultSortOrder</c> property.
		/// </summary>
		private SortSpecificationCollection _defaultSortOrder = new SortSpecificationCollection();
		
		/// <summary>
		/// Defines the field to use with <c>ExtendedProperties</c> when this type is used to create a <c>DataTable</c>.
		/// </summary>
		public const string EntityTypeKey = "EntityType";
		
		/// <summary>
		/// Private field to support <c>Fields</c> property.
		/// </summary>
		private EntityFieldCollection _fields;
		
		/// <summary>
		/// Private field to support <c>Properties</c> property.
		/// </summary>
		private EntityPropertyCollection _properties;
		
		/// <summary>
		/// Private field to support <c>NativeName</c> property.
		/// 		/// </summary>
		private NativeName _nativeName;
		
		/// <summary>
		/// Private field to support <c>Type</c> property.
		/// </summary>
		private Type _type;

		/// <summary>
		/// Private field to support <c>CollectionType</c> property.
		/// </summary>
		private Type _collectionType;
		
		/// <summary>
		/// Private field to support <see cref="IsSystemTable"/> property.
		/// </summary>
		private bool _isSystemTable;
		
		/// <summary>
		/// Private field to support <c>Storage</c> property.
		/// </summary>
		private IEntityStorage _storage;

        public ISqlFilterSink SqlFilterSink { get; set; }

        private EntityField[] _keyFields = null;
        private EntityField[] _autoIncrementFields = null;

        public DtoType DtoType { get; set; }

        private static List<Assembly> OtherAssemblies { get; set; }
        private static object _otherAssembliesLock = new object();

        /// <summary>
        /// Constructor.
        /// </summary>
        internal EntityType(Type type, Type collectionType, string nativeName, string databaseName, Type serviceInterfaceType, bool isSystemTable) : 
			this(type, collectionType, NativeName.GetNativeName(nativeName), databaseName, isSystemTable)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		// mbr - 06-09-2007 - case 668 - made protected.		
		//		internal EntityType(Type type, Type collectionType, NativeName nativeName, string databaseName, bool isSystemTable)
		protected EntityType(Type type, Type collectionType, NativeName nativeName, string databaseName, bool isSystemTable)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(collectionType == null)
				throw new ArgumentNullException("collectionType");
			if(nativeName == null)
				throw new ArgumentNullException("nativeName");

			// create...
			_fields = new EntityFieldCollection(this);
			_fields.FieldAdded += new EntityFieldEventHandler(_fields_FieldAdded);
			_links = new EntityLinkCollection(this);
			_links.LinkAdded += new EntityLinkEventHandler(_links_LinkAdded);
			_properties = new EntityPropertyCollection(this);
			_properties.PropertyAdded += new EntityPropertyEventHandler(_properties_PropertyAdded);

			// set...
			_type = type;
			_collectionType = collectionType;
			_nativeName = nativeName;
			_databaseName = databaseName;
			_isSystemTable = isSystemTable;

			// setup services...
			if(typeof(Entity).IsAssignableFrom(type) == true)
			{
				// setup...
				_storage = new EntityManagedStorage(this);
			}
			else
			{
				// TODO: This method has not been implemented.  Check whether it is a valid not implementated, or a code path that was not completed.
				throw new NotImplementedException("The operation has not been implemented.");
			}
		}

        static EntityType()
        {
            OtherAssemblies = new List<Assembly>();
        }

		/// <summary>
		/// Initializes the metadata subsystem.
		/// </summary>
		// mbr - 04-10-2007 - case 851 - added assumed boot assembly...		
		internal static void Initialize(IEntityTypeFactory factory)
		{
			// set...
			_entityTypeFactory = factory;

			// mbr - 06-12-2005 - load the extended stuff...
			Database.DefaultDatabaseChanged += new EventHandler(Database_DefaultDatabaseChanged);

			// setup...
			_entityTypesLookup = new Lookup();
			_entityTypesLookup.CreateItemValue += new CreateLookupItemEventHandler(_entityTypesLookup_CreateItemValue);
			_persistenceLookup = new Lookup();
			_persistenceLookup.CreateItemValue += new CreateLookupItemEventHandler(_persistenceLookup_CreateItemValue);			
		}

		/// <summary>
		/// Gets the issystemtable.
		/// </summary>
		public bool IsSystemTable
		{
			get
			{
				return _isSystemTable;
			}
			set
			{
				_isSystemTable = value;
			}
		}

		/// <summary>
		/// Gets the storage.
		/// </summary>
		public IEntityStorage Storage
		{
			get
			{
				return _storage;
			}
		}

		/// <summary>
		/// Gets the persistence.
		/// </summary>
		public IEntityPersistence Persistence
		{
			get
			{
				return (IEntityPersistence)PersistenceLookup[this];
			}
		}

		/// <summary>
		/// Gets the collectiontype.
		/// </summary>
		public Type CollectionType
		{
			get
			{
				return _collectionType;
			}
			set
			{
				if(value != null)
				{
					// check...
					if(typeof(IList).IsAssignableFrom(value) == false)
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Type '{0}' does not implement IList.", value));
				}
				if(_collectionType != value)
					_collectionType = value;
			}
		}

        ///// <summary>
        ///// Gets or sets the cachepolicy
        ///// </summary>
        //public EntityCachePolicy CachePolicy
        //{
        //    get
        //    {
        //        return _cachePolicy;
        //    }
        //    set
        //    {
        //        lock(this.cacheLock)
        //        {
        //            // check to see if the value has changed...
        //            if(value != _cachePolicy)
        //            {
        //                // set the value...
        //                _cachePolicy = value;

        //                // update the cache...
        //                ResetCache();
        //            }
        //        }
        //    }
        //}

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
		/// Gets the type.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Gets a collection of Fields objects.
		/// </summary>
		public EntityFieldCollection Fields
		{
			get
			{
				return _fields;
			}
		}

		/// <summary>
		/// Creates an instance of this entity type.
		/// </summary>
		/// <returns></returns>
		// mbr - 28-09-2007 - for c7 - made virtual.		
		public virtual object CreateInstance()
		{
			if(Type == null)
				throw new ArgumentNullException("Type");
			return Activator.CreateInstance(this.Type);
		}

		/// <summary>
		/// Creates an instance using lookahead values.
		/// </summary>
		/// <param name="lookaheadValues"></param>
		/// <returns></returns>
		protected internal virtual object CreateInstance(IEntityReadState reader)
		{
			return this.CreateInstance();
		}

		/// <summary>
		/// Creates a collection containing the given entities.
		/// </summary>
		/// <returns></returns>
		public static IList CreateCollectionInstance(ICollection entities)
		{
			if(entities == null)
				throw new ArgumentNullException("entities");
			
			// count?
			if(entities.Count == 0)
				return new object[] {};
			else if(entities.Count == 1)
			{
				// return...
				if(entities is IList)
					return CreateCollectionInstance(((IList)entities)[0]);
				else
				{
					// get...
					IEnumerator enumerator = entities.GetEnumerator();
					try
					{
						// get...
						if(enumerator.MoveNext())
							return CreateCollectionInstance(enumerator.Current);
						else
							return new object[] {};
					}
					finally
					{
						if(enumerator is IDisposable)
							((IDisposable)enumerator).Dispose();
					}
				}
			}
			else
			{
				// TODO: examine the entities and if they are all of the same type then return the specific
				// collection for that type.
				return new ArrayList(entities);
			}
		}

		/// <summary>
		/// Creates a collection containing the given entity.
		/// </summary>
		/// <returns></returns>
		public static IList CreateCollectionInstance(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// get...
			EntityType et = GetEntityType(entity, OnNotFound.ThrowException);
			if(et == null)
				throw new InvalidOperationException("et is null.");

			// create...
			IList results = et.CreateCollectionInstance();
			if(results == null)
				throw new InvalidOperationException("results is null.");

			// add...
			results.Add(entity);

			// return...
			return results;
		}

		/// <summary>
		/// Creates an instance of a collection for this type.
		/// </summary>
		/// <returns></returns>
		public IList CreateCollectionInstance()
		{
			if(CollectionType == null)
				throw new ArgumentNullException("CollectionType");
			return (IList)Activator.CreateInstance(this.CollectionType);
		}

        public IEnumerable<T> CreateCollectionInstance<T>()
            where T : Entity
        {
            return (IEnumerable<T>)this.CreateCollectionInstance();
        }

		/// <summary>
		/// Asserts the given type is valid for this enitty.
		/// </summary>
		/// <param name="checkType"></param>
		// mbr - 26-01-2006 - change to deferral.		
		public void AssertIsOfType(Type checkType)
		{
			if(checkType == null)
				throw new ArgumentNullException("checkType");
			
			// mbr - 17-03-2006 - added check to see if we have an entity type.
			if(typeof(EntityType).IsAssignableFrom(checkType))
				throw new ArgumentException("Incorrect assertion against EntityType object itself.  Check that this method has not been passed the EntityType instance.");

			// are we?
			if(!(this.IsOfType(checkType)))
			{
				// mbr - 22-12-2005 - added this to debug a problem with database update.				
				throw new InvalidOperationException(string.Format("'{0}' is not valid for use with '{1}'.\r\nMaster: {2} ({3})\r\nChecking: {4} ({5})", checkType, this,
					this.Type.AssemblyQualifiedName, this.Type.Assembly.CodeBase, checkType.AssemblyQualifiedName, checkType.Assembly.CodeBase));
			}
		}

		/// <summary>
		/// Returns true if the given entity type is compatible with the type given.
		/// </summary>
		/// <param name="checkType"></param>
		/// <returns></returns>
		// mbr - 26-01-2006 - added.		
		public bool IsOfType(EntityType checkType)
		{
			if(checkType == null)
				throw new ArgumentNullException("checkType");
			
			// defer...
			return this.IsOfType(checkType.Type);
		}

		/// <summary>
		/// Returns true if the given entity type is compatible with the type given.
		/// </summary>
		/// <param name="checkType"></param>
		/// <returns></returns>
		// mbr - 26-01-2006 - added.		
		public bool IsOfType(Type checkType)
		{
			if(checkType == null)
				throw new ArgumentNullException("checkType");
			
			// check...
			if(Type == null)
				throw new ArgumentNullException("Type");
			if(!(Type.IsAssignableFrom(checkType)))
				return false;
			else
				return true;
		}

		/// <summary>
		/// Asserts the given entity is valid for this entity type.
		/// </summary>
		/// <param name="entity"></param>
		public void AssertIsOfType(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			
			// assert...
			this.AssertIsOfType(entity.GetType());
		}

		/// <summary>
		/// Creates a blank data table that is compatible with the schema for this entity type.
		/// </summary>
		/// <returns></returns>
		public DataTable CreateDataTable()
		{
			// create...
			DataTable table = new DataTable(this.NativeName.ToString());
			table.Locale = Cultures.System;
			table.ExtendedProperties[EntityTypeKey] = this;

			// add...
			foreach(EntityField field in this.Fields)
				table.Columns.Add(field.CreateDataColumn());

			// return...
			return table;
		}

		/// <summary>
		/// Gets the entity types' key fields.
		/// </summary>
		/// <returns></returns>
		public EntityField[] GetCommonFields()
		{
			ArrayList results = new ArrayList();
			foreach(EntityField field in this.Fields)
			{
				if(field.IsCommon() == true)
					results.Add(field);
			}

			// get...
			return (EntityField[])results.ToArray(typeof(EntityField));
		}

		/// <summary>
		/// Gets the entity types' key fields.
		/// </summary>
		/// <returns></returns>
		public EntityField[] GetKeyFields()
		{
			return this.GetKeyFields(true);
		}

		/// <summary>
		/// Gets the entity types' key fields.
		/// </summary>
		/// <returns></returns>
		public EntityField[] GetNonKeyFields()
		{
			return this.GetKeyFields(false);
		}

		/// <summary>
		/// Gets the entity types' auto increment fields.
		/// </summary>
		/// <returns></returns>
		public EntityField[] GetAutoIncrementFields()
		{
			return this.GetAutoIncrementFields(true);
		}

		/// <summary>
		/// Gets the entity types' key fields.
		/// </summary>
		/// <returns></returns>
		private EntityField[] GetKeyFields(bool isKey)
		{
            if (isKey && _keyFields != null)
                return _keyFields;

            var results = new List<EntityField>();
			foreach(EntityField field in this.Fields)
			{
				if(field.IsKey() == isKey)
					results.Add(field);
			}

			var asArray = results.ToArray();

            // cache...
            if (isKey)
                _keyFields = asArray;

            return _keyFields;
		}

		/// <summary>
		/// Gets the entity types' lookup fields.
		/// </summary>
		/// <returns></returns>
		public EntityLookupField[] GetLookupFields()
		{
			ArrayList results = new ArrayList();
			foreach(EntityField field in this.Fields)
			{
				if(field.IsLookupProperty)
					results.Add(field);
			}

			// return...
			return (EntityLookupField[])results.ToArray(typeof(EntityLookupField));
		}


		/// <summary>
		/// Gets the entity types auto increment fields.
		/// </summary>
		/// <returns></returns>
		private EntityField[] GetAutoIncrementFields(bool isAutoIncrement)
		{
            if (_autoIncrementFields == null)
            {
                ArrayList results = new ArrayList();
                foreach (EntityField field in this.Fields)
                {
                    // mbr - 04-07-2007 - changed.
                    //				if(field.IsAutoIncrement() == isAutoIncrement)
                    if (field.IsAutoIncrement == isAutoIncrement)
                        results.Add(field);
                }

                // return...
                _autoIncrementFields = (EntityField[])results.ToArray(typeof(EntityField));
            }
            return _autoIncrementFields;
		}

		/// <summary>
		/// Gets the name of the entity.
		/// </summary>
		public string Name
		{
			get
			{
				if(Type == null)
					throw new ArgumentNullException("Type");
				return this.Type.Name;
			}
		}

		/// <summary>
		/// Gets the name of the entity.
		/// </summary>
		public string FullName
		{
			get
			{
				if(Type == null)
					throw new ArgumentNullException("Type");
				return this.Type.FullName;
			}
		}

		/// <summary>
		/// Gets a collection of SortSpecification objects.
		/// </summary>
		public SortSpecificationCollection DefaultSortOrder
		{
			get
			{
				return _defaultSortOrder;
			}
		}

		private void _links_LinkAdded(object sender, EntityLinkEventArgs e)
		{
			// set it...
			e.EntityLink.SetEntityType(this);
			e.EntityLink.SetOrdinal(this.Links.Count - 1);
		}

		private void _fields_FieldAdded(object sender, EntityFieldEventArgs e)
		{
			// set it...
			e.EntityField.SetEntityType(this);
			e.EntityField.SetOrdinal(this.Fields.Count - 1);

			// mbr - 13-11-2005 - added partitioning invalidation...
            //if(e.EntityField.IsPartitionId())
            //{
            //    // set...
            //    if(this.PartitioningStrategy != null)
            //    {
            //        throw new InvalidOperationException(string.Format("Cannot set a field to have a partition ID when a partitioning strategy of '{0}' has already been defined.  (This may occur is multiple partition ID fields are on a table.)", 
            //            this.PartitioningStrategy));
            //    }

            //    // mbr - 04-09-2007 - for c7 - changed this so that it will tell it it needs one, but demand load it
            //    // when it needs it.  c7 needed the ability to change the default strategy.
            //    //				_partitioningStrategy = new SingleColumnPartitioningStategy(e.EntityField);
            //    _partitioningStrategyField = e.EntityField;
            //}
		}

		private void _properties_PropertyAdded(object sender, EntityPropertyEventArgs e)
		{
			// set it...
			e.EntityProperty.SetEntityType(this);
			e.EntityProperty.SetOrdinal(this.Properties.Count - 1);
		}

		/// <summary>
		/// Loads entity types from the given assembly.
		/// </summary>
		/// <param name="asm"></param>
		/// <returns></returns>
		// mbr - 04-10-2007 - case 851 - and made internal...		
		[Obsolete("No longer the preferred method for loading entities.")]
		public static EntityType[] LoadFromAttributes(string assemblyPath)
		{
			if(assemblyPath == null || assemblyPath == string.Empty)
				throw new ArgumentNullException("assemblyPath");

			// get...
			TypeFinder finder = new TypeFinder(typeof(Entity));
			finder.AddAttributeSpecification(typeof(EntityAttribute),true);
			Type[] types = finder.GetTypes(assemblyPath);
			
			// return...
			return GetEntityTypesForTypes(types);
		}

		/// <summary>
		/// Loads entity types from the currently loaded assemblies
		/// </summary>
		/// <returns></returns>
		// mbr - 04-10-2007 - made internal.		
		internal static EntityType[] LoadFromAttributes()
		{
			// get...
			TypeFinder finder = new TypeFinder(typeof(Entity));
			finder.AddAttributeSpecification(typeof(EntityAttribute),true);
			Type[] types = finder.GetTypes();

			// get...
			return GetEntityTypesForTypes(types);
		}
		
		/// <summary>
		/// Loads entity types from the given assembly.
		/// </summary>
		/// <param name="asm"></param>
		/// <returns></returns>
		public static EntityType[] LoadFromAttributes(Assembly theAssembly)
		{
			if(theAssembly == null)
				throw new ArgumentNullException("theAssembly");

			// get...
			TypeFinder finder = new TypeFinder(typeof(Entity));
			finder.AddAttributeSpecification(typeof(EntityAttribute),true);
			Type[] types = finder.GetTypes(theAssembly);

            // mbr - 2009-09-22 - DEBUG!  reorder surveys so that it loads first...
            List<Type> allTypes = new List<Type>(types);
            Type surveys = null;
            Type customers = null;
            foreach (Type type in allTypes)
            {
                if (type.FullName == "Shl.Mobile.Survey")
                    surveys = type;
                else if (type.FullName == "Shl.Mobile.Customer")
                    customers = type;
            }

            // rejig...
            if (surveys != null)
            {
                // add to the top...
                allTypes.Remove(surveys);
                allTypes.Insert(0, surveys);
            }
            if (customers != null)
            {
                // add to the end...
                allTypes.Remove(customers);
                allTypes.Add(customers);
            }

			// defer...
			return GetEntityTypesForTypes(allTypes.ToArray());
		}

		/// <summary>
		/// Gets the entity type instances for the given CLR types.
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		private static EntityType[] GetEntityTypesForTypes(Type[] types)
		{
			// walk...
			ArrayList results = new ArrayList();
			foreach(Type type in types)
			{
				EntityType entityType = (EntityType) EntityTypesLookup[type];
				if(entityType != null)
					results.Add(entityType);
			}
	
			// return...
			return (EntityType[])results.ToArray(typeof(EntityType));
		}

		/// <summary>
		/// Loads an entity type from attributes on the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static EntityType LoadFromAttributes(Type type, OnNotFound onNotFound)
		{
			// try and demand load it...
			EntityAttribute[] entityAttributes = (EntityAttribute[])type.GetCustomAttributes(typeof(EntityAttribute), false);
			if(entityAttributes.Length != 0)
			{
				// return...
				EntityType entityType = LoadFromAttributes(type, entityAttributes[0]);

				// return...
				return entityType;
			}
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Type '{0}' is not decorated with entity attributes.", type));

					default:
						throw ExceptionHelper.CreateCannotHandleException(onNotFound);
				}
			}
		}

		/// <summary>
		/// Load an entity type from the given attribute.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="entityAttribute"></param>
		/// <returns></returns>
		private static EntityType LoadFromAttributes(Type type, EntityAttribute entityAttribute)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(entityAttribute == null)
				throw new ArgumentNullException("entityAttribute");

			// create...
			string nativeName = entityAttribute.NativeName;

			// mbr - 06-09-2007 - case 668 for c7 - create factory...
			//			EntityType entityType = new EntityType(type, entityAttribute.CollectionType, NativeName.GetNativeName(nativeName), 
			//				entityAttribute.DatabaseName, entityAttribute.IsSystemTable);
			EntityType entityType = null;
			if(EntityTypeFactory != null)
			{
				try
				{
					entityType = EntityTypeFactory.CreateEntityType(type, entityAttribute.CollectionType, 
						NativeName.GetNativeName(nativeName), entityAttribute.DatabaseName, entityAttribute.IsSystemTable);
					if(entityType == null)
						throw new InvalidOperationException("entityType is null.");
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format("Entity type factory '{0}' returned null when asked to create entity for type '{1}'.", 
						EntityTypeFactory, type), ex);
				}
			}
			else
			{
				entityType = new EntityType(type, entityAttribute.CollectionType, NativeName.GetNativeName(nativeName), 
					entityAttribute.DatabaseName, entityAttribute.IsSystemTable);
			}

			// do the fields...
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach(PropertyInfo property in properties)
			{
				// get...
				EntityFieldAttribute[] fieldAttributes = (EntityFieldAttribute[])property.GetCustomAttributes(typeof(EntityFieldAttribute), true);
				foreach(EntityFieldAttribute fieldAttribute in fieldAttributes)
				{
					// add...
					EntityField newField = null;
					try
					{
						if(fieldAttribute.SizeDefined == false)
							newField = new EntityField(property.Name, fieldAttribute.NativeName, fieldAttribute.DBType, fieldAttribute.Flags);
						else
							newField = new EntityField(property.Name, fieldAttribute.NativeName, fieldAttribute.DBType, fieldAttribute.Flags, fieldAttribute.Size);
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to create field for property '{0}' on '{1}'.", property.Name, type), ex);
					}

					// check...
					if(newField == null)
						throw new ArgumentNullException("newField");

					// stuff...
					newField.Default = fieldAttribute.Default;

					// get it...
					DBNullEquivalentAttribute[] equivalentAttrs = (DBNullEquivalentAttribute[])property.GetCustomAttributes(typeof(DBNullEquivalentAttribute), false);
					if(equivalentAttrs != null && equivalentAttrs.Length > 0)
						newField.DBNullEquivalent = equivalentAttrs[0].DBNullEquivalent;

					// mapping?
					EnumerationMappingAttribute[] mappingAttrs = (EnumerationMappingAttribute[])property.GetCustomAttributes(typeof(EnumerationMappingAttribute), false);
					if(mappingAttrs != null && mappingAttrs.Length > 0)
						newField.EnumerationType = mappingAttrs[0].EnumerationType;
					else
					{
						// mbr - 15-12-2006 - added...
						if(property.PropertyType.IsEnum)
							newField.EnumerationType = property.PropertyType;
					}

					// encryption?
					//EncryptedAttribute[] encryptedAttrs = (EncryptedAttribute[])property.GetCustomAttributes(typeof(EncryptedAttribute), false);
					//if(encryptedAttrs != null && encryptedAttrs.Length > 0)
					//	newField.EncryptionKeyName = encryptedAttrs[0].KeyName;

					// format string...
					DefaultFormatStringAttribute[] defaultFormatStringAttrs = (DefaultFormatStringAttribute[])property.GetCustomAttributes(typeof(DefaultFormatStringAttribute), false);
					if(defaultFormatStringAttrs != null && defaultFormatStringAttrs.Length > 0)
						newField.DefaultFormatString = defaultFormatStringAttrs[0].FormatString;

					// mbr - 01-11-2005 - default?
					DatabaseDefaultAttribute[] databaseDefaultAttrs = (DatabaseDefaultAttribute[])property.GetCustomAttributes(typeof(DatabaseDefaultAttribute), false);
					if(databaseDefaultAttrs != null && databaseDefaultAttrs.Length > 0)
						newField.DefaultExpression = new SqlDatabaseDefault(databaseDefaultAttrs[0].Type, databaseDefaultAttrs[0].Value);

					// add...
					entityType.Fields.Add(newField);
				}

				// links...
				EntityLinkToParentAttribute[] toParentAttributes = (EntityLinkToParentAttribute[])property.GetCustomAttributes(typeof(EntityLinkToParentAttribute), true);
				foreach(EntityLinkToParentAttribute toParentAttribute in toParentAttributes)
				{
					// create...
					ChildToParentEntityLink link = new ChildToParentEntityLink(toParentAttribute.Name,toParentAttribute.NativeName, toParentAttribute.ParentEntityType, toParentAttribute.GetLinkFieldNames());
					entityType.Links.Add(link);
				}

				// properties...
				if(property.IsDefined(typeof(EntityPropertyAttribute), true))
					entityType.Properties.Add(new EntityProperty(type, property));
			}

			// rjp 7-11-2005 - indexes
			IndexAttribute[] databaseIndexAttrs = (IndexAttribute[])type.GetCustomAttributes(typeof(IndexAttribute), false);
			if(databaseIndexAttrs != null && databaseIndexAttrs.Length > 0)
			{
				foreach(IndexAttribute indexAttr in databaseIndexAttrs)
					entityType.Indexes.Add(new EntityIndex(entityType,indexAttr));
			}

            //// caching...
            //EntityCachePolicyAttribute[] cacheAttributes = (EntityCachePolicyAttribute[])type.GetCustomAttributes(typeof(EntityCachePolicyAttribute), false);
            //if(cacheAttributes.Length > 0)
            //    entityType.CachePolicy = new EntityCachePolicy(cacheAttributes[0].TimeToLive, cacheAttributes[0].Group);

			// sort...
			SortSpecificationAttribute[] specificationAttributes = (SortSpecificationAttribute[])type.GetCustomAttributes(typeof(SortSpecificationAttribute), false);
			if(specificationAttributes.Length > 0)
			{
				// set...
				entityType.DefaultSortOrder.Clear();
				entityType.DefaultSortOrder.AddRange(specificationAttributes[0].GetSortSpecification(entityType));
			}

			// mbr - 02-11-2005 - indexes...
			IndexAttribute[] indexAttributes = (IndexAttribute[])type.GetCustomAttributes(typeof(IndexAttribute), true);
			if(indexAttributes.Length > 0)
			{
				foreach(IndexAttribute indexAttribute in indexAttributes)
					entityType.Indexes.Add(new EntityIndex(entityType, indexAttribute));
			}

			// add...
			//EntityTypesLookup.Add(entityType);
			EntityTypesLookup[type] = entityType;

            // mbr - 2015-05-13 - dtos...
            var dtoAttrs = (DtoAttribute[])type.GetCustomAttributes(typeof(DtoAttribute), true);
            if (dtoAttrs.Any())
                entityType.DtoType = new DtoType(dtoAttrs[0].Type, entityType);

			// fixup...
			entityType.FixupAfterLoad();

			// return...
			return entityType;			
		}

		/// <summary>
		/// Fixes up entities after loading.
		/// </summary>
        private void FixupAfterLoad()
        {
            // mbr - 2009-09-22 - troubleshooting problem with reference load order...
            EntityTypeLoadContext context = EntityTypeLoadContext.Current;

            // if we have a context, but this item in the bucket and this will give us a chance to order it...
            if (context != null)
            {
                context.AddToFixupList(this);
                return;
            }
            else
                this.DoFixup();
        }

        internal static void HandleDeferredFixups(List<EntityType> ets)
        {
            if (ets == null)
                throw new ArgumentNullException("ets");
            if (ets.Count == 0)
                throw new ArgumentException("'ets' is zero-length.");

            // order them by dependency...
            ets = SortByDependency(ets);
            if (ets == null)
                throw new InvalidOperationException("'ets' is null.");
            if (ets.Count == 0)
                throw new InvalidOperationException("'ets' is zero-length.");

            // do the list backwards as the item with the most dependencies is at the bottom...
            for (int index = ets.Count - 1; index >= 0; index--)
                ets[index].DoFixup();
        }

        private void DoFixup()
        {
            // walk and add parent inclusions into the partitioning stategies...
            //foreach (EntityLink link in this.Links)
            //{
            //    ChildToParentEntityLink parentLink = link as ChildToParentEntityLink;
            //    if (parentLink != null && parentLink.ParentEntityTypeAsType != this.Type)
            //    {
            //        // do we support partitioning?
            //        if (parentLink.ParentEntityType == null)
            //            throw new InvalidOperationException("link.ParentEntityType is null.");
            //        if (parentLink.ParentEntityType.SupportsPartitioning)
            //        {
            //            // jmm - 8/10/2007 - changed to use the first non-nullable link
            //            if (!(parentLink.IsNullable()) && this.PartitioningStrategy == null)
            //                _partitioningStrategy = new ParentInclusionPartitioningStrategy(parentLink);
            //        }
            //    }
            //}

            // mbr - 07-12-2005 - extended...
            this.LoadExtendedProperties();

            // mbr - 02-10-2007 - for c7 - event...
            this.OnAfterLoad();
        }

		/// <summary>
		/// Will load any extended properties from the database applying them to the schema
		/// </summary>
		private void LoadExtendedProperties()
		{
			// mbr - 06-12-2005 - if it's a system table, don't...
			if(this.IsSystemTable)
				return;

			// remove existing ones...
			ArrayList toRemove = new ArrayList();
			foreach(EntityField field in _fields)
			{
				if(field.IsExtendedProperty)
					toRemove.Add(field);
			}

			// remove them...
			foreach(EntityField field in toRemove)
				_fields.Remove(field);

			// get the new ones...
			if(ExtendedPropertySettings != null)
			{
				EntityField[] fields = ExtendedPropertySettings.GetExtendedPropertiesForEntityType(this);
				if(fields == null)
					throw new InvalidOperationException("fields is null.");

				// add...
				if(fields.Length > 0)
					_fields.AddRange(fields);
			}
		}

		/// <summary>
		/// Gets a collection of EntityLink objects.
		/// </summary>
		public EntityLinkCollection Links
		{
			get
			{
				return _links;
			}
		}

		/// <summary>
		/// Gets a collection of link objects that go FROM the parent TO this entity type (see remarks).
		/// </summary>
		/// <remarks>This property is counter-intuitive.  The links that are returned DO NOT have an EntityType that
		/// matches the entity type of the one making the call.  For example, if THIS was 'Invoice', child links returned
		/// would have a <c>ParentEntityType</c> of THIS (i.e. 'Invoice') and an <c>EntityType</c> of the associated type
		/// (e.g. 'Customer').  Use <c>GetParentLinks</c> for something more intuitive.</remarks>
		public EntityLinkCollection ChildLinks
		{
			get
			{
				if(_childLinks == null)
				{
					_childLinks = new EntityLinkCollection(this);
					
					// mbr - 04-10-2007 - case 851 - changed to use the loaded et's only...					
//					EntityType[] childTypes = EntityType.GetAllEntityTypes();
					EntityType[] childTypes = EntityType.GetEntityTypes();
					foreach(EntityType childEntityType in childTypes)
					{
						MemberInfo[] members = childEntityType.Type.GetMembers();
						foreach(MemberInfo member in members)
						{
							// We now get the attributes for the type so we can walk them
							EntityLinkToParentAttribute[] attributes = (EntityLinkToParentAttribute[]) member.GetCustomAttributes(typeof(EntityLinkToParentAttribute),true);
							foreach(EntityLinkToParentAttribute linkAttribute in attributes)
							{
								if(!linkAttribute.ParentEntityType.Equals(this.Type))
									continue;
							
								// Now we add the new child to parent link
								ChildToParentEntityLink childParentLink = new ChildToParentEntityLink(childEntityType.Name,linkAttribute.NativeName,this.Type,linkAttribute.GetLinkFieldNames());
								childParentLink.SetEntityType(childEntityType);
								_childLinks.Add(childParentLink);
							}
						}
					}
				}
				
				return _childLinks;
			}
		}

		/// <summary>
		/// Gets the links on this object that link up to parent items (see remarks).
		/// </summary>
		/// <remarks>See notes against the 'Remarks' section of <c>ChildLinks</c>.</remarks>
		public ChildToParentEntityLink[] GetParentLinks()
		{
			// walk all the entity types and all of the links...
			ArrayList results = new ArrayList();
			foreach(EntityType et in EntityType.GetEntityTypes())
			{
				foreach(EntityLink link in et.ChildLinks)
				{
					if(link is ChildToParentEntityLink)
					{
						ChildToParentEntityLink toParentLink = (ChildToParentEntityLink)link;
						if(toParentLink.EntityType == this)
							results.Add(link);
					}
				}
			}

			// return...
			return (ChildToParentEntityLink[])results.ToArray(typeof(ChildToParentEntityLink));
		}
		
		/// <summary>
		/// Gets property descriptors for all members.
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptorCollection GetPropertyDescriptors()
		{
			return this.GetPropertyDescriptors(EntityMemberType.All);
		}

		/// <summary>
		/// Gets property descriptors for the given members.
		/// </summary>
		/// <returns></returns>
		public PropertyDescriptorCollection GetPropertyDescriptors(EntityMemberType types)
		{
			// create...
			PropertyDescriptorCollection properties = new PropertyDescriptorCollection(new PropertyDescriptor[] {});

			// equiv...
			bool doEquiv = false;
			if((types & EntityMemberType.IncludeStringEquivalents) != 0)
				doEquiv = true;

			// seen...
			ArrayList seen = new ArrayList();

			// add...
			PropertyDescriptor prop = null;
			PropertyDescriptor equivProp = null;
			if((types & EntityMemberType.Field) != 0)
			{
				foreach(EntityField field in this.Fields)
				{
					prop = field.GetPropertyDescriptor();
					if(prop == null)
						throw new InvalidOperationException("prop is null.");
					properties.Add(prop);

					// add?
					if(doEquiv)
					{
						equivProp = this.GetStringEquivalent(prop, field.DefaultFormatString);
						if(equivProp != null)
						{
							string check = equivProp.Name.ToLower();
							if(!(seen.Contains(check)))
							{
								properties.Add(equivProp);
								seen.Add(check);
							}
						}
					}
				}
			}

			if((types & EntityMemberType.Link) != 0)
			{
				foreach(EntityLink link in this.Links)
				{
					prop = link.GetPropertyDescriptor();
					if(prop == null)
						throw new InvalidOperationException("prop is null.");
					properties.Add(prop);

					// add?
					if(doEquiv)
					{
						equivProp = this.GetStringEquivalent(prop, null);
						if(equivProp != null)
						{
							string check = equivProp.Name.ToLower();
							if(!(seen.Contains(check)))
							{
								properties.Add(equivProp);
								seen.Add(check);
							}
						}
					}
				}
			}

			if((types & EntityMemberType.Property) != 0)
			{
				foreach(EntityProperty property in this.Properties)
				{
					prop = property.GetPropertyDescriptor();
					if(prop == null)
						throw new InvalidOperationException("prop is null.");
					properties.Add(prop);

					// add?
					if(doEquiv)
					{
						equivProp = this.GetStringEquivalent(prop, null);
						if(equivProp != null)
						{
							string check = equivProp.Name.ToLower();
							if(!(seen.Contains(check)))
							{
								properties.Add(equivProp);
								seen.Add(check);
							}
						}
					}
				}
			}

			// return...
			return properties;
		}

		/// <summary>
		/// Gets the string equivalent, if any.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		private PropertyDescriptor GetStringEquivalent(PropertyDescriptor descriptor, string defaultFormatString)
		{
			if(descriptor == null)
				throw new ArgumentNullException("descriptor");
			
			if(descriptor is IStringEquivalentPropertyDescriptor)
			{
				IStringEquivalentPropertyDescriptor equiv = (IStringEquivalentPropertyDescriptor)descriptor;
				if(equiv.HasStringEquivalent)
					return equiv.GetStringEquivalentDescriptor(defaultFormatString);
				else
					return null;
			}
			else
				return null;
		}

		/// <summary>
		/// Gets item properties.
		/// </summary>
		/// <returns></returns>
		public EntityViewProperty[] GetViewPropertiesForFields()
		{
			return this.GetViewPropertiesForMembers(this.Fields.ToArray());
		}

		/// <summary>
		/// Gets item properties.
		/// </summary>
		/// <returns></returns>
		public EntityViewProperty[] GetViewPropertiesForLinks()
		{
			return this.GetViewPropertiesForMembers(this.Links.ToArray());
		}

		/// <summary>
		/// Gets item properties.
		/// </summary>
		/// <returns></returns>
		public EntityViewProperty[] GetViewPropertiesForMembers()
		{
			return this.GetViewPropertiesForMembers(this.GetMembers());
		}

		/// <summary>
		/// Gets item properties.
		/// </summary>
		/// <returns></returns>
		public EntityViewProperty[] GetViewPropertiesForMembers(EntityMember[] members)
		{
			if(members == null)
				throw new ArgumentNullException("members");
			
			// create...
			EntityViewProperty[] results = new EntityViewProperty[members.Length];
			for(int index = 0; index < members.Length; index++)
				results[index] = members[index].GetViewProperty();

			// return...
			return results;
		}

		public override string ToString()
		{
			return string.Format(Cultures.System, "{0}: {1}, {2}", base.ToString(), this.Type.FullName, this.NativeName);
		}

		/// <summary>
		/// Creates a view property for the given expression.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public EntityViewProperty CreateViewProperty(string expression)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			if(expression.Length == 0)
				throw ExceptionHelper.CreateZeroLengthArgumentException("expression");
			
			// is it a field?
			string format = string.Empty;
			if(expression.IndexOf(":") > -1)
			{
				format = expression.Substring(expression.IndexOf(":")+1);
				expression = expression.Substring(0,expression.IndexOf(":"));
			}

			EntityField field = this.Fields[expression];
			if(field != null && format == string.Empty)
				return new EntityFieldViewProperty(field);
			else if (field != null && format != string.Empty)
				return new EntityFieldViewProperty(field,format);

			// defer...
			return EntityViewProperty.GetViewProperty(this, expression);
		}

		/// <summary>
		/// Gets all members for the type.
		/// </summary>
		/// <returns></returns>
		public EntityMember[] GetMembers()
		{
			ArrayList members = new ArrayList(this.Fields);
			members.AddRange(this.Links);
			members.AddRange(this.Properties);

			// return...
			return (EntityMember[])members.ToArray(typeof(EntityMember));
		}

		/// <summary>
		/// Gets the given member.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		internal EntityMember GetMember(EntityMemberName name, OnNotFound onNotFound)
		{ 
			if(name.Name == null)
				throw new InvalidOperationException("'name.Name' is null.");
			if(name.Name.Length == 0)
				throw new InvalidOperationException("'name.Name' is zero-length.");

			// get...
			if((name.MemberType & EntityMemberType.Field) != 0)
				return (EntityField)this.Fields.GetField(name.Name, onNotFound);
			else
				throw new NotSupportedException(string.Format("Cannot handle '{0}'.", name.MemberType));
		}

		/// <summary>
		/// Gets the given member.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public EntityMember GetMember(string name, OnNotFound onNotFound)
		{
			// fields...
			EntityField field = this.Fields.GetField(name, OnNotFound.ReturnNull);
			if(field != null)
				return field;

			// fields...
			EntityLink link = this.Links.GetLink(name, OnNotFound.ReturnNull);
			if(link != null)
				return link;

			// mbr - 2007-04-03 - added support for props...
			EntityProperty prop = this.Properties.GetProperty(name, OnNotFound.ReturnNull);
			if(prop != null)
				return prop;

			// nope...
			switch(onNotFound)
			{
				case OnNotFound.ReturnNull:
					return null;

				case OnNotFound.ThrowException:
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Could not find a member with name '{0}'.", name));

				default:
					throw ExceptionHelper.CreateCannotHandleException(onNotFound);
			}
		}

		/// <summary>
		/// Resets the cache.
		/// </summary>
        //private void ResetCache()
        //{
        //    lock(cacheLock)
        //    {
        //        if (this.CachePolicy != null)
        //            _cache = EntityCache.GetByIdEntityCache(this.CachePolicy, this);
        //        else
        //            _cache = null;
        //    }
        //}

        ///// <summary>
        ///// Returns true if the entity supports caching.
        ///// </summary>
        //public bool HasCache
        //{
        //    get
        //    {
        //        lock(cacheLock)
        //        {
        //            if(this.CachePolicy != null)
        //                return true;
        //            else
        //                return false;
        //        }
        //    }
        //}

		/// <summary>
		/// Gets the cache.
		/// </summary>
        //public EntityCache Cache
        //{
        //    get
        //    {
        //        return _cache;
        //    }
        //}

		/// <summary>
		/// Gets the given entity type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		public static EntityType GetEntityType(object entity, OnNotFound onNotFound = OnNotFound.ThrowException)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");

			// check...
			if(entity is Type)
				return GetEntityType((Type)entity, onNotFound);
			else	
				return GetEntityType(entity.GetType(), onNotFound);
		}

		/// <summary>
		/// Gets the given entity type.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		internal static EntityType GetEntityType(EntityTypeName name, OnNotFound onNotFound)
		{
			if(name.Name == null)
				throw new InvalidOperationException("'name.Name' is null.");
			if(name.Name.Length == 0)
				throw new InvalidOperationException("'name.Name' is zero-length.");
			
			Type type = name.GetClrType();
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// defer...
			return GetEntityType(type, onNotFound);
		}

        public static EntityType GetEntityType<T>(OnNotFound onNotFound = OnNotFound.ThrowException)
        {
            return GetEntityType(typeof(T), onNotFound);
        }

        /// <summary>
        /// Gets the given entity type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onNotFound"></param>
        /// <returns></returns>
        public static EntityType GetEntityType(Type type, OnNotFound onNotFound)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// mbr - 04-10-2007 - case 851 - is this the first time we've been called for a non-bfx type?
			if(type.Assembly != typeof(EntityType).Assembly)
			{
                // interlocked...
                /*				long count = Interlocked.Increment(ref _getEntityTypeRequestCount);
                                if(count == 1)*/

                var doLoad = false;
                lock (_otherAssembliesLock)
                {
                    doLoad = !(OtherAssemblies.Contains(type.Assembly));
                    if (doLoad)
                        OtherAssemblies.Add(type.Assembly);
                }

                // mbr - 2017-06-07 - changed this to load if it's the first time we've seen it...
                if(doLoad)
				{
					// load for attributes...
					LoadFromAttributes(type.Assembly);
				}
			}
			
			// mbr - 26-08-2005 - rejigged to use lookup...
			if(onNotFound == OnNotFound.ReturnNull)
			{
				// do we have it already?
				if(!(EntityTypesLookup.Contains(type)))
					return null;
			}

			// find it...
			//return EntityTypes.GetEntityType(type, onNotFound);
			if(EntityTypesLookup == null)
				throw new InvalidOperationException("EntityTypes is null.");
			EntityType entityType = (EntityType)EntityTypesLookup[type];
			if(entityType == null)
				throw new InvalidOperationException("entityType is null.");
			
			// return...
			return entityType;
		}

		/// <summary>
		/// Gets the complete list of entity types, loading entity types as required
		/// </summary>
		/// <returns></returns>
		// mbr - 04-10-2007 - case 851 - removed.		
		[Obsolete("Do not use this method - it will grovel through the bin folder and load all assemblies, which is sub-optimal.")]
		public static EntityType[] GetAllEntityTypes()
		{
			return LoadFromAttributes();
		}
		
		/// <summary>
		/// Gets the complete list of entity types currently loaded
		/// </summary>
		/// <returns></returns>
		public static EntityType[] GetEntityTypes()
		{
			// mbr - 10-10-2007 - for c7 - skip any non-null values...  these should legitimately be 
			// in the colleciton, but should not be returned from this method as they are just noise.
//			return (EntityType[])EntityTypesLookup.ToArray(typeof(EntityType));
			return (EntityType[])EntityTypesLookup.ToArray(typeof(EntityType), false);
		}

		/// <summary>
		/// Asserts that the given object is an entity.
		/// </summary>
		/// <param name="entity"></param>
		public static void AssertIsEntity(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			AssertIsEntity(entity.GetType());
		}

		/// <summary>
		/// Asserts that the given object is an entity.
		/// </summary>
		/// <param name="entity"></param>
		public static void AssertIsEntity(Type entityType)
		{
			if(entityType == null)
				throw new ArgumentNullException("entityType");
			if(IsEntity(entityType) == false)
				throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Type '{0}' does not represent an entity.", entityType));
		}

		/// <summary>
		/// Returns true if the given object is an entity.
		/// </summary>
		/// <param name="entity"></param>
		public static bool IsEntity(object entity)
		{
			if(entity == null)
				throw new ArgumentNullException("entity");
			return IsEntity(entity.GetType());
		}

		/// <summary>
		/// Returns true if the given object is an entity.
		/// </summary>
		/// <param name="entity"></param>
		public static bool IsEntity(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// mbr - 26-08-2005 - previously, this only checked to see if it had *already been* loaded...
			// what we're doing now is getting the type back.
			//			return EntityTypesLookup.Contains(entityType);

			EntityType entityType = GetEntityType(type, OnNotFound.ReturnNull);
			if(entityType != null)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Gets the entity types.
		/// </summary>
		private static Lookup EntityTypesLookup
		{
			get
			{
				if(_entityTypesLookup == null)
					throw new InvalidOperationException("Metadata subsystem is initializing.");
				return _entityTypesLookup;
			}
		}

		/// <summary>
		/// Gets a collection of EntityProperty objects.
		/// </summary>
		public EntityPropertyCollection Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Gets the name of the database that the entity binds to. If null, this is the default database.
		/// </summary>
		public string DatabaseName
		{
			get
			{
                // mbr - 2014-05-28 - added ability to redirect this...
                if (this.IsSystemTable && Database.SystemDatabaseProvider != null)
                    return Database.SystemDatabaseProvider.GetConnectionSettings().Name;
                else
				    return _databaseName;
			}
            set
            {
                _databaseName = value;
            }
		}

		/// <summary>
		/// Returns true if the type has a database name.
		/// </summary>
		internal bool HasDatabaseName
		{
			get
			{
				if(this.DatabaseName == null || this.DatabaseName.Length == 0)
					return false;
				else
					return true;
			}
		}

		/// <summary>
		/// Puts the entity into the given info for serialization.
		/// </summary>
		internal void Store(SerializationInfo info)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			this.Store(info, DefaultStoredName);
		}
	
		/// <summary>
		/// Puts the entity into the given info for serialization.
		/// </summary>
		internal void Store(SerializationInfo info, string name)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// store...
			info.AddValue(name, this.EntityTypeName);
		}

		/// <summary>
		/// Gets the entity type out of a serialization bucket.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		internal static EntityType Restore(SerializationInfo info, OnNotFound onNotFound)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			return Restore(info, DefaultStoredName, onNotFound);
		}

		/// <summary>
		/// Gets the entity type out of a serialization bucket.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		internal static EntityType Restore(SerializationInfo info, string name, OnNotFound onNotFound)
		{
			if(info == null)
				throw new ArgumentNullException("info");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			EntityTypeName typeName = (EntityTypeName)info.GetValue(name, typeof(EntityTypeName));
			return EntityType.GetEntityType(typeName, onNotFound);
		}

		/// <summary>
		/// Gets the entity type info.
		/// </summary>
		internal EntityTypeName EntityTypeName
		{
			get
			{
				return new EntityTypeName(this);
			}	
		}

		/// <summary>
		/// Gets the persistenceobjects.
		/// </summary>
		private static Lookup PersistenceLookup
		{
			get
			{
				// returns the value...
				if(_persistenceLookup == null)
					throw new InvalidOperationException("Metadata subsystem is initializing.");
				return _persistenceLookup;
			}
		}

		private static void _persistenceLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			EntityType entityType = (EntityType)e.Key;
			if(entityType == null)
				throw new InvalidOperationException("entityType is null.");

			// return...
			//if(PersistenceFactory == null)
			//	throw new InvalidOperationException("PersistenceFactory is null.");
			e.NewValue = Runtime.Current.EntityPersistenceFactory.GetPersistence(entityType.Type.AssemblyQualifiedName);
		}

		/// <summary>
		/// Gets the persistencefactory.
		/// </summary>
		//private static IEntityPersistenceFactory PersistenceFactory
		//{
		//	get
		//	{
		//		// returns the value...
  //              if (_persistenceFactory == null)
  //                  _persistenceFactory = new EntityPersistenceFactory();

		//		// return...
		//		return _persistenceFactory;
		//	}
		//}

		private static void _entityTypesLookup_CreateItemValue(object sender, CreateLookupItemEventArgs e)
		{
			// mbr - 26-08-2005 - try and create an entity type...
			Type type = (Type)e.Key;

			// mbr - 13-10-2005 - added an "initializing stack" that will prevent overloads...
			if(InitializingStack.Contains(type))
				throw new InvalidOperationException(string.Format("The entity for '{0}' is already being loaded.", type));

			// add and load...
			InitializingStack.Add(type);
            try
            {
                e.NewValue = LoadFromAttributes(type, OnNotFound.ReturnNull);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed when initializing '{0}'.", type), ex);
            }
			finally
			{
				InitializingStack.Remove(type);
			}
		}

		/// <summary>
		/// Gets the dialect.
		/// </summary>
		public SqlDialect Dialect
		{
			get
			{
				if(_dialect == null)
				{
					if(!(this.HasDatabaseName))
						_dialect = Database.DefaultDialect;
					else
					{
						// find it...
						using(IConnection connection = Database.CreateNewConnection(this.DatabaseName))
							_dialect = connection.Dialect;
					}
				}
				return _dialect;
			}
		}

        ///// <summary>
        ///// Gets or sets the partitioning strategy to use.
        ///// </summary>
        //// mbr - 02-10-2007 - for c7 - made protected, added setter.		
        //protected internal PartitioningStrategy PartitioningStrategy
        //{
        //    get
        //    {
        //        // mbr - 04-09-2007 - for c7 - provide the ability to demand load this if it's needed.
        //        if(_partitioningStrategy == null && this.PartitioningStrategyField != null)
        //        {
        //            lock(_partitioningStrategyLock)
        //            {
        //                // check...
        //                if(_partitioningStrategy == null)
        //                {
        //                    // check...
        //                    IPartitioningStrategyFactory factory = PartitioningStrategy.StrategyFactory;
        //                    if(factory == null)
        //                        throw new InvalidOperationException("factory is null.");

        //                    // demand load it!
        //                    try
        //                    {
        //                        _partitioningStrategy = factory.CreateStrategy(this.PartitioningStrategyField);
        //                        if(_partitioningStrategy == null)
        //                            throw new InvalidOperationException("_partitioningStrategy is null.");
        //                    }
        //                    catch(Exception ex)
        //                    {
        //                        throw new InvalidOperationException(string.Format("Failed to create partitioning strategy for '{0}' on '{1}' using '{2}'.", 
        //                            this.PartitioningStrategyField, this.FullName, factory), ex);
        //                    }
        //                }
        //            }
        //        }

        //        // return...
        //        return _partitioningStrategy;
        //    }
        //    set
        //    {
        //        lock(_partitioningStrategyLock)
        //        {
        //            _partitioningStrategy = value;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Returns true if the type has a partitioning strategy.
        ///// </summary>
        ///// <returns></returns>
        //public bool SupportsPartitioning
        //{
        //    get
        //    {
        //        // jmm - 18-01-2006 - added the concept of a null provider to turn off paritioning for a service
        //        // that can range accross parititons...
        //        //				if(this.PartitioningStrategy == null)
        //        if(this.PartitioningStrategy == null || !(PartitionIdProvider.PartitioningEnabled))
        //            return false;
        //        else
        //            return true;
        //    }
        //}

		/// <summary>
		/// Gets the initializingstack.
		/// </summary>
		private static IList InitializingStack
		{
			get
			{
				return _initializingStack;
			}
		}

		/// <summary>
		/// Gets a collection of EntityIndex objects.
		/// </summary>
		public EntityIndexCollection Indexes
		{
			get
			{
				return _indexes;
			}
		}

		public string Id
		{
			get
			{
				return this.TypePartiallyQualifiedName;
			}
		}

		public string TypeAssemblyQualifiedName
		{
			get
			{
				if(Type == null)
					throw new InvalidOperationException("Type is null.");
				return this.Type.AssemblyQualifiedName;
			}
		}

		public string TypePartiallyQualifiedName
		{
			get
			{
				if(Type == null)
					throw new InvalidOperationException("Type is null.");
                
                // mbr - 2009-04-15 - moved into helper method on Runtime...
				//return string.Format("{0}, {1}", this.Type.FullName, this.Type.Assembly.GetName().Name);
                return Runtime.GetPartiallyQualifiedNameInternal(this.Type);
			}
		}

		public static EntityType GetEntityTypeForId(string id, OnNotFound onNotFound)
		{
			if(id == null)
				throw new ArgumentNullException("id");
			if(id.Length == 0)
				throw new ArgumentOutOfRangeException("'id' is zero-length.");
			
			// get...
			Type type = Type.GetType(id, false, true);
			if(type != null)
				return EntityType.GetEntityType(type, onNotFound);
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ReturnNull:
						return null;

					case OnNotFound.ThrowException:
						throw new InvalidOperationException(string.Format("An type with name '{0}' was not found.", id));

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}

		
		/// <summary>
		/// Gets or sets the connection string.
		/// </summary>
		public static ExtendedPropertySettings ExtendedPropertySettings
		{
			get
			{
				return _extendedPropertySettings;
			}
		}

		/// <summary>
		/// Ensures the extended properties are saved
		/// </summary>
		public static void SaveExtendedPropertySettings()
		{
			ExtendedPropertiesLock.AcquireReaderLock(-1);
			try
			{
				// save them?
				if(_extendedPropertySettings == null)
					return;

				// mbr - 02-10-2007 - case serializer?
				if(Database.ExtensibilityProvider == null)
					throw new InvalidOperationException("Database.ExtensibilityProvider is null.");
				if(Database.ExtensibilityProvider.UseDefaultSerialization)
				{
					// ensure...
					ConfigItem.EnsureConfigTableExists();

					// find...
					ConfigItem item = ConfigItem.GetByName(ExtendedPropertiesKey);
					if(item == null)
					{
						// mbr - 21-09-2007 - added company name and product name...
						item = new ConfigItem();
						item.CompanyName = Runtime.Current.Application.ProductCompany;
						item.ProductName = Runtime.Current.Application.ProductName;
						item.Name = ExtendedPropertiesKey;
						item.Version = 1;
					}
					else
						item.Version++;

					// set...
					item.Data = ExtendedPropertySettings.ToXml();

					// save...
					item.SaveChanges();

					// set...
					_extendedPropertySettings.Version = item.Version;
				}
				else
				{
					// defer...
					Database.ExtensibilityProvider.SaveConfiguration(_extendedPropertySettings);
				}
			}
			finally
			{
				ExtendedPropertiesLock.ReleaseLock();
			}

			// mbr - 02-10-2007 - for c7 - here is where we now do the ensuring...
			foreach(EntityType et in GetEntityTypes())
			{
				// patch in the extended properties...
				et.LoadExtendedProperties();

				// check...
				if(et.HasExtendedProperties)
					ExtendedPropertySettings.EnsureExtendedTableUpToDate(et);
			}
		}

		/// <summary>
		/// Gets the extendedpropertieslock.
		/// </summary>
		private static ReaderWriterLock ExtendedPropertiesLock
		{
			get
			{
				// returns the value...
				return _extendedPropertiesLock;
			}
		}

		private static void Database_DefaultDatabaseChanged(object sender, EventArgs e)
		{
			ExtendedPropertiesLock.AcquireWriterLock(-1);
			try
			{
				// load...
				ConfigItem item = null; 

				// mbr - 07-12-2005 - do we have a default database, and if so, can we connect to it?
				bool canConnect = false;
				if(Database.HasDefaultDatabaseSettings())
				{
					try
					{
						// check...
						Connection.TestConnection(Database.GetDefaultConnectionSettings());
						
						// set...
						canConnect = true;
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
						canConnect = false;
					}
				}

				// mbr - 02-10-2007 - case 826 - custom?
				if(Database.ExtensibilityProvider == null)
					throw new InvalidOperationException("Database.ExtensibilityProvider is null.");

				// check...
				if(Database.ExtensibilityProvider.UseDefaultSerialization)
				{
					if(canConnect && ConfigItem.DoesTableExist())
						item = ConfigItem.GetByName(ExtendedPropertiesKey);

					// what did we get?
					if(item != null && item.Data != null && item.Data.Length > 0)
					{
						try
						{
							_extendedPropertySettings = ExtendedPropertySettings.FromXml(item.Data);
						}
						catch(Exception ex)
						{
							// mbr - 09-12-2005 - clear it...  (we have to do it explictly as we have half-a-runtime at this point...						
							item.Name = string.Format("{0}_{1}", item.Name, Guid.NewGuid().ToString().Replace("-", string.Empty));
							SqlEntityPersistence persistence = new SqlEntityPersistence(item.EntityType);
							persistence.SaveChanges(item);

							// rethrow...
							throw new InvalidOperationException(string.Format("The extended property settings could not be loaded.  The application must be restarted.  A backup of the settings have been stored in the BfxConfig table under '{0}'.", 
								item.Name), ex);
						}

						// set...
						_extendedPropertySettings.Version = item.Version;
					}
					else
						_extendedPropertySettings = new ExtendedPropertySettings();
				}
				else
				{
					// can we connect?
					if(canConnect)
					{
						// create a new set...
						ExtendedPropertySettings settings = new ExtendedPropertySettings();
						Database.ExtensibilityProvider.LoadConfiguration(settings);

						// set...
						_extendedPropertySettings = settings;
					}
					else
						_extendedPropertySettings = new ExtendedPropertySettings();
				}
			}
			finally
			{			
				ExtendedPropertiesLock.ReleaseLock();
			}

			// walk and reset...
			foreach(EntityType entityType in GetEntityTypes())
				entityType.DefaultDatabaseChanged();
		}

		/// <summary>
		/// Called when the default database changes.
		/// </summary>
		private void DefaultDatabaseChanged()
		{
			// reload the custom properties...
			LoadExtendedProperties();
		}

		/// <summary>
		/// Returns true if the entity type defines extended properties.
		/// </summary>
		public bool HasExtendedProperties
		{
			get
			{
				foreach(EntityField field in this.Fields)
				{
					if(field.IsExtendedProperty)
						return true;
				}

				// nope...
				return false;
			}
		}

		/// <summary>
		/// Gets the extended properties.
		/// </summary>
		/// <returns></returns>
		public EntityField[] GetExtendedProperties()
		{
			ArrayList results = new ArrayList();
			foreach(EntityField field in this.Fields)
			{
				if(field.IsExtendedProperty)
					results.Add(field);
			}

			// return...
			return (EntityField[])results.ToArray(typeof(EntityField));
		}

		/// <summary>
		/// Gets the name of the database table that holds extended fields.
		/// </summary>
		public NativeName NativeNameExtended
		{
			get
			{
				NativeName name = this.NativeName;
				if(name == null)
					throw new InvalidOperationException("name is null.");

				// mbr - 25-09-2007 - provider mangles the name...				
//				string newName = name.Name + "Bfx";
				string newName = Database.ExtensibilityProvider.GetExtendedTableName(name);
				if(newName == null)
					throw new InvalidOperationException("'newName' is null.");
				if(newName.Length == 0)
					throw new InvalidOperationException("'newName' is zero-length.");

				// recreate...
				if(name.HasCatalogName)
					return new NativeName(name.CatalogName, name.UserName, newName);
				else if(name.HasUserName)
					return new NativeName(name.UserName, newName);
				else
					return new NativeName(newName);
			}
		}

		/// <summary>
		/// Ensures that the given name is CLR legal.
		/// </summary>
		/// <param name="name"></param>
		public static void AssertIsLegalIdentifierName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// does it begin alpha?
			bool ok = false;
			if((name[0] >= 'a' && name[0] <= 'z') || (name[0] >= 'A' && name[0] <= 'Z') || name[0] == '_') 
				ok = true;

			// check the rest...
			if(ok)
			{
				foreach(char c in name)
				{
					if(IsLegalForIdenitifer(c))
						break;
				}
			}

			// ok?
			if(!(ok))
				throw new InvalidOperationException(string.Format("The name '{0}' is invalid.  Idenitifiers must begin with a letter, and can only contain 'A-Z', '0-9' and underscores.", name));
		}

		internal static bool IsLegalForIdenitifer(char c)
		{
			if(c >= 'a' && c <= 'z')
				return true;
			else if(c >= 'A' && c <= 'Z')
				return true;
			else if(c >= '0' && c <= '9')
				return true;
			else if(c == '_')
				return true;
			else
				return false;
		}

		public bool UsesDefaultDatabase
		{
			get
			{
				if(this.DatabaseName == null || this.DatabaseName.Length == 0)
					return true;
				else
					return false;
			}
		}

		// mbr - 04-07-2007 - added.		
		/// <summary>
		/// Returns true if the table has auto-increment keys.
		/// </summary>
		public bool HasAutoIncrementKey
		{
			get
			{
				foreach(EntityField field in this.GetKeyFields())
				{
					if(field.IsAutoIncrement)
						return true;
				}

				// nope...
				return false;
			}
		}

		// mbr - 04-07-2007 - added.		
		/// <summary>
		/// Returns true if *this* entity type is dependent on the one provided. 
		/// </summary>
		/// <param name="toCheck"></param>
		/// <returns></returns>
		/// <remarks>This will check the child-to-parent links on <c>toCheck</c> and return false if any reference
		/// this type.</remarks>
		public bool IsParentOf(EntityType toCheck)
		{
			if(toCheck == null)
				throw new ArgumentNullException("toCheck");
			if(toCheck == this)
				throw new ArgumentException("Cannot run this method against itself.");

			// walk...
			foreach(ChildToParentEntityLink link in toCheck.Links)
			{
				if(link.ParentEntityType == this)
					return true;
			}

			// nope...
			return false;
		}

        private static List<EntityType> SortByDependency(List<EntityType> entityTypes)
        {
            if(entityTypes == null)
	            throw new ArgumentNullException("entityTypes");
            if(entityTypes.Count == 0)
	            throw new ArgumentException("'entityTypes' is zero-length.");

            // get...
            EntityType[] results = SortByDependency(entityTypes.ToArray());
            if (results == null)
                throw new InvalidOperationException("'results' is null.");
            if (results.Length == 0)
                throw new InvalidOperationException("'results' is zero-length.");

            // return...
            return new List<EntityType>(results);
        }

		/// <summary>
		/// Sorts the given entities by dependency order, giving the least dependent first.
		/// </summary>
		/// <param name="ets"></param>
		/// <returns></returns>
		public static EntityType[] SortByDependency(EntityType[] entityTypes)
		{
			if(entityTypes == null)
				throw new ArgumentNullException("entityTypes");
			if(entityTypes.Length == 0)
				throw new ArgumentException("'entityTypes' is zero-length.");

			// do we have just one?
			if(entityTypes.Length == 1)
				return (EntityType[])entityTypes.Clone();

			// create...
			ArrayList ets = new ArrayList(entityTypes);
			int maxIterations = entityTypes.Length * 100;
			int iteration = 0;
			while (true)
			{
				// walk...
				bool changed = false;
				for (int i = 1; i < ets.Count; i++)
				{
					EntityType scanI = (EntityType)ets[i];
					for (int j = 0; j < i; j++)
					{
						EntityType scanJ = (EntityType)ets[j];
						if (scanJ.IsParentOf(scanI))
						{
							// swap them and restart...
							ets[i] = scanJ;
							ets[j] = scanI;

							// stop...
							changed = true;
							break;
						}
					}

					// changed?
					if (changed)
						break;
				}

				// nope...
				if (!(changed))
					break;

				// stop?
                iteration++;
				if(iteration == maxIterations)
					throw new InvalidOperationException("Maximum number of iterations reached.  A circular reference may exist within the set of entities that you have provided.");
			}

			// return...
			return (EntityType[])ets.ToArray(typeof(EntityType));
		}

		/// <summary>
		/// Gets the types that are dependent on this type - i.e. the children of this type.
		/// </summary>
		/// <returns></returns>
		public EntityType[] GetChildEntityTypes(bool walkAllDescendents)
		{
			return this.GetChildParentEntityTypes(walkAllDescendents, true);
		}

		/// <summary>
		/// Gets the types that are dependent on this type - i.e. the parents of this type.
		/// </summary>
		/// <returns></returns>
		public EntityType[] GetParentEntityTypes(bool walkAllAntecedents)
		{
			return this.GetChildParentEntityTypes(walkAllAntecedents, false);
		}

		/// <summary>
		/// Gets the types that are dependent on this type - i.e. the children of this type.
		/// </summary>
		/// <returns></returns>
		private EntityType[] GetChildParentEntityTypes(bool walkAllDescendents, bool child)
		{
			ArrayList results = new ArrayList();
			results.Add(this);

			// mbr - 04-10-2007 - case 851 - consolidated this call...			
//			ArrayList ets = new ArrayList(GetAllEntityTypes());
			ArrayList ets = new ArrayList(GetEntityTypes());
			if(!(ets.Contains(this)))
				throw new InvalidOperationException("Candidate list does not contain source entity type.");
			ets.Remove(this);

			// walk...
			int index = 0;
			while(index < results.Count)
			{
				EntityType current = (EntityType)results[index];

				// walk all the types...
				foreach(EntityType et in ets)
				{
					// have we already found it?
					if(!(results.Contains(et)))
					{
						bool match = false;
						if(child && current.IsParentOf(et))
							match = true;
						else if(!(child) && et.IsParentOf(current))
							match = true;

						// ok?
						if(match)
							results.Add(et);
					}
				}

				// stop?
				if(!(walkAllDescendents))
					break;

				// next...
				index++;
			}

			// remove the zeroth...
			results.RemoveAt(0);

			// return...
			return (EntityType[])results.ToArray(typeof(EntityType));
		}

		/// <summary>
		/// Gets the haspartitioningstrategy.
		/// </summary>
		// mbr - 04-09-2007 - for c7 - added.		
		private EntityField PartitioningStrategyField
		{
			get
			{
				return _partitioningStrategyField;
			}
		}

		/// <summary>
		/// Gets the entitytypefactory.
		/// </summary>
		private static IEntityTypeFactory EntityTypeFactory
		{
			get
			{
				return _entityTypeFactory;
			}
		}

		/// <summary>
		/// Raises the <c>AfterLoad</c> event.
		/// </summary>
		private void OnAfterLoad()
		{
			OnAfterLoad(EventArgs.Empty);
		}
		
		/// <summary>
		/// Raises the <c>AfterLoad</c> event.
		/// </summary>
		protected virtual void OnAfterLoad(EventArgs e)
		{
			// raise...
			if(AfterLoad != null)
				AfterLoad(this, e);
		}

		/// <summary>
		/// Gets or sets the ruleset
		/// </summary>
		public EntityRuleSet RuleSet
		{
			get
			{
				return _ruleSet;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _ruleSet)
				{
					// set the value...
					_ruleSet = value;
				}
			}
		}

        internal static EntityType GetEntityTypeByDtoType(Type type, bool throwIfNotFound)
        {
            while (type != null)
            {
                foreach (var et in GetEntityTypes())
                {
                    if (et.DtoType != null && type.IsAssignableFrom(et.DtoType.Type))
                        return et;
                }

                // up...
                type = type.BaseType;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException(string.Format("An entity type for DTO type '{0}' was not found.", type));
            else
                return null;
        }
    }
}
