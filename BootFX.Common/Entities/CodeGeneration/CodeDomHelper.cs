// BootFX - Application framework for .NET applications
// 
// File: CodeDomHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.CSharp;
using BootFX.Common.Entities;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Helper class for CodeDOM.
	/// </summary>
	public sealed class CodeDomHelper
	{
		/// <summary>
		/// Private field to support <c>Regex</c> property.
		/// </summary>
		private static Regex _csPartialRegex = new Regex(@"^(?<pre>\s*)(?<access>(public|internal))\s+(?<type>(class|interface))\b", RegexOptions.Multiline | RegexOptions.IgnoreCase);

		/// <summary>
		/// Private field to support <c>IdRegex</c> property.
		/// </summary>
		private static Regex _idRegex;
		
		/// <summary>
		/// Private field to support <c>AcronymRegex</c> property.
		/// </summary>
		private static Regex _acronymRegex;
		
		/// <summary>
		/// Constructor,
		/// </summary>
		private CodeDomHelper()
		{
		}

		/// <summary>
		/// Static constructor.
		/// </summary>
		static CodeDomHelper()
		{
			_idRegex = new Regex(@"(?<pre>\w*)id", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			_acronymRegex = new Regex(@"(?<acronym>[A-Z]{3,})", RegexOptions.Singleline);
		}

		/// <summary>
		/// Gets the acronymregex.
		/// </summary>
		private static Regex AcronymRegex
		{
			get
			{
				// returns the value...
				return _acronymRegex;
			}
		}

		/// <summary>
		/// Gets the text of the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToString(CodeStatement statement, Language language)
		{
			CodeTypeDeclaration type = new CodeTypeDeclaration("Foo");
			CodeMemberMethod method = new CodeMemberMethod();
			type.Members.Add(method);
			method.Name = "Bar";
			method.Statements.Add(statement);

			// return...
			//return ToString(type, language);
            throw new NotImplementedException();
		}

		/// <summary>
		/// Gets the text of the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToString(GeneratedCompileUnit unit, Language language)
		{
			if(unit == null)
				throw new ArgumentNullException("unit");
			
			// return...
			return ToStringInternal(unit, language);
		}

		/// <summary>
		/// Gets the text of the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string ToString(GeneratedNamespace ns, Language language)
		{
			if(ns == null)
				throw new ArgumentNullException("ns");

			// return...
			return ToStringInternal(ns, language);
		}

		/// <summary>
		/// Gets the text of the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns> 
		public static string ToString(GeneratedTypeDeclaration type, Language language)
		{
			if(type == null)
				throw new ArgumentNullException("type");

			// return...
			return ToStringInternal(type, language);
		}
		
		/// <summary>
		/// Gets the text of the type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string ToStringInternal(IGeneratedCode codeObject, Language language)
		{
			if(codeObject == null)
				throw new ArgumentNullException("codeObject");

			// opens...
			CodeGeneratorOptions options = GetDefaultGeneratorOptions(language);
			if(options == null)
				throw new InvalidOperationException("options is null.");

			// get it...
			GeneratorWrapper generator = new GeneratorWrapper(language);
			using(StringWriter writer = new StringWriter())
			{
				// run...
				if (codeObject is GeneratedTypeDeclaration)
					generator.GenerateCodeFromType(((GeneratedTypeDeclaration)codeObject).CodeItem, writer, options);
                else if (codeObject is GeneratedNamespace)
                    generator.GenerateCodeFromNamespace(((GeneratedNamespace)codeObject).CodeItem, writer, options);
                //else if (codeObject is CodeExpression)
                //    generator.GenerateCodeFromExpression((CodeExpression)codeObject, writer, options);
                //else if (codeObject is CodeStatement)
                //    generator.GenerateCodeFromStatement((CodeStatement)codeObject, writer, options);
				else if (codeObject is GeneratedCompileUnit)
                    generator.GenerateCodeFromCompileUnit(((GeneratedCompileUnit)codeObject).CodeItem, writer, options);
				else
					throw new NotSupportedException(string.Format("Cannot handle '{0}'.", codeObject.GetType()));

				// return...
				writer.Flush();

                // replace problems where codedom does `1 instead of <Foo>...
				var code = writer.GetStringBuilder().ToString();
                foreach (string key in codeObject.GenericMappings.Keys)
                    code = code.Replace(key, codeObject.GenericMappings[key]);
                return code;
			}
		}

		/// <summary>
		/// Gets a code generator.
		/// </summary>
		/// <param name="language"></param>
		/// <returns></returns>
		private static CodeGeneratorOptions GetDefaultGeneratorOptions(Language language)
		{
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			switch(language)
			{
				case Language.CSharp:
					options.BracingStyle = "C";
					break;

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", language, language.GetType()));
			}

			// return...
			return options;
		}

		/// <summary>
		/// Adds a summary comment.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="comment"></param>
		public static void AddSummaryComment(CodeTypeMember member, string comment)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			if(GetAttributeForMethod(member,"WebMethod") != null)
			{
				// Slightly hacky but it will do for the moment
				string description = comment.Replace("<see cref=\"","");
				description = description.Replace("\"/>","");
				GetAttributeForMethod(member,"WebMethod").Arguments.Add(new CodeAttributeArgument("Description",new CodePrimitiveExpression(description)));
			}

			// add...
			AddXmlComment(member, "summary", comment);
		}

		public static CodeAttributeDeclaration GetAttributeForMethod(CodeTypeMember member, string attributeName)
		{
			foreach(CodeAttributeDeclaration attribute in member.CustomAttributes)
			{
				if(attribute.Name == attributeName)
					return attribute;
			}

			return null;
		}

		/// <summary>
		/// Adds a remarks comment.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="comment"></param>
		public static void AddFxComment(CodeTypeMember member, string comment)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			// add...
			AddXmlComment(member, "bootfx", comment);
		}

		/// <summary>
		/// Adds a remarks comment.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="comment"></param>
		public static void AddRemarksComment(CodeTypeMember member, string comment)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			
			// add...
			AddXmlComment(member, "remarks", comment);
		}

		/// <summary>
		/// Adds a remarks comment.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="comment"></param>
		private static void AddXmlComment(CodeTypeMember member, string name, string comment)
		{
			if(member == null)
				throw new ArgumentNullException("member");
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(comment == null || comment.Length == 0)
				return;
			
			// add...
			member.Comments.Add(new CodeCommentStatement("<" + name + ">", true));
			member.Comments.Add(new CodeCommentStatement(comment, true));
			member.Comments.Add(new CodeCommentStatement("</" + name + ">", true));
		}

		/// <summary>
		/// Suggests a Pascal case name.
		/// </summary>
		/// <param name="name"></param>
		public static string GetPascalName(string name)
		{
			if(name == null || name.Length == 0)
				return string.Empty;

			try
			{
				// is the name all upper case or all lower case?
				int numUpper = 0;
				int numLower = 0;
				int numNonChar = 0;
				foreach(char c in name)
				{
					if(char.IsLetter(c))
					{
						if(char.IsUpper(c))
							numUpper++;
						else
							numLower++;
					}
					else
						numNonChar++;
				}

				// check...
				if((numUpper == name.Length - numNonChar) || (numLower == name.Length - numNonChar))
					return GetPascalNameForSingleCaseName(name);
				else
					return GetPascalNameForMixedCaseName(name);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to get Pascal version of '{0}'.", name), ex);
			}
		}

		/// <summary>
		/// Gets the Pascal case name for mixed case name, e.g. XMLDocument.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetPascalNameForMixedCaseName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");

			// match them...
			AcronymContext context = new AcronymContext();
			context.Name = name;
			name = AcronymRegex.Replace(name, new MatchEvaluator(context.Evaluate));

			// set the first upper case...
			name = name.Substring(0, 1).ToUpper() + name.Substring(1);
			return name;
		}

		/// <summary>
		/// Context for use with <see cref="GetPascalNameForMixedCaseName"></see>.
		/// </summary>
		private class AcronymContext
		{
			public string Name;

			/// <summary>
			/// Evaluates a match.
			/// </summary>
			/// <param name="match"></param>
			/// <returns></returns>
			public string Evaluate(Match match)
			{
				if(match == null)
					throw new ArgumentNullException("match");
				
				// eval...
				string value = match.Value;
				if(match.Index + match.Length == this.Name.Length)
				{
					value = value.Substring(0, 1).ToUpper() + value.Substring(1, value.Length - 1).ToLower();
				}
				else
				{
					value = value.Substring(0, 1).ToUpper() + value.Substring(1, value.Length - 2).ToLower() + value.Substring(value.Length - 1);
				}

				// return...
				return value;
			}
		}

		/// <summary>
		/// Gets the Pascal case name for ALL UPPER or all lower names.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private static string GetPascalNameForSingleCaseName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// return...
			name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

			// look for an id match...
			Match match = IdRegex.Match(name);
			if(match.Success)
			{
				// mangle the ID...
				name = match.Groups["pre"].Value + "Id";
			}

			// return...
			return name;
		}

		/// <summary>
		/// Suggests a camel case name.
		/// </summary>
		/// <param name="name"></param>
		public static string GetCamelName(string name)
		{
			// get...
			name = GetPascalName(name);
			if(name == null || name.Length == 0)
				return name;

			// return...
			return name.Substring(0, 1).ToLower() + name.Substring(1);
		}

		/// <summary>
		/// Gets the idregex.
		/// </summary>
		private static Regex IdRegex
		{
			get
			{
				// returns the value...
				return _idRegex;
			}
		}

		/// <summary>
		/// Combines the two expressions.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static CodeExpression CombineFlagsExpressions(CodeExpression left, CodeExpression right)
		{
			if(left == null && right == null)
				return null;
			if(left == null)
				return right;
			if(right == null)
				return left;

			// actually do a combine...
			return new CodeBinaryOperatorExpression(left, CodeBinaryOperatorType.BitwiseOr, right);
		}

		/// <summary>
		/// Compiles the given unit.
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
		public static Assembly Compile(GeneratedCompileUnit unit, Language language)
		{
			return Compile(unit, GetDefaultCompilerOptions(language), language);
		}

		/// <summary>
		/// Compiles the given unit.
		/// </summary>
		/// <param name="unit"></param>
		/// <returns></returns>
        public static Assembly Compile(GeneratedCompileUnit wrapper, CompilerParameters options, Language language)
		{
            //if (wrapper == null)
            //    throw new ArgumentNullException("wrapper");
            //if(options == null)
            //    throw new ArgumentNullException("options");

            //// compile it...
            //CompilerResults results = null;
            //switch(language)
            //{
            //    case Language.CSharp:
            //        results = new CSharpCodeProvider().CompileAssemblyFromDom(options, unit);
            //        break;

            //    default:
            //        throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", language, language.GetType()));
            //}
            //if(results == null)
            //    throw new InvalidOperationException("results is null.");

            //// errors?
            //if(results.Errors.Count > 0)
            //{
            //    // save it...
            //    string filePath = Runtime.Current.GetTempFilePath(GetCodeFileExtension(language));
            //    if(filePath == null)
            //        throw new InvalidOperationException("'filePath' is null.");
            //    if(filePath.Length == 0)
            //        throw new InvalidOperationException("'filePath' is zero-length.");
            //    Save(filePath, unit, language);

            //    // throw...
            //    throw new CompilationException("Failed to compile code unit.", results, filePath, null);
            //}

            //// return...
            //return results.CompiledAssembly;
            throw new NotImplementedException();
		}

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="unit"></param>
		/// <param name="language"></param>
		public static void Save(string filePath, GeneratedCompileUnit unit, Language language)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			// write it...
			using(StreamWriter writer = new StreamWriter(filePath))
			{
				// saves the file...
				Save(writer, unit, language);
			}
		}	

		/// <summary>
		/// Saves the file.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="unit"></param>
		/// <param name="language"></param>
        public static void Save(TextWriter writer, GeneratedCompileUnit unit, Language language)
		{
			if(writer == null)
				throw new ArgumentNullException("writer");
			if(unit == null)
				throw new ArgumentNullException("unit");

			// write it...
			writer.Write(ToString(unit, language));
		}			

		/// <summary>
		/// Gets the extension for the given language.
		/// </summary>
		/// <param name="language"></param>
		/// <returns></returns>
		private static string GetCodeFileExtension(Language language)
		{
			switch(language)
			{
				case Language.CSharp:
					return ".cs";

				default:
					throw new NotSupportedException(string.Format("Cannot handle '{0}' ({1}).", language, language.GetType()));
			}
		}

		/// <summary>
		/// Gets compiler options.
		/// </summary>
		/// <param name="language"></param>
		/// <returns></returns>
		public static CompilerParameters GetDefaultCompilerOptions(Language language)
		{
			// create...
			CompilerParameters options = new CompilerParameters();

			// add references...
			options.ReferencedAssemblies.Add(GetAssemblyReference(typeof(object)));
			options.ReferencedAssemblies.Add(GetAssemblyReference(typeof(IListSource)));
			options.ReferencedAssemblies.Add(GetAssemblyReference(typeof(DataTable)));
			options.ReferencedAssemblies.Add(GetAssemblyReference(typeof(XmlDocument)));
			options.ReferencedAssemblies.Add(GetAssemblyReference(typeof(WebService)));
			// us...
			options.ReferencedAssemblies.Add(GetAssemblyReference(typeof(Entity)));

			// return...
			return options;
		}

		/// <summary>
		/// Gets the assembly reference for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string GetAssemblyReference(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			return GetAssemblyReference(type.Assembly);
		}

		/// <summary>
		/// Gets the assembly reference for the given type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string GetAssemblyReference(Assembly asm)
		{
			if(asm == null)
				throw new ArgumentNullException("asm");
			
			// get...
			return new Uri(asm.CodeBase).LocalPath;
		}

		/// <summary>
		/// Adds the <c>partial</c> keyword to the declaration in the given type.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		public static string AddPartialKeyword(string code, Language language)
		{
			if(code == null)
				throw new ArgumentNullException("code");
			if(code.Length == 0)
				throw new ArgumentOutOfRangeException("'code' is zero-length.");
		
			// replace...
			if(CsPartialRegex == null)
				throw new InvalidOperationException("csPartialRegex is null.");
			return CsPartialRegex.Replace(code, new MatchEvaluator(CsPartialEvaluator));
		}
		
		private static string CsPartialEvaluator(Match match)
		{
			if(match == null)
				throw new ArgumentNullException("match");
			return string.Format("{0}{1} partial {2}", match.Groups["pre"].Value, match.Groups["access"].Value, match.Groups["type"].Value);
		}

		/// <summary>
		/// Gets the regex.
		/// </summary>
		private static Regex CsPartialRegex
		{
			get
			{
				// returns the value...
				return _csPartialRegex;
			}
		}
		
		public static string SanitizeName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// walk...
			StringBuilder builder = new StringBuilder();
			foreach(char c in name)
			{
				if(char.IsLetterOrDigit(c))
					builder.Append(c);
				else
					builder.Append("_");
			}

			// get...
			name = builder.ToString();
			if(name == null)
				throw new InvalidOperationException("'name' is null.");
			if(name.Length == 0)
				throw new InvalidOperationException("'name' is zero-length.");

			// check...
			if(char.IsDigit(name[0]))
				name = "_" + name;

			// return...
			return name;
		}

		public static string GetNextIdentifierName(string name, IList otherNames)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			if(otherNames == null)
				throw new ArgumentNullException("otherNames");
			if(otherNames.Count == 0)
				throw new ArgumentOutOfRangeException("'otherNames' is zero-length.");
			
			// walk..
			int index = 1;
			while(true)
			{
				string useName = name;
				if(index > 1)
					useName = name + index.ToString();

				// check..
				bool found = false;
				foreach(string checkName in otherNames)
				{
					if(string.Compare(checkName, useName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
					{
						found = true;
						break;
					}
				}

				// found?
				if(!(found))
					return useName;

				// next...
				index++;
			}
		}

		/// <summary>
		/// Gets a plural name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetPluralName(string name)
		{
			if(name == null)
				throw new ArgumentNullException("name");
			if(name.Length == 0)
				throw new ArgumentOutOfRangeException("'name' is zero-length.");
			
			// get...
			string asLower = name.ToLower();
			if(asLower.EndsWith("s"))
				return name + "Items";
			else
				return name + "s";
		}
	}
}
