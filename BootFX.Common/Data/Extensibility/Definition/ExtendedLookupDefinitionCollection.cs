// BootFX - Application framework for .NET applications
// 
// File: ExtendedLookupDefinitionCollection.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Data;

namespace BootFX.Common.Entities
{
	/// <summary>
	/// Summary description for ExtendedLookupCollection.
	/// </summary>
	public class ExtendedLookupDefinitionCollection : CollectionBase
	{
		public ExtendedLookupDefinitionCollection()
		{
		}

		public int Add(ExtendedLookupDefinition lookup)
		{
			if(InnerList.Contains(lookup))
				return InnerList.IndexOf(lookup);

			if(Contains(lookup))
				throw new DuplicateNameException(lookup.Name);

			return InnerList.Add(lookup);
		}

		public void Remove(ExtendedLookupDefinition lookup)
		{
			if(!Contains(lookup))
				return;

			InnerList.RemoveAt(IndexOf(lookup));
		}

		public int IndexOf(ExtendedLookupDefinition lookup)
		{
			return IndexOf(lookup.Name);
		}

		public int IndexOf(string name)
		{
			for(int i = 0; i < InnerList.Count; i++)
			{
				ExtendedLookupDefinition existingProperty = (ExtendedLookupDefinition) InnerList[i];
				if(existingProperty.Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		public bool Contains(string name)
		{
			if(IndexOf(name) > -1)
				return true;

			return false;
		}

		public bool Contains(ExtendedLookupDefinition lookup)
		{
			return Contains(lookup.Name);
		}

		public ExtendedLookupDefinition this[int index]
		{
			get
			{
				return (ExtendedLookupDefinition) InnerList[index];
			}
		}

		public ExtendedLookupDefinition this[string name]
		{
			get
			{
				int index = IndexOf(name);
				if(index != -1)
					return this[index];
				else
					return null;
			}
		}

		public ExtendedLookupDefinition[] GetLookups()
		{
			ArrayList lookups = new ArrayList();
			foreach(ExtendedLookupDefinition lookup in InnerList)
			{
				lookups.Add(lookup);
			}

			return (ExtendedLookupDefinition[]) lookups.ToArray(typeof(ExtendedLookupDefinition));
		}
	}
}
