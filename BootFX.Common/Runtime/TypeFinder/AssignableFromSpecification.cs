// BootFX - Application framework for .NET applications
// 
// File: AssignableFromSpecification.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;

namespace BootFX.Common
{
	/// <summary>
	///	 Defines a type finder specification that checks to see if a type is assignable from the given base type.
	/// </summary>
	internal class AssignableFromSpecification : FindSpecification
	{
		/// <summary>
		/// Private field to support <c>BaseType</c> property.
		/// </summary>
		private Type _baseType;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="baseType"></param>
		public AssignableFromSpecification(Type baseType)
		{
			if(baseType == null)
				throw new ArgumentNullException("baseType");
			
			_baseType = baseType;
		}

		/// <summary>
		/// Gets the basetype.
		/// </summary>
		public Type BaseType
		{
			get
			{
				return _baseType;
			}
		}

		public override bool Match(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			if(BaseType == null)
				throw new ArgumentNullException("BaseType");
			return this.BaseType.IsAssignableFrom(type);
		}
	}
}
