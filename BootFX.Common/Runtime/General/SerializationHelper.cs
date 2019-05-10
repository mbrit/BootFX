// BootFX - Application framework for .NET applications
// 
// File: SerializationHelper.cs
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
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Reflection;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

namespace BootFX.Common
{
	/// <summary>
	/// Helper class for serialization.
	/// </summary>
	public sealed class SerializationHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private SerializationHelper()
		{
		}

		/// <summary>
		/// Serializes an object to a base-64 string.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string SerializeToBase64String(object obj, ICryptoTransform transform)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(transform == null)
				throw new ArgumentNullException("transform");
			
			// get...
			byte[] bs = SerializeToByteArray(obj, transform);
			if(bs == null)
				throw new InvalidOperationException("bs is null.");

			// return...
			return Convert.ToBase64String(bs);
		}

		/// <summary>
		/// Serializes an object to a stream of bytes.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="transform">The transform object to use to encrypt the stream.  (Can be null.)</param>
		/// <returns></returns>
		public static byte[] SerializeToByteArray(object obj, ICryptoTransform transform)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			if(transform == null)
				throw new ArgumentNullException("transform");

			// transform the lot...
			using(MemoryStream stream = new MemoryStream())
			{
				IFormatter formatter = CreateFormatter();
				if(formatter == null)
					throw new InvalidOperationException("formatter is null.");

				// crypto...
				if(transform != null)
				{
					using(CryptoStream crypto = new CryptoStream(stream, transform, CryptoStreamMode.Write))
					{
						// formatter...
						formatter.Serialize(crypto, obj);

						// flush...
						crypto.FlushFinalBlock();
					}
				}
				else
					formatter.Serialize(stream, obj);

				// get...
				byte[] bs = GetWrittenBytes(stream);
				if(bs == null)
					throw new InvalidOperationException("'bs' is null.");
				if(bs.Length == 0)
					throw new InvalidOperationException("'bs' is zero-length.");

				// return...
				return bs;
			}
		}

		/// <summary>
		/// Gets the written bytes from a memory stream.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		private static byte[] GetWrittenBytes(MemoryStream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// mbr - 28-09-2005 - redone...
//			byte[] result = new byte[stream.Length];
//			Array.Copy(stream.GetBuffer(), 0, result, 0, stream.Length);
//			return result;

			// return...
			return stream.ToArray();
		}

		/// <summary>
		/// Serializes an object to a base-64 string.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static string SerializeToBase64String(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			
			// get...
			byte[] bs = SerializeToByteArray(obj);
			if(bs == null)
				throw new InvalidOperationException("bs is null.");

			// return...
			return Convert.ToBase64String(bs);
		}

		/// <summary>
		/// Serializes an object to a stream of bytes.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public static byte[] SerializeToByteArray(object obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj");
			
			// create...
			using(MemoryStream stream = new MemoryStream())
			{
				// create...
				IFormatter formatter = CreateFormatter();
				formatter.Serialize(stream, obj);

				// return...
				return GetWrittenBytes(stream);
			}
		}

		/// <summary>
		/// Creates a formatter.
		/// </summary>
		/// <returns></returns>
		private static IFormatter CreateFormatter()
		{
			var formatter = new BinaryFormatter();
			formatter.AssemblyFormat = FormatterAssemblyStyle.Full;

			// return...
			return formatter;
		}

		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static object Deserialize(string base64String, ICryptoTransform transform)
		{
			if(base64String == null)
				throw new ArgumentNullException("base64String");
			if(transform == null)
				throw new InvalidOperationException("transform is null.");
			
			// get...
			byte[] bs = Convert.FromBase64String(base64String);
			if(bs == null)
				throw new InvalidOperationException("bs is null.");

			// defer...
			return Deserialize(bs, transform);
		}

		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static object Deserialize(byte[] bytes, ICryptoTransform transform)
		{
			if(bytes == null)
				throw new ArgumentNullException("bytes");
			if(bytes.Length == 0)
				throw new ArgumentOutOfRangeException("'bytes' is zero-length.");
			if(transform == null)
				throw new ArgumentNullException("transform");

			// create a stream...
			using(MemoryStream stream = new MemoryStream(bytes))
			{
				// create a cryptography stream...
				using(CryptoStream cryptoStream = new CryptoStream(stream, transform, CryptoStreamMode.Read))
				{
					// formatter...
					IFormatter formatter = CreateFormatter();
					if(formatter == null)
						throw new InvalidOperationException("formatter is null.");

					// decrypt...
					return formatter.Deserialize(cryptoStream);
				}
			}
		}

		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static object Deserialize(string base64String)
		{
			if(base64String == null)
				throw new ArgumentNullException("base64String");
			
			// get...
			byte[] bs = Convert.FromBase64String(base64String);
			if(bs == null)
				throw new InvalidOperationException("bs is null.");

			// defer...
			return Deserialize(bs);
		}

		/// <summary>
		/// Deserializes the given object.
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns></returns>
		public static object Deserialize(byte[] bytes)
		{
			if(bytes == null)
				throw new ArgumentNullException("bytes");
			if(bytes.Length == 0)
				throw new ArgumentOutOfRangeException("'bytes' is zero-length.");
			
			// stream...
			using(MemoryStream stream = new MemoryStream(bytes))
			{
				// formatter...
				IFormatter formatter = CreateFormatter();
				if(formatter == null)
					throw new InvalidOperationException("formatter is null.");

				// return...
				return formatter.Deserialize(stream);
			}
		}
	}
}
