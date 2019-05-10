// BootFX - Application framework for .NET applications
// 
// File: EntityGenerator.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.ComponentModel;
using System.Text;
using System.Data;
using System.Runtime.Serialization;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using BootFX.Common.Data;
using BootFX.Common.Data.Schema;
using BootFX.Common.Entities;
using BootFX.Common.Entities.Attributes;
using BootFX.Common.CodeGeneration;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Describes a class that can create code for an entity.
	/// </summary>
	public class EntityGenerator
	{
		/// <summary>
		/// Private field to support <c>GenerateParentPropertySetters</c> property.
		/// </summary>
		private bool _generateParentPropertySetters = true;
		
		/// <summary>
		/// Private field to support <c>TargetVersion</c> property.
		/// </summary>
		private DotNetVersion _targetVersion;
		
		/// <summary>
		/// Defines the property name of the business service accessor for base classes.
		/// </summary>
		private const string BusinessServiceBasePropertyName = "BusinessServiceBase";

		/// <summary>
		/// Private field to support <see cref="CompilationOptions"/> property.
		/// </summary>
		private EntityCompilationOptions _compilationOptions;
		
		/// <summary>
		/// Private field to support <c>Language</c> property.
		/// </summary>
		private Language _language = Language.CSharp;
		
		/// <summary>
		/// Private field to support <c>DefaultNamespaceName</c> property.
		/// </summary>
		private string _defaultNamespaceName = "Foo";
		
		/// <summary>
		/// Private field to support <c>ServiceBaseType</c> property.
		/// </summary>
		private string _dtoBaseTypeName = "";

		/// <summary>
		/// Private field to support <c>BaseType</c> property.
		/// </summary>
		private string _entityBaseTypeName = "";

		/// <summary>
		/// Private field to support <c>NamespaceImports</c> property.
		/// </summary>
		private StringCollection _namespaceImports = new StringCollection();
		
        ///// <summary>
        ///// Private field to support <see cref="WebServiceNamespaceImports"/> property.
        ///// </summary>
        //private StringCollection _webServiceNamespaceImports = new StringCollection();
		
		/// <summary>
		/// Constructor.
		/// </summary>
		public EntityGenerator(DotNetVersion targetVersion)
		{
			// options...
			_compilationOptions = new EntityCompilationOptions(this.Language);
			_targetVersion = targetVersion;

			// .NET...
			this.NamespaceImports.Add("System");
			this.NamespaceImports.Add("System.IO");
			this.NamespaceImports.Add("System.Text");
			this.NamespaceImports.Add("System.Text.RegularExpressions");
			this.NamespaceImports.Add("System.Data");
            this.NamespaceImports.Add("System.Linq");
            this.NamespaceImports.Add("System.Collections");
            this.NamespaceImports.Add("System.Collections.Generic");
            this.NamespaceImports.Add("System.Collections.Specialized");

			// us..
			this.NamespaceImports.Add("BootFX.Common");
            this.NamespaceImports.Add("BootFX.Common.Dto");
            this.NamespaceImports.Add("BootFX.Common.Data");
			this.NamespaceImports.Add("BootFX.Common.Entities");
			this.NamespaceImports.Add("BootFX.Common.Entities.Attributes");
			this.NamespaceImports.Add("BootFX.Common.BusinessServices");
		}

        ///// <summary>
        ///// Gets the webservicenamespaceimports.
        ///// </summary>
        //internal StringCollection WebServiceNamespaceImports
        //{
        //    get
        //    {
        //        return _webServiceNamespaceImports;
        //    }
        //}

		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public GeneratedCompileUnit GetCompileUnit(GeneratedTypeDeclaration type, EntityGenerationContext context)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(context == null)
				throw new ArgumentNullException("context");

			// defer...
			return this.GetCompileUnit(this.DefaultNamespaceName, type, context);
		}

		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public GeneratedCompileUnit GetCompileUnit(string namespaceName, GeneratedTypeDeclaration type, EntityGenerationContext context)
		{
			if(namespaceName == null)
				throw new ArgumentNullException("namespaceName");
			if(namespaceName.Length == 0)
				throw new ArgumentOutOfRangeException("'namespaceName' is zero-length.");
			if(type == null)
				throw new ArgumentNullException("type");
			
			// return...
			var ns = this.GetNamespace(namespaceName, type, context);
			ns.CodeItem.Types.Add(type.CodeItem);

			// retunr...
			return this.GetCompileUnit(ns, context);
		}

		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public GeneratedCompileUnit GetCompileUnit(GeneratedNamespace ns, EntityGenerationContext context)
		{
			if(ns == null)
				throw new ArgumentNullException("ns");
			if(context == null)
				throw new ArgumentNullException("context");

			// create...
			CodeCompileUnit compileUnit = new CodeCompileUnit();
			compileUnit.Namespaces.Add(ns.CodeItem);

			// return...
			return new GeneratedCompileUnit(compileUnit);
		}

		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public GeneratedNamespace GetNamespace(GeneratedTypeDeclaration type, EntityGenerationContext context)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(context == null)
				throw new ArgumentNullException("context");

			// defer...
			return this.GetNamespace(this.DefaultNamespaceName, type, context);
		}
		
		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public CodeNamespace GetNamespace(string name, EntityGenerationContext context)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(context == null)
				throw new ArgumentNullException("context");

			// return...
			CodeNamespace ns = new CodeNamespace(name);

			// ns...
			this.AddNamespaces(ns, this.NamespaceImports);

			// retunr...
			return ns;
		}

		private void AddNamespaces(CodeNamespace ns, IList namespaces)
		{
			if(ns == null)
				throw new ArgumentNullException("ns");
			if(namespaces == null)
				throw new ArgumentNullException("namespaces");
			
			foreach(string namespaceImport in namespaces)
				ns.Imports.Add(new CodeNamespaceImport(namespaceImport));
		}

		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
        public GeneratedNamespace GetNamespace(string name, GeneratedTypeDeclaration[] types, EntityGenerationContext context)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(types == null)
				throw new ArgumentNullException("types");
			if(context == null)
				throw new ArgumentNullException("context");

			// return...
			CodeNamespace ns = this.GetNamespace(name, context);
			if(ns == null)
				throw new InvalidOperationException("ns is null.");

			// add...
            foreach (var type in types)
                ns.Types.Add(type.CodeItem);

			// retunr...
            var results = new GeneratedNamespace(ns);
            foreach (var key in types[0].GenericMappings.Keys)
                results.GenericMappings[key] = types[0].GenericMappings[key];
			return results;
		}

		/// <summary>
		/// Creates a namespace with the given name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
        public GeneratedNamespace GetNamespace(string name, GeneratedTypeDeclaration type, EntityGenerationContext context)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(type == null)
				throw new ArgumentNullException("type");
			if(context == null)
				throw new ArgumentNullException("context");

			// defer...
            return this.GetNamespace(name, new GeneratedTypeDeclaration[] { type }, context);
		}

		/// <summary>
		/// Gets the namespaceimports.
		/// </summary>
		protected StringCollection NamespaceImports
		{
			get
			{
				// returns the value...
				return _namespaceImports;
			}
		}

		/// <summary>
		/// Creates the base class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 06-09-2007 - for c7 - made virtual...
		// mbr - 21-09-2007 - added context.		
        public virtual GeneratedTypeDeclaration GetEntityClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetEntityClassName(table);
			CodeTypeDeclaration type = new CodeTypeDeclaration(className);
			type.TypeAttributes = table.GetTypeAttributes(false, context);
			CodeDomExtender.AddSummaryComment(type, string.Format("Defines the entity type for '{0}'.", table.NativeName));

			// only if 1.0...
			if(this.TargetVersion == DotNetVersion.V1)
				type.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

			// attribute...
			// 	[Entity(typeof(UniversalEntityCollection), "tableUniversal", typeof(IUniversalService)), SortSpecification("auniqueidentifier", SortDirection.Ascending)]
			CodeAttributeDeclaration attr = new CodeAttributeDeclaration("Entity", new CodeAttributeArgument(new CodeTypeOfExpression(this.GetEntityCollectionClassName(table))),
				new CodeAttributeArgument(new CodePrimitiveExpression(table.NativeName)));

			// do we have a database name?
			if(this.CompilationOptions.HasDatabaseName)
				attr.Arguments.Add(new CodeAttributeArgument("DatabaseName", new CodePrimitiveExpression(this.CompilationOptions.DatabaseName)));

			// add the attribute...
			type.CustomAttributes.Add(attr);

			// If we have a Name column we will add a sort declaration for it
			if(table.Columns.Contains("Name"))
			{
				CodeAttributeDeclaration sortAttr = new CodeAttributeDeclaration();
				sortAttr.Name = "SortSpecification";

				CodeArrayCreateExpression columnNames = new CodeArrayCreateExpression(typeof(string));
				CodeArrayCreateExpression columnSortOrder = new CodeArrayCreateExpression(typeof(SortDirection));
				columnNames.Initializers.Add(new CodePrimitiveExpression("Name"));
				columnSortOrder.Initializers.Add(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(SortDirection)),"Ascending"));

				sortAttr.Arguments.Add(new CodeAttributeArgument(columnNames));
				sortAttr.Arguments.Add(new CodeAttributeArgument(columnSortOrder));
				
				// add the attribute...
				type.CustomAttributes.Add(sortAttr);
			}

			// only do inheritence and constructor if 1.0...
			if(this.TargetVersion == DotNetVersion.V1)
				type.BaseTypes.Add(this.GetEntityBaseClassName(table));

			// no constructor...
			type.Members.Add(this.CreateNoOpConstructor(MemberAttributes.Public | MemberAttributes.Final));
			type.Members.Add(this.CreateDeserializationConstructor());

			// return...
			return new GeneratedTypeDeclaration(type, table);
		}

		/// <summary>
		/// Creates the web service entity class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
        public GeneratedTypeDeclaration GetEntityWsClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetEntityWsClassName(table);
			CodeTypeDeclaration type = new CodeTypeDeclaration(className);
			type.TypeAttributes = table.GetTypeAttributes(false, context);
			type.BaseTypes.Add(new CodeTypeReference(GetEntityWsBaseClassName(table)));

			CodeDomExtender.AddSummaryComment(type, string.Format("Defines the webservice entity type for '{0}'.", table.Name));
			type.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

			// return...
            return new GeneratedTypeDeclaration(type, table);
        }

		/// <summary>
		/// Creates the web service entity class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
        public GeneratedTypeDeclaration GetEntityWsBaseClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetEntityWsBaseClassName(table);
			CodeTypeDeclaration type = new CodeTypeDeclaration(className);
			type.TypeAttributes = table.GetTypeAttributes(false, context);
			type.BaseTypes.Add(typeof(WsEntity));
			CodeDomExtender.AddSummaryComment(type, string.Format("Defines the base webservice entity type for '{0}'.", table.Name));
			type.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

			// loop the columns...
			foreach(SqlColumn column in table.Columns)
			{
				// If we don't generate we skip it in the ws entity base
				if(!column.Generate)
					continue;

				// create a property...
				CodeMemberField field = this.CreateFieldForWsClass(table, column);
				if(field == null)
					throw new InvalidOperationException("field is null.");
				type.Members.Add(field);
			}

			// return...
            return new GeneratedTypeDeclaration(type, table);
        }

		/// <summary>
		/// Creates a field to access a column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		private CodeMemberField CreateFieldForWsClass(SqlTable table, SqlColumn column)
		{
			CodeMemberField field = new CodeMemberField();
			field.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			field.Name = column.Name;
			field.Type = this.GetColumnType(column);
			CodeDomExtender.AddSummaryComment(field, string.Format("Gets or sets the value for '{0}'.", column.Name));
			CodeDomExtender.AddRemarksComment(field, string.Format("This field maps to the '{0}' column.", column.NativeName));

			return field;
		}

		/// <summary>
		/// Gets a call to GetEntityType for the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMethodInvokeExpression GetGetEntityTypeMethod(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(EntityType)), 
				"GetEntityType", new CodeTypeOfExpression(this.GetEntityClassName(table)), new CodeFieldReferenceExpression(
				new CodeTypeReferenceExpression(typeof(OnNotFound)), "ThrowException"));
		}

		/// <summary>
		/// Creates the base class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 06-09-2007 - for c7 - made virtual...
		// mbr - 21-09-2007 - added context.		
        public virtual GeneratedTypeDeclaration GetEntityCollectionClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetEntityCollectionClassName(table);
			CodeTypeDeclaration type = new CodeTypeDeclaration(className);
			type.TypeAttributes = table.GetTypeAttributes(false, context);
			CodeDomExtender.AddSummaryComment(type, string.Format("Defines the collection for entities of type <see cref=\"{0}\"/>.", 
				this.GetEntityClassName(table)));
			type.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

			// inherits...
			type.BaseTypes.Add(this.GetEntityCollectionBaseClassName(table));

			// no constructor...
			type.Members.Add(this.CreateNoOpConstructor(MemberAttributes.Public | MemberAttributes.Final));
			type.Members.Add(this.CreateDeserializationConstructor());

			// return...
            return new GeneratedTypeDeclaration(type, table);
        }

		/// <summary>
		/// Creates the base class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 06-09-2007 - for c7 - made virtual...
		// mbr - 21-09-2007 - added context.		
        public virtual GeneratedTypeDeclaration GetEntityCollectionBaseClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetEntityCollectionBaseClassName(table);
			CodeTypeDeclaration type = new CodeTypeDeclaration(className);
			type.TypeAttributes = table.GetTypeAttributes(true, context);
			CodeDomExtender.AddSummaryComment(type, string.Format("Defines the base collection for entities of type <see cref=\"{0}\"/>.", 
				this.GetEntityClassName(table)));
			type.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

			// inherits...
			type.BaseTypes.Add(typeof(EntityCollection));

            // mbr - 2012-06-07 - linq...
            var enumerable = new CodeTypeReference("IEnumerable");
            enumerable.TypeArguments.Add(table.Name);
            type.BaseTypes.Add(enumerable);

			// no constructor...
			CodeConstructor constructor = this.CreateNoOpConstructor(MemberAttributes.Family | MemberAttributes.Final);
			constructor.BaseConstructorArgs.Add(new CodeTypeOfExpression(this.GetEntityClassName(table)));
			type.Members.Add(constructor);

			// constructor...
			type.Members.Add(this.CreateDeserializationConstructor());

			// add...
			type.Members.Add(this.GetCollectionIndexer(table));
			type.Members.Add(this.GetCollectionAddMethod(table));
			type.Members.Add(this.GetCollectionAddRangeFromArrayMethod(table));
			type.Members.Add(this.GetCollectionAddRangeFromCollectionMethod(table));
            type.Members.Add(this.GetCollectionEnumeratorMethod(table));

			// return...
            return new GeneratedTypeDeclaration(type, table);
		}

        private CodeMemberMethod GetCollectionEnumeratorMethod(SqlTable table)
        {
            //IEnumerator<Address> IEnumerable<Address>.GetEnumerator()
            //{
            //    return GetEnumerator<Address>();
            //}

            var enumerableRef = new CodeTypeReference("IEnumerable");
            enumerableRef.TypeArguments.Add(table.Name);

            var enumeratorRef = new CodeTypeReference("IEnumerator");
            enumeratorRef.TypeArguments.Add(table.Name);

            var method = new CodeMemberMethod();
            method.Name = "GetEnumerator";
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
            method.PrivateImplementationType = enumerableRef;
            method.ReturnType = enumeratorRef;

            var methodRef = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetEnumerator");
            methodRef.TypeArguments.Add(table.Name);
            method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(methodRef)));

            return method;
        }

		/// <summary>
		/// Gets the method to add a rangeof item.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberMethod GetCollectionAddRangeFromCollectionMethod(SqlTable table)
		{
			// defer...
			return this.GetCollectionAddRangeMethod(table, 
				new CodeParameterDeclarationExpression(this.GetEntityCollectionClassName(table), "items"));
		}

		/// <summary>
		/// Gets the method to add a rangeof item.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberMethod GetCollectionAddRangeFromArrayMethod(SqlTable table)
		{
			// defer...
			return this.GetCollectionAddRangeMethod(table, 
				new CodeParameterDeclarationExpression(new CodeTypeReference(new CodeTypeReference(this.GetEntityClassName(table)), 1), "items"));
		}

		/// <summary>
		/// Gets the method to add a range of items.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		private CodeMemberMethod GetCollectionAddRangeMethod(SqlTable table, CodeParameterDeclarationExpression parameter)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "AddRange";
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			method.Parameters.Add(parameter);
			CodeDomExtender.AddSummaryComment(method, string.Format("Adds a range of <see cref=\"{0}\"/> instances to the collection.", this.GetEntityClassName(table)));

			// defer...
			method.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), 
				"AddRange", new CodeArgumentReferenceExpression(parameter.Name)));

			// return...
			return method;
		}

		/// <summary>
		/// Gets the method to add an item.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberMethod GetCollectionAddMethod(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "Add";
			method.ReturnType = new CodeTypeReference(typeof(int));
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetEntityClassName(table), "item"));
			CodeDomExtender.AddSummaryComment(method, string.Format("Adds a <see cref=\"{0}\"/> instance to the collection.", this.GetEntityClassName(table)));

			// defer...
			method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), 
				"Add", new CodeArgumentReferenceExpression("item"))));

			// return...
			return method;
		}

		/// <summary>
		/// Gets the collection indexer.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberProperty GetCollectionIndexer(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			//			/// <summary>
			//			/// Gets or sets an item.
			//			/// </summary>
			//			public UniversalEntity this[int index]
			//			{
			//				get
			//				{
			//					return (UniversalEntity)this.GetItem(index);
			//				}
			//				set
			//				{
			//					this.SetItem(index, value);
			//				}
			//			}

			// create...
			CodeMemberProperty property = new CodeMemberProperty();
			property.Name = "Item";
			property.Parameters.Add(new CodeParameterDeclarationExpression(typeof(int), "index"));
			property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			property.Type = new CodeTypeReference(this.GetEntityClassName(table));
			CodeDomExtender.AddSummaryComment(property, "Gets or sets the item with the given index.");

			// get...
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(property.Type, 
				new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "GetItem", 
				new CodeExpression[] { new CodeArgumentReferenceExpression("index") }))));

			// set...
			property.SetStatements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "SetItem", 
				new CodeExpression[] { new CodeArgumentReferenceExpression("index"), new CodeArgumentReferenceExpression("value") }));

			// return...
			return property;
		}

		private CodeAttributeDeclaration CreateIndexAttribute(SqlIndex index)
		{
			if(index == null)
				throw new ArgumentNullException("index");
			
			// create...
			CodeAttributeDeclaration attr = new CodeAttributeDeclaration("Index");
			attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(index.Name)));
			attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(index.NativeName)));
			attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(index.HasUniqueValues)));

			// create a string array...
			attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(index.Columns.GetNamesAsCsvString())));

            // if...
            if (index.IncludedColumns.Count > 0)
                attr.Arguments.Add(new CodeAttributeArgument("IncludedColumns", new CodePrimitiveExpression(index.IncludedColumns.GetNamesAsCsvString())));
            if (index.ComputedColumns.Count > 0)
                attr.Arguments.Add(new CodeAttributeArgument("ComputedColumns", new CodePrimitiveExpression(index.ComputedColumns.GetNamesAsCsvString())));

			// return...
			return attr;
		}

		/// <summary>
		/// Creates the base class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 06-09-2007 - for c7 - made virtual...
		// mbr - 21-09-2007 - added context.	
		public virtual GeneratedTypeDeclaration GetEntityBaseClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetEntityBaseClassName(table);
			CodeTypeDeclaration type = new CodeTypeDeclaration(className);
			CodeDomExtender.AddSummaryComment(type, string.Format("Defines the base entity type for '{0}'.", table.NativeName));
			type.CustomAttributes.Add(new CodeAttributeDeclaration("Serializable"));

            // dto...
            if (table.GenerateDto)
            {
                var dto = new CodeAttributeDeclaration("Dto");
                dto.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(this.GetDtoClassName(table))));
                type.CustomAttributes.Add(dto);
            }

			// mbr - 02-11-2005 - indexes...	
			SqlIndexCollection indexes = new SqlIndexCollection();
			indexes.AddRange(table.Indexes);
			indexes.SortByNativeName();
			foreach(SqlIndex index in indexes)
			{
				if(index.Generate)
					type.CustomAttributes.Add(this.CreateIndexAttribute(index));
			}

			// target...
			if(this.TargetVersion == DotNetVersion.V1)
				type.TypeAttributes = table.GetTypeAttributes(true, context);
			else
				type.TypeAttributes = TypeAttributes.Public;		// we have to add "partial" later...

			// inherits...
			type.BaseTypes.Add(EntityBaseTypeName);

			// constructors...
			if(this.TargetVersion == DotNetVersion.V1)
			{
				type.Members.Add(this.CreateNoOpConstructor(MemberAttributes.Family | MemberAttributes.Final));
				type.Members.Add(this.CreateDeserializationConstructor());
			}

			// Create a static create filter method
			type.Members.Add(GetCreateFilterStaticMethod(table, context));
            type.Members.Add(GetCreateFilterStaticMethod2(table, context));

			type.Members.Add(this.GetSetPropertyMethod(table, MemberTarget.Concrete));

			// loop the columns...
			foreach(SqlColumn column in table.Columns)
			{
				// create a property...
				CodeMemberProperty prop = this.GetFieldAccessProperty(table, column, context);
				if(prop == null)
					throw new InvalidOperationException("prop is null.");
				type.Members.Add(prop);
			}

			// get all method...
			ArrayList signatures = new ArrayList();
			this.AddMethod(type, this.GetGetAllMethod(table, MemberTarget.Concrete, context), signatures, null);

			// add the 'get by id' method...  if this is null, the table does not have key columns...
			this.AddMethods(type, this.GetGetByIdMethods(table, MemberTarget.Concrete, context), signatures, null);

			// add methods for indexes...
			foreach(SqlIndex index in table.Indexes)
			{
				//				this.AddMethods(type, this.GetIndexMethods(table, index, MemberTarget.Stub), signatures, 
				//					string.Format("Created for index <c>{0}</c>.", index.Name));

				this.AddMethods(type, this.GetIndexMethods(table, index, MemberTarget.Concrete, context), signatures, 
					string.Format("Created for index <c>{0}</c>.", index.Name));
			}

			// add methods for individual columns...
			foreach(SqlColumn column in table.Columns)
			{
				// does the table have an index with only this column in it that's unique?  if so, it does not return a collection
				// but rather returns a single item...
				bool returnsCollection = true;
				foreach(SqlIndex index in table.Indexes)
				{
					if(index.HasUniqueValues && index.Columns.Count == 1)
					{
						// this one?
						if(index.Columns[0] == column)
						{
							returnsCollection  = false;
							break;
						}
					}
				}

				// add...
				//this.AddMethods(type, this.GetColumnMethods(table, column, returnsCollection, MemberTarget.Stub), signatures,
				//	string.Format("Created for column <c>{0}</c>", column.Name));
				this.AddMethods(type, this.GetColumnMethods(table, column, returnsCollection, MemberTarget.Concrete, context), signatures,
					string.Format("Created for column <c>{0}</c>", column.Name));
			}

			// child tables...
			foreach(SqlChildToParentLink associatedLink in table.AssociatedLinks)
			{
				if(associatedLink.Table == null)
					throw new InvalidOperationException("associatedLink.Table is null.");
				if(associatedLink.Table.Generate)
				{
					CodeMemberMethod[] methods = this.GetChildLinkMethods(table, associatedLink, MemberTarget.Stub, context);
					foreach(CodeMemberMethod method in methods)
						method.Attributes = associatedLink.Table.GetMemberAttributes(false, context);
					this.AddMethods(type, methods, signatures, string.Format("Created for link <c>{0}</c>.  (Stub method.)", associatedLink));
				}
			}

			// child tables...
			foreach(SqlChildToParentLink associatedLink in table.AssociatedLinks)
			{
				if(associatedLink.Table == null)
					throw new InvalidOperationException("associatedLink.Table is null.");
				if(associatedLink.Table.Generate)
				{
					this.AddMethods(type, this.GetChildLinkMethods(table, associatedLink, MemberTarget.Concrete, context), signatures, 
						string.Format("Created for link <c>{0}</c>.  (Concrete method.)", associatedLink.Name));
				}
			}

			// add methods for parent links...
			foreach(SqlChildToParentLink parentLink in table.LinksToParents)
			{
				if(parentLink.ParentTable.Generate)
					type.Members.Add(this.GetChildToParentLinkProperty(table, parentLink, context));
			}

			// search...
			//this.AddMethods(type, this.GetSearchMethods(table, MemberTarget.Stub), signatures, string.Empty);
			this.AddMethods(type, this.GetSearchMethods(table, MemberTarget.Concrete, context), signatures, string.Empty);

			// to string?
			CodeMemberMethod toString = this.GetToStringMethod(table);
			if(toString != null)
				type.Members.Add(toString);

			// return...
            return new GeneratedTypeDeclaration(type, table);
		}

		/// <summary>
		/// Creates a method to allow the creation of an entity.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod GetCreateFilterStaticMethod(SqlTable table, EntityGenerationContext context)
		{
			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "CreateFilter";

			// We have a web method so remove the static and add the attribute
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			method.ReturnType = new CodeTypeReference(typeof(SqlFilter));

			CodeDomExtender.AddSummaryComment(method, string.Format("Creates a SqlFilter for an instance of '{0}'.", table.Name));
			CodeObjectCreateExpression filterCreate = new CodeObjectCreateExpression();
			filterCreate.CreateType = new CodeTypeReference(typeof(SqlFilter));
			filterCreate.Parameters.Add(new CodeTypeOfExpression(GetEntityClassName(table)));

			method.Statements.Add(new CodeMethodReturnStatement(filterCreate));

			return method;
		}

        protected virtual CodeMemberMethod GetCreateFilterStaticMethod2(SqlTable table, EntityGenerationContext context)
        {
            // create...
            CodeMemberMethod method = new CodeMemberMethod();
            method.Name = "CreateFilter2";

            // We have a web method so remove the static and add the attribute
            method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
            method.ReturnType = new CodeTypeReference(typeof(SqlFilter));
            method.ReturnType.TypeArguments.Add(table.Name);

            CodeDomExtender.AddSummaryComment(method, string.Format("Creates a SqlFilter for an instance of '{0}'.", table.Name));
            CodeObjectCreateExpression filterCreate = new CodeObjectCreateExpression();
            filterCreate.CreateType = new CodeTypeReference(typeof(SqlFilter));
            filterCreate.CreateType.TypeArguments.Add(table.Name);
            //            filterCreate.Parameters.Add(new CodeTypeOfExpression(GetEntityClassName(table)));

            method.Statements.Add(new CodeMethodReturnStatement(filterCreate));

            return method;
        }

        /// <summary>
        /// Creates the base web service class for a table.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        // mbr - 21-09-2007 - added context.		
        public GeneratedTypeDeclaration GetDtoBaseClass(SqlTable table, EntityGenerationContext context)
        {
            if (table == null)
                throw new ArgumentNullException("table");
            if (context == null)
                throw new ArgumentNullException("context");

            // create a type...
            string className = this.GetDtoBaseClassName(table);
            CodeTypeDeclaration type = new CodeTypeDeclaration(className);
            type.TypeAttributes = TypeAttributes.Public | TypeAttributes.Abstract;
            CodeDomExtender.AddSummaryComment(type, string.Format("Defines the base DTO object for entity type '{0}'.", table.Name));

            // base...
            var baseRef = new CodeTypeReference(this.DtoBaseTypeName);
            baseRef.TypeArguments.Add(new CodeTypeReference(this.GetEntityClassName(table)));
            type.BaseTypes.Add(baseRef);
            
            // constructor...
            type.Members.Add(CreateNoOpConstructor(MemberAttributes.Family | MemberAttributes.Final));

            // go through the properties...
            foreach (SqlColumn column in table.Columns)
            { 
                if(column.GenerateDtoField)
                {
                    var prop = new CodeMemberProperty()
                    {
                        Name = column.Name,
                        Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    };

                    if (string.Compare(column.Name, "Type", true) == 0)
                        prop.Attributes |= MemberAttributes.New;

                    // what's the type?
                    if (column.HasEnumerationTypeName)
                        prop.Type = new CodeTypeReference(column.EnumerationTypeName);
                    else
                        prop.Type = new CodeTypeReference(column.Type);

                    // get...
                    var getMethod = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetValue");
                    getMethod.TypeArguments.Add(prop.Type);
                    prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(getMethod)));

                    // set...
                    var setMethod = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "SetValue");
                    prop.SetStatements.Add(new CodeMethodInvokeExpression(setMethod, new CodeVariableReferenceExpression("value")));

                    // attributes...
                    var attr = new CodeAttributeDeclaration(new CodeTypeReference("DtoField"));
                    attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(column.JsonName)));
                    prop.CustomAttributes.Add(attr);

                    // add...
                    type.Members.Add(prop);
                }
            }

            // links...
            foreach (SqlChildToParentLink link in table.LinksToParents)
            {
                if (link.GenerateDtoField)
                {
                    var prop = new CodeMemberProperty()
                    {
                        Name = link.Name,
                        Attributes = MemberAttributes.Public | MemberAttributes.Final,
                    };

                    // what's the type?
                    prop.Type = new CodeTypeReference(this.GetDtoClassName(link.ParentTable));

                    // get...
                    var getMethod = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "GetLink");
                    getMethod.TypeArguments.Add(prop.Type);
                    prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(getMethod)));

                    // set...
                    var setMethod = new CodeMethodReferenceExpression(new CodeThisReferenceExpression(), "SetLink");
                    prop.SetStatements.Add(new CodeMethodInvokeExpression(setMethod, new CodeVariableReferenceExpression("value")));

                    // attributes...
                    var attr = new CodeAttributeDeclaration(new CodeTypeReference("DtoLink"));
                    attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(link.JsonName)));
                    prop.CustomAttributes.Add(attr);

                    // add...
                    type.Members.Add(prop);
                }
            }

            // return...
            return new GeneratedTypeDeclaration(type, table);
        }

		/// <summary>
		/// Creates the webservice class for a table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public GeneratedTypeDeclaration GetDtoClass(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a type...
			string className = this.GetDtoClassName(table);
			var type = new CodeTypeDeclaration(className);
            type.BaseTypes.Add(new CodeTypeReference(this.GetDtoBaseClassName(table)));
            CodeDomExtender.AddSummaryComment(type, string.Format("Defines the DTO object for entity type '{0}'.", table.Name));
            //type.TypeAttributes = table.GetTypeAttributes(false, context);

            //string wsNamespace = string.Format("http://www.mbrit.com/{1}",this.DefaultNamespaceName,table.Name);
            //string wsDescription = string.Format("Defines the web service type for '{0}'.", table.Name);
            //type.CustomAttributes.Add(new CodeAttributeDeclaration("WebService",new CodeAttributeArgument("Namespace",new CodePrimitiveExpression(wsNamespace)), new CodeAttributeArgument("Description",new CodePrimitiveExpression(wsDescription))));

            //CodeDomExtender.AddSummaryComment(type,wsDescription);

            //// inherits...
            //type.BaseTypes.Add(this.GetDtoBaseClassName(table));

            //CodeMemberField components = new CodeMemberField(typeof(IContainer),"components");
            //components.InitExpression = new CodePrimitiveExpression(null);

            //// no constructor...
            //type.Members.Add(components);
            //type.Members.Add(CreateNoOpInitializeConstructor(MemberAttributes.Public | MemberAttributes.Final));
            //type.Members.Add(CreateInitializeComponentMethod());
            //type.Members.Add(CreateDisposeMethodForWebService());

			// return...
            return new GeneratedTypeDeclaration(type, table);
		}

		private CodeMemberMethod CreateInitializeComponentMethod()
		{
			CodeMemberMethod initializeMethod = new CodeMemberMethod();
			initializeMethod.Name = "InitializeComponent";

			initializeMethod.Comments.Add(new CodeCommentStatement("/// <summary>"));
			initializeMethod.Comments.Add(new CodeCommentStatement("/// Required method for Designer support - do not modify"));
			initializeMethod.Comments.Add(new CodeCommentStatement("/// the contents of this method with the code editor."));
			initializeMethod.Comments.Add(new CodeCommentStatement("/// </summary>"));
			
			return initializeMethod;
		}

        //private CodeMemberMethod CreateDisposeMethodForWebService()
        //{
        //    CodeMemberMethod disposeMethod = new CodeMemberMethod();
        //    disposeMethod.Name = "Dispose";
        //    disposeMethod.Attributes = MemberAttributes.Family | MemberAttributes.Override;
        //    disposeMethod.Parameters.Add(new CodeParameterDeclarationExpression(typeof(bool),"disposing"));

        //    disposeMethod.Comments.Add(new CodeCommentStatement("/// <summary>"));
        //    disposeMethod.Comments.Add(new CodeCommentStatement("/// Clean up any resources being used."));
        //    disposeMethod.Comments.Add(new CodeCommentStatement("/// </summary>"));

        //    CodeBinaryOperatorExpression op = new CodeBinaryOperatorExpression();
        //    op.Left = new CodeArgumentReferenceExpression("disposing");
        //    op.Operator = CodeBinaryOperatorType.BooleanAnd;
        //    op.Right = new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"components"),CodeBinaryOperatorType.IdentityInequality,new CodePrimitiveExpression(null));

        //    CodeConditionStatement ifDisposing = new CodeConditionStatement();
        //    ifDisposing.Condition = op;
        //    ifDisposing.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),"components"),"Dispose"));

        //    disposeMethod.Statements.Add(ifDisposing);
        //    disposeMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(),"Dispose",new CodeArgumentReferenceExpression("disposing")));
        //    return disposeMethod;
        //}


		/// <summary>
		/// Gets a ToString method.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		/// <remarks>If this method has a column called <c>Name</c>, this method will create a <c>ToString</c> method that returns it.</remarks>
		private CodeMemberMethod GetToStringMethod(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// walk...
			foreach(SqlColumn column in table.Columns)
			{
				if(string.Compare(column.Name, "Name", true, System.Globalization.CultureInfo.InvariantCulture) == 0)
				{
					// create...
					CodeMemberMethod toString = new CodeMemberMethod();
					toString.Name = "ToString";
					toString.Attributes = MemberAttributes.Override | MemberAttributes.Public;
					toString.ReturnType = new CodeTypeReference(typeof(string));
					CodeDomExtender.AddSummaryComment(toString, string.Format("Returns the value in the '{0}' property.", column.Name));

					// statements...
					toString.Statements.Add(new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), 
						column.Name)));

					// return...
					return toString;
				}
			}

			// nope...
			return null;
		}

		/// <summary>
		/// Creates a field changed method.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		private CodeMemberMethod CreateOnFieldChangedMethod(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");

			//			/// <summary>
			//			/// Raises the <see cref="FooChanged"></see> event.
			//			/// </summary>
			//			/// <param name="e"></param>
			//			protected virtual void OnFooChanged(EventArgs e)
			//			{
			//				if(Foo != null)
			//					Foo(this, e);
			//			}

			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = string.Format("On{0}Changed", column.Name);
			method.Attributes = MemberAttributes.Family;
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(EventArgs), "e"));
			CodeDomExtender.AddSummaryComment(method, string.Format("Raises the <see cref=\"{0}Changed\"/> event.", column.Name));

			// if...
			CodeConditionStatement ifAssigned = new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), column.Name + "Changed"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)));
			ifAssigned.TrueStatements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), column.Name + "Changed", 
				new CodeThisReferenceExpression(), new CodeArgumentReferenceExpression("e")));
			method.Statements.Add(ifAssigned);

			// return...
			return method;
		}

		/// <summary>
		/// Creates a field changed event.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		private CodeMemberEvent CreateFieldChangedEvent(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");

			//			/// <summary>
			//			/// Raised when <see cref="DateTime"></see> is changed.
			//			/// </summary>
			//			public event EventHandler ProjectIdChanged
			//			{
			//				add
			//				{
			//					this.AddFieldChangedHandler("ProjectId", value);
			//				}
			//				remove
			//				{
			//					this.RemoveFieldChangedHandler("ProjectId", value);
			//				}
			//			}

			// create...
			CodeMemberEvent changedEvent = new CodeMemberEvent();
			changedEvent.Attributes = MemberAttributes.Public;
			changedEvent.Name = column + "Changed";
			changedEvent.Type = new CodeTypeReference(typeof(EventHandler));
			CodeDomExtender.AddSummaryComment(changedEvent, string.Format("Raised when <see cref=\"{0}\"/> is changed.", column.Name));

			// return...
			return changedEvent;
		}

		/// <summary>
		/// Gets child link methods.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="childTable"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod[] GetChildLinkMethods(SqlTable table, SqlChildToParentLink associatedLink, MemberTarget target,
			EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(associatedLink == null)
				throw new ArgumentNullException("associatedLink");
			
			//			/// <summary>
			//			/// Gets the child items.
			//			/// </summary>
			//			/// <returns></returns>
			//			public ChildEntityCollection GetChildren()
			//			{
			//				SqlFilter filter = new SqlFilter(typeof(ChildEntity));
			//				filter.Constraints.Add("parentid", SqlOperator.EqualTo, this.BookingId);
			//
			//				// return...
			//				return (ChildEntityCollection)filter.ExecuteEntityCollection();
			//			}

			// get the child table...
			SqlTable childTable = associatedLink.Table;
			if(childTable == null)
				throw new InvalidOperationException("childTable is null.");

			// basic one...
			ArrayList methods = new ArrayList();
			methods.Add(this.GetChildLinkMethod(table, associatedLink, childTable, new SqlColumn[] {}, false, target, context));

			// mjr - 20-07-2005 - updated to only do this if we're flagged to...			
			if(associatedLink.GenerateParentAccessMethods)
			{
				// now do all the others...
				foreach(SqlColumn childColumn in childTable.Columns)
				{
					// add...
					methods.Add(this.GetChildLinkMethod(table, associatedLink, childTable, new SqlColumn[] { childColumn }, false, target, context));
					methods.Add(this.GetChildLinkMethod(table, associatedLink, childTable, new SqlColumn[] { childColumn }, true, target, context));
				}
			}

			// return...
			return (CodeMemberMethod[])methods.ToArray(typeof(CodeMemberMethod));
		}

		/// <summary>
		/// Gets a child link method.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="associatedLink"></param>
		/// <param name="additionalColumns"></param>
		/// <returns></returns>
		private CodeMemberMethod GetChildLinkMethod(SqlTable table, SqlChildToParentLink associatedLink, SqlTable childTable, 
			SqlColumn[] additionalColumns, bool addOperatorArguments, MemberTarget target, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(associatedLink == null)
				throw new ArgumentNullException("associatedLink");
			if(childTable == null)
				throw new ArgumentNullException("childTable");
			if(additionalColumns == null)
				throw new ArgumentNullException("additionalColumns");
			if(context == null)
				throw new ArgumentNullException("context");

			// Use an array so we can remove them if they already exist
			ArrayList additional = new ArrayList(additionalColumns);

			// get the name...
			StringBuilder builder = new StringBuilder();
			builder.Append("Get");
			builder.Append(childTable.Name);
			builder.Append("Items");
			if(additionalColumns.Length > 0)
			{
				builder.Append("For");

				// walk...
				for(int index = 0; index < additionalColumns.Length; index++)
				{
					if(index > 0)
						builder.Append("And");
					builder.Append(additionalColumns[index].Name);
				}
			}

			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = builder.ToString();
			method.ReturnType = new CodeTypeReference(this.GetEntityCollectionClassName(childTable));
//			if(!((target & MemberTarget.Stub) == MemberTarget.Stub))
				method.Attributes = childTable.GetMemberAttributes(false, context) | MemberAttributes.Static;
//			else
//				method.Attributes = MemberAttributes.Public | MemberAttributes.Static;

			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				method.Attributes -= MemberAttributes.Static;
				method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));

				if(addOperatorArguments)
					method.Name += "WithOperator";
			}

			if(additionalColumns.Length == 0)
				CodeDomExtender.AddSummaryComment(method, string.Format("Get all of the child '{0}' entities.", this.GetEntityClassName(childTable)));
			else
				CodeDomExtender.AddSummaryComment(method, string.Format("Get the child '{0}' entities constrained by the given parameters.", this.GetEntityClassName(childTable)));

			// concrete?
			IDictionary keyColumnNames = new HybridDictionary();
			SqlColumn[] keyColumns = table.GetKeyColumns();
			if(!((target & MemberTarget.Stub)==MemberTarget.Stub))
			{
				// add the keys...
				foreach(SqlColumn keyColumn in keyColumns)
				{
					// get the name...
					string name = keyColumn.CamelName;
					int index = 0;
					while(true)
					{
						if(index == 0)
							name = keyColumn.CamelName;
						else
							name = string.Format("{0}_{1}", keyColumn.CamelName, index);

						bool found = false;
						foreach(SqlColumn additionalColumn in additionalColumns)
						{
							// check...
							if(additionalColumn.CamelName == name)
							{
								// flag...
								found = true;
								break;
							}
						}

						// next...
						if(found == false)
							break;
						index++;
					}

					// set...
					keyColumnNames[keyColumn] = name;

					// add...
					method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetColumnType(keyColumn), name));
				}
			}

			// parmas...
			foreach(SqlColumn additionalColumn in additionalColumns)
			{
				// add...
				method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetColumnType(additionalColumn), additionalColumn.CamelName));
				if(addOperatorArguments)
					method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(typeof(SqlOperator)), additionalColumn.CamelName + "Operator"));
			}

			switch(target)
			{
				case MemberTarget.Concrete:
				case MemberTarget.Concrete | MemberTarget.WebMethod:
					// defer...
					if(!(additionalColumns == null) || addOperatorArguments)
					{
						// create a filter...
						CodeVariableDeclarationStatement filter = this.CreateSqlFilterStatement(childTable, "filter");
						if(filter == null)
							throw new InvalidOperationException("filter is null.");
						method.Statements.Add(filter);

						// get this table's key columns...
						if(associatedLink.Columns.Count == keyColumns.Length)
						{
							// add constraints...
							CodePropertyReferenceExpression constraints = new CodePropertyReferenceExpression(
								new CodeVariableReferenceExpression(filter.Name), "Constraints");
							for(int index = 0; index < associatedLink.Columns.Count; index++)
							{
								// columns...
								SqlColumn column = associatedLink.Columns[index];
								SqlColumn keyColumn = keyColumns[index];

								// create...
								CodeMethodInvokeExpression add = new CodeMethodInvokeExpression(constraints, "Add");
								add.Parameters.Add(new CodePrimitiveExpression(column.Name));
								add.Parameters.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOperator)), "EqualTo"));
								add.Parameters.Add(new CodeVariableReferenceExpression((string)keyColumnNames[keyColumn]));

								// invoke...
								method.Statements.Add(add);
							}

							// additional...
							foreach(SqlColumn additionalColumn in additionalColumns)
							{
								// create...
								CodeMethodInvokeExpression add = new CodeMethodInvokeExpression(constraints, "Add");
								add.Parameters.Add(new CodePrimitiveExpression(additionalColumn.Name));
								if(addOperatorArguments)
									add.Parameters.Add(new CodeArgumentReferenceExpression(additionalColumn.CamelName + "Operator"));
								else
									add.Parameters.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOperator)), "EqualTo"));
								add.Parameters.Add(new CodeArgumentReferenceExpression(additionalColumn.CamelName));

								// invoke...
								method.Statements.Add(add);
							}

							// return...


							if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
							{
								method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeCastExpression(method.ReturnType, 
									new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(filter.Name), "ExecuteEntityCollection")),GetToWsArrayMethodName(childTable))));

								method.ReturnType = new CodeTypeReference(this.GetEntityWsClassArrayName(childTable));
							}
							else
							{
								method.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(method.ReturnType, 
									new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(filter.Name), "ExecuteEntityCollection"))));
							}
						}
						else
						{
							method.Statements.Add(this.CreateThrowNotImplementedException());
						}
					}
					else
					{
						// defer to the other version...
						method.Statements.Add(this.CreateThrowNotImplementedException());
					}

					break;

				case MemberTarget.Interface:
					break;

				case MemberTarget.Stub:
				case MemberTarget.Stub | MemberTarget.WebMethod:
					// walk...
					string[] propertyNames = new string[keyColumns.Length];
					for(int index = 0; index < keyColumns.Length; index++)
						propertyNames[index] = keyColumns[index].Name;
					
					// add...
					this.AddStubCall(table, method, null, propertyNames,target);

					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", target, target.GetType()));
			}



			// return...
			return method;
		}


		private string GetToWsEntityMethodName(SqlTable table)
		{
			return "To" + GetEntityWsClassName(table);
		}

		private string GetToWsArrayMethodName(SqlTable table)
		{
			return GetToWsEntityMethodName(table) + "Array";
		}

		/// <summary>
		/// Adds a property that can link to the parent.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="link"></param>
		// mbr - 21-09-2007 - added context.
		protected virtual CodeMemberProperty GetChildToParentLinkProperty(SqlTable table, SqlChildToParentLink link, EntityGenerationContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");			
			if(table == null)
				throw new ArgumentNullException("table");
			if(link == null)
				throw new ArgumentNullException("link");

			//			/// <summary>
			//			/// Gets the parent item.
			//			/// </summary>
			//			[EntityLinkToParent("Parent", typeof(UniversalEntity), "ParentId")]
			//			public UniversalEntity Parent
			//			{
			//				get
			//				{
			//					return (UniversalEntity)this.GetParent("Parent");
			//				}
			//				set
			//				{
			//					this.SetParent("Parent", value);
			//				}
			//			}

			// check...
			if(link.ParentTable == null)
				throw new InvalidOperationException("link.ParentTable is null.");
			if(link.Columns == null)
				throw new InvalidOperationException("'link.Columns' is null.");
			if(link.Columns.Count == 0)
				throw new InvalidOperationException("'link.Columns' is zero-length.");
			
			// create a property...
			CodeMemberProperty property = new CodeMemberProperty();
			property.Name = link.Name;
			property.HasSet = false;
			property.Attributes = link.ParentTable.GetMemberAttributes(false, context); // false = "final"
			property.Type = new CodeTypeReference(this.GetEntityClassName(link.ParentTable));
			CodeDomExtender.AddSummaryComment(property, string.Format("Gets the link to '{0}'.", this.GetEntityClassName(link.ParentTable)));
			CodeDomExtender.AddRemarksComment(property, string.Format("This property maps to the '{0}' constraint.", link.NativeName));

			// create parent links...
			CodeArrayCreateExpression array = new CodeArrayCreateExpression(typeof(string));
			foreach(SqlColumn column in link.Columns)
				array.Initializers.Add(new CodePrimitiveExpression(column.Name));

			// attributes...
			CodeAttributeDeclaration attr = new CodeAttributeDeclaration("EntityLinkToParent");
			attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(link.Name)));
			attr.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(link.NativeName)));
			attr.Arguments.Add(new CodeAttributeArgument(new CodeTypeOfExpression(this.GetEntityClassName(link.ParentTable))));
			attr.Arguments.Add(new CodeAttributeArgument(array));
			property.CustomAttributes.Add(attr);

			// get...
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(property.Type, 
				new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "GetParent", 
				new CodePrimitiveExpression(link.Name)))));

			// mbr - 06-09-2007 - case 649 for c7 - this needs to be optional.			
			if(this.GenerateParentPropertySetters)
			{
				property.SetStatements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "SetParent", 
					new CodePrimitiveExpression(link.Name), new CodeArgumentReferenceExpression("value")));
			}

			// return it...
			return property;
		}

		/// <summary>
		/// Adds methods to the type, providing a method with the given signature is not already defined.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="signatures"></param>
		private void AddMethods(CodeTypeDeclaration type, CodeMemberMethod[] methods, ArrayList signatures, string remarks)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(methods == null)
				throw new ArgumentNullException("methods");
			if(signatures == null)
				throw new ArgumentNullException("signatures");

			// walk...
			foreach(CodeMemberMethod method in methods)
				this.AddMethod(type, method, signatures, remarks);
		}

		/// <summary>
		/// Adds methods to the type, providing a method with the given signature is not already defined.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="signatures"></param>
		private void AddMethod(CodeTypeDeclaration type, CodeMemberMethod method, ArrayList signatures, string remarks)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			if(method == null)
				throw new ArgumentNullException("method");
			if(signatures == null)
				throw new ArgumentNullException("signatures");
			
			// add...
			// create a signature...
			StringBuilder builder = new StringBuilder();
			builder.Append(method.Attributes);
			builder.Append(" ");
			builder.Append(method.ReturnType.BaseType);
			builder.Append(" ");
			builder.Append(method.Name);
			builder.Append("(");
			for(int i = 0; i < method.Parameters.Count; i++)
			{
				if(i > 0)
					builder.Append(", ");
				builder.Append(method.Parameters[i].Type.BaseType);
				builder.Append(" ");
				builder.Append(method.Parameters[i].Name);
			}
			builder.Append(")");

			// walk...
			string signature = builder.ToString().ToLower();
			if(signatures.Contains(signature) == false)
			{
				// add...
				type.Members.Add(method);
				signatures.Add(signature);

				// remarks?
				if(remarks != null && remarks.Length > 0)
					CodeDomExtender.AddRemarksComment(method, remarks);
			}
		}

		/// <summary>
		/// Creates a property to access a column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		// mbr - 06-09-2007 - for c7 - made protected, virtual and changed name.		
		protected virtual CodeMemberProperty GetFieldAccessProperty(SqlTable table, SqlColumn column, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(column == null)
				throw new ArgumentNullException("column");
			if(context == null)
				throw new ArgumentNullException("context");			
			
			//			/// <summary>
			//			/// Gets or sets the ID.
			//			/// </summary>
			//			[EntityField("columnId", DbType.Int32, EntityFieldFlags.Key), DBNullEquivalent(0)]
			//			public int Id
			//			{
			//				get
			//				{
			//					return (int)this["Id"];
			//				}
			//				set
			//				{
			//					this["id"] = value;
			//				}
			//			}

			// create a property...
			CodeMemberProperty property = new CodeMemberProperty();
			property.Attributes = column.GetMemberAttributes(false, context);  // false = "final"
			property.Name = column.Name;
			property.Type = this.GetColumnType(column);
			CodeDomExtender.AddSummaryComment(property, string.Format("Gets or sets the value for '{0}'.", column.Name));
			CodeDomExtender.AddRemarksComment(property, string.Format("This property maps to the '{0}' column.", column.NativeName));

			// create the flags expression...
			CodeExpression flagsExpression = this.GetEntityFlagsExpression(column);

			// create the arguments...
			ArrayList attrArguments = new ArrayList();
			attrArguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(column.NativeName)));
			attrArguments.Add(new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DbType)), column.DbType.ToString())));
			attrArguments.Add(new CodeAttributeArgument(flagsExpression));
			if(column.IsFixedLength)
				attrArguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(column.Length)));

			// attribute...
			CodeAttributeDeclaration attr = new CodeAttributeDeclaration("EntityField", (CodeAttributeArgument[])attrArguments.ToArray(typeof(CodeAttributeArgument)));
			property.CustomAttributes.Add(attr);

			// does this column have links against it...
			bool needsDBNullEquivalent = false;
			foreach(SqlChildToParentLink link in table.LinksToParents)
			{
				foreach(SqlColumn linkColumn in link.Columns)
				{
					if(linkColumn == column)
					{
						needsDBNullEquivalent = true;
						break;
					}
				}
			}

			// create the db null equivalent...
			if(needsDBNullEquivalent)
			{
				// create...
				object equivalent = ConversionHelper.GetClrLegalDBNullEquivalent(column.DbType);
				if(equivalent != null && !(equivalent is DBNull))
				{
					// mbr - 2008-09-02 - for some reason there is a problem with decimals...
					if(equivalent is Decimal)
						equivalent = ConversionHelper.ToDouble(equivalent, Cultures.System);

					// set...
					property.CustomAttributes.Add(new CodeAttributeDeclaration("DBNullEquivalent", new CodeAttributeArgument(new CodePrimitiveExpression(equivalent))));
				}
			}

			// mbr - 01-11-2005 - default expression?
			if(column.DefaultExpression != null)
			{
                if (column.DefaultExpression.Type != SqlDatabaseDefaultType.CurrentDateTime)
                {
                    object toUse = column.DefaultExpression.Value;
                    if (toUse is decimal)
                        toUse = ConversionHelper.ToDouble(toUse, Cultures.System);
                    property.CustomAttributes.Add(new CodeAttributeDeclaration("DatabaseDefault", new CodeAttributeArgument(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlDatabaseDefaultType)), column.DefaultExpression.Type.ToString())),
                        new CodeAttributeArgument(new CodePrimitiveExpression(toUse))));
                }
                else
                    property.CustomAttributes.Add(new CodeAttributeDeclaration("DatabaseDefault", new CodeAttributeArgument(new CodeFieldReferenceExpression(
                        new CodeTypeReferenceExpression(typeof(SqlDatabaseDefaultType)), column.DefaultExpression.Type.ToString()))));
			}


			// indexer...
			CodeIndexerExpression indexer = new CodeIndexerExpression(new CodeThisReferenceExpression(), 
				new CodePrimitiveExpression(column.Name));

			// get...
			property.GetStatements.Add(new CodeMethodReturnStatement(new CodeCastExpression(property.Type, indexer)));

			// set...
			property.SetStatements.Add(new CodeAssignStatement(indexer, new CodeArgumentReferenceExpression("value")));

			// return...
			return property;
		}

		/// <summary>
		/// Gets the flags for the given column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		private CodeExpression GetEntityFlagsExpression(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			
			// combine...
			CodeExpression expression = this.CombineEntityFlags(column, null, EntityFieldFlags.Key);
			expression = this.CombineEntityFlags(column, expression, EntityFieldFlags.Nullable);
			expression = this.CombineEntityFlags(column, expression, EntityFieldFlags.Common);
			expression = this.CombineEntityFlags(column, expression, EntityFieldFlags.Large);
			expression = this.CombineEntityFlags(column, expression, EntityFieldFlags.AutoIncrement);

			// null?
			if(expression == null)
				expression = this.GetSingleEntityFlagReference(EntityFieldFlags.Normal);

			// return...
			return expression;
		}

		/// <summary>
		/// Combines an entity flag into an existing expression.
		/// </summary>
		/// <param name="flagsExpression"></param>
		/// <param name="flag"></param>
		/// <returns></returns>
		private CodeExpression CombineEntityFlags(SqlColumn column, CodeExpression flagsExpression, EntityFieldFlags flag)
		{
			if(column == null)
				throw new ArgumentNullException("column");
			
			// get the one to add...
			CodeExpression newFlagExpression = null;
			if(column.GetEntityFieldFlags(flag))
				newFlagExpression = this.GetSingleEntityFlagReference(flag);

			// combine...
			return CodeDomExtender.CombineFlagsExpressions(flagsExpression, newFlagExpression);
		}

		/// <summary>
		/// Gets the reference to a single flag.
		/// </summary>
		/// <param name="flags"></param>
		/// <returns></returns>
		private CodeFieldReferenceExpression GetSingleEntityFlagReference(EntityFieldFlags flag)
		{
			return new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(EntityFieldFlags)), flag.ToString());
		}

		/// <summary>
		/// Gets the constructor for a base entity.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeConstructor CreateNoOpConstructor(MemberAttributes attributes)
		{
			// creates an empty constructor...
			CodeConstructor constructor = new CodeConstructor();
			constructor.Attributes = attributes;
			CodeDomExtender.AddSummaryComment(constructor, "Constructor.");

			// return...
			return constructor;
		}

		/// <summary>
		/// Gets the constructor for a base entity.
		/// </summary>
		/// <returns></returns>
		private CodeConstructor CreateNoOpInitializeConstructor(MemberAttributes attributes)
		{
			// creates an empty constructor...
			CodeConstructor constructor = new CodeConstructor();
			constructor.Attributes = attributes;
			CodeDomExtender.AddSummaryComment(constructor, "Constructor.");

			constructor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(),"InitializeComponent",new CodeExpression[] {}));

			// return...
			return constructor;
		}

		/// <summary>
		/// Creates a deserialization constructor.
		/// </summary>
		/// <returns></returns>
		private CodeConstructor CreateDeserializationConstructor()
		{
			// create...
			CodeConstructor constructor = new CodeConstructor();
			constructor.Attributes = MemberAttributes.Family;
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(SerializationInfo), "info"));
			constructor.Parameters.Add(new CodeParameterDeclarationExpression(typeof(StreamingContext), "context"));
			CodeDomExtender.AddSummaryComment(constructor, "Deserialization constructor.");

			// base...
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("info"));
			constructor.BaseConstructorArgs.Add(new CodeArgumentReferenceExpression("context"));

			// return...
			return constructor;
		}

		/// <summary>
		/// Creates a mehod that serializes the entity into WsEntity.
		/// </summary>
		/// <returns></returns>
		private CodeMemberMethod CreateToWsEntityMethod(SqlTable table)
		{
			// create...
			CodeMemberMethod toWsEntityMethod = new CodeMemberMethod();
			toWsEntityMethod.Name = GetToWsEntityMethodName(table);
			toWsEntityMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Override;
			toWsEntityMethod.ReturnType = new CodeTypeReference(GetEntityWsClassName(table));

			CodeDomExtender.AddSummaryComment(toWsEntityMethod, string.Format("Gets the 'Ws{0} of this entity'.", table.Name));

			CodeVariableDeclarationStatement declWsEntity = new CodeVariableDeclarationStatement(GetEntityWsClassName(table),"wsEntity",new CodeObjectCreateExpression(GetEntityWsClassName(table)));
			toWsEntityMethod.Statements.Add(declWsEntity);

			CodeMethodInvokeExpression invokeToWsEntity = new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(),"PopulateWsEntity",new CodeVariableReferenceExpression(declWsEntity.Name));
			toWsEntityMethod.Statements.Add(invokeToWsEntity);

			// loop the columns...
			foreach(SqlColumn column in table.Columns)
			{
				if(!column.Generate)
					continue;

				// create a property...

				CodeAssignStatement assignment = new CodeAssignStatement();
				assignment.Left = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(declWsEntity.Name),column.Name);
				assignment.Right = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),column.Name);
				
				if(assignment == null)
					throw new InvalidOperationException("assignment is null.");
				toWsEntityMethod.Statements.Add(assignment);
			}

			toWsEntityMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(declWsEntity.Name)));

			// return...
			return toWsEntityMethod;
		}

		/// <summary>
		/// Creates a method that serializes the entities in a collection into a WsEntity[].
		/// </summary>
		/// <returns></returns>
		private CodeMemberMethod CreateToWsEntityArrayMethod(SqlTable table)
		{
			// create...
			CodeMemberMethod toWsArrayMethod = new CodeMemberMethod();
			toWsArrayMethod.Name = GetToWsArrayMethodName(table);
			toWsArrayMethod.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Override;
			toWsArrayMethod.ReturnType = new CodeTypeReference(GetEntityWsClassArrayName(table));

			CodeDomExtender.AddSummaryComment(toWsArrayMethod, string.Format("Gets a 'Ws{0}' array of items for this collection.", table.Name));

			CodeVariableDeclarationStatement declWsEntity = new CodeVariableDeclarationStatement(toWsArrayMethod.ReturnType,"wsEntities");
			CodeMethodInvokeExpression createArray = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(Array)),"CreateInstance");
			createArray.Parameters.Add(new CodeTypeOfExpression(GetEntityWsClassName(table)));
			createArray.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Count"));
			declWsEntity.InitExpression = new CodeCastExpression(toWsArrayMethod.ReturnType,createArray);

			toWsArrayMethod.Statements.Add(declWsEntity);

			CodeVariableDeclarationStatement index = new CodeVariableDeclarationStatement(typeof(int), "index", new CodePrimitiveExpression(0) );
			toWsArrayMethod.Statements.Add(index);

			// Creates a for loop that sets testInt to 0 and continues incrementing testInt by 1 each loop until testInt is not less than 10.
			CodeIterationStatement forLoop = new CodeIterationStatement();
			forLoop.InitStatement =	new CodeAssignStatement(new CodeVariableReferenceExpression("index"), new CodePrimitiveExpression(0));
			forLoop.TestExpression = new CodeBinaryOperatorExpression( new CodeVariableReferenceExpression("index"),CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(new CodeThisReferenceExpression(),"Count"));
			forLoop.IncrementStatement = new CodeAssignStatement( new CodeVariableReferenceExpression("index"), new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("index"), CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1)));

			CodeAssignStatement assignItemToArray = new CodeAssignStatement();
			assignItemToArray.Left = new CodeIndexerExpression(new CodeVariableReferenceExpression(declWsEntity.Name),new CodeVariableReferenceExpression("index"));
			assignItemToArray.Right = new CodeMethodInvokeExpression(new CodeIndexerExpression(new CodeThisReferenceExpression(),new CodeVariableReferenceExpression("index")),GetToWsEntityMethodName(table));
			forLoop.Statements.Add(assignItemToArray);

			toWsArrayMethod.Statements.Add(forLoop);
			toWsArrayMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(declWsEntity.Name)));

			// return...
			return toWsArrayMethod;
		}

		/// <summary>
		/// Gets the name of the entity class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected string GetEntityClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return table.Name;
		}

		/// <summary>
		/// Gets the name of the webservice entity class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private string GetEntityWsClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return "Ws" + table.Name;
		}

		/// <summary>
		/// Gets the name of the webservice entity base class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private string GetEntityWsBaseClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return "Ws" + table.Name + "Base";
		}

		/// <summary>
		/// Gets the name of the entity base class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected string GetEntityBaseClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			if(this.TargetVersion == DotNetVersion.V1)
				return table.Name + "Base";
			else
				return table.Name;
		}

		/// <summary>
		/// Gets the name of the entity web service base class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private string GetDtoBaseClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return table.Name + "DtoBase";
		}

		/// <summary>
		/// Gets the name of the entity web service class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private string GetDtoClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return table.Name + "Dto";
		}

		/// <summary>
		/// Gets the name of the entity base class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected string GetEntityCollectionClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return table.Name + "Collection";
		}

	
		/// <summary>
		/// Gets the name of the entity base class.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected string GetEntityCollectionBaseClassName(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return table.Name + "CollectionBase";
		}

		/// <summary>
		/// Gets or sets the defaultnamespacename
		/// </summary>
		public string DefaultNamespaceName
		{
			get
			{
				return _defaultNamespaceName;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _defaultNamespaceName)
				{
					// set the value...
					_defaultNamespaceName = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the BaseType
		/// </summary>
		public string EntityBaseTypeName
		{
			get
			{
				if(_entityBaseTypeName != null && _entityBaseTypeName != string.Empty)
					return _entityBaseTypeName;
				else
					return DefaultEntityBaseType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _entityBaseTypeName)
				{
					// set the value...
					_entityBaseTypeName = value;
				}
			}
		}

		/// <summary>
		/// Gets or sets the ServiceBaseType
		/// </summary>
		public string DtoBaseTypeName
		{
			get
			{
				if(_dtoBaseTypeName != null && _dtoBaseTypeName != string.Empty)
					return _dtoBaseTypeName;
				else
					return DefaultDtoBaseType;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _dtoBaseTypeName)
				{
					// set the value...
					_dtoBaseTypeName = value;
				}
			}
		}

		/// <summary>
		/// Creates a method to allow the creation of an entity.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberMethod GetSetPropertyMethod(SqlTable table, MemberTarget target)
		{
			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "SetProperties";

			if((target & MemberTarget.Concrete)==MemberTarget.Concrete)
				method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;	
		
			// We have a web method so remove the static and add the attribute
			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				method.Attributes = MemberAttributes.Public | MemberAttributes.FamilyOrAssembly;
				//method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));
			}

			CodeDomExtender.AddSummaryComment(method, string.Format("Sets properties on an instance of '{0}'.", table.Name));
			
			CodeMethodInvokeExpression getById = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.DefaultNamespaceName + "." + GetEntityClassName(table)),"GetById");
			CodeVariableDeclarationStatement entity = new CodeVariableDeclarationStatement(GetEntityClassName(table),"entity", getById );
			method.Statements.Add(entity);

			// loop the columns...
			foreach(SqlColumn column in table.Columns)
			{
				if(column.IsKey == true)
				{
					getById.Parameters.Add(new CodeArgumentReferenceExpression( column.CamelName));
					method.Parameters.Add(new CodeParameterDeclarationExpression(column.Type,column.CamelName));
				}
			}

			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string[]),"propertyNames"));
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(object[]),"propertyValues"));

			CodeMethodInvokeExpression invokeSetProperties = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(entity.Name),"SetProperties");
			invokeSetProperties.Parameters.Add(new CodeVariableReferenceExpression(entity.Name));
			invokeSetProperties.Parameters.Add(new CodeArgumentReferenceExpression("propertyNames"));
			invokeSetProperties.Parameters.Add(new CodeArgumentReferenceExpression("propertyValues"));

			method.Statements.Add(invokeSetProperties);
			method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(entity.Name),"SaveChanges"));

			return method;
		}


		/// <summary>
		/// Creates a method to allow the creation of an entity.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberMethod GetCreateInstanceMethod(SqlTable table, MemberTarget target)
		{
			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "CreateInstance";

			method.Attributes = MemberAttributes.Public | MemberAttributes.FamilyOrAssembly;
			//method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));
			method.ReturnType = new CodeTypeReference(GetEntityWsClassName(table));

			CodeDomExtender.AddSummaryComment(method, string.Format("Creates and instance of '{0}'.", table.Name));
			
			CodeVariableDeclarationStatement entity = new CodeVariableDeclarationStatement(GetEntityClassName(table),"entity", new CodeObjectCreateExpression(GetEntityClassName(table)));
			method.Statements.Add(entity);

			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
				method.Statements.Add(new CodeMethodReturnStatement(GetInvokeWrapForWebMethod(new CodeVariableReferenceExpression(entity.Name),table,false)));
			else
				method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(entity.Name)));

			return method;
		}

		/// <summary>
		/// Creates a method to allow the update of an entity.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeMemberMethod GetUpdateInstanceMethod(SqlTable table)
		{
			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "UpdateInstance";

			// We have a web method so remove the static and add the attribute
			method.Attributes = MemberAttributes.Public | MemberAttributes.FamilyOrAssembly;
			//method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));
			method.Parameters.Add(new CodeParameterDeclarationExpression(new CodeTypeReference(GetEntityWsClassName(table)),"wsEntity"));
			method.ReturnType = new CodeTypeReference(GetEntityWsClassName(table));

			CodeDomExtender.AddSummaryComment(method, string.Format("Updates an instance of '{0}'.", table.Name));
			
			CodeVariableDeclarationStatement entity = new CodeVariableDeclarationStatement(GetEntityClassName(table),"entity", new CodePrimitiveExpression(null));
			method.Statements.Add(entity);

			CodeMethodInvokeExpression invokePopulate = new CodeMethodInvokeExpression();
			invokePopulate.Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression("wsEntity"),"PopulateEntity");
			invokePopulate.Parameters.Add(new CodeTypeOfExpression(GetEntityClassName(table)));

			method.Statements.Add(new CodeAssignStatement(new CodeVariableReferenceExpression("entity"),new CodeCastExpression(GetEntityClassName(table),invokePopulate)));
			method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(entity.Name),"SaveChanges"));

			CodeMethodInvokeExpression webMethodInvoke = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("entity"),GetToWsEntityMethodName(table));
			method.Statements.Add(new CodeMethodReturnStatement(webMethodInvoke));

			return method;
		}

		/// <summary>
		/// Creates a method to return all entities of the given type.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod GetGetAllMethod(SqlTable table, MemberTarget target, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			//	SqlFilter filter = SqlFilter.CreateGetAllFilter(typeof(Dbtbmbooking));
			//	this.dataGrid1.DataSource = (DbtbmbookingCollection)filter.ExecuteEntityCollection();

			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "GetAll";
			if(!((target & MemberTarget.Stub)==MemberTarget.Stub))
				method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			else
				method.Attributes = MemberAttributes.Public | MemberAttributes.Static;

			method.ReturnType = new CodeTypeReference(this.GetEntityCollectionClassName(table));
			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				method.Attributes -= MemberAttributes.Static;
				method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));
				method.ReturnType = new CodeTypeReference(GetEntityWsClassArrayName(table));
			}

			CodeDomExtender.AddSummaryComment(method, string.Format("Get all <see cref=\"{0}\"/> entities.", this.GetEntityClassName(table)));

			switch(target)
			{
				case MemberTarget.Concrete:
				case MemberTarget.Concrete | MemberTarget.WebMethod:

					// create...
					CodeVariableDeclarationStatement filter = new CodeVariableDeclarationStatement(typeof(SqlFilter), "filter");
					method.Statements.Add(filter);
					filter.InitExpression = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(SqlFilter)), 
						"CreateGetAllFilter", new CodeExpression[] { this.GetEntityTypeOfExpression(table) });

					// return...


					if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
						method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeCastExpression(this.GetEntityCollectionClassName(table), 
							new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("filter"), "ExecuteEntityCollection")),GetToWsArrayMethodName(table))));
					else
						method.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(this.GetEntityCollectionClassName(table), 
							new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("filter"), "ExecuteEntityCollection"))));

					break;

				case MemberTarget.Interface:
					break;

				case MemberTarget.Stub:
				case MemberTarget.Stub | MemberTarget.WebMethod:
					this.AddStubCall(table, method, null, null,target);
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", target, target.GetType()));
			}

			// return...
			return method;
		}

		/// <summary>
		/// Creates a stub call.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="servicePropertyName"></param>
		private void AddStubCall(SqlTable table, CodeMemberMethod method, string deferName, string[] shimNames, MemberTarget target)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(method == null)
				throw new ArgumentNullException("method");

			// defer...
			if(deferName == null || deferName.Length == 0)
				deferName = method.Name;
			
			// create a invoke call...
			method.Statements.Add(new CodeCommentStatement("defer..."));
			CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression();
			invoke.Method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(GetEntityClassName(table)), deferName);

			// shim?
			if(shimNames != null && shimNames.Length > 0)
			{
				// add...
				foreach(string shimName in shimNames)
					invoke.Parameters.Add(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), shimName));
			}

			// walk the properties...
			foreach(CodeParameterDeclarationExpression param in method.Parameters)
				invoke.Parameters.Add(new CodeVariableReferenceExpression(param.Name));

			// return?
			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				if(method.ReturnType.ArrayRank == 1)
					method.Statements.Add(new CodeMethodReturnStatement(GetInvokeWrapForWebMethod(invoke,table,true)));
				else
					method.Statements.Add(new CodeMethodReturnStatement(GetInvokeWrapForWebMethod(invoke,table,false)));
			}
			else
			{
				if(method.ReturnType != null)
					method.Statements.Add(new CodeMethodReturnStatement(invoke));
				else
					method.Statements.Add(invoke);
			}
		}

		/// <summary>
		/// Creates a method to return all entities of the given type.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod[] GetGetByIdMethods(SqlTable table, MemberTarget target, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
		
			//			/// <summary>
			//			/// Gets the <see cref="Dbtbmbooking"> entity with the given ID.
			//			/// </summary>
			//			public static Dbtbmbooking GetById(int bookingId)
			//			{
			//				Iridium.IridiFX.Data.SqlFilter filter = Iridium.IridiFX.Data.SqlFilter.CreateGetByIdFilter(typeof(Dbtbmbooking), bookingId);
			//				return ((Dbtbmbooking)(filter.ExecuteEntity()));
			//			}

			// get the key columns...
			SqlColumn[] keyColumns = table.GetKeyColumns();
			if(keyColumns == null)
				throw new InvalidOperationException("keyColumns is null.");
			if(keyColumns.Length == 0)
				return new CodeMemberMethod[] {};

			// if we're stub, we need to do the cache checking...
			ArrayList methods = new ArrayList();
			if((target & MemberTarget.Stub)==MemberTarget.Stub)
			{
				methods.Add(this.GetGetByIdWrapper(table, false));
				methods.Add(this.GetGetByIdWrapper(table, true));
				methods.Add(this.GetGetByIdUsingCacheMethod(table));
			}

			// return...
			for(int loop = 0; loop < 2; loop++)
			{
				bool addOnNotFound = false;
				if(loop == 1)
					addOnNotFound = true;

				// set...
				string methodName = "GetById";
				if((target & MemberTarget.Stub)==MemberTarget.Stub)
					methodName = "GetByIdInternal";
				CodeMemberMethod method = this.CreateEntityFilterEqualToMethod(table, methodName, "GetById", string.Format("Gets the <see cref=\"{0}\"/> entity with the given ID.", 
					this.GetEntityClassName(table)), null, keyColumns, false, addOnNotFound, false, target);
				if((target & MemberTarget.Stub)==MemberTarget.Stub)
					method.Attributes = MemberAttributes.Private | MemberAttributes.Static;
				methods.Add(method);

				// remarks...
				string remarks = null;
				if((target & MemberTarget.Stub)==MemberTarget.Stub)
					remarks = "This overload will always bypass the cache.  If you want to use the cache, use the overload that does not take operator arguments.";
				methods.Add(this.CreateEntityFilterWhereMethod(table, "GetById", string.Format("Gets the <see cref=\"{0}\"/> entity where the ID matches the given specification.",
					this.GetEntityClassName(table)), remarks, keyColumns, false, addOnNotFound, false, target));
				//				methods[(loop * 2) + 1].Attributes = MemberAttributes.Private | MemberAttributes.Static;
			}

			// return...
			return (CodeMemberMethod[])methods.ToArray(typeof(CodeMemberMethod));
		}

		private CodeMemberMethod GetGetByIdWrapper(SqlTable table, bool includeOnNotFound)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// methods...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "GetById";
			method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			method.ReturnType = new CodeTypeReference(this.GetEntityClassName(table));
			CodeDomExtender.AddSummaryComment(method, "Gets the entity with the given ID.");
			CodeDomExtender.AddRemarksComment(method, "This method checks to see if the entity type supports caching and, if so, defers to the cache first.");

			// get the params...
			SqlColumn[] keyColumns = table.GetKeyColumns();
			if(keyColumns == null)
				throw new InvalidOperationException("'keyColumns' is null.");
			if(keyColumns.Length == 0)
				throw new InvalidOperationException("'keyColumns' is zero-length.");

			// args...
			CodeExpression[] argReferences = new CodeExpression[keyColumns.Length + 1];
			for(int index = 0; index < keyColumns.Length; index++)
			{
				method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetColumnType(keyColumns[index]), keyColumns[index].CamelName));
				argReferences[index] = new CodeArgumentReferenceExpression(keyColumns[index].CamelName);
			}

			if(includeOnNotFound)
			{
				// one more arg...
				method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(OnNotFound), "onNotFound"));
				argReferences[argReferences.Length - 1] = new CodeArgumentReferenceExpression("onNotFound");

				// entity type...
				CodeVariableDeclarationStatement entityType = new CodeVariableDeclarationStatement(typeof(EntityType), "entityType");
				method.Statements.Add(entityType);
				entityType.InitExpression = this.GetGetEntityTypeMethod(table);

				// args...
				CodeConditionStatement condition = new CodeConditionStatement();
				method.Statements.Add(condition);
				condition.Condition = new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(entityType.Name), 
					"Cache"), CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
				condition.TrueStatements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.GetEntityClassName(table)), "GetByIdUsingCache", argReferences)));
				condition.FalseStatements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.GetEntityClassName(table)), "GetByIdInternal", argReferences)));
			}
			else
			{
				// update...
				argReferences[argReferences.Length - 1] = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(OnNotFound)), "ReturnNull");
				
				// defer...
				method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.GetEntityClassName(table)), "GetById", 
					argReferences)));
			}

			// return...
			return method;
		}

		private CodeMemberMethod GetGetByIdUsingCacheMethod(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");

			// methods...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "GetByIdUsingCache";
			method.ReturnType = new CodeTypeReference(this.GetEntityClassName(table));
			method.Attributes = MemberAttributes.Private | MemberAttributes.Static;
			CodeDomExtender.AddSummaryComment(method, "Gets the entity with the given ID using the cache for this entity type.");

			// get the params...
			SqlColumn[] keyColumns = table.GetKeyColumns();
			if(keyColumns == null)
				throw new InvalidOperationException("'keyColumns' is null.");
			if(keyColumns.Length == 0)
				throw new InvalidOperationException("'keyColumns' is zero-length.");

			// args...
			CodeArgumentReferenceExpression[] argReferences = new CodeArgumentReferenceExpression[keyColumns.Length];
			for(int index = 0; index < keyColumns.Length; index++)
			{
				method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetColumnType(keyColumns[index]), keyColumns[index].CamelName));
				argReferences[index] = new CodeArgumentReferenceExpression(keyColumns[index].CamelName);
			}
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(OnNotFound), "onNotFound"));

			CodeVariableDeclarationStatement entityType = new CodeVariableDeclarationStatement(typeof(EntityType), "entityType");
			method.Statements.Add(entityType);
			entityType.InitExpression = this.GetGetEntityTypeMethod(table);

			// ref...
			CodeExpression cacheReference = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(entityType.Name), "Cache");
			
			// load it...
			CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement(this.GetEntityClassName(table), "result");
			method.Statements.Add(result);
			result.InitExpression = new CodeCastExpression(this.GetEntityClassName(table), new CodeMethodInvokeExpression(cacheReference, "GetEntity", new CodeVariableReferenceExpression("entityType"), new CodeArrayCreateExpression(typeof(object), argReferences), 
				new CodeArgumentReferenceExpression("onNotFound")));

			// result...
			CodeVariableReferenceExpression resultReference = new CodeVariableReferenceExpression(result.Name);

			// check it...
			CodeConditionStatement check = new CodeConditionStatement();
			method.Statements.Add(check);
			check.Condition = new CodeBinaryOperatorExpression(resultReference, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null));
			check.TrueStatements.Add(new CodeAssignStatement(resultReference, new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.GetEntityClassName(table)), "GetByIdInternal", argReferences)));
			CodeConditionStatement check2 = new CodeConditionStatement();
			check.TrueStatements.Add(check2);
			check2.Condition = new CodeBinaryOperatorExpression(resultReference, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null));
			check2.TrueStatements.Add(new CodeSnippetStatement("entityType.Cache.Add(result);"));

			// return...
			method.Statements.Add(new CodeMethodReturnStatement(resultReference));

			// return...
			return method;
		}

		/// <summary>
		/// Creates a set of methods that filters against the given column.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="name"></param>
		/// <param name="columns"></param>
		/// <param name="returnsCollection"></param>
		/// <returns></returns>
		private CodeMemberMethod CreateEntityFilterEqualToMethod(SqlTable table, string name, string deferName, string summary, string remarks, 
			SqlColumn[] columns, bool returnsCollection, bool addOnNotFound, bool deferToNotFound, MemberTarget target)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(columns == null)
				throw new ArgumentNullException("columns");
			if(columns.Length == 0)
				throw new ArgumentOutOfRangeException("'columns' is zero-length.");

			//			/// <summary>
			//			/// Gets entities where Alpha2code is equal to the given value.
			//			/// </summary>
			//			/// <remarks>
			//			/// Based on the <c>CLIX_DBTBMCOUNTRIES_Alpha2Code</c> index.
			//			/// </remarks>
			//			public static DbtbmcountryCollection GetByAlpha2code(string alpha2code)
			//			{
			//				return GetWhereAlpha2Code(alpha2code, SqlOperator.EqualTo);
			//			}

			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = name;
			if(!((target & MemberTarget.Stub)==MemberTarget.Stub))
				method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			else
				method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			if(returnsCollection)
				method.ReturnType = new CodeTypeReference(this.GetEntityCollectionClassName(table));
			else
				method.ReturnType = new CodeTypeReference(this.GetEntityClassName(table));

			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				method.Attributes -= MemberAttributes.Static;
				method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));

				if(addOnNotFound)
					method.Name += "WithOnNotFound";

				if(returnsCollection)
					method.ReturnType = new CodeTypeReference(GetEntityWsClassArrayName(table));
				else
					method.ReturnType = new CodeTypeReference(this.GetEntityWsClassName(table));
			}

			// xml...
			CodeDomExtender.AddSummaryComment(method, summary);
			CodeDomExtender.AddRemarksComment(method, remarks);
			CodeDomExtender.AddFxComment(method, string.Format("CreateEntityFilterEqualToMethod - {0}", table));

			foreach(SqlColumn column in columns)
				method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetColumnType(column), column.CamelName));

			// add...
			if(addOnNotFound)
				method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(OnNotFound), "onNotFound"));

			switch(target)
			{
				case MemberTarget.Concrete:
				case MemberTarget.Concrete | MemberTarget.WebMethod:


					// add params...
					CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(this.DefaultNamespaceName + "." + GetEntityClassName(table)), deferName);
					for(int index = 0; index < columns.Length; index++)
					{
						CodeFieldReferenceExpression equalTo = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOperator)), "EqualTo");
						if(deferToNotFound)
							equalTo = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(OnNotFound)), "ReturnNull");

						SqlColumn column = columns[index];

						// add...
						bool addEqualTo = false;
						if(returnsCollection)
							addEqualTo = true;
						else if(index == columns.Length - 1 || column.IsKey)
							addEqualTo = true;

						// invoke...
						invoke.Parameters.Add(new CodeArgumentReferenceExpression(column.CamelName));
						if(addEqualTo)
							invoke.Parameters.Add(equalTo);
					}

					// onnotfound?
					if(addOnNotFound)
						invoke.Parameters.Add(new CodeArgumentReferenceExpression("onNotFound"));

					// return...

					// If we have a webmethod we must first return the correct type
					if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
					{
						CodeMethodInvokeExpression webMethodInvoke = new CodeMethodInvokeExpression(invoke,GetToWsEntityMethodName(table));
						if(returnsCollection)
							webMethodInvoke.Method.MethodName = GetToWsArrayMethodName(table);

						method.Statements.Add(new CodeMethodReturnStatement(webMethodInvoke));
					}
					else
						method.Statements.Add(new CodeMethodReturnStatement(invoke));

					break;

				case MemberTarget.Interface:
					break;

				case MemberTarget.Stub:
				case MemberTarget.Stub | MemberTarget.WebMethod:
					this.AddStubCall(table, method, deferName, null,target);
					break;
			
				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", target, target.GetType()));
			}

			// return...
			return method;
		}

		private string GetEntityWsClassArrayName(SqlTable table)
		{
			return this.GetEntityWsClassName(table) + "[]";
		}

		/// <summary>
		/// Creates a set of methods that filters against the given column.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="name"></param>
		/// <param name="columns"></param>
		/// <param name="returnsCollection"></param>
		/// <returns></returns>
		private CodeMemberMethod CreateEntityFilterWhereMethod(SqlTable table, string name, string summary, string remarks, 
			SqlColumn[] columns, bool returnsCollection, bool addOnNotFound, bool assumeEqualTo, MemberTarget target)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(columns == null)
				throw new ArgumentNullException("columns");
			if(columns.Length == 0)
				throw new ArgumentOutOfRangeException("'columns' is zero-length.");

			//			/// <summary>
			//			/// Gets entities where Alpha2code matches the given value according to the operator.
			//			/// </summary>
			//			/// <remarks>
			//			/// Based on the <c>CLIX_DBTBMCOUNTRIES_Alpha2Code</c> index.
			//			/// </remarks>
			//			public static DbtbmcountryCollection GetWhereAlpha2Code(string alpha2code, SqlOperator alpha2codeOperator)
			//			{
			//				Iridium.IridiFX.Data.SqlFilter filter = new Iridium.IridiFX.Data.SqlFilter(typeof(Dbtbmcountry));
			//				filter.Constraints.Add("Alpha2code", alpha2codeOperator, alpha2code);
			//				return ((DbtbmcountryCollection)(filter.ExecuteEntityCollection()));
			//			}

			// create...
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = name;
			if(!((target & MemberTarget.Stub)==MemberTarget.Stub))
				method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;
			else
				method.Attributes = MemberAttributes.Public | MemberAttributes.Static;
			if(returnsCollection)
				method.ReturnType = new CodeTypeReference(this.GetEntityCollectionClassName(table));
			else
				method.ReturnType = new CodeTypeReference(this.GetEntityClassName(table));

			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				method.Attributes -= MemberAttributes.Static;
				method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));

				if(!assumeEqualTo && addOnNotFound)
					method.Name += "WithOperatorAndOnNotFound";
				else if(!assumeEqualTo && !addOnNotFound)
					method.Name += "WithOperator";
				else if(addOnNotFound)
					method.Name += "WithOnNotFound";
			}

			// xml...
			CodeDomExtender.AddSummaryComment(method, summary);
			CodeDomExtender.AddRemarksComment(method, remarks);

			// add params...
			foreach(SqlColumn column in columns)
			{
				method.Parameters.Add(new CodeParameterDeclarationExpression(this.GetColumnType(column), column.CamelName));
				if(!(assumeEqualTo))
					method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(SqlOperator), column.CamelName + "Operator"));
			}

			// add...
			if(addOnNotFound)
				method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(OnNotFound), "onNotFound"));

			switch(target)
			{
				case MemberTarget.Concrete:
				case MemberTarget.Concrete | MemberTarget.WebMethod:
					// create...
					CodeVariableDeclarationStatement filter = this.CreateSqlFilterStatement(table, "filter");
					if(filter == null)
						throw new InvalidOperationException("filter is null.");
					method.Statements.Add(filter);

					// add constraints...
					CodePropertyReferenceExpression constraintsProperty = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("filter"), "Constraints");
					foreach(SqlColumn column in columns)
					{
						// operator...
						CodeExpression operatorReference = new CodeArgumentReferenceExpression(column.CamelName + "Operator");
						if(assumeEqualTo)
							operatorReference = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(SqlOperator)), 
								"EqualTo");

						// create an add calll...
						CodeMethodInvokeExpression addMethod = new CodeMethodInvokeExpression(constraintsProperty, "Add", 
							new CodeExpression[] { new CodePrimitiveExpression(column.Name), 
													 operatorReference, 
													 new CodeArgumentReferenceExpression(column.CamelName) } );

						// invoke it...
						method.Statements.Add(addMethod);
					}

					// return...
					string methodName = "ExecuteEntity";
					if(returnsCollection)
						methodName = "ExecuteEntityCollection";

					// invoke...
					CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("filter"), methodName);

					// last bit...
					if(addOnNotFound)
					{
						// mbr - 27-06-2006 - results name...
						string[] checkNames = new string[columns.Length];
						for(int index = 0; index < checkNames.Length; index++)
							checkNames[index] = columns[index].CamelName;
						string resultsName = CodeDomExtender.GetNextIdentifierName("results", checkNames);
						if(resultsName == null)
							throw new InvalidOperationException("'resultsName' is null.");
						if(resultsName.Length == 0)
							throw new InvalidOperationException("'resultsName' is zero-length.");

						// handle...
						CodeVariableDeclarationStatement result = new CodeVariableDeclarationStatement(method.ReturnType, resultsName, 
							new CodeCastExpression(method.ReturnType, invoke));
						method.Statements.Add(result);
							
						// handler...
						this.AddOnNotFoundHandler(method, result.Name, "onNotFound",table, target,returnsCollection);
					}
					else
					{
						// return...

						// If we have a webmethod we must first return the correct type
						if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
						{
							CodeMethodInvokeExpression webMethodInvoke = GetInvokeWrapForWebMethod(new CodeCastExpression(method.ReturnType, invoke), table, returnsCollection);

							method.Statements.Add(new CodeMethodReturnStatement(webMethodInvoke));
						}
						else
							method.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(method.ReturnType, invoke)));
					}

					break;

				case MemberTarget.Interface:
					break;

				case MemberTarget.Stub:
				case MemberTarget.Stub | MemberTarget.WebMethod:
					this.AddStubCall(table, method, null, null,target);
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", target, target.GetType()));
			}


			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				if(returnsCollection)
					method.ReturnType = new CodeTypeReference(this.GetEntityWsClassArrayName(table));
				else
					method.ReturnType = new CodeTypeReference(this.GetEntityWsClassName(table));
			}
			
			// return...
			return method;
		}

		private CodeMethodInvokeExpression GetInvokeWrapForWebMethod(CodeExpression expression, SqlTable table, bool returnsCollection)
		{
			CodeMethodInvokeExpression webMethodInvoke = new CodeMethodInvokeExpression(expression,GetToWsEntityMethodName(table));
			if(returnsCollection)
				webMethodInvoke.Method.MethodName = GetToWsArrayMethodName(table);

			return webMethodInvoke;
		}

		/// <summary>
		/// Adds an 'on not found' handler to the given method.
		/// </summary>
		/// <param name="variableName"></param>
		/// <param name="onNotFoundVariableName"></param>
		private void AddOnNotFoundHandler(CodeMemberMethod method, string variableName, string onNotFoundVariableName, SqlTable table, MemberTarget target, bool returnsCollection)
		{
			if(method == null)
				throw new ArgumentNullException("method");
			if(variableName == null)
				throw new ArgumentNullException("variableName");
			if(variableName.Length == 0)
				throw new ArgumentOutOfRangeException("'variableName' is zero-length.");
			if(onNotFoundVariableName == null)
				throw new ArgumentNullException("onNotFoundVariableName");
			if(onNotFoundVariableName.Length == 0)
				throw new ArgumentOutOfRangeException("'onNotFoundVariableName' is zero-length.");
			
			// if...

			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
				method.Statements.Add(new CodeMethodReturnStatement(GetInvokeWrapForWebMethod(new CodeVariableReferenceExpression(variableName),table,returnsCollection)));
			else
				method.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression(variableName)));
		}

		/// <summary>
		/// Creates a statement that throws a 'not implemented' exception.
		/// </summary>
		/// <returns></returns>
		private CodeThrowExceptionStatement CreateThrowNotImplementedException()
		{
			return new CodeThrowExceptionStatement(new CodeObjectCreateExpression(typeof(NotImplementedException)));
		}

		/// <summary>
		/// Gets an 'typeof' expression for the given entity.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeTypeOfExpression GetEntityTypeOfExpression(SqlTable table)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// return...
			return new CodeTypeOfExpression(this.GetEntityClassName(table));
		}

		/// <summary>
		/// Gets a statement that creates a <see cref="SqlFilter"></see> variable.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		private CodeVariableDeclarationStatement CreateSqlFilterStatement(SqlTable table, string variableName)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(variableName == null)
				throw new ArgumentNullException("variableName");
			if(variableName.Length == 0)
				throw new ArgumentOutOfRangeException("'variableName' is zero-length.");
			
			// create...
			return new CodeVariableDeclarationStatement(typeof(SqlFilter), variableName, 
				new CodeObjectCreateExpression(typeof(SqlFilter), new CodeExpression[] { new CodeTypeOfExpression(this.GetEntityClassName(table)) }));
		}

		/// <summary>
		/// Gets the method for the given index.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod[] GetIndexMethods(SqlTable table, SqlIndex index, MemberTarget target, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(index == null)
				throw new ArgumentNullException("index");

			// are all the columns key columns?
			int numKey = 0;
			foreach(SqlColumn column in index.Columns)
			{
				if(column.IsKey)
					numKey++;
			}
			if(numKey == index.Columns.Count)
				return new CodeMemberMethod[] {};

			// multiple?  if we don't have unique values, we support multiple values...
			bool multiple = !(index.HasUniqueValues);

			// defer...
			CodeMemberMethod[] methods = this.GetColumnMethods(table, index.Columns.ToArray(), multiple, target, context);
			if(methods == null)
				throw new InvalidOperationException("methods is null.");
			return methods;
		}

		/// <summary>
		/// Gets the method for the given column.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private CodeMemberMethod[] GetColumnMethods(SqlTable table, SqlColumn column, bool returnsCollection, MemberTarget target, 
			EntityGenerationContext context)
		{
			// defer...
			return this.GetColumnMethods(table, new SqlColumn[] { column }, returnsCollection, target, context);
		}

		/// <summary>
		/// Gets the method for the given column.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod[] GetColumnMethods(SqlTable table, SqlColumn[] columns, bool returnsCollection, MemberTarget target,
			EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(columns == null)
				throw new ArgumentNullException("columns");
			if(columns.Length == 0)
				throw new ArgumentOutOfRangeException("'columns' is zero-length.");

			// are all the columns key columns?
			int numKey = 0;
			foreach(SqlColumn column in columns)
			{
				if(column.IsKey)
					numKey++;
			}
			if(numKey == columns.Length)
				return new CodeMemberMethod[] {};
			
			// builders...
			StringBuilder equalToName = new StringBuilder();
			StringBuilder whereName = new StringBuilder();
			StringBuilder equalToSummary = new StringBuilder();
			StringBuilder whereSummary = new StringBuilder();

			// make up a name...
			equalToName.Append("GetBy");
			whereName.Append("GetBy");
			if(returnsCollection)
			{
				equalToSummary.Append("Gets entities where ");
				whereSummary.Append("Gets entities where ");
			}
			else
			{
				equalToSummary.Append("Gets an entity where ");
				whereSummary.Append("Gets an entity where ");
			}
			for(int i = 0; i < columns.Length; i++)
			{
				if(i > 0)
				{
					equalToName.Append("And");
					whereName.Append("And");

					// summary text...
					if(i == columns.Length - 1)
					{
						equalToSummary.Append(" and ");
						whereSummary.Append(" and ");
					}
					else
					{
						equalToSummary.Append(", ");
						whereSummary.Append(", ");
					}
				}
				equalToName.Append(columns[i].Name);
				whereName.Append(columns[i].Name);
				equalToSummary.Append(columns[i].Name);
				whereSummary.Append(columns[i].Name);
			}
			if(columns.Length > 1)
				equalToSummary.Append(" are equal to the given values.");
			else
				equalToSummary.Append(" is equal to the given value.");
			whereSummary.Append(" matches the given specification.");

			// create...
			ArrayList methods = new ArrayList();
			//			methods.Add(this.CreateEntityFilterEqualToMethod(table, equalToName.ToString(), whereName.ToString(), equalToSummary.ToString(), 
			//				null, columns, returnsCollection, false, target));
			if(!(returnsCollection))
			{
				methods.Add(this.CreateEntityFilterEqualToMethod(table, equalToName.ToString(), whereName.ToString(), equalToSummary.ToString(), 
					null, columns, returnsCollection, false, true, target));
				methods.Add(this.CreateEntityFilterWhereMethod(table, equalToName.ToString(), equalToSummary.ToString(), 
					null, columns, returnsCollection, true, true, target));
			}
			else
			{
				methods.Add(this.CreateEntityFilterEqualToMethod(table, equalToName.ToString(), whereName.ToString(), equalToSummary.ToString(), 
					null, columns, returnsCollection, false, false, target));
				methods.Add(this.CreateEntityFilterWhereMethod(table, whereName.ToString(), whereSummary.ToString(), 
					null, columns, returnsCollection, false, false, target));
				methods.Add(this.CreateEntityFilterWhereMethod(table, whereName.ToString(), whereSummary.ToString(),  
					null, columns, returnsCollection, true, false, target));
			}

			// return...
			return (CodeMemberMethod[])methods.ToArray(typeof(CodeMemberMethod));
		}

		/// <summary>
		/// Gets the type for the given column.
		/// </summary>
		/// <param name="column"></param>
		/// <returns></returns>
		private CodeTypeReference GetColumnType(SqlColumn column)
		{
			if(column == null)
				throw new ArgumentNullException("column");

			// get...
			if(column.HasEnumerationTypeName)
				return new CodeTypeReference(column.EnumerationTypeName);
			else
				return new CodeTypeReference(ConversionHelper.GetClrTypeForDBType(column.DbType));
		}

		/// <summary>
		/// Compiles the given table to an assembly.
		/// </summary>
		/// <returns></returns>
		public Assembly CompileToAssembly(string namespaceName, SqlTable table, EntityGenerationContext context)
		{
			if(namespaceName == null)
				throw new ArgumentNullException("namespaceName");
			if(namespaceName.Length == 0)
				throw new ArgumentOutOfRangeException("'namespaceName' is zero-length.");
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// defer...
			return this.CompileToAssembly(namespaceName, new SqlTable[] { table }, context);
		}

		/// <summary>
		/// Compiles the given table to an assembly.
		/// </summary>
		/// <returns></returns>
		// mbr - 21-09-2007 - context.		
		public Assembly CompileToAssembly(string namespaceName, SqlTable[] tables, EntityGenerationContext context)
		{
			if(namespaceName == null)
				throw new ArgumentNullException("namespaceName");
			if(namespaceName.Length == 0)
				throw new ArgumentOutOfRangeException("'namespaceName' is zero-length.");
			if(tables == null)
				throw new ArgumentNullException("tables");
			if(tables.Length == 0)
				throw new ArgumentOutOfRangeException("'tables' is zero-length.");
			if(context == null)
				throw new ArgumentNullException("context");

			// create a namespace...
			CodeNamespace ns = this.GetNamespace(namespaceName, context);

			// remove the custom enumerations...
			IDictionary[] customEnumerations = new IDictionary[tables.Length];
			if(this.CompilationOptions.AllowCustomEnumerations == false)
			{
				// walk the tables...
				for(int index = 0; index < tables.Length; index++)
				{
					// walk...
					foreach(SqlColumn column in tables[index].Columns)
					{
						if(column.HasEnumerationTypeName)
						{
							// check...
							if(customEnumerations[index] == null)
								customEnumerations[index] = new HybridDictionary();

							// set...
							customEnumerations[index][column] = column.EnumerationTypeName;
							column.EnumerationTypeName = null;
						}
					}				
				}
			}

			// try...
			try
			{
				// loop...
				foreach(SqlTable table in tables)
				{
					// try...
					var classes = this.GetEntityClasses(table, context);
					if(classes == null)
						throw new InvalidOperationException("'classes' is null.");
					if(classes.Length == 0)
						throw new InvalidOperationException("'classes' is zero-length.");

					// add..
                    foreach (var c in classes)
                        ns.Types.Add(c.CodeItem);
				}

				// create a compile unit...
				var unit = this.GetCompileUnit(new GeneratedNamespace(ns), context);
				if(unit == null)
					throw new InvalidOperationException("unit is null.");

				// compile it...
				return CodeDomExtender.Compile(unit, this.CompilationOptions.CompilerOptions, Language.CSharp);
			}
			finally
			{
				// reset the custom enumeations...
				for(int index = 0; index < tables.Length; index++)
				{
					// custom...
					if(customEnumerations[index] != null)
					{
						// walk...
						SqlTable table = tables[index];
						foreach(DictionaryEntry entry in customEnumerations[index])
							((SqlColumn)entry.Key).EnumerationTypeName = (string)entry.Value;
					}
				}
			}
		}

		/// <summary>
		/// Gets the entity classes for the given table.
		/// </summary>
		/// <param name="table"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
        public GeneratedTypeDeclaration[] GetEntityClasses(SqlTable table, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");
			
			// create...
            var types = new GeneratedTypeDeclaration[6];
			types[0] = this.GetEntityBaseClass(table, context);
			types[1] = this.GetEntityClass(table, context);
			types[2] = this.GetEntityCollectionBaseClass(table, context);
			types[3] = this.GetEntityCollectionClass(table, context);
			types[4] = this.GetEntityWsBaseClass(table, context);
			types[5] = this.GetEntityWsClass(table, context);

			// return...
			return types;
		}

		/// <summary>
		/// Gets the language.
		/// </summary>
		private Language Language
		{
			get
			{
				// returns the value...
				return _language;
			}
		}

		/// <summary>
		/// Gets the compilationoptions.
		/// </summary>
		public EntityCompilationOptions CompilationOptions
		{
			get
			{
				return _compilationOptions;
			}
		}

		/// <summary>
		/// Gets search methods.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		protected virtual CodeMemberMethod[] GetSearchMethods(SqlTable table, MemberTarget target, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			
			// create the declaration...
			CodeMemberMethod method = new CodeMemberMethod();
			method.ReturnType = new CodeTypeReference(this.GetEntityCollectionClassName(table));
			method.Parameters.Add(new CodeParameterDeclarationExpression(typeof(string), "terms"));
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final | MemberAttributes.Static;

			if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
			{
				method.Attributes -= MemberAttributes.Static;
				method.CustomAttributes.Add(new CodeAttributeDeclaration("WebMethod"));
				method.ReturnType = new CodeTypeReference(GetEntityWsClassArrayName(table));
			}

			method.Name = "Search";
			CodeDomExtender.AddSummaryComment(method, string.Format("Searches for <see cref=\"{0}\"/> items with the given terms.", this.GetEntityClassName(table)));

			switch(target)
			{
				case MemberTarget.Interface:
					break;

				case MemberTarget.Concrete:
				case MemberTarget.Concrete | MemberTarget.WebMethod:

					//					SqlSearcher searcher = new SqlSearcher(typeof(Foo), terms);
					//					return (FooCollection)searcher.ExecuteEntityCollection();

					// create...
					CodeVariableDeclarationStatement var = new CodeVariableDeclarationStatement(typeof(SqlSearcher), "searcher",
						new CodeObjectCreateExpression(typeof(SqlSearcher), new CodeTypeOfExpression(this.GetEntityClassName(table)), 
						new CodeArgumentReferenceExpression("terms")));
					method.Statements.Add(var);

					// run...

					if((target & MemberTarget.WebMethod)==MemberTarget.WebMethod)
						method.Statements.Add(new CodeMethodReturnStatement(new CodeMethodInvokeExpression(new CodeCastExpression(GetEntityCollectionClassName(table), 
							new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(var.Name), "ExecuteEntityCollection")),GetToWsArrayMethodName(table))));
					else
						method.Statements.Add(new CodeMethodReturnStatement(new CodeCastExpression(this.GetEntityCollectionClassName(table), 
							new CodeMethodInvokeExpression(new CodeVariableReferenceExpression(var.Name), "ExecuteEntityCollection"))));

					break;

				case MemberTarget.Stub:
				case MemberTarget.Stub | MemberTarget.WebMethod:
					this.AddStubCall(table, method, null, null,target);
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", target, target.GetType()));
			}

			// return...
			return new CodeMemberMethod[] { method };
		}

		/// <summary>
		/// Gets the given compile unit.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
		public GeneratedTypeDeclaration GetCompileUnit(ISqlProgrammable obj, EntityCodeFileType type, EntityGenerationContext context)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(context == null)
				throw new ArgumentNullException("context");
	
			// what is it?
			if(obj is SqlTable)
				return this.GetCompileUnit((SqlTable)obj, type, context);
			else if(obj is SqlProcedure)
				return this.GetCompileUnit((SqlProcedure)obj, type, context);
			else
				throw new InvalidOperationException(string.Format("The type '{0}' is not supported.", obj));
		}

		/// <summary>
		/// Gets the given compile unit.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
        private GeneratedTypeDeclaration GetCompileUnit(SqlProcedure proc, EntityCodeFileType type, EntityGenerationContext context)
		{
			if(proc == null)
				throw new ArgumentNullException("proc");
			if(context == null)
				throw new ArgumentNullException("context");

			switch(type)
			{
				case EntityCodeFileType.Entity:
                    return this.GetEntityClass(proc, context);
				case EntityCodeFileType.EntityBase:
                    return this.GetEntityBaseClass(proc, context);

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}
		}

		private string GetEntityClassName(SqlProcedure proc)
		{
			if(proc == null)
				throw new ArgumentNullException("proc");
			
			return proc.Name + "Statement";
		}

		private string GetEntityBaseClassName(SqlProcedure proc)
		{
			if(proc == null)
				throw new ArgumentNullException("proc");
			
			return this.GetEntityClassName(proc) + "Base";
		}

		// mbr - 21-09-2007 - added context.		
		public GeneratedTypeDeclaration GetEntityClass(SqlProcedure proc, EntityGenerationContext context)
		{
			if(proc == null)
				throw new ArgumentNullException("proc");
			if(context == null)
				throw new ArgumentNullException("context");

			// create...
			CodeTypeDeclaration type = new CodeTypeDeclaration(this.GetEntityClassName(proc));
			CodeDomExtender.AddSummaryComment(type, string.Format("Provides access to the <c>{0}</c> procedure.", proc.Name));
			type.BaseTypes.Add(this.GetEntityBaseClassName(proc));

			// cons...
			CodeConstructor cons = new CodeConstructor();
			type.Members.Add(cons);
            CodeDomExtender.AddSummaryComment(cons, "Creates a new instance of the object.");
			cons.Attributes = MemberAttributes.Public;

			// return...
            return new GeneratedTypeDeclaration(type);
		}

		// mbr - 21-09-2007 - added context.		
		public GeneratedTypeDeclaration GetEntityBaseClass(SqlProcedure proc, EntityGenerationContext context)
		{
			if(proc == null)
				throw new ArgumentNullException("proc");
			if(context == null)
				throw new ArgumentNullException("context");

			// create...
			CodeTypeDeclaration type = new CodeTypeDeclaration(this.GetEntityBaseClassName(proc));
            CodeDomExtender.AddSummaryComment(type, string.Format("Base class for providing access to the <c>{0}</c> procedure.", proc.Name));
            CodeDomExtender.AddRemarksComment(type, string.Format("SQL: {0}", proc.Body));
			type.BaseTypes.Add(typeof(SqlStatement));
			type.Attributes = MemberAttributes.Abstract | MemberAttributes.Public;

			// constructor - this sets up the parameters...
			CodeConstructor cons = new CodeConstructor();
            CodeDomExtender.AddSummaryComment(cons, "Constructs the object and configures parameters for the procedure.");
			type.Members.Add(cons);
			cons.Attributes = MemberAttributes.Family;
			cons.BaseConstructorArgs.Add(new CodePrimitiveExpression("[" + proc.NativeName + "]"));
			cons.BaseConstructorArgs.Add(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(CommandType)), "StoredProcedure"));

			// walk the args...
			cons.Statements.Add(new CodeCommentStatement("set up the parameters..."));
			foreach(SqlStatementParameter param in proc.Parameters)
			{
				cons.Statements.Add(this.GetParameterCreateStatement(param));
				type.Members.Add(this.GetParameterAccessProperty(param));
			}

			// return...
            return new GeneratedTypeDeclaration(type);
		}

		private CodeMemberProperty GetParameterAccessProperty(SqlStatementParameter param)
		{
			if(param == null)
				throw new ArgumentNullException("param");
			
			// create...
			CodeMemberProperty prop = new CodeMemberProperty();
			CodeDomExtender.AddSummaryComment(prop, string.Format("Provides access to the <c>{0}</c> parameter, type=<c>{1}</c>, default=<c>{2}</c>, direction=<c>{3}</c>.", 
				param.Name, param.DBType, param.Value, param.Direction));
			prop.Name = CodeDomHelper.GetPascalName(CodeDomHelper.SanitizeName(param.Name));
			prop.Type = new CodeTypeReference(typeof(SqlStatementParameter));
			prop.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			
			// get...
			prop.GetStatements.Add(new CodeMethodReturnStatement(new CodeIndexerExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Parameters"), 
				new CodePrimitiveExpression(param.Name))));

			// return...
			return prop;
		}

		private CodeMethodInvokeExpression GetParameterCreateStatement(SqlStatementParameter param)
		{
			if(param == null)
				throw new ArgumentNullException("param");
			
			// default...
			CodeExpression defValue = null;
			if(param.Value == null || param.Value is DBNull)
				defValue = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DBNull)), "Value");
			else
				defValue = new CodePrimitiveExpression(param.Value);

			// create...
			return new CodeMethodInvokeExpression(new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "Parameters"), "Add", 
				new CodeObjectCreateExpression(typeof(SqlStatementParameter), new CodePrimitiveExpression(param.Name), 
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(DbType)), param.DBType.ToString()), defValue,
				new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(typeof(ParameterDirection)), param.Direction.ToString())));
		}

		/// <summary>
		/// Gets the given compile unit.
		/// </summary>
		/// <param name="table"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		// mbr - 21-09-2007 - added context.		
        public GeneratedTypeDeclaration GetCompileUnit(SqlTable table, EntityCodeFileType type, EntityGenerationContext context)
		{
			if(table == null)
				throw new ArgumentNullException("table");
			if(context == null)
				throw new ArgumentNullException("context");

			switch(type)
			{
				case EntityCodeFileType.Entity:
					return this.GetEntityClass(table, context);
				case EntityCodeFileType.EntityBase:
                    return this.GetEntityBaseClass(table, context);

				case EntityCodeFileType.Collection:
                    return this.GetEntityCollectionClass(table, context);
                case EntityCodeFileType.CollectionBase:
                    return this.GetEntityCollectionBaseClass(table, context);

                case EntityCodeFileType.Dto:
                    return this.GetDtoClass(table, context);
                case EntityCodeFileType.DtoBase:
                    return this.GetDtoBaseClass(table, context);

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", type, type.GetType()));
			}
		}

		/// <summary>
		/// Gets the targetversion.
		/// </summary>
		private DotNetVersion TargetVersion
		{
			get
			{
				// returns the value...
				return _targetVersion;
			}
		}

		/// <summary>
		/// Private field to support <see cref="DefaultEntityBaseType"/> property.
		/// </summary>
		private static string _defaultEntityBaseType = "BootFX.Common.Entities.Entity";

		/// <summary>
		/// Private field to support <see cref="DefaultDtoBaseType"/> property.
		/// </summary>
		private static string _defaultWebServiceBaseType = "BootFX.Common.Dto.DtoBase";
		
		/// <summary>
		/// Gets the defaultwebservicebasetype.
		/// </summary>
		public static string DefaultDtoBaseType
		{
			get
			{
				return _defaultWebServiceBaseType;
			}
		}
		
		/// <summary>
		/// Gets the defaultentitybasetype.
		/// </summary>
		public static string DefaultEntityBaseType
		{
			get
			{
				return _defaultEntityBaseType;
			}
		}
		
		/// <summary>
		/// Gets or sets the generateParentPropertySetters
		/// </summary>
		// mbr - 06-09-2007 - case 649 for c7 - added.		
		public bool GenerateParentPropertySetters
		{
			get
			{
				return _generateParentPropertySetters;
			}
			set
			{
				// check to see if the value has changed...
				if(value != _generateParentPropertySetters)
				{
					// set the value...
					_generateParentPropertySetters = value;
				}
			}
		}
	}
}
