// BootFX - Application framework for .NET applications
// 
// File: LateBoundObject.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Wraps an object that is to be accessed through late-binding (i.e. Reflection).
	/// </summary>
	// mbr - 11-04-2006 - added.	
	public class LateBoundObject : Loggable
	{
		/// <summary>
		/// Private field to support <c>IsDisposed</c> property.
		/// </summary>
		private bool _isDisposed = false;
		
		/// <summary>
		/// Private field to support <see cref="Instance"/> property.
		/// </summary>
		private object _instance;
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		protected LateBoundObject(Type type) : this(CreateInstance(type))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		protected LateBoundObject(string progId) : this(CreateInstanceFromProgId(progId))
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type"></param>
		private LateBoundObject(object instance)
		{
			if(instance == null)
				throw new ArgumentNullException("instance");
			_instance = instance;
		}

		~LateBoundObject()
		{
			try
			{
				this.Dispose(false);
			}
			finally
			{
				_isDisposed = true;
			}
		}

		public static LateBoundObject Create(Type type)
		{
			return new LateBoundObject(type);
		}

		public static LateBoundObject Create(string typeName)
		{
			if(typeName == null)
				throw new ArgumentNullException("typeName");
			if(typeName.Length == 0)
				throw new ArgumentOutOfRangeException("'typeName' is zero-length.");
			
			// get...
			Type type = Type.GetType(typeName, true);
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// defer...
			return Create(type);
		}

		public static LateBoundObject CreateFromProgId(string progId)
		{
			return new LateBoundObject(CreateInstanceFromProgId(progId));
		}

		private static object CreateInstanceFromProgId(string progId)
		{
			if(progId == null)
				throw new ArgumentNullException("progId");
			if(progId.Length == 0)
				throw new ArgumentOutOfRangeException("'progId' is zero-length.");
			
			// get...
			Type type = Type.GetTypeFromProgID(progId, true);
			if(type == null)
				throw new InvalidOperationException("type is null.");

			// return...
			return CreateInstance(type);
		}

		private static object CreateInstance(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type");
			
			// try...
			try
			{
				return Activator.CreateInstance(type);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("Failed to create an instance of type '{0}' ({1}).", type.AssemblyQualifiedName, type.Assembly.CodeBase), ex);
			}
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public object Instance
		{
			get
			{
				return _instance;
			}
		}

		public Type InstanceType
		{
			get
			{
				if(Instance == null)
					throw new InvalidOperationException("Instance is null.");
				return this.Instance.GetType();
			}
		}

		/// <summary>
		/// Sets a property on the pop3 object using reflection
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="value"></param>
		public void SetProperty(string propertyName, object value)
		{
			if(Instance == null)
				throw new InvalidOperationException("Instance is null.");
			this.InstanceType.InvokeMember(propertyName,BindingFlags.SetProperty,null,this.Instance,new object[] {value});
		}

		/// <summary>
		/// Gets a property on the pop3 object using reflection
		/// </summary>
		/// <param name="propertyName"></param>
		public object GetProperty(string propertyName)
		{
			return InstanceType.InvokeMember(propertyName,BindingFlags.GetProperty,null,Instance,new object[] {});
		}
		
		/// <summary>
		/// Calls a method on the pop3 object using reflection
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="args"></param>
		public object CallMethod(string propertyName, params object[] args)
		{
			return InstanceType.InvokeMember(propertyName,BindingFlags.InvokeMethod,null,Instance,args);
		}

		public void Dispose()
		{
			try
			{
				this.Dispose(true);
			}
			finally
			{
				_isDisposed = true;
			}
		}

		protected virtual void Dispose(bool explicitCall)
		{
			if(this.Instance is IDisposable)
				((IDisposable)this.Instance).Dispose();

			// suppress...
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Gets the isdisposed.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				// returns the value...
				return _isDisposed;
			}
		}

	}
}
