// BootFX - Application framework for .NET applications
// 
// File: StreamHelper.cs
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
using System.Collections;
using System.IO.Compression;

namespace BootFX.Common
{
	/// <summary>
	/// Defines an instance of <c>StreamHelper</c>.
	/// </summary>
	public sealed class StreamHelper
	{
		private StreamHelper()
		{
		}

		/// <summary>
		/// Copies the contents of one stream to the other.
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		public static void CopyStream(Stream a, Stream b)
		{
			if(a == null)
				throw new ArgumentNullException("a");
			if(b == null)
				throw new ArgumentNullException("b");
			
			// create...
			const int bufLen = 10240;
			byte[] buf = new byte[bufLen];
			while(true)
			{
				int read = a.Read(buf, 0, bufLen);
				if(read == 0)
					break;
				b.Write(buf, 0, read);
			}
		}

		/// <summary>
		/// Copies the stream to a temporary file.
		/// </summary>
		/// <param name="stream"></param>
		public static string CopyStreamToTempFile(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// defer...
			return CopyStreamToTempFile(stream, null);
		}

		/// <summary>
		/// Copies the stream to a temporary file.
		/// </summary>
		/// <param name="stream"></param>
		public static string CopyStreamToTempFile(Stream stream, string extension)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// get...
			string tempPath = null;
			if(Runtime.IsStarted)
				tempPath = Runtime.Current.GetTempFilePath(extension);
			else
			{
				tempPath = Path.GetTempFileName();
				if(extension != null)
					tempPath += extension;
			}

			// defer...
			CopyStreamToFile(stream, tempPath);

			// return...
			return tempPath;
		}

		/// <summary>
		/// Copies the stream to the given file.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="filePath"></param>
		public static void CopyStreamToFile(Stream stream, string filePath)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			
			// open...
			using(Stream outStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
				CopyStream(stream, outStream);
		}

		/// <summary>
		/// Copies the file to the given stream.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="stream"></param>
		public static void CopyFileToStream(string filePath, Stream stream)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// get...
			using(Stream inStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				CopyStream(inStream, stream);
		}

		/// <summary>
		/// Block copies the given stream to a byte array.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static byte[] CopyStreamToBytes(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			
			// create a byte array...
			long length = 0;
			long chunkLength = 10240;
			byte[] chunk = new byte[chunkLength];
			byte[] bs = new byte[chunkLength];
			while(true)
			{
				// read...
				int read = stream.Read(chunk, 0, (int)chunkLength);
				if(read == 0)
					break;

				// copy...
				while(length + read > bs.Length)
				{
					// create...
					long newBsLength = bs.Length * 2;
					byte[] newBs = new byte[newBsLength];
					bs.CopyTo(newBs, 0);

					// reset...
					bs = newBs;
				}

				// copy...
				Array.Copy(chunk, 0, bs, length, read);
				length += read;
			}

			// set...
			byte[] theData = new byte[length];
			Array.Copy(bs, 0, theData, 0, length);

