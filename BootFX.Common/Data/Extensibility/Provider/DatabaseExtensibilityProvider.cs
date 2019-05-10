// BootFX - Application framework for .NET applications
// 
// File: DatabaseExtensibilityProvider.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using BootFX.Common;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common.Data
{
	/// <summary>
	/// Base class for database extensibility.
	/// </summary>
	public abstract class DatabaseExtensibilityProvider : Loggable
	{
		public const int MaxNativeNameLength = 128;

		/// <summary>
		/// Constructor.
		/// </summary>
		protected DatabaseExtensibilityProvider()
		{
		}

		/// <summary>
		/// Ensures that the extended properties table has been created.
		/// </summary>
		/// <param name="entityType"></param>
		public abstract void EnsureExtendedTableUpToDate(EntityType entityType);

		/// <summary>
		/// Gets the extended table name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public abstract string GetExtendedTableName(NativeName name);

		/// <summary>
		/// Append SQL to select the extended properties from the extended table for the entity
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		/// <param name="field"></param>
		public abstract void AddExtendedPropertyToSelectStatement(SqlStatementCreator creator, SqlStatement statement, StringBuilder builder, 
			EntityField field);

		/// <summary>
		/// Asserts that the definition is valid.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="name"></param>
		/// <param name="nativeName"></param>
		/// <param name="type"></param>
		/// <param name="size"></param>
		/// <param name="multiValue"></param>
		public virtual void AssertDefinition(EntityType entityType, string name, string nativeName, ExtendedPropertyDataType dataType, long size, 
			bool multiValue)
		{
		}

		/// <summary>
		/// Returns true if the property is being used.
		/// </summary>
		/// <param name="entityType"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public abstract bool IsPropertyInUse(EntityType entityType, string name);

		/// <summary>
		/// Gets the insert work units.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="et"></param>
		/// <param name="modifiedFields"></param>
		/// <returns></returns>
		public abstract IWorkUnit[] GetInsertWorkUnits(object entity, EntityType et, EntityField[] modifiedFields);

		/// <summary>
		/// Gets the work units to use with an update.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="et"></param>
		/// <param name="modifiedFields"></param>
		/// <returns></returns>
		public abstract IWorkUnit[] GetUpdateWorkUnits(object entity, EntityType et, EntityField[] modifiedFields);

		/// <summary>
		/// Gets the delete work units.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="et"></param>
		/// <param name="modifiedFields"></param>
		/// <returns></returns>
		public abstract IWorkUnit[] GetDeleteWorkUnits(object entity, EntityType et, EntityField[] modifiedFields);

		/// <summary>
		/// Appends joins to the statement.
		/// </summary>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		public abstract void AppendJoinsToSelectStatement(SqlStatement statement, StringBuilder builder);

		/// <summary>
		/// Appends the column name for a field constraint.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="statement"></param>
		/// <param name="builder"></param>
		/// <param name="field"></param>
		public virtual void AppendColumnNameForEntityFieldFilterConstraint(FilterConstraintAppendContext context, 
			EntityFieldFilterConstraint constraint, SqlStatement statement, StringBuilder builder, EntityField field)
		{
			// defer...
			this.AddExtendedPropertyToSelectStatement(context.Creator, statement, builder, field);
		}

		/// <summary>
		/// Adds schema tables to the system.
		/// </summary>
		/// <param name="et"></param>
		/// <param name="type">The type related to the entity type.  This is provided because in some instances
		/// the the type loaded in is not the right type and needs to reloaded.</param>
		/// <param name="entitySchema"></param>
		public virtual void AddSchemaTables(EntityType et, Type type, SqlTable coreTable, SqlSchema entitySchema)
		{
			if(et == null)
				throw new ArgumentNullException("et");
			if(type == null)
				throw new ArgumentNullException("type");
			if(coreTable == null)
				throw new ArgumentNullException("coreTable");		
			if(entitySchema == null)
				throw new ArgumentNullException("entitySchema");
			
			// no-op by default.
		}

		/// <summary>
		/// Gets whether to use the default serialization method.
		/// </summary>
		public virtual bool UseDefaultSerialization
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Saves configuration to whichever store the application wishes to use.
		/// </summary>
		public virtual void SaveConfiguration(ExtendedPropertySettings settings)
		{
			// this should never be called.  inheriting classes need to override UseDefaultSerialization, this method and the load method.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		/// <summary>
		/// Loads configuration from whichever store the application wishes to use.
		/// </summary>
		public virtual void LoadConfiguration(ExtendedPropertySettings settings)
		{
			// this should never be called.  inheriting classes need to override UseDefaultSerialization, this method and the save method.
			throw new NotImplementedException(string.Format("Not implemented for '{0}'.", this.GetType()));
		}

		/// <summary>
		/// Gets replacement work units.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="unit"></param>
		/// <returns></returns>
		// mbr - 29-11-2007 - for c7 - added.
		internal IWorkUnit[] GetReplacementWorkUnits(IExtendedDataWorkUnit unit)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");
			
			// sql?
			if(unit.EntityType == null)
				throw new InvalidOperationException("unit.EntityType is null.");
			if(unit.EntityType.Persistence == null)
				throw new InvalidOperationException("unit.EntityType.Persistence is null.");
			if(!(unit.EntityType.Persistence is SqlEntityPersistence))
				throw new NotSupportedException(string.Format("A persistence service of '{0}' is not supported.", unit.EntityType.Persistence.GetType()));

			// get the modified fields out of the entity...
			EntityField[] modifiedFields = ((SqlEntityPersistence)unit.EntityType.Persistence).GetModifiedExtendedFieldsExcludingKeys(unit.Entity);
			if(modifiedFields == null)
				throw new InvalidOperationException("'modifiedFields' is null.");

			// the rules go that to create an extended work unit you need fields, so if you get here, something has
			// gone quite badly wrong.
			if(modifiedFields.Length == 0)
				throw new InvalidOperationException("'modifiedFields' is zero-length.");

			// get...
			switch(unit.WorkUnitType)
			{
				case WorkUnitType.Insert:
					return this.GetInsertWorkUnits(unit.Entity, unit.EntityType, modifiedFields);

				case WorkUnitType.Update:
					return this.GetUpdateWorkUnits(unit.Entity, unit.EntityType, modifiedFields);

				case WorkUnitType.Delete:
					return this.GetDeleteWorkUnits(unit.Entity, unit.EntityType, modifiedFields);

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", unit.WorkUnitType, unit.WorkUnitType.GetType()));
			}		
		}
	}
}
