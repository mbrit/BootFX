// BootFX - Application framework for .NET applications
// 
// File: ReflectedStream.cs
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
using System.Collections;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>ReflectedStream</c>.
	/// </summary>
	internal class ReflectedStream : Stream
	{
		/// <summary>
		/// Private field to support <see cref="InnerStream"/> property.
		/// </summary>
		private LateBoundObject _innerStream;
		
		internal ReflectedStream(Type baseType)
		{
			if(baseType == null)
				throw new ArgumentNullException("baseType");			
			_innerStream = LateBoundObject.Create(baseType);
		}

		/// <summary>
		/// Gets the innerstream.
		/// </summary>
		internal LateBoundObject InnerStream
		{
			get
			{
				return _innerStream;
			}
		}

		public override bool CanRead
		{
			get
			{
				if(InnerStream == null)
					throw new InvalidOperationException("InnerStream is null.");
				return (bool)this.InnerStream.GetProperty("CanRead");
			}
		}

		public override bool CanSeek
		{
			get
			{
				if(InnerStream == null)
					throw new InvalidOperationException("InnerStream is null.");
				return (bool)this.InnerStream.GetProperty("CanSeek");
			}
		}

		public override bool CanWrite
		{
			get
			{
				if(InnerStream == null)
					throw new InvalidOperationException("InnerStream is null.");
				return (bool)this.InnerStream.GetProperty("CanWrite");
			}
		}

		public override long Length
		{
			get
			{
				if(InnerStream == null)
					throw new InvalidOperationException("InnerStream is null.");
				return (long)this.InnerStream.GetProperty("Length");
			}
		}

		public override long Position
		{
			get
			{
				if(InnerStream == null)
					throw new InvalidOperationException("InnerStream is null.");
				return (long)this.InnerStream.GetProperty("Position");
			}
			set
			{
				if(InnerStream == null)
					throw new InvalidOperationException("InnerStream is null.");
				this.InnerStream.SetProperty("Position", value);
			}
		}

		public override void Flush()
		{
			if(InnerStream == null)
				throw new InvalidOperationException("InnerStream is null.");
			this.InnerStream.CallMethod("Flush");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if(InnerStream == null)
				throw new InvalidOperationException("InnerStream is null.");
			return (long)this.InnerStream.CallMethod("Seek", offset, origin);
		}

		public override void SetLength(long value)
		{
			if(InnerStream == null)
				throw new InvalidOperationException("InnerStream is null.");
			this.InnerStream.CallMethod("SetLength", value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if(InnerStream == null)
				throw new InvalidOperationException("InnerStream is null.");
			return (int)this.InnerStream.CallMethod("Read", buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if(InnerStream == null)
				throw new InvalidOperationException("InnerStream is null.");
			this.InnerStream.CallMethod("Write", buffer, offset, count);
		}
	}
}
