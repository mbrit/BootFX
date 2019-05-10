// BootFX - Application framework for .NET applications
// 
// File: CompilationException.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.CodeDom.Compiler;
using System.Security;

namespace BootFX.Common.CodeGeneration
{
	/// <summary>
	/// Raised when there is an error with compilation.
	/// </summary>
	[Serializable()]
	public class CompilationException : ExceptionWithDumpData
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		// TODO: Add a default message here.
		internal CompilationException() 
			: base()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal CompilationException(string message) 
			: base(message)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal CompilationException(string message, Exception innerException) 
			: base(message, innerException)
		{
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		internal CompilationException(string message, CompilerResults results, string codePath, Exception innerException) 
			: base(message, innerException)
		{
			// builder...
			StringBuilder builder = new StringBuilder();
			if(results.Errors.Count > 0)
			{
				builder.Append(results.Errors.Count);
				builder.Append(" error(s) occurred:");
				foreach(CompilerError error in results.Errors)
				{
					builder.Append("\r\n\t - ");
					builder.Append(error);
				}
			}
			else
				builder.Append("(No errors occurred.)");

			// add...
			if(codePath != null && codePath.Length > 0)
			{
				builder.Append("\r\nA copy of the failed source file has been written to: ");
				builder.Append(codePath);
			}
			else
				builder.Append("\r\nA copy of the failed source file was not saved.");

			// set...
			this.SetDump(builder.ToString());
		}

		/// <summary> 
		/// Deserialization constructor.
		/// </summary> 
		protected CompilationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary> 
		/// Provides data to serialization.
		/// </summary> 
        [SecurityCritical]
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			// base...
			base.GetObjectData(info, context);

			// add code here to stream extra state into 'info'.
			// remember to update the deserializtion constructor.
		}
	}
}
