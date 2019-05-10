// BootFX - Application framework for .NET applications
// 
// File: TokenEvaluationResults.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using BootFX.Common.Entities;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for TokenEvaluationResults.
	/// </summary>
	public class TokenEvaluationResults
	{
        public object Owner { get; private set; }

		/// <summary>
		/// Private field to support <see cref="Member"/> property.
		/// </summary>
		private EntityMember _member;
		
		/// <summary>
		/// Private field to support <see cref="Value"/> property.
		/// </summary>
		private object _value;
		
		/// <summary>
		/// Private field to support <see cref="Exception"/> property.
		/// </summary>
		private Exception _exception;

		/// <summary>
		/// Private field to support <see cref="HasError"/> property.
		/// </summary>
		private bool _hasError;
		
		/// <summary>
		/// Private field to support <see cref="Error"/> property.
		/// </summary>
		private string _error;

		internal TokenEvaluationResults(object owner, EntityMember member, object value)
		{
            this.Owner = owner;
			_member = member;
			_value = value;
		}

		// 2008-18-8 - JM - changing signature so system can differentiate when the first parameter is null
		//internal TokenEvaluationResults(Exception ex, string error)
		internal TokenEvaluationResults(object owner, EntityMember member, object value, Exception ex, string error)
            : this(owner, member, value)
		{
			// set...
            //_member = member;
            //_value = value;
			_exception = ex;
			_error = error;
			_hasError = true;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public object Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Gets the error.
		/// </summary>
		public string Error
		{
			get
			{
				return _error;
			}
		}

		/// <summary>
		/// Gets the haserror.
		/// </summary>
		public bool HasError
		{
			get
			{
				return _hasError;
			}
		}
		
		/// <summary>
		/// Gets the exception.
		/// </summary>
		public Exception Exception
		{
			get
			{
				return _exception;
			}
		}

		/// <summary>
		/// Gets the member.
		/// </summary>
		public EntityMember Member
		{
			get
			{
				return _member;
			}
		}

		public EntityField Field
		{
			get
			{
				return this.Member as EntityField;
			}
		}

		public EntityProperty Property
		{
			get
			{
				return this.Member as EntityProperty;
			}
		}

		public ChildToParentEntityLink Link
		{
			get
			{
				return this.Member as ChildToParentEntityLink;
			}
		}
	}
}