			// return...
			return theData;
		}

		/// <summary>
		/// Copies the given stream to the given writer.
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="writer"></param>
		/// <param name="encoding"><c>null</c> to use the encoding on the writer, or a specific encoding.</param>
		public static void CopyStreamToTextWriter(Stream stream, TextWriter writer, Encoding encoding)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");
			if(writer == null)
				throw new ArgumentNullException("writer");
			if(writer.Encoding == null)
				throw new InvalidOperationException("writer.Encoding is null.");

			// encoding...
			if(encoding == null)
			{
				encoding = writer.Encoding;
				if(encoding == null)
					throw new InvalidOperationException("encoding is null.");
			}

			// while...
			const int bufLen = 10240;
			byte[] buf = new byte[bufLen];
			while(true)
			{
				int read = stream.Read(buf, 0, bufLen);
				if(read == 0)
					break;

				// get...
				string asString = encoding.GetString(buf, 0, read);
				writer.Write(asString);
			}
		}

		/// <summary>
		/// Copies the contents of a stream to a base-64 string.
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public static string CopyStreamToBase64String(Stream stream)
		{
			if(stream == null)
				throw new ArgumentNullException("stream");

			// create...
			using(MemoryStream memoryStream = new MemoryStream())
			{
				// copy...
				CopyStream(stream, memoryStream);

				// return...
				return Convert.ToBase64String(memoryStream.ToArray());
			}
		}

		/// <summary>
		/// Copies the contents of a file to a base-64 string.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static string CopyFileToBase64String(string filePath)
		{
			using(FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				return CopyStreamToBase64String(stream);
		}

		/// <summary>
		/// Creates a file from a base-64 string.
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="contentsAsBase64"></param>
		/// <returns></returns>
		public static void CreateFileFromBase64String(string filePath, string contentsAsBase64)
		{
			// if...
			if(File.Exists(filePath))
				throw new InvalidOperationException(string.Format("The file '{0}' already exists."));

			// create...
			using(Stream stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
			{
				byte[] bs = Convert.FromBase64String(contentsAsBase64);
				stream.Write(bs, 0, bs.Length);
			}	
		}

		/// <summary>
		/// Retrieves the contents of a file as a byte array.
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public static byte[] CopyFileToBytes(string filePath)
		{
			if(filePath == null)
				throw new ArgumentNullException("filePath");
			if(filePath.Length == 0)
				throw new ArgumentOutOfRangeException("'filePath' is zero-length.");

			using(Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
				return CopyStreamToBytes(stream);
		}

        public static byte[] ZipString(string buf, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (var input = new MemoryStream(encoding.GetBytes(buf)))
            {
                using (var output = new MemoryStream())
                {
                    using (var compressorStream = new DeflateStream(output, CompressionMode.Compress, true))
                        input.CopyTo(compressorStream);
                    return output.ToArray();
                }
            }
        }

        public static string ZipStringToBase64String(string buf, Encoding encoding = null)
        {
            var bs = ZipString(buf);
            return Convert.ToBase64String(bs);
        }

        public static string UnzipStringFromBase64String(string base64, Encoding encoding = null)
        {
            var zipped = Convert.FromBase64String(base64);
            return UnzipString(zipped, encoding);
        }

        public static string UnzipString(byte[] zipped, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            using (var input = new MemoryStream(zipped))
            {
                using (var output = new MemoryStream())
                {
                    using (var compressorStream = new DeflateStream(input, CompressionMode.Decompress, true))
                        compressorStream.CopyTo(output);

                    var bs = output.ToArray();
                    return encoding.GetString(bs);
                }
            }
        }

        public static string DetectEncodingAndGetString(byte[] buf)
        {
            int start = 0;
            var encoding = GetEncodingForBytes(buf, ref start);

            var asString = encoding.GetString(buf, start, buf.Length - start);
            return asString;
        }

        public static bool HasEncodingBom(byte[] buf, ref Encoding encoding)
        {
            encoding = null;

            if (buf.Length >= 4)
            {
                var bom = new byte[4];
                Array.Copy(buf, bom, 4);

                encoding = GetEncodingFromBomInternal(bom);
                return encoding != null;
            }
            else
                return false;
        }

        private static Encoding GetEncodingForBytes(byte[] buf, ref int start)
        {
            start = 0;

            Encoding encoding = null;
            if (HasEncodingBom(buf, ref encoding))
            {
                start = 4;
                return encoding;
            }
            else
                return Encoding.ASCII;
        }

        private static Encoding GetEncodingFromBomInternal(byte[] bom)
        {
            if (bom == null)
                throw new ArgumentNullException("bom");
            if (bom.Length != 4)
                throw new ArgumentException($"BOM is an invalid length ({bom.Length}).");

            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76)
                return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
                return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe)
                return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff)
                return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff)
                return Encoding.UTF32;

            return null;
        }
    }
}
