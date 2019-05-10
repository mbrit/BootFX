// BootFX - Application framework for .NET applications
// 
// File: TypeFinder.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Data;

namespace BootFX.Common
{
	/// <summary>
	///	 Defines a class that can search the assemblies in the current app domain for a given specification.
	/// </summary>
	/// <example>
	/// <code>
	/// public static SystemInformation GetSystemInformation()
	/// {
	/// 	// get the pages...
	/// 	TypeFinder finder = new TypeFinder(typeof(SystemInformationPage));
	/// 	finder.AddAttributeSpecification(typeof(SystemInformationPageAttribute), false);
	/// 
	/// 	// return...
	/// 	SystemInformationPage[] pages = (SystemInformationPage[])finder.CreateInstances();
	/// 	return new SystemInformation(pages);
	/// }
	/// </code>
	/// </example>
	public class TypeFinder
	{
		/// <summary>
		/// Private field to support <c>Specifications</c> property.
		/// </summary>
		private FindSpecificationCollection _specifications = new FindSpecificationCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public TypeFinder(Type baseType)
		{
			if(baseType == null)
				throw new ArgumentNullException("baseType");

			// add...
			this.Specifications.Add(new AssignableFromSpecification(baseType));
		}

		/// <summary>
		/// Gets a collection of FindSpecification objects.
		/// </summary>
		private FindSpecificationCollection Specifications
		{
			get
			{
				return _specifications;
			}
		}

		/// <summary>
		/// Gets the basetype.
		/// </summary>
		public Type BaseType
		{
			get
			{
				foreach(FindSpecification specification in this.Specifications)
				{
					if(specification is AssignableFromSpecification)
						return ((AssignableFromSpecification)specification).BaseType;
				}

				// nope...
				throw new InvalidOperationException("Base type not found.");
			}
		}

		/// <summary>
		/// Creates instances of the given types.
		/// </summary>
		/// <returns></returns>
		public object[] CreateInstances()
		{
			return this.CreateInstances(new object[] {});
		}

		/// <summary>
		/// Creates instances of the given types.
		/// </summary>
		/// <returns></returns>
		public object[] CreateInstances(object[] parameters)
		{
			if(parameters == null)
				throw new ArgumentNullException("parameters");
			
			// get the types...
			Type[] types = this.GetTypes();
			if(types == null)
				throw new ArgumentNullException("types");
			
			// create...
			if(BaseType == null)
				throw new ArgumentNullException("BaseType");
			Array results = Array.CreateInstance(this.BaseType, types.Length);
			for(int index = 0; index < types.Length; index++)
			{
				// create...
				object value = null;
				try
				{
					// mbr - 27-01-2006 - changed to support non-public instantiation...
					//					value = Activator.CreateInstance(types[index], parameters);
					value = Activator.CreateInstance(types[index], BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, parameters, Cultures.System);
					if(value == null)
						throw new InvalidOperationException("CreateInstance returned null.");
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to create instance of '{0}'.", types[index]), ex);
				}

				// set...
				try
				{
					results.SetValue(value, index);
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to set instance of '{0}' to an array of type '{1}', index {2}.", types[index], BaseType, index), ex);
				}
			}

			// return...
			return (object[])results;
		}

		/// <summary>
		/// Adds an attribute specification.
		/// </summary>
		/// <param name="attributeType"></param>
		public void AddAttributeSpecification(Type attributeType, bool inherit, string matchPropertyName, object matchPropertyValue)
		{
			if(attributeType == null)
				throw new ArgumentNullException("attributeType");
			if(matchPropertyName == null)
				throw new ArgumentNullException("matchPropertyName");
			if(matchPropertyName.Length == 0)
				throw new ArgumentOutOfRangeException("'matchPropertyName' is zero-length.");

			// defer...
			this.AddAttributeSpecification(attributeType, inherit, new string[] { matchPropertyName }, new object[] { matchPropertyValue });
		}

