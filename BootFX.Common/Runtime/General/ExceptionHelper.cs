// BootFX - Application framework for .NET applications
// 
// File: ExceptionHelper.cs
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
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Provides access to helper functions for creating standard exceptions.
	/// </summary>
	internal sealed class ExceptionHelper
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		private ExceptionHelper()
		{
		}

        /// <summary>
        /// Creates a not implemented exception for the given object.
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        internal static Exception CreateCannotHandleException(object obj)
        {
            return new NotSupportedException(string.Format("Cannot handle '{0}'.", obj));
        }

        ///// <summary>
        ///// Creates a not implemented exception for the given object.
        ///// </summary>
        ///// <param name="forType"></param>
        ///// <returns></returns>
        //internal static Exception CreateNotImplementedException(object obj)
        //{
        //    // create the values...
        //    object[] values = null;
        //    if(obj == null)
        //        values = new object[] { "(null)" };
        //    else if(obj is Type)
        //        values = new object[] { obj };
        //    else 
        //        values = new object[] { obj.GetType() };

        //    // defer...
        //    return CreateException(typeof(NotImplementedException), typeof(ExceptionHelper).Assembly, "NotImplementedException", null, values);
        //}

        /// <summary>
        /// Creates a not implemented exception for the given object.
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        internal static Exception CreateLengthMismatchException(string item1, string item2, int item1Length, int item2Length)
        {
            // defer...
            return CreateLengthMismatchException(item1, item2, (long)item1Length, (long)item2Length);
        }

        /// <summary>
        /// Creates a not implemented exception for the given object.
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        internal static Exception CreateLengthMismatchException(string item1, string item2, long item1Length, long item2Length)
        {
            throw new InvalidOperationException(string.Format("The two lists were not of the same length, {0} and {1}, {2} cf. {3}.", 
                item1, item2, item1Length, item2Length));
        }

        ///// <summary>
        ///// Creates a not implemented exception for the given object.
        ///// </summary>
        ///// <param name="forType"></param>
        ///// <returns></returns>
        //internal static Exception CreateConversionFailedException(object value, Type type, Exception innerException)
        //{
        //    object[] values = new object[] { LogSet.ToString(value), type };

        //    // defer...
        //    return CreateException(typeof(InvalidOperationException), typeof(ExceptionHelper).Assembly, "ConversionFailedException", innerException, values);
        //}

        ///// <summary>
        ///// Creates a not implemented exception for the given object.
        ///// </summary>
        ///// <param name="forType"></param>
        ///// <returns></returns>
        //internal static Exception CreateNullArgumentException(string name)
        //{
        //    // use a system one...
        //    return new ArgumentNullException(name);
        //}

        ///// <summary>
        ///// Creates a not implemented exception for the given object.
        ///// </summary>
        ///// <param name="forType"></param>
        ///// <returns></returns>
        //internal static Exception CreateNullObjectException(string name)
        //{
        //    // use a system one...
        //    return CreateException(typeof(InvalidOperationException), typeof(ExceptionHelper).Assembly, "NullException", null, new object[] { name });
        //}

        /// <summary>
        /// Creates a not implemented exception for the given object.
        /// </summary>
        /// <param name="forType"></param>
        /// <returns></returns>
        internal static Exception CreateZeroLengthArgumentException(string name)
        {
            return new ArgumentOutOfRangeException(string.Format("'{0}' is zero-length.", name));
        }

        ///// <summary>
        ///// Creates a not implemented exception for the given object.
        ///// </summary>
        ///// <param name="forType"></param>
        ///// <returns></returns>
        //internal static Exception CreateZeroLengthObjectException(string name)
        //{
        //    // use a system one...
        //    return CreateException(typeof(InvalidOperationException), typeof(ExceptionHelper).Assembly, "ZeroLengthException", null, new object[] { name });
        //}
	}
}
