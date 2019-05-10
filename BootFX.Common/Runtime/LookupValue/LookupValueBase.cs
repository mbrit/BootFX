// BootFX - Application framework for .NET applications
// 
// File: LookupValueBase.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common.Data
{
    using System;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Data;
    using System.Collections;
    using System.Collections.Specialized;
    using BootFX.Common;
    using BootFX.Common.Data;
    using BootFX.Common.Entities;
    using BootFX.Common.Entities.Attributes;
    
    
    /// <summary>
    /// Defines the base entity type for 'LookupValue'.
    /// </summary>
    /// <remarks>
    /// This entity maps to the 'LookupValuesBFX' table.  It has a minimum size of 518 byte(s) and 0 large column(s).
    /// </remarks>
    [Serializable()]
    public abstract class LookupValueBase : BootFX.Common.Entities.Entity
    {
        
        /// <summary>
        /// Constructor.
        /// </summary>
        protected LookupValueBase()
        {
        }
        
        /// <summary>
        /// Deserialization constructor.
        /// </summary>
        protected LookupValueBase(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : 
                base(info, context)
        {
        }
        
        /// <summary>
        /// Gets or sets the value for 'LookupId'.
        /// </summary>
        /// <remarks>
        /// This property maps to the 'LookupId' column.
        /// </remarks>
        [EntityField("LookupId", System.Data.DbType.Int32, ((BootFX.Common.Entities.EntityFieldFlags.Key | BootFX.Common.Entities.EntityFieldFlags.Common) 
                    | BootFX.Common.Entities.EntityFieldFlags.AutoIncrement))]
        public int LookupId
        {
            get
            {
                return ((int)(this["LookupId"]));
            }
            set
            {
                this["LookupId"] = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the value for 'Name'.
        /// </summary>
        /// <remarks>
        /// This property maps to the 'Name' column.
        /// </remarks>
        [EntityField("Name", System.Data.DbType.String, BootFX.Common.Entities.EntityFieldFlags.Common, 255)]
        public string Name
        {
            get
            {
                return ((string)(this["Name"]));
            }
            set
            {
                this["Name"] = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the value for 'Description'.
        /// </summary>
        /// <remarks>
        /// This property maps to the 'Description' column.
        /// </remarks>
        [EntityField("Description", System.Data.DbType.String, (BootFX.Common.Entities.EntityFieldFlags.Nullable | BootFX.Common.Entities.EntityFieldFlags.Common), 255)]
        public string Description
        {
            get
            {
                return ((string)(this["Description"]));
            }
            set
            {
                this["Description"] = value;
            }
        }
        
        /// <summary>
        /// Gets or sets the value for 'Value'.
        /// </summary>
        /// <remarks>
        /// This property maps to the 'Value' column.
        /// </remarks>
        [EntityField("Value", System.Data.DbType.Int32, BootFX.Common.Entities.EntityFieldFlags.Common)]
        public int Value
        {
            get
            {
                return ((int)(this["Value"]));
            }
            set
            {
                this["Value"] = value;
            }
        }
        
        /// <summary>
        /// Creates and instance of 'LookupValue'.
        /// </summary>
        public static LookupValue CreateInstance(string name, int value)
        {
            LookupValue entity = new LookupValue();
            entity.Name = name;
            entity.Value = value;
            entity.SaveChanges();
            return entity;
        }
        
        /// <summary>
        /// Sets properties on an instance of 'LookupValue'.
        /// </summary>
        public static void SetProperties(int lookupId, string[] propertyNames, object[] propertyValues)
        {
            LookupValue entity = BootFX.Common.Data.LookupValue.GetById(lookupId);
            entity.SetProperties(entity, propertyNames, propertyValues);
            entity.SaveChanges();
        }
        
        /// <summary>
        /// Get all <see cref="LookupValue"/> entities.
        /// </summary>
        public static LookupValueCollection GetAll()
        {
            BootFX.Common.Data.SqlFilter filter = BootFX.Common.Data.SqlFilter.CreateGetAllFilter(typeof(LookupValue));
            return ((LookupValueCollection)(filter.ExecuteEntityCollection()));
        }
        
        /// <summary>
        /// Gets the <see cref="LookupValue"/> entity with the given ID.
        /// </summary>
        /// <bootfx>
        /// CreateEntityFilterEqualToMethod - LookupValue
        /// </bootfx>
        public static LookupValue GetById(int lookupId)
        {
            return BootFX.Common.Data.LookupValue.GetById(lookupId, BootFX.Common.Data.SqlOperator.EqualTo);
        }
        
        /// <summary>
        /// Gets the <see cref="LookupValue"/> entity where the ID matches the given specification.
        /// </summary>
        public static LookupValue GetById(int lookupId, BootFX.Common.Data.SqlOperator lookupIdOperator)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("LookupId", lookupIdOperator, lookupId);
            return ((LookupValue)(filter.ExecuteEntity()));
        }
        
        /// <summary>
        /// Gets the <see cref="LookupValue"/> entity with the given ID.
        /// </summary>
        /// <bootfx>
        /// CreateEntityFilterEqualToMethod - LookupValue
        /// </bootfx>
        public static LookupValue GetById(int lookupId, BootFX.Common.OnNotFound onNotFound)
        {
            return BootFX.Common.Data.LookupValue.GetById(lookupId, BootFX.Common.Data.SqlOperator.EqualTo, onNotFound);
        }
        
        /// <summary>
        /// Gets the <see cref="LookupValue"/> entity where the ID matches the given specification.
        /// </summary>
        public static LookupValue GetById(int lookupId, BootFX.Common.Data.SqlOperator lookupIdOperator, BootFX.Common.OnNotFound onNotFound)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("LookupId", lookupIdOperator, lookupId);
            LookupValue results = ((LookupValue)(filter.ExecuteEntity()));
            return results;
        }
        
        /// <summary>
        /// Gets entities where Name is equal to the given value.
        /// </summary>
        /// <bootfx>
        /// CreateEntityFilterEqualToMethod - LookupValue
        /// </bootfx>
        /// <remarks>
        /// Created for column <c>Name</c>
        /// </remarks>
        public static LookupValueCollection GetByName(string name)
        {
            return BootFX.Common.Data.LookupValue.GetByName(name, BootFX.Common.Data.SqlOperator.EqualTo);
        }
        
        /// <summary>
        /// Gets entities where Name matches the given specification.
        /// </summary>
        /// <remarks>
        /// Created for column <c>Name</c>
        /// </remarks>
        public static LookupValueCollection GetByName(string name, BootFX.Common.Data.SqlOperator nameOperator)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("Name", nameOperator, name);
            return ((LookupValueCollection)(filter.ExecuteEntityCollection()));
        }
        
        /// <summary>
        /// Gets entities where Name matches the given specification.
        /// </summary>
        /// <remarks>
        /// Created for column <c>Name</c>
        /// </remarks>
        public static LookupValueCollection GetByName(string name, BootFX.Common.Data.SqlOperator nameOperator, BootFX.Common.OnNotFound onNotFound)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("Name", nameOperator, name);
            LookupValueCollection results = ((LookupValueCollection)(filter.ExecuteEntityCollection()));
            return results;
        }
        
        /// <summary>
        /// Gets entities where Description is equal to the given value.
        /// </summary>
        /// <bootfx>
        /// CreateEntityFilterEqualToMethod - LookupValue
        /// </bootfx>
        /// <remarks>
        /// Created for column <c>Description</c>
        /// </remarks>
        public static LookupValueCollection GetByDescription(string description)
        {
            return BootFX.Common.Data.LookupValue.GetByDescription(description, BootFX.Common.Data.SqlOperator.EqualTo);
        }
        
        /// <summary>
        /// Gets entities where Description matches the given specification.
        /// </summary>
        /// <remarks>
        /// Created for column <c>Description</c>
        /// </remarks>
        public static LookupValueCollection GetByDescription(string description, BootFX.Common.Data.SqlOperator descriptionOperator)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("Description", descriptionOperator, description);
            return ((LookupValueCollection)(filter.ExecuteEntityCollection()));
        }
        
        /// <summary>
        /// Gets entities where Description matches the given specification.
        /// </summary>
        /// <remarks>
        /// Created for column <c>Description</c>
        /// </remarks>
        public static LookupValueCollection GetByDescription(string description, BootFX.Common.Data.SqlOperator descriptionOperator, BootFX.Common.OnNotFound onNotFound)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("Description", descriptionOperator, description);
            LookupValueCollection results = ((LookupValueCollection)(filter.ExecuteEntityCollection()));
            return results;
        }
        
        /// <summary>
        /// Gets entities where Value is equal to the given value.
        /// </summary>
        /// <bootfx>
        /// CreateEntityFilterEqualToMethod - LookupValue
        /// </bootfx>
        /// <remarks>
        /// Created for column <c>Value</c>
        /// </remarks>
        public static LookupValueCollection GetByValue(int value)
        {
            return BootFX.Common.Data.LookupValue.GetByValue(value, BootFX.Common.Data.SqlOperator.EqualTo);
        }
        
        /// <summary>
        /// Gets entities where Value matches the given specification.
        /// </summary>
        /// <remarks>
        /// Created for column <c>Value</c>
        /// </remarks>
        public static LookupValueCollection GetByValue(int value, BootFX.Common.Data.SqlOperator valueOperator)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("Value", valueOperator, value);
            return ((LookupValueCollection)(filter.ExecuteEntityCollection()));
        }
        
        /// <summary>
        /// Gets entities where Value matches the given specification.
        /// </summary>
        /// <remarks>
        /// Created for column <c>Value</c>
        /// </remarks>
        public static LookupValueCollection GetByValue(int value, BootFX.Common.Data.SqlOperator valueOperator, BootFX.Common.OnNotFound onNotFound)
        {
            BootFX.Common.Data.SqlFilter filter = new BootFX.Common.Data.SqlFilter(typeof(LookupValue));
            filter.Constraints.Add("Value", valueOperator, value);
            LookupValueCollection results = ((LookupValueCollection)(filter.ExecuteEntityCollection()));
            return results;
        }
        
        /// <summary>
        /// Searches for <see cref="LookupValue"/> items with the given terms.
        /// </summary>
        public static LookupValueCollection Search(string terms)
        {
            BootFX.Common.Data.SqlSearcher searcher = new BootFX.Common.Data.SqlSearcher(typeof(LookupValue), terms);
            return ((LookupValueCollection)(searcher.ExecuteEntityCollection()));
        }
        
        /// <summary>
        /// Returns the value in the 'Name' property.
        /// </summary>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
