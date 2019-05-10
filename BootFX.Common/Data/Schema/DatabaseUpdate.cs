// BootFX - Application framework for .NET applications
// 
// File: DatabaseUpdate.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;
using BootFX.Common.Management;
using System.Collections.Generic;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Summary description for DatabaseUpdate.
	/// </summary>
	public class DatabaseUpdate : Loggable
	{
		/// <summary>
		/// Private field to hold the singleton instance.
		/// </summary>
		private static DatabaseUpdate _current = null;
		
		/// <summary>
		/// Private field for property <c>AssemblyPath </c>.
		/// </summary>
		private string _assemblyPath;

		/// <summary>
		/// Private field for property <c>Assembly </c>.
		/// </summary>
		private ArrayList _assemblies = new ArrayList();

        public event EventHandler Updated;

		//		/// <summary>
		//		/// Private field for property <c>DatabaseSchema </c>.
		//		/// </summary>
		//		private SqlSchema _databaseSchema ;
		//
		//		/// <summary>
		//		/// Private field for property <c>EntitySchema </c>.
		//		/// </summary>
		//		private SqlSchema _entitySchema ;
		private DatabaseUpdate()
		{
			_assemblyPath = new FileInfo(new Uri(Assembly.GetCallingAssembly().CodeBase).LocalPath).DirectoryName;
		}

		/// <summary>
		/// Static constructor.
		/// </summary>
		static DatabaseUpdate()
		{
			_current = new DatabaseUpdate();
		}
		
		/// <summary>
		/// Gets the singleton instance.
		/// </summary>
		public static DatabaseUpdate Current
		{
			get
			{
				return _current;
			}
		}

//		/// <summary>
//		/// Constructor for database update that uses only the specified assembly
//		/// </summary>
//		/// <param name="asm"></param>
//		private DatabaseUpdate(Assembly asm)
//		{
//			_assembly = asm;
//		}

		/// <summary>
		/// Gets or sets the AssemblyPath .
		/// </summary>
		public string AssemblyPath 
		{
			get
			{
				return _assemblyPath;
			}
			set
			{
				_assemblyPath  = value;
			}
		}

		/// <summary>
		/// Gets or sets the Assembly .
		/// </summary>
		public IList Assemblies
		{
			get
			{
				return _assemblies;
			}
		}

		/// <summary>
		/// Creates a database.
		/// </summary>
		/// <param name="operation"></param>
		[Obsolete("Use the version that takes a DatabseUpdateArgs argument.")]
		public void Create(IOperationItem operation)
		{
			this.Create(operation, new DatabaseUpdateArgs());
		}

		/// <summary>
		/// Creates a database.
		/// </summary>
		/// <param name="operation"></param>
		public void Create(IOperationItem operation, DatabaseUpdateArgs args)
		{
			this.CreateUpdateDatabase(operation, true, false, args);
		}

		/// <summary>
		/// Updates a database.
		/// </summary>
		/// <param name="operation"></param>
		[Obsolete("Use the version that takes a DatabseUpdateArgs argument.")]
		public void Update(IOperationItem operation)
		{
			this.Update(operation, new DatabaseUpdateArgs());
		}

		/// <summary>
		/// Updates a database.
		/// </summary>
		/// <param name="operation"></param>
		public DatabaseUpdateCheckResults Update(IOperationItem operation, DatabaseUpdateArgs args)
		{
			return this.CreateUpdateDatabase(operation, false, false, args);
		}

		/// <summary>
		/// Checks to see what needs to be done in order to update the database.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		[Obsolete("Use the version that takes a DatabseUpdateArgs argument.")]
		public DatabaseUpdateCheckResults Check(IOperationItem operation)
		{
			return this.Check(operation, new DatabaseUpdateArgs());
		}

		/// <summary>
		/// Checks to see what needs to be done in order to update the database.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		public DatabaseUpdateCheckResults Check(IOperationItem operation, DatabaseUpdateArgs args)
		{
			return this.CreateUpdateDatabase(operation, false, true, args);
		}

		private void MergeEntityTypes(IList a, IList b)
		{
			if(a == null)
				throw new ArgumentNullException("a");
			if(b == null)
				throw new ArgumentNullException("b");
			
			// add...
			foreach(EntityType toAdd in b)
			{
				bool found = false;
				foreach(EntityType check in a)
				{
					if(string.Compare(check.FullName, toAdd.FullName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					{
						found = true;
						break;
					}
				}

				// not...
				if(!(found))
					a.Add(toAdd);
			}
		}

		/// <summary>
		/// Gets the schema to use.
		/// </summary>
		/// <param name="args"></param>
		/// <returns></returns>
		private SqlSchema GetSchema(DatabaseUpdateArgs args)
		{
			if(args == null)
				throw new ArgumentNullException("args");
		
			// mbr - 02-10-2007 - build a search spec...
            GetSchemaArgs schemaArgs = new GetSchemaArgs()
            {
                DatabaseName = args.DatabaseName
            };
			foreach(EntityType et in args.LimitEntityTypes)
			{
				schemaArgs.ConstrainTableNames.Add(et.NativeName.Name);
				schemaArgs.ConstrainTableNames.Add(et.NativeNameExtended.Name);
			}

			// get the schema...
			SqlSchema schema = Database.GetSchema(schemaArgs);
			if(schema == null)
				throw new InvalidOperationException("schema is null.");

			// return...
			return schema;
		}

		/// <summary>
		/// Creates ot updates the database.
		/// </summary>
		/// <param name="create"></param>
		/// <param name="checkOnly"></param>
		// mbr - 28-09-2007 - case 814 - added args.		
		private DatabaseUpdateCheckResults CreateUpdateDatabase(IOperationItem operation, bool create, bool checkOnly,
			DatabaseUpdateArgs args)
		{
			if(args == null)
				throw new ArgumentNullException("args");

			// operation...
			if(operation == null)
				operation = new OperationItem();
			
			// set...
			operation.Status = "Loading existing schema...";

			// database...
			SqlSchema databaseSchema = null;
			if(create)
				databaseSchema = new SqlSchema();
			else
			{
                if (args.Trace)
                    this.LogInfo(() => "Loading schema...");

				// mbr - 28-09-2007 - case 814 - defer to a method that can limit the types...				
//				databaseSchema = Database.GetSchema();fic
				databaseSchema = this.GetSchema(args);
				if(databaseSchema == null)
					throw new InvalidOperationException("databaseSchema is null.");
			}

			// Create an entity schema
			SqlSchema entitySchema = new SqlSchema();

			// Load all the entity types from the path
			ArrayList entityTypes = new ArrayList();

			// mbr - 02-10-2007 - for c7 - changed so that we can limit the entity types...
			if(args.LimitEntityTypes.Count == 0)
			{
				// mbr - 04-10-2007 - case 851 - do not do this behaviour (oddly both of these statements are
				// actually identical), 
//				entityTypes.AddRange(EntityType.GetAllEntityTypes());
//				this.MergeEntityTypes(entityTypes, EntityType.LoadFromAttributes(AssemblyPath));
//				foreach(Assembly asm in this.Assemblies)
//					this.MergeEntityTypes(entityTypes, EntityType.LoadFromAttributes(asm));

				// load...
				this.MergeEntityTypes(entityTypes, EntityType.GetEntityTypes());
			}
			else
				this.MergeEntityTypes(entityTypes, args.LimitEntityTypes);

            // log...
            if (args.Trace)
                this.LogInfo(() => string.Format("Found '{0}' entity types.", entityTypes.Count));

			// steps...
			DatabaseUpdateStepCollection steps = new DatabaseUpdateStepCollection();

            if (args.AddArrayParameterUdts)
                steps.Add(new AddArrayParameterUdtsUpdateStep());

            SyncSchemaDatabaseUpdateStep syncStep = new SyncSchemaDatabaseUpdateStep();
			steps.Add(syncStep);

			// mbr - 02-03-2006 - we can have update steps that don't have an entity type...
			TypeFinder finder = new TypeFinder(typeof(DatabaseUpdateStep));
			finder.AddAttributeSpecification(typeof(DatabaseUpdateStepAttribute), false);
			Type[] stepTypes = finder.GetTypes();
			if(stepTypes == null)
				throw new InvalidOperationException("stepTypes is null.");

			// mbr - 02-10-2007 - for c7...			
			if(Database.ExtensibilityProvider == null)
				throw new InvalidOperationException("Database.ExtensibilityProvider is null.");

			// Walk each entity type and add to entitySchema
			foreach(EntityType entityType in entityTypes)
			{
				// mbr - 14-12-2005 - should we do it?
				bool skip = entityType.Type.IsDefined(typeof(SkipDatabaseUpdateAttribute), false);

                // mbr - 2010-01-29 - changed the log here to allow db update to do named databases...
//				if(!(skip) && entityType.UsesDefaultDatabase)
                bool ok = false;
                if (!(skip))
                {
                    // no named database, and entity has no named database...
                    if (!(args.HasDatabaseName) && entityType.UsesDefaultDatabase)
                        ok = true;
                    else if (args.HasDatabaseName && string.Compare(entityType.DatabaseName, args.DatabaseName, true, Cultures.System) == 0)
                        ok = true;
                }
                else
                    ok = false;

                // do we do it?
                if (ok)
                {
                    if (args.Trace)
                        this.LogInfo(() => string.Format("Touching '{0}' ({1})...", entityType.Name, entityType.Type.Assembly.GetName().Name));

                    // add the base table...
                    SqlTable coreTable = SqlTable.GetTable(entitySchema, entityType);
                    if (coreTable == null)
                        throw new InvalidOperationException("coreTable is null.");
                    entitySchema.Tables.Add(coreTable);

                    // type...
                    Type type = entityType.Type;
                    if (type == null)
                        throw new InvalidOperationException("type is null.");

                    // reload it - something weird happens with these and they can't be used with the metadata so reload it so that we're
                    // certain we have the right ones...
                    type = Type.GetType(type.AssemblyQualifiedName, true, true);
                    if (type == null)
                        throw new InvalidOperationException("type is null.");

                    // mbr - 02-10-2007 - for c7 - add other tables that we need...
                    Database.ExtensibilityProvider.AddSchemaTables(entityType, type, coreTable, entitySchema);

                    // add the custom units...
                    foreach (Type stepType in stepTypes)
                    {
                        // get the attribute...
                        DatabaseUpdateStepAttribute[] attrs = (DatabaseUpdateStepAttribute[])stepType.GetCustomAttributes(typeof(DatabaseUpdateStepAttribute), true);
                        if (attrs == null)
                            throw new InvalidOperationException("attrs is null.");

                        // walk...
                        foreach (DatabaseUpdateStepAttribute attr in attrs)
                        {
                            if (attr.EntityType != null && attr.EntityType.IsAssignableFrom(type))
                            {
                                // create...
                                DatabaseUpdateStep step = (DatabaseUpdateStep)Activator.CreateInstance(stepType, new object[] { type });
                                if (step == null)
                                    throw new InvalidOperationException("step is null.");

                                // add..
                                steps.Add(step);
                            }
                        }
                    }
                }
                else
                {
                    if (args.Trace)
                        this.LogInfo(() => string.Format("Skipping '{0}'.", entityType.Name));
                }
            }

			// mbr - 02-10-2007 - for c7 - don't do custom steps if we're limiting entity types...
			if(args.LimitEntityTypes.Count == 0)
			{
				// do the ones that don't have entity types...
				foreach(Type stepType in stepTypes)
				{
					// get the attribute...
					DatabaseUpdateStepAttribute[] attrs = (DatabaseUpdateStepAttribute[])stepType.GetCustomAttributes(typeof(DatabaseUpdateStepAttribute), true);
					if(attrs == null)
						throw new InvalidOperationException("attrs is null.");

					// walk...
					foreach(DatabaseUpdateStepAttribute attr in attrs)
					{
						if(attr.EntityType == null)
						{
							// create...
							DatabaseUpdateStep step = (DatabaseUpdateStep)Activator.CreateInstance(stepType);
							if(step == null)
								throw new InvalidOperationException("step is null.");

                            // add..
							steps.Add(step);
						}
					}
				}
			}

			// get the work units...
			operation.Status = "Creating schema delta...";

			// mbr - 02-10-2007 - for c7 - changed to deferral.			
//			syncStep.WorkUnits.AddRange(entitySchema.GetSchemaWorkUnits(databaseSchema, operation));
			syncStep.Initialize(entitySchema, databaseSchema);

            // run...
            if (!(checkOnly))
            {
                if (args.Trace)
                    this.LogInfo(() => string.Format("Applying '{0}' steps...", steps.Count));

                // context...
                DatabaseUpdateContext context = new DatabaseUpdateContext(operation, args.Trace);

                // mbr - 21-12-2005 - run the steps...
                foreach (DatabaseUpdateStep step in steps)
                {
                    try
                    {
                        if (args.Trace)
                            this.LogInfo(() => string.Format("Applying step: {0}", step));

                        // set...
                        operation.Status = string.Format("Running step '{0}'...", step);
                        step.Execute(context);
                    }
                    catch (Exception ex)
                    {
                        // log...
                        string message = string.Format("Failed database update when running step '{0}'.", step);
                        if (this.Log.IsErrorEnabled)
                            this.Log.Error(message, ex);

                        // throw...
                        throw new InvalidOperationException(message, ex);
                    }
                }
            }
            else
            {
                if (args.Trace)
                    this.LogInfo(() => "Checking only -- not doing work.");
            }

            if (args.Trace)
                this.LogInfo(() => "Database update finished.");

            this.OnUpdated();

			// return...
			return new DatabaseUpdateCheckResults(steps);
		}

        private void OnUpdated()
        {
            if (this.Updated != null)
                this.Updated(this, EventArgs.Empty);
        }

        //        internal static void DebugSupportNeeds(IConnection connection)
        //        {
        //            if (connection == null)
        //                connection = Database.CreateConnection();

        //            // test...
        //            if (connection.DoesTableExist("supportneeds"))
        //            {
        //                var names = new List<string>(connection.ExecuteValuesVerticalString(new SqlStatement(@"select distinct name from sys.objects where object_id in 
        //                                    (   select fk.constraint_object_id from sys.foreign_key_columns as fk
        //                                        where fk.parent_object_id = 
        //    	                                    (select object_id from sys.tables where name = 'supportneeds')
        //                                    )")));
        //                if (names.Count < 11)
        //                    throw new InvalidOperationException("Gone?");
        //            }
        //        }
    }
}
