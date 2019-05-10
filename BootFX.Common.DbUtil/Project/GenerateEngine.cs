// BootFX - Application framework for .NET applications
// 
// File: GenerateEngine.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.CodeDom;
using System.Collections;
using BootFX.Common.Data.Schema;
using BootFX.Common.Management;
using BootFX.Common.CodeGeneration;
using BootFX.Common.Entities;

namespace BootFX.Common.DbUtil
{
	/// <summary>
	/// Summary description for GenerateEngine.
	/// </summary>
	internal class GenerateEngine : Loggable
	{
		internal GenerateEngine()
		{
		}
		
		/// <summary>
		/// Generates the selected tables.
		/// </summary>
		internal void Generate(Project project, string entitiesFolderPath, string servicesFolderPath, string proceduresFolderPath, 
			ArrayList lockedFiles, IOperationItem item)
		{
			if(project == null)
				throw new ArgumentNullException("project");
			if(entitiesFolderPath == null)
				throw new ArgumentNullException("entitiesFolderPath");
			if(entitiesFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'entitiesFolderPath' is zero-length.");
			if(proceduresFolderPath == null)
				throw new ArgumentNullException("proceduresFolderPath");
			if(proceduresFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'proceduresFolderPath' is zero-length.");			

			// item?
			if(item == null)
				item = new OperationItem();

			// busy...
			item.ProgressValue = 0;
			item.Status = "Retrieving tables...";
			SqlTable[] tables = project.Schema.GetTablesForGeneration();
			item.ProgressMaximum = tables.Length;

			// walk the tables...
			foreach(SqlTable table in tables)
			{
                if (!table.Generate)
                    continue;

				item.Status = string.Format("Generating '{0}' classes...", table.Name);
				this.Generate(project, entitiesFolderPath, servicesFolderPath, table, lockedFiles);
				item.IncrementProgress();
			}

			// mbr - 15-06-2006 - walk the procs...
			item.ProgressValue = 0;
			item.Status = "Retrieving tables...";
			SqlProcedure[] procs = project.Schema.GetProceduresForGeneration();
			item.ProgressMaximum = procs.Length;

			// walk the tables...
			foreach(SqlProcedure proc in procs)
			{
                if (!proc.Generate)
                    continue;

				item.Status = string.Format("Generating '{0}' classes...", proc.Name);
				this.Generate(project, proceduresFolderPath, proc, lockedFiles);
				item.IncrementProgress();
			}			
		}

		/// <summary>
		/// Generate the given table to the given root folder.
		/// </summary>
		/// <param name="rootFolderPath"></param>
		/// <param name="table"></param>
		private void Generate(Project project, string proceduresRootFolderPath, SqlProcedure proc, ArrayList lockedFiles)
		{
			if(project == null)
				throw new ArgumentNullException("project");
			if(proceduresRootFolderPath == null)
				throw new ArgumentNullException("proceduresFolderPath");
			if(proceduresRootFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'proceduresFolderPath' is zero-length.");
			if(lockedFiles == null)
				throw new ArgumentNullException("lockedFiles");
			if(proc == null)
				throw new ArgumentNullException("proc");
			
			// get the path...
			string procedureFolderPath = proceduresRootFolderPath + "\\" + proc.Name;
			if(Directory.Exists(procedureFolderPath) == false)
				Directory.CreateDirectory(procedureFolderPath);

			// get the path...
			string procedureBaseFolderPath = proceduresRootFolderPath + "\\!Base\\" + proc.Name;
			if(Directory.Exists(procedureBaseFolderPath) == false)
				Directory.CreateDirectory(procedureBaseFolderPath);

			// mbr - 21-09-2007 - context...
			EntityGenerationContext context = null;
			EntityGenerator generator = this.GetEntityGenerator(project, ref context);
			if(generator == null)
				throw new InvalidOperationException("generator is null.");
			if(context == null)
				throw new InvalidOperationException("context is null.");

			// run...
			string[] paths = this.SaveType(project, generator, procedureBaseFolderPath, generator.GetEntityBaseClass(proc, context), true, false, context);
			if(paths != null)
				lockedFiles.Add(paths);
			paths = this.SaveType(project, generator, procedureFolderPath, generator.GetEntityClass(proc, context), false, false, context);
			if(paths != null)
				lockedFiles.Add(paths);
		}

		/// <summary>
		/// Generate the given table to the given root folder.
		/// </summary>
		/// <param name="rootFolderPath"></param>
		/// <param name="table"></param>
		private void Generate(Project project, string entitiesRootFolderPath, string servicesRootFolderPath, 
			SqlTable table, ArrayList lockedFiles)
		{
			if(project == null)
				throw new ArgumentNullException("project");
			if(entitiesRootFolderPath == null)
				throw new ArgumentNullException("entitiesRootFolderPath");
			if(entitiesRootFolderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'entitiesRootFolderPath' is zero-length.");
			if(lockedFiles == null)
				throw new ArgumentNullException("lockedFiles");
			if(table == null)
				throw new ArgumentNullException("table");
			
			// get the path...
			string entitiesFolderPath = entitiesRootFolderPath + "\\" + table.Name;
			if(Directory.Exists(entitiesFolderPath) == false)
				Directory.CreateDirectory(entitiesFolderPath);

			// get the path...
			string entitiesBaseFolderPath = entitiesRootFolderPath + "\\!Base\\" + table.Name;
			if(Directory.Exists(entitiesBaseFolderPath) == false)
				Directory.CreateDirectory(entitiesBaseFolderPath);

			// mbr - 21-09-2007 - context...
			EntityGenerationContext context = null;
			EntityGenerator generator = this.GetEntityGenerator(project, ref context);
			if(generator == null)
				throw new InvalidOperationException("generator is null.");
			if(context == null)
				throw new InvalidOperationException("context is null.");

			// get...
			var paths = this.SaveType(project, generator, entitiesBaseFolderPath, generator.GetEntityBaseClass(table, context), true, false, context);
			if(paths != null)
				lockedFiles.Add(paths);
			paths = this.SaveType(project, generator, entitiesFolderPath, generator.GetEntityClass(table, context), false, false, context);
			if(paths != null)
				lockedFiles.Add(paths);
			paths = this.SaveType(project, generator, entitiesBaseFolderPath, generator.GetEntityCollectionBaseClass(table, context), true, false, context);
			if(paths != null)
				lockedFiles.Add(paths);
			paths = this.SaveType(project, generator, entitiesFolderPath, generator.GetEntityCollectionClass(table, context), false, false, context);
			if(paths != null)
				lockedFiles.Add(paths);

            // table...
            if (table.GenerateDto)
            {
                string dtoBaseFolderPath = servicesRootFolderPath + "\\!Base\\" + table.Name;
                Runtime.Current.EnsureFolderCreated(dtoBaseFolderPath);

                string dtoFolderPath = servicesRootFolderPath + "\\" + table.Name;
                Runtime.Current.EnsureFolderCreated(dtoFolderPath);

                paths = this.SaveType(project, generator, dtoBaseFolderPath, generator.GetDtoBaseClass(table, context), true, true, context);
                if (paths != null)
                    lockedFiles.Add(paths);
                paths = this.SaveType(project, generator, dtoFolderPath, generator.GetDtoClass(table, context), false, true, context);
                if (paths != null)
                    lockedFiles.Add(paths);
            }
		}
		
		/// <summary>
		/// Saves the given type.
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="type"></param>
		/// <returns>Null if the file was written or doesn't need to be written, otherwise the name of the file that needs to be checked out.</returns>
		private string[] SaveType(Project project, EntityGenerator generator, string folderPath, GeneratedTypeDeclaration type, 
			bool isBase, bool forWebService, EntityGenerationContext context)
		{
			if(project == null)
				throw new ArgumentNullException("project");
			if(generator == null)
				throw new ArgumentNullException("generator");
			if(folderPath == null)
				throw new ArgumentNullException("folderPath");
			if(folderPath.Length == 0)
				throw new ArgumentOutOfRangeException("'folderPath' is zero-length.");
			if(type == null)
				throw new ArgumentNullException("type");
			if(context == null)
				throw new ArgumentNullException("context");

			// get the path...
			string path = string.Format("{0}\\{1}.cs", folderPath, type.CodeItem.Name);
			if(project.Settings.TargetVersion != DotNetVersion.V1 && isBase)
			{
				// if we're base, we're actually doing a partial file so change the name...
				path = string.Format("{0}\\{1}.Base.cs", folderPath, type.CodeItem.Name);
			}

			// if we don't want to overwrite it and it exists, go back...
			bool exists = File.Exists(path);
			if(!(isBase) && exists)
				return null;

			// processing...
			if(this.Log.IsInfoEnabled)
				this.Log.Info("Processing: " + path);

			// get the namespace...
			var ns = generator.GetNamespace(project.Settings.NamespaceName, type, context);
			if(ns == null)
				throw new InvalidOperationException("ns is null.");

			// get the code...
			string code = CodeDomExtender.ToString(ns, Language.CSharp);
			if(code == null)
				throw new InvalidOperationException("'code' is null.");
			if(code.Length == 0)
				throw new InvalidOperationException("'code' is zero-length.");

			// make it partial...  can't do this with the 1.1 CodeDOM.
			//if(project.Settings.TargetVersion != DotNetVersion.V1)
			//	code = CodeDomExtender.AddPartialKeyword(code, project.Settings.Language);

			// do we want to write it?
			bool doWrite = false;
			if(exists)
			{
				// check it...
				byte[] codeHash = this.GetCodeHash(code);
				if(codeHash == null)
					throw new InvalidOperationException("'codeHash' is null.");
				if(codeHash.Length == 0)
					throw new InvalidOperationException("'codeHash' is zero-length.");

				// get the file...
				byte[] fileHash = this.GetFileHash(path);
				if(fileHash == null)
					throw new InvalidOperationException("'fileHash' is null.");
				if(fileHash.Length == 0)
					throw new InvalidOperationException("'fileHash' is zero-length.");

				// compare...
				if(codeHash.Length != fileHash.Length)
					throw new InvalidOperationException(string.Format("Length mismatch for 'codeHash' and 'fileHash': {0} cf {1}.", codeHash.Length, fileHash.Length));
				for(int index = 0; index < codeHash.Length; index++)
				{
					if(codeHash[index] != fileHash[index])
					{
						if(this.Log.IsInfoEnabled)
							this.Log.Info("\tFile hash has changed");
						doWrite = true;
						break;
					}
				}
			}
			else
			{
				if(this.Log.IsInfoEnabled)
					this.Log.Info("\tFile does not exist");
				doWrite = true;
			}

			// write it...
			if(doWrite)
			{
				// can we?
				if(File.Exists(path) && (File.GetAttributes(path) & FileAttributes.ReadOnly) != 0)
				{
					// mbr - 14-10-2005 - spit the code to a temp file...
					string tempPath = Runtime.Current.GetTempFilePath(Path.GetExtension(path));
					using(StreamWriter writer = new StreamWriter(tempPath))
						writer.Write(code);

					// require checkout...
					if(this.Log.IsInfoEnabled)
						this.Log.Info("\tFile is read-only");
					return new string[] { path, tempPath };
				}

				// log...
				if(this.Log.IsInfoEnabled)
					this.Log.Info("\tWriting file");

				// create a file...
				using(StreamWriter writer = new StreamWriter(path))
					writer.Write(code);
			}

			// ok...
			return null;
		}

		/// <summary>
		/// Gets the hash of the given code.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		private byte[] GetCodeHash(string code)
		{
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
			return md5.ComputeHash(Encoding.ASCII.GetBytes(code));
		}

		/// <summary>
		/// Gets the hash of the given file.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private byte[] GetFileHash(string path)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// open...
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
				return md5.ComputeHash(stream);
			}
		}
		
		/// <summary>
		/// Creates an entity generator.
		/// </summary>
		/// <returns></returns>
		// mbr - 21-09-2007 - changed this to also return a context.		
		internal EntityGenerator GetEntityGenerator(Project project, ref EntityGenerationContext context)
		{
			if(project == null)
				throw new ArgumentNullException("project");
			if(project.Settings == null)
				throw new InvalidOperationException("project.Settings is null.");

			// mbr - 21-09-2007 - do the context.
			context = null;

			// create...
            EntityGenerator generator = new EntityGenerator(project.Settings.TargetVersion);
			generator.CompilationOptions.DatabaseName = project.Settings.DatabaseName;
			generator.DefaultNamespaceName = project.Settings.NamespaceName;
			generator.EntityBaseTypeName = project.Settings.BaseType;
			generator.DtoBaseTypeName = project.Settings.DtoBaseType;

			// setup...
			context = new EntityGenerationContext(generator, null, null);

			// return...
			return generator;
		}
	}
}
