// BootFX - Application framework for .NET applications
// 
// File: SqlJoin.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common;
using BootFX.Common.Entities;
using System.Text;

namespace BootFX.Common.Data
{
    /// <summary>
    /// Defines a class tha holds a join.
    /// </summary>
    [Serializable]
    public class SqlJoin
    {
        private string _alias;
        internal bool UseAlias { get; private set; }
        private JoinType _type;
        private EntityField[] _targetKeyFields;
        private EntityField[] _sourceFields;
        private EntityType _targetEntityType;
        private EntityMember _targetMember;
        public string SourceAlias { get; set; }

        protected SqlJoin(string alias, JoinType type)
        {
            _alias = alias;
            _type = type;
        }

        internal SqlJoin(string alias, JoinType type, ChildToParentEntityLink link)
            : this(alias, type)
        {
            if (link == null)
                throw new ArgumentNullException("link");

            // set...
            _targetMember = link;

            // et...
            _targetEntityType = link.ParentEntityType;
            if (_targetEntityType == null)
                throw new InvalidOperationException("_targetEntityType is null.");

            // keys...
            _targetKeyFields = _targetEntityType.GetKeyFields();
            if (_targetKeyFields == null)
                throw new InvalidOperationException("'_targetKeyFields' is null.");
            if (_targetKeyFields.Length == 0)
                throw new InvalidOperationException("'_targetKeyFields' is zero-length.");

            // get...
            _sourceFields = link.GetLinkFields();
            if (_sourceFields == null)
                throw new InvalidOperationException("_sourceFields is null.");
            if (_targetKeyFields.Length != _sourceFields.Length)
                throw new InvalidOperationException(string.Format("Length mismatch for '_targetKeyFields' and '_sourceFields': {0} cf {1}.", _targetKeyFields.Length, _sourceFields.Length));
        }

        internal SqlJoin(string alias, JoinType type, EntityType et, EntityField[] sourceFields, EntityField[] targetFields)
            : this(alias, type)
        {
            if (sourceFields.Length != targetFields.Length)
                throw new InvalidOperationException(string.Format("Length mismatch for 'targetFields' and 'sourceFields': {0} cf {1}.", targetFields.Length, sourceFields.Length));

            _targetEntityType = et;
            _sourceFields = (EntityField[])sourceFields.Clone();
            _targetKeyFields = (EntityField[])targetFields.Clone();
        }

        /// <summary>
        /// Gets the targetmember.
        /// </summary>
        private EntityMember TargetMember
        {
            get
            {
                return _targetMember;
            }
        }

        /// <summary>
        /// Gets the targetentitytype.
        /// </summary>
        internal EntityType TargetEntityType
        {
            get
            {
                return _targetEntityType;
            }
        }

        /// <summary>
        /// Gets the sourcefields.
        /// </summary>
        public EntityField[] SourceFields
        {
            get
            {
                return _sourceFields;
            }
        }

        /// <summary>
        /// Gets the targetkeyfields.
        /// </summary>
        public EntityField[] TargetKeyFields
        {
            get
            {
                return _targetKeyFields;
            }
        }

        /// <summary>
        /// Gets the link.
        /// </summary>
        internal ChildToParentEntityLink ChildToParentEntityLink
        {
            get
            {
                return this.TargetMember as ChildToParentEntityLink;
            }
        }

        internal bool IncludeInTreeFetch
        {
            get
            {
                if (this.ChildToParentEntityLink != null)
                    return true;
                else
                    return false;
            }
        }

        internal JoinType Type
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// Appends joins to the query.
        /// </summary>
        protected internal virtual void AppendJoin(SqlStatement statement, StringBuilder builder)
        {
            if (statement == null)
                throw new ArgumentNullException("statement");
            if (builder == null)
                throw new ArgumentNullException("builder");

            // create...
            if (this.Type == JoinType.InnerJoin)
                builder.Append("INNER JOIN");
            else if (this.Type == JoinType.LeftOuterJoin)
                builder.Append("LEFT OUTER JOIN");
            else if (this.Type == JoinType.RightOuterJoin)
                builder.Append("RIGHT OUTER JOIN");
            else
                throw new NotSupportedException(string.Format("Join type '{0}' not supported.", this.Type));
            builder.Append(" ");

            if (this.UseAlias)
                builder.Append(statement.Dialect.FormatTableName(this.TargetEntityType.NativeName, this.Alias));
            else
                builder.Append(statement.Dialect.FormatTableName(this.TargetEntityType.NativeName));

            builder.Append(" ON ");

			// fields...
			EntityField[] sourceFields  = this.SourceFields;
			if(sourceFields == null)
				throw new ArgumentNullException("sourceFields");
			if(sourceFields.Length == 0)
				throw new ArgumentOutOfRangeException("'sourceFields' is zero-length.");			

			// target...
			EntityField[] targetKeyFields = this.TargetKeyFields;
			if(targetKeyFields == null)
				throw new InvalidOperationException("targetKeyFields is null.");
			if(sourceFields.Length != targetKeyFields.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'keyFields' and 'targetKeyFields': {0} cf {1}.", sourceFields.Length, targetKeyFields.Length));

			// walk...
			for(int index = 0; index < sourceFields.Length; index++)
			{
				if(index > 0)
					builder.Append(", ");

				// format...
                if(this.HasSourceAlias)
                    builder.Append(statement.Dialect.FormatColumnName(sourceFields[index], this.SourceAlias));
                else
                    builder.Append(statement.Dialect.FormatColumnName(sourceFields[index], true));
				builder.Append("=");

                if (this.UseAlias)
                    builder.Append(statement.Dialect.FormatColumnName(targetKeyFields[index], this.Alias));
                else
				    builder.Append(statement.Dialect.FormatColumnName(targetKeyFields[index], true));
			}
        }

        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("value");

                if (_alias != value)
                {
                    _alias = value;
                    this.UseAlias = true;
                }
            }
        }

        private bool HasSourceAlias
        {
            get
            {
                return !(string.IsNullOrEmpty(this.SourceAlias));
            }
        }
    }
}
