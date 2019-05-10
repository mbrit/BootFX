// BootFX - Application framework for .NET applications
// 
// File: EntityMemberComparer.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Text;

namespace BootFX.Common.Entities
{
    public class EntityMemberComparer : IComparer, IEntityType
    {
        /// <summary>
        /// Private field to support <c>InnerComparer</c> property.
        /// </summary>
        private IComparer _innerComparer;

        /// <summary>
        /// Private field to support <c>Field</c> property.
        /// </summary>
        private EntityMember _member;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="field"></param>
        internal EntityMemberComparer(IComparer innerComparer, EntityMember member)
        {
            if(innerComparer == null)
                throw new ArgumentNullException("innerComparer");
            if (member == null)
                throw new ArgumentNullException("member");

            // set...
            _innerComparer = innerComparer;
            _member = member;
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        public EntityMember Member
        {
            get
            {
                return _member;
            }
        }

        /// <summary>
        /// Gets or sets the innercomparer
        /// </summary>
        public IComparer InnerComparer
        {
            get
            {
                return _innerComparer;
            }
            set
            {
                // check to see if the value has changed...
                if(value != _innerComparer)
                {
                    // set the value...
                    _innerComparer = value;
                }
            }
        }

        /// <summary>
        /// Compares two values.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(object x, object y)
        {
            if(x == null)
                throw new ArgumentNullException("x");
            if(y == null)
                throw new ArgumentNullException("y");

            // check...
            if(InnerComparer == null)
                throw new ArgumentNullException("InnerComparer");

            // check...
            if(EntityType == null)
                throw new ArgumentNullException("EntityType");
            this.EntityType.AssertIsOfType(x);
            this.EntityType.AssertIsOfType(y);

            // get values...
            object valueA = this.Member.GetValue(x);
            object valueB = this.Member.GetValue(y);

            // compare...
            int result = this.InnerComparer.Compare(valueA, valueB);
            return result;
        }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public EntityType EntityType
        {
            get
            {
                if (Member == null)
                    throw new InvalidOperationException("'Member' is null.");
                return this.Member.EntityType;
            }
        }
    }
}
