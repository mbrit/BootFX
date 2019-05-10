// BootFX - Application framework for .NET applications
// 
// File: AttributeSpecification.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Reflection;

namespace BootFX.Common
{
	/// <summary>
	///	 Defines a type finder speification that looks for the existance of a given attribute.
	/// </summary>
	internal class AttributeSpecification : FindSpecification
	{
		/// <summary>
		/// Private field to support <c>MatchProperties</c> property.
		/// </summary>
		private PropertyInfo[] _matchProperties;
		
		/// <summary>
		/// Private field to support <c>AttributeType</c> property.
		/// </summary>
		private Type _attributeType;

		/// <summary>
		/// Private field to support <c>Inherit</c> property.
		/// </summary>
		private bool _inherit;

		/// <summary>
		/// Private field to support <c>MatchPropertyValues</c> property.
		/// </summary>
		private object[] _matchPropertyValues;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="attributeType"></param>
		/// <param name="inherit"></param>
		public AttributeSpecification(Type attributeType, bool inherit, string[] matchPropertyNames, object[] matchPropertyValues)
		{
			if(attributeType == null)
				throw new ArgumentNullException("attributeType");
			if(matchPropertyNames == null)
				throw new ArgumentNullException("matchPropertyNames");
			if(matchPropertyValues == null)
				throw new ArgumentNullException("matchPropertyValues");
			if(matchPropertyNames.Length != matchPropertyValues.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'matchPropertyNames' and 'matchPropertyValues': {0} cf {1}.", matchPropertyNames.Length, matchPropertyValues.Length));
			
			_attributeType = attributeType;
			_inherit = inherit;
			_matchPropertyValues = matchPropertyValues;

			// check each one...
			_matchProperties = new PropertyInfo[matchPropertyNames.Length];
			for(int index = 0; index < matchPropertyNames.Length; index++)
			{
				// get it...
				_matchProperties[index] = attributeType.GetProperty(matchPropertyNames[index], BindingFlags.Public | BindingFlags.Instance);
				if(_matchProperties[index] == null)
					throw new InvalidOperationException(string.Format("Cannot find property '{0}' on '{1}'.  (The property must be a public instance property.)", 
						matchPropertyNames[index], attributeType));
			}
		}

		/// <summary>
		/// Gets the matchproperties.
		/// </summary>
		private PropertyInfo[] MatchProperties
		{
			get
			{
				// returns the value...
				return _matchProperties;
			}
		}

		/// <summary>
		/// Gets the matchpropertyvalues.
		/// </summary>
		private object[] MatchPropertyValues
		{
			get
			{
				// returns the value...
				return _matchPropertyValues;
			}
		}

		/// <summary>
		/// Gets the inherit.
		/// </summary>
		public bool Inherit
		{
			get
			{
				return _inherit;
			}
		}
		
		/// <summary>
		/// Gets the attributetype.
		/// </summary>
		public Type AttributeType
		{
			get
			{
				return _attributeType;
			}
		}

		public override bool Match(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// get...
			if(type.IsDefined(this.AttributeType, this.Inherit) == false)
				return false;

			// ok, now do the property check...
			object[] attrs = type.GetCustomAttributes(this.AttributeType, this.Inherit);
			if(attrs == null)
				throw new InvalidOperationException("'attrs' is null.");
			if(attrs.Length == 0)
				throw new InvalidOperationException("'attrs' is zero-length.");

			// get it...
			object attr = attrs[0];

			// walk and match...
			for(int index = 0; index < this.MatchProperties.Length; index++)
			{
				// get the value...
				object attrValue = this.MatchProperties[index].GetValue(attr, new object[] {});

				// equals?
				if(object.Equals(this.MatchPropertyValues[index], attrValue) == false)
					return false;
			}

			// we matched ok...
			return true;
		}
	}
}

