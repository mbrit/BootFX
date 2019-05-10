// BootFX - Application framework for .NET applications
// 
// File: ChildToParentEntityLinkPropertyDescriptor.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using BootFX.Common.Data;
using BootFX.Common.Management;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Describes a property descriptor that access data via an <see cref="ChildToParentEntityLink"></see>.
	/// </summary>
	public class ChildToParentEntityLinkPropertyDescriptor : PropertyDescriptorBase
	{
		/// <summary>
		/// Private Link to support <c>Link</c> property.
		/// </summary>
		private ChildToParentEntityLink _link;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="Link"></param>
		public ChildToParentEntityLinkPropertyDescriptor(ChildToParentEntityLink link) : base(link.Name, new Attribute[] {})
		{
			if(link == null)
				throw new ArgumentNullException("link");
			
			// set...
			_link = link;
		}

		/// <summary>
		/// Gets the Link.
		/// </summary>
		public ChildToParentEntityLink Link
		{
			get
			{
				return _link;
			}
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		public override Type ComponentType
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				return this.EntityType.Type;
			}
		}

		public override object GetValue(object component)
		{
			if(component == null)
				throw new ArgumentNullException("component");

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// check...
			object result = null;
			if(!(component is EntityView))
			{
				this.EntityType.AssertIsOfType(component);

				// anything?
				if(Link == null)
					throw new ArgumentNullException("Link");
				if(Storage == null)
					throw new ArgumentNullException("Storage");
			
				// get the value...
				result = this.Storage.GetParent(component, this.Link);
			}
			else
				result = ((EntityView)component).GetValue(this.Link);

			// return...
			return result;
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public override Type PropertyType
		{
			get
			{ 
				if(Link == null)
					throw new ArgumentNullException("Link");
				if(Link.ParentEntityType == null)
					throw new ArgumentNullException("Link.ParentEntityType");
				return this.Link.ParentEntityType.Type;
			}
		}

		public override void ResetValue(object component)
		{
            throw new NotImplementedException("This operation has not been implemented.");
		}

		public override void SetValue(object component, object value)
		{
			if(component == null)
				throw new ArgumentNullException("component");

			// check...
			if(EntityType == null)
				throw new ArgumentNullException("EntityType");

			// check...
			if(!(component is EntityView))
			{
				this.EntityType.AssertIsOfType(component);

				// anything?
				if(Link == null)
					throw new ArgumentNullException("Link");
				if(Storage == null)
					throw new ArgumentNullException("Storage");
			
				// get the value...
				this.Storage.SetParent(component, this.Link, value);
			}
			else
				((EntityView)component).SetValue(this.Link, value);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Gets the entitytype.
		/// </summary>
		private EntityType EntityType
		{
			get
			{
				if(Link == null)
					throw new ArgumentNullException("Link");
				return this.Link.EntityType;
			}
		}	

		/// <summary>
		/// Gets the Storage for the entity.
		/// </summary>
		private IEntityStorage Storage
		{
			get
			{
				if(EntityType == null)
					throw new ArgumentNullException("EntityType");
				return this.EntityType.Storage;
			}
		}
	}
}