		/// <summary>
		/// Adds an attribute specification.
		/// </summary>
		/// <param name="attributeType"></param>
		public void AddAttributeSpecification(Type attributeType, bool inherit, string[] matchPropertyNames, object[] matchPropertyValues)
		{
			if(attributeType == null)
				throw new ArgumentNullException("attributeType");
			if(matchPropertyNames == null)
				throw new ArgumentNullException("matchPropertyNames");
			if(matchPropertyValues == null)
				throw new ArgumentNullException("matchPropertyValues");
			if(matchPropertyNames.Length != matchPropertyValues.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'matchPropertyNames' and 'matchPropertyValues': {0} cf {1}.", matchPropertyNames.Length, matchPropertyValues.Length));

			// add...
			this.Specifications.Add(new AttributeSpecification(attributeType, inherit, matchPropertyNames, matchPropertyValues));
		}
	
		/// <summary>
		/// Adds an attribute specification.
		/// </summary>
		/// <param name="attributeType"></param>
		public void AddAttributeSpecification(Type attributeType, bool inherit)
		{
			if(attributeType == null)
				throw new ArgumentNullException("attributeType");

			// defer...
			this.AddAttributeSpecification(attributeType, inherit, new string[] {}, new object[] {});
		}
	
		/// <summary>
		/// Gets the types according to the specification for the assemblies in a path
		/// </summary>
		/// <returns></returns>
		// mbr - 04-10-2007 - case 851 - removed.		
		[Obsolete("Do not use this method - it's typically non-optimial.")]	
		public Type[] GetTypes(string assemblyPath)
		{
			ArrayList assemblies = new ArrayList();
			ArrayList files = new ArrayList(Directory.GetFiles(assemblyPath,"*.dll"));
			files.AddRange(Directory.GetFiles(assemblyPath,"*.exe"));
			foreach(string path in files)
			{
				try
				{
					// Try to load the assembly into the current appdomain
					assemblies.Add(Assembly.LoadFile(path));
				}
				catch
				{
					// Ignore any load failures as we can't always
					// be sure the dll is a .net dll.
					Debug.WriteLine(string.Format("Failed to load assembly {0}.",path));
				}
			}

			return GetTypes((Assembly[]) assemblies.ToArray(typeof (Assembly)));
		}

		/// <summary>
		/// Gets the types according to the specification for the assembly.
		/// </summary>
		/// <returns></returns>
		public Type[] GetTypes(Assembly asm)
		{
			return GetTypes(new Assembly[] {asm});
		}

		/// <summary>
		/// Gets the types according to the specification from the current domains assemblies.
		/// </summary>
		/// <returns></returns>
		public Type[] GetTypes()
		{
			return GetTypes(AppDomain.CurrentDomain.GetAssemblies());
		}

		/// <summary>
		/// Gets the types according to the specification.
		/// </summary>
		/// <returns></returns>
		public Type[] GetTypes(Assembly[] assemblies)
		{
			// mbr - 02-03-2006 - sometimes, in ASP.NET assemblies can be loaded twice.  I presume this is related to 
			// ASP.NET copying the assembly.
			ArrayList results = new ArrayList();
			IDictionary byName = CollectionsUtil.CreateCaseInsensitiveHashtable();

			// walk everything...
			foreach(Assembly asm in assemblies)
			{
				// mjr - 23-02-2005 - added this code to skip interop assemblies...	 perhaps a touch clunky...
				bool skip = false;
				string name = asm.GetName().Name;
				if(name != null && name.Length > 0)
				{
					name = name.ToLower();
					if(name.StartsWith("interop.") || name.StartsWith("axinterop."))
						skip = true;
				}

				// skip?
				if(!(skip))
				{
					Type[] types = null;
					try
					{
						types = asm.GetTypes();
					}
					catch(ReflectionTypeLoadException ex)
					{	
						// if we have an exception we can use the types 
						// that could be loaded
						types = ex.Types;

						// Write a log of all the exceptions
						foreach(Exception exception in ex.LoaderExceptions)
							Debug.WriteLine(exception.ToString());
					}
					catch(Exception ex)
					{
						throw new InvalidOperationException(string.Format(Cultures.Exceptions, "Failed to load types for '{0}' ({1}).", asm, asm.CodeBase), ex);
					}

					// type...
					foreach(Type type in types)
					{
						if(type == null)
							continue;

						// walk...
						bool match = true;
						for(int index = 0; index < this.Specifications.Count; index++)
						{
							// match...
							if(!(this.Specifications[index].Match(type)))
							{
								match = false;
								break;
							}
						}

						// add...
						if(match)
						{
							if(!(byName.Contains(type.AssemblyQualifiedName)))
							{
								results.Add(type);
								byName[type.AssemblyQualifiedName] = type;
							}
						}
					}
				}
			}

			// return...
			return (Type[])results.ToArray(typeof(Type));
		}

		/// <summary>
		/// Gets the first found type.
		/// </summary>
		/// <param name="onNotFound"></param>
		/// <returns></returns>
		/// <remarks>This method has a strange name to stop a collision with <see cref="Object.GetType"></see>.</remarks>
		public Type GetFirstFoundType(OnNotFound onNotFound)
		{
			Type[] types = this.GetTypes();
			if(types == null)
				throw new InvalidOperationException("types is null.");

			// check...
			if(types.Length > 0)
				return types[0];
			else
			{
				switch(onNotFound)
				{
					case OnNotFound.ThrowException:
						throw new InvalidOperationException("No types were found.");

					case OnNotFound.ReturnNull:
						return null;

					default:
						throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", onNotFound, onNotFound.GetType()));
				}
			}
		}
	}
}

