// BootFX - Application framework for .NET applications
// 
// File: MbrApplication.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Reflection;

namespace BootFX.Common
{
	/// <summary>
	/// Describes an application used by the framework.
	/// </summary>
	public sealed class MbrApplication
	{
		/// <summary>
		/// Private field to support <see cref="StartArgs"/> property.
		/// </summary>
		private RuntimeStartArgs _startArgs;
		
		/// <summary>
		/// Private field to support <see cref="TempFilenamePrefix"/> property.
		/// </summary>
		private string _tempFilenamePrefix;
		
		/// <summary>
		/// Private field to support <see cref="ProductModule"/> property.
		/// </summary>
		private string _productModule = null;
		
		/// <summary>
		/// Private field to support <c>SupportUrl</c> property.
		/// </summary>
		private string _productSupportUrl = null;
		
		/// <summary>
		/// Private field to support <c>ProductName</c> property.
		/// </summary>
		private string _productName;

		/// <summary>
		/// Private field to support <c>ProductCompany</c> property.
		/// </summary>
		private string _productCompany;

		/// <summary>
		/// Private field to support <c>ProductVersion</c> property.
		/// </summary>
		private Version _productVersion;
		
		/// <summary>
		/// Private field to support <c>Copyright</c> property.
		/// </summary>
		private string _copyright = null;
		
		/// <summary>
		/// Private field to support <c>AdditionalCopyright</c> property.
		/// </summary>
		private string _additionalCopyright;

		// mbr - 03-11-2005 - deprecated...
//		/// <summary>
//		/// Constructor.
//		/// </summary>
//		internal MbrApplication()
//		{
//			// configure from attributes...
//			Assembly assembly = Assembly.GetEntryAssembly();
//			if(assembly == null)
//				throw new InvalidOperationException("assembly is null.");
//
//			// version...
//			_productVersion = assembly.GetName().Version;
//
//			// basics...
//			AssemblyProductAttribute[] productAttrbutes = (AssemblyProductAttribute[])assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
//			if(productAttrbutes.Length > 0)
//				_productName = productAttrbutes[0].Product;
//			else
//				_productName = "Unknown Name";
//
//			// basics...
//			AssemblyCompanyAttribute[] CompanyAttrbutes = (AssemblyCompanyAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
//			if(CompanyAttrbutes.Length > 0)
//				_productCompany = CompanyAttrbutes[0].Company;
//			else
//				_productCompany = "Unknown Company";
//
//			// basics...
//			AssemblySupportUrlAttribute[] SupportUrlAttrbutes = (AssemblySupportUrlAttribute[])assembly.GetCustomAttributes(typeof(AssemblySupportUrlAttribute), false);
//			if(SupportUrlAttrbutes.Length > 0)
//				_productSupportUrl = SupportUrlAttrbutes[0].Url;
//
//			// copyrights...
//			_copyright = this.GetCopyright(assembly);
//			_additionalCopyright = this.GetCopyright(this.GetType().Assembly);
//		}

		/// <summary>
		/// Creates a <see cref="MbrApplication"></see> from the given settings.
		/// </summary>
		/// <param name="productCompany"></param>
		/// <param name="productName"></param>
		/// <param name="productVersion"></param>
		/// <param name="productSupportUrl"></param>
		/// <param name="copyright"></param>
		/// <param name="additionalCopyright"></param>
		/// <remarks>Usually, the parameterless constructor should be used as that will infer the settings from the entry assembly's 
		/// attributes.  However, for unit testing, that doesn't work, hence this version and the corollory in <see cref="Runtime.Start"></see>.</remarks>
		// mbr - 05-09-2007 - added args.		
//		internal MbrApplication(string productCompany, string productName, string productModule, Version productVersion) 
		internal MbrApplication(string productCompany, string productName, string productModule, Version productVersion,
			RuntimeStartArgs args) 
		{
			if(productCompany == null)
				throw new ArgumentNullException("productCompany");
			if(productCompany.Length == 0)
				throw new ArgumentOutOfRangeException("'productCompany' is zero-length.");
			if(productName == null)
				throw new ArgumentNullException("productName");
			if(productName.Length == 0)
				throw new ArgumentOutOfRangeException("'productName' is zero-length.");
			if(productModule == null)
				throw new ArgumentNullException("productModule");
			if(productModule.Length == 0)
				throw new ArgumentOutOfRangeException("'productModule' is zero-length.");
			if(args == null)
				throw new ArgumentNullException("args");
			
			// set...
			_productCompany = productCompany;
			_productName = productName;
			_productModule = productModule;
			_productVersion = productVersion;
//			_productSupportUrl = productSupportUrl;
			if(Assembly.GetEntryAssembly() != null)
				_copyright = this.GetCopyright(Assembly.GetEntryAssembly());

			_additionalCopyright = this.GetCopyright(this.GetType().Assembly);

			// mbr - 12-10-2006 - temp filename prefix...
			StringBuilder builder = new StringBuilder();
			builder.Append("bfx_");
			this.BuildTempNamePrefix(builder, productCompany);
			this.BuildTempNamePrefix(builder, productName);
			this.BuildTempNamePrefix(builder, productModule);

			// set...
			_tempFilenamePrefix = builder.ToString();

			// set...
			_startArgs = args;
		}

