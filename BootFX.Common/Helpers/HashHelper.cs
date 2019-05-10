// BootFX - Application framework for .NET applications
// 
// File: HashHelper.cs
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
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>HashHalper</c>.
	/// </summary>
	public sealed class HashHelper
	{
		private HashHelper()
		{
		}

        static HashHelper()
        {
        }

		public static bool CompareMd5Hashes(string base64Hash1, string base64Hash2)
		{
			if(base64Hash1 == null)
				throw new ArgumentNullException("base64Hash1");
			if(base64Hash1.Length == 0)
				throw new ArgumentOutOfRangeException("'base64Hash1' is zero-length.");
			if(base64Hash2 == null)
				throw new ArgumentNullException("base64Hash2");
			if(base64Hash2.Length == 0)
				throw new ArgumentOutOfRangeException("'base64Hash2' is zero-length.");
			
			return CompareMd5Hashes(ConvertMd5Hash(base64Hash1), ConvertMd5Hash(base64Hash2));
		}

		public static byte[] ConvertMd5Hash(string base64Hash)
		{
			if(base64Hash == null)
				throw new ArgumentNullException("base64Hash");
			if(base64Hash.Length == 0)
				throw new ArgumentOutOfRangeException("'base64Hash' is zero-length.");
			
			try
			{
				return Convert.FromBase64String(base64Hash);
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("The string '{0}' did not appear to be a valid base-64 string.", base64Hash), ex);
			}
		}
		
		public static bool CompareMd5Hashes(byte[] hash1, byte[] hash2)
		{
			return CompareAssertMd5Hashes(hash1, hash2, false);
		}

		private static bool CompareAssertMd5Hashes(byte[] hash1, byte[] hash2, bool assert)
		{
			if(hash1 == null)
				throw new ArgumentNullException("hash1");
			if(hash1.Length == 0)
				throw new ArgumentOutOfRangeException("'hash1' is zero-length.");
			if(hash2 == null)
				throw new ArgumentNullException("hash2");
			if(hash2.Length == 0)
				throw new ArgumentOutOfRangeException("'hash2' is zero-length.");
			if(hash1.Length != hash2.Length)
				throw new InvalidOperationException(string.Format("Length mismatch for 'hash1' and 'hash2': {0} cf {1}.", hash1.Length, hash2.Length));

			// walk...
			for(int index = 0; index < hash1.Length; index++)
			{
				if(hash1[index] != hash2[index])
				{
					if(assert)
					{
						throw new InvalidOperationException(string.Format("Hashes differ at index '{0}'.\r\nHash 1: {1}\r\nHash 2: {2}", index, HashToString(hash1), HashToString(hash2)));
					}
					else
						return false;
				}
			}

			// ok...
			return true;
		}

		private static string HashToString(byte[] hash)
		{
			if(hash == null)
				throw new ArgumentNullException("hash");
			if(hash.Length == 0)
				throw new ArgumentOutOfRangeException("'hash' is zero-length.");
			
			// walk...
			StringBuilder builder = new StringBuilder();
			foreach(byte b in hash)
				builder.Append(b.ToString("x2"));

			// return...
			return builder.ToString();
		}

		public static void AssertMd5HashForFile(string path, string base64Hash)
		{
			AssertMd5HashForFile(path, ConvertMd5Hash(base64Hash));
		}

		public static void AssertMd5HashForFile(string path, byte[] hash)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// get...
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				AssertMd5HashForStream(stream, hash);
		}

		public static void AssertMd5HashForStream(Stream stream, string base64Hash)
		{
			AssertMd5HashForStream(stream, ConvertMd5Hash(base64Hash));
		}

		public static void AssertMd5HashForStream(Stream stream, byte[] hash)
		{
			CompareAssertMd5HashForStream(stream, hash, true);
		}

		public static bool CompareMd5HashForFile(string path, string base64Hash)
		{
			return CompareMd5HashForFile(path, ConvertMd5Hash(base64Hash));
		}

		public static bool CompareMd5HashForFile(string path, byte[] hash)
		{
			if(path == null)
				throw new ArgumentNullException("path");
			if(path.Length == 0)
				throw new ArgumentOutOfRangeException("'path' is zero-length.");
			
			// get...
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				return CompareMd5HashForStream(stream, hash);
		}

		public static bool CompareMd5HashForStream(Stream stream, string base64Hash)
		{
			return CompareMd5HashForStream(stream, ConvertMd5Hash(base64Hash));
		}

		public static bool CompareMd5HashForStream(Stream stream, byte[] hash)
		{
			return CompareAssertMd5HashForStream(stream, hash, false);
		}

		private static bool CompareAssertMd5HashForStream(Stream stream, byte[] hash, bool assert)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			if(hash == null)
				throw new ArgumentNullException("hash");
			if(hash.Length == 0)
				throw new ArgumentOutOfRangeException("'hash' is zero-length.");

			// get...
			byte[] check = GetMd5Hash(stream);
			if(check == null)
				throw new InvalidOperationException("'check' is null.");
			if(check.Length == 0)
				throw new InvalidOperationException("'check' is zero-length.");

			// comapre...
			return CompareAssertMd5Hashes(hash, check, assert);
		}

		public static byte[] GetMd5Hash(string path)
		{
			using(FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
				return GetMd5Hash(stream);
		}

		public static byte[] GetMd5Hash(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// get...
			return new MD5CryptoServiceProvider().ComputeHash(stream);
		}

		
		public static string GetMd5HashAsBase64(string path)
		{
			return Convert.ToBase64String(GetMd5Hash(path));
		}

		public static string GetMd5HashAsBase64(Stream stream)
		{
			return Convert.ToBase64String(GetMd5Hash(stream));
		}

		public static string GetMd5HashOfStringAsBase64(string buf)
		{
			if(buf == null)
				throw new ArgumentNullException("buf");
			
			// get...
			using(MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(buf)))
				return GetMd5HashAsBase64(stream);
		}

        public static byte[] GetMd5HashOfString(string buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");

            // get...
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(buf)))
                return stream.ToArray();
        }

        public static string GetMd5HashOfStringAsHex(string buf)
        {
            if (buf == null)
                throw new ArgumentNullException("buf");

            // get...
            var bs = GetMd5HashOfString(buf);
            StringBuilder builder = new StringBuilder();
            foreach (var c in bs)
                builder.Append(c.ToString("x2"));

            return builder.ToString();
        }
    }
}
