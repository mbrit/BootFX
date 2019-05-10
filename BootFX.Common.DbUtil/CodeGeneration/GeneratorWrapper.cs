// BootFX - Application framework for .NET applications
// 
// File: GeneratorWrapper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Defines a class that wraps generation.
	/// </summary>
	/// <remarks>This class was added as the CodeDOM interfaces have been deprecated in .NET 2.0.</remarks>
	internal class GeneratorWrapper
	{
		private Language _language;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="language"></param>
		internal GeneratorWrapper(Language language)
		{
			_language = language;
		}

		internal void GenerateCodeFromType(CodeTypeDeclaration type, TextWriter writer, CodeGeneratorOptions options)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (writer == null)
				throw new ArgumentNullException("writer");

			switch (Language)
			{
				case Language.CSharp:
					new CSharpCodeProvider().GenerateCodeFromType(type, writer, options);
					return;

				default:
					throw new InvalidOperationException(string.Format("Cannot handle '{0}'.", this.Language));
			}
		}

		internal void GenerateCodeFromNamespace(CodeNamespace ns, TextWriter writer, CodeGeneratorOptions options)
		{
			if (ns == null)
				throw new ArgumentNullException("ns");
			if (writer == null)
				throw new ArgumentNullException("writer");

			switch (Language)
			{
				case Language.CSharp:
					new CSharpCodeProvider().GenerateCodeFromNamespace(ns, writer, options);
					return;

				default:
					throw new InvalidOperationException(string.Format("Cannot handle '{0}'.", this.Language));
			}
		}

		internal void GenerateCodeFromExpression(CodeExpression expression, TextWriter writer, CodeGeneratorOptions options)
		{
			if (expression == null)
				throw new ArgumentNullException("expression");
			if (writer == null)
				throw new ArgumentNullException("writer");

			switch (Language)
			{
				case Language.CSharp:
					new CSharpCodeProvider().GenerateCodeFromExpression(expression, writer, options);
					return;

				default:
					throw new InvalidOperationException(string.Format("Cannot handle '{0}'.", this.Language));
			}
		}

		internal void GenerateCodeFromStatement(CodeStatement statement, TextWriter writer, CodeGeneratorOptions options)
		{
			if (statement == null)
				throw new ArgumentNullException("statement");
			if (writer == null)
				throw new ArgumentNullException("writer");

			switch (Language)
			{
				case Language.CSharp:
					new CSharpCodeProvider().GenerateCodeFromStatement(statement, writer, options);
					return;

				default:
					throw new InvalidOperationException(string.Format("Cannot handle '{0}'.", this.Language));
			}
		}

		internal void GenerateCodeFromCompileUnit(CodeCompileUnit unit, TextWriter writer, CodeGeneratorOptions options)
		{
			if (unit == null)
				throw new ArgumentNullException("unit");
			if (writer == null)
				throw new ArgumentNullException("writer");

			switch (Language)
			{
				case Language.CSharp:
					new CSharpCodeProvider().GenerateCodeFromCompileUnit(unit, writer, options);
					return;

				default:
					throw new InvalidOperationException(string.Format("Cannot handle '{0}'.", this.Language));
			}
		}

		private Language Language
		{
			get
			{
				return _language;
			}
		}
	}
}