		private void BuildTempNamePrefix(StringBuilder builder, string buf)
		{
			if(builder == null)
				throw new ArgumentNullException("builder");
			if(buf == null)
				throw new ArgumentNullException("buf");
			if(buf.Length == 0)
				throw new ArgumentOutOfRangeException("'buf' is zero-length.");
			
			// walk...
			int index = 0;
			foreach(char c in buf)
			{
				if(char.IsLetterOrDigit(c))
				{
					builder.Append(c);
					index++;
					if(index == 8)
						break;
				}
			}

			// end...
			builder.Append("_");
		}

		/// <summary>
		/// Gets the copyright message from the given assembly, if any.
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		private string GetCopyright(Assembly assembly)
		{
			if(assembly == null)
				throw new ArgumentNullException("assembly");
			
			// get...
			StringBuilder builder = new StringBuilder();
			AssemblyCopyrightAttribute[] attrs = (AssemblyCopyrightAttribute[])assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
			foreach(AssemblyCopyrightAttribute attr in attrs)
			{
				if(builder.Length > 0)
					builder.Append("\r\n");
				builder.Append(attr.Copyright);
			}

			// return...
			return builder.ToString();
		}

		/// <summary>
		/// Gets the copyright.
		/// </summary>
		public string Copyright
		{
			get
			{
				return _copyright;
			}
		}

		/// <summary>
		/// Gets the additionalcopyright.
		/// </summary>
		public string AdditionalCopyright
		{
			get
			{
				return _additionalCopyright;
			}
		}

		/// <summary>
		/// Gets the productversion.
		/// </summary>
		public Version ProductVersion
		{
			get
			{
				return _productVersion;
			}
		}
		
		/// <summary>
		/// Gets the productcompany.
		/// </summary>
		public string ProductCompany
		{
			get
			{
				return _productCompany;
			}
		}
		
		/// <summary>
		/// Gets the productname.
		/// </summary>
		public string ProductName
		{
			get
			{
				return _productName;
			}
		}

		/// <summary>
		/// Gets the productmodule.
		/// </summary>
		public string ProductModule
		{
			get
			{
				if(_productModule == null || _productModule.Length == 0)
					return "Application";
				else
					return _productModule;
			}
		}

		/// <summary>
		/// Gets the complete copyright notice.
		/// </summary>
		public string CopyrightNotice
		{
			get
			{
				StringBuilder builder = new StringBuilder();
				builder.Append(this.Copyright);
				if(this.AdditionalCopyright != null && this.AdditionalCopyright.Length > 0)
				{
					builder.Append("\r\n\r\n");
					builder.Append("--------------------------------------\r\n");
					builder.Append(this.AdditionalCopyright);
				}

				// return...
				return builder.ToString();
			}
		}

		/// <summary>
		/// Gets the supporturl.
		/// </summary>
		public string ProductSupportUrl
		{
			get
			{
				return _productSupportUrl;
			}
		}

		public override string ToString()
		{
			return string.Format("{0} {1} v{3} ({2} module)", this.ProductCompany, this.ProductName, this.ProductModule, this.ProductVersion);
		}

		/// <summary>
		/// Gets the tempfilenameprefix.
		/// </summary>
		internal string TempFilenamePrefix
		{
			get
			{
				return _tempFilenamePrefix;
			}
		}

		/// <summary>
		/// Gets the startargs.
		/// </summary>
		internal RuntimeStartArgs StartArgs
		{
			get
			{
				return _startArgs;
			}
		}
	}
}
