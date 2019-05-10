// BootFX - Application framework for .NET applications
// 
// File: EncryptionHelper.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text;
using System.Security.Cryptography;

namespace BootFX.Common.Crypto
{
	/// <summary>
	/// Summary description for EncryptionHelper.
	/// </summary>
	public sealed class EncryptionHelper
	{
		private EncryptionHelper()
		{
		}

		public static int GetRandomSalt()
		{
			Byte[] _saltBytes = new Byte[4];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes(_saltBytes);

			return ((((int)_saltBytes[0]) << 24) + (((int)_saltBytes[1]) << 16) + 
				(((int)_saltBytes[2]) << 8) + ((int)_saltBytes[3]));
		}

        public static byte[] HashPassword(string password, int salt)
        {
            return HashPasswordInternal(password, salt, false);
        }

        public static byte[] HashPasswordUsingSha256(string password, int salt)
        {
            return HashPasswordInternal(password, salt, true);
        }

		private static byte[] HashPasswordInternal(string password, int salt, bool use256)
		{
			if(password == null)
				throw new ArgumentNullException("password");
			if(password.Length == 0)
				throw new ArgumentOutOfRangeException("'password' is zero-length.");
			
			// hash...
			byte[] bs = Encoding.Unicode.GetBytes(password);
      
			// Create a new salt
			Byte[] saltBs = new Byte[4];
			saltBs[0] = (byte)(salt >> 24);
			saltBs[1] = (byte)(salt >> 16);
			saltBs[2] = (byte)(salt >> 8);
			saltBs[3] = (byte)(salt);

			// append the two arrays
			Byte[] toHash = new Byte[bs.Length + saltBs.Length];
			Array.Copy(bs, 0, toHash, 0, bs.Length);
			Array.Copy(saltBs, 0, toHash, bs.Length, saltBs.Length);

			// return...
            if (use256)
            {
                var sha256 = SHA256.Create();
                byte[] hash = sha256.ComputeHash(toHash);
                return hash;
            }
            else
            {
                var sha1 = SHA1.Create();
                byte[] hash = sha1.ComputeHash(toHash);
                return hash;
            }
		}

		public static string HashPasswordToBase64String(string password, int salt)
		{
            return HashPasswordToBase64StringInternal(password, salt, false);
		}

        public static string HashPasswordToBase64StringUsingSha256(string password, int salt)
        {
            return HashPasswordToBase64StringInternal(password, salt, true);
        }

        private static string HashPasswordToBase64StringInternal(string password, int salt, bool use256)
        {
            byte[] bs = HashPasswordInternal(password, salt, use256);
            return Convert.ToBase64String(bs);
        }

        public static byte[] GetRandomBytes(int length)
        {
            var crypto = new RNGCryptoServiceProvider();
            var bs = new byte[length];
            crypto.GetNonZeroBytes(bs);
            return bs;
        }

        public static string GetRandomBytesAsBase64String(int byteLength)
        {
            var bs = GetRandomBytes(byteLength);
            return Convert.ToBase64String(bs);
        }

        public static string GetRandomBytesAsHexString(int byteLength)
        {
            var bs = GetRandomBytes(byteLength);

            StringBuilder builder = new StringBuilder();
            foreach (var b in bs)
                builder.Append(b.ToString("x2"));
            return builder.ToString();
        }

        public static string GetToken(bool asHex = true)
        {
            if (asHex)
                return GetRandomBytesAsHexString(24);
            else
                return GetRandomBytesAsBase64String(24);
        }
    }
}
