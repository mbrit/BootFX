// BootFX - Application framework for .NET applications
// 
// File: TokenEvaluator.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Collections;
using BootFX.Common;
using BootFX.Common.Data;
using BootFX.Common.Entities;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for TokenEvaluator.
	/// </summary>
	public class TokenEvaluator : Loggable
	{
		/// <summary>
		/// Private field to support <see cref="Context"/> property.
		/// </summary>
		private object _context;

        private ITokenEvaluatorHost _host;

        public event TokenEvaluatorEventHandler TokenNotFound;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="context"></param>
		public TokenEvaluator(object context)
            : this(context, new NullEvaluatorHost())
		{
		}

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="context"></param>
        internal TokenEvaluator(object context, ITokenEvaluatorHost host)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            if (host == null)
                throw new ArgumentNullException("host");

            // set...
            _context = context;
            _host = host;
        }

		/// <summary>
		/// Gets the context.
		/// </summary>
		private object Context
		{
			get
			{
				return _context;
			}
		}

		/// <summary>
		/// Evaluates the property expansion.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		[Obsolete("Use EvaluateToString or the overload with EvaluateFlags.")]
		public string Evaluate(string propertyName)
		{
			return EvaluateToString(propertyName);
		}

		/// <summary>
		/// Evaluates the property expansion.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public string EvaluateToString(string propertyName)
		{
			return (string)Evaluate(propertyName, EvaluateFlags.ConvertResultToString);
		}

		/// <summary>
		/// Evaluates the property expansion.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public object Evaluate(string propertyName, EvaluateFlags flags)
		{
			TokenEvaluationResults results = this.EvaluateInternal(propertyName, flags, false);
            if (results == null)
                return string.Format("[Fatal: {0}]", propertyName);

			// what happened?
			if(results.HasError)
				return string.Format("[{0}]", results.Error);
			else
				return results.Value;
		}

		/// <summary>
		/// Evaluates the property expansion, returning more details.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public TokenEvaluationResults Evaluate2(string propertyName, EvaluateFlags flags)
		{
			return this.EvaluateInternal(propertyName, flags, true);
		}

		/// <summary>
		/// Evaluates the property expansion, returning more details.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private TokenEvaluationResults EvaluateInternal(string propertyName, EvaluateFlags flags, bool useAdvancedMethod)
		{
			if(propertyName == null)
				throw new ArgumentNullException("propertyName");
			if(propertyName.Length == 0)
				throw new ArgumentOutOfRangeException("'propertyName' is zero-length.");
			
			// mbr - 13-06-2007 - force string?
			bool forceString = false;
			if((int)(flags & EvaluateFlags.ConvertResultToString) != 0)
				forceString = true;

            // check...
            if (Host == null)
                throw new InvalidOperationException("'Host' is null.");

			// walk...
			try
			{
				// mbr - 02-02-2006 - if the context is dictionary, then go in one level...
				object useObject = null;
				string toEvaluate = propertyName;
				if(Context is IDictionary)
				{
					// trim...
					int index = propertyName.IndexOf(".");
					string remaining = null;
					if(index != -1)
					{
						propertyName = toEvaluate.Substring(0, index).Trim();
						remaining = toEvaluate.Substring(index + 1).Trim();
					}

					// get...
					useObject = ((IDictionary)this.Context)[propertyName];
                    if (useObject == null)
                    {
                        var e = new TokenEvaluatorEventArgs(propertyName);
                        this.OnTokenNotFound(e);

                        // return...
                        if(e.WasResultSet)
                        {
                            // mbr - 2013-06-07 - step through into the remaining items...
                            //return new TokenEvaluationResults(null, DoStringConversion(e.Result));
                            useObject = e.Result;
                        }
                        else
                        {
                            // mbr - 2009-04-24 - if we are ignoring missing values, send back an error...
                            if ((int)(flags & EvaluateFlags.IgnoreMissingValues) != 0)
                                return new TokenEvaluationResults(useObject, null, null, null, string.Format("(Couldn't evaluate [{0}])", propertyName));
                            else
                                return null;
                        }
                    }

					// anything else?
					if(remaining == null || remaining.Length == 0)
					{
                        if (forceString)
                            return new TokenEvaluationResults(useObject, null, DoStringConversion(useObject));
                        else
                            return new TokenEvaluationResults(useObject, null, useObject);
					}

					// flip...
					toEvaluate = remaining;
				}
				else
					useObject = this.Context;

				// mbr - 25-07-2008 - ok, if we have an entity and a single part expression, look for the field, otherwise use data binder...
				if(useAdvancedMethod && useObject is Entity && toEvaluate.IndexOf(".") == -1)
				{
					// find that member!
					EntityType et = EntityType.GetEntityType(useObject, OnNotFound.ThrowException);
					if(et == null)
						throw new InvalidOperationException("et is null.");

					// we're looking for a value...
					object value = null;
					EntityMember usedMember = null;

					// find...
					EntityField field = et.Fields[toEvaluate];
					if(field != null)
					{
						usedMember = field;
						value = field.GetValue(useObject);
					}
					else
					{
						EntityProperty prop = et.Properties[toEvaluate];
						if(prop != null)
						{
							usedMember = prop;
							value = prop.GetValue(useObject);
						}
						else
						{
//							ChildToParentEntityLink link = (ChildToParentEntityLink)et.ChildLinks[toEvaluate];
//							if(link != null)
//							{
//								usedMember = link;
//								value = link.GetValue(useObject);
//							}
//							else
								value = DataBinder.Eval(useObject, toEvaluate);
						}
					}

					// convert...
                    if (forceString)
                        value = DoStringConversion(value);

					// return the value...
                    return new TokenEvaluationResults(useObject, usedMember, value);
				}
				else
				{
					// eval from there...
					object value = DataBinder.Eval(useObject, toEvaluate);
                    if (forceString)
                        value = DoStringConversion(value);

					// return the value...
                    return new TokenEvaluationResults(useObject, null, value);
				}
			}
			catch(Exception ex)
			{
				if(this.Log.IsWarnEnabled)
					this.Log.Warn(string.Format("Failed to evaluate '{0}'.", propertyName), ex);

				// mbr - 25-07-2008 - changed to use object...				
				//return string.Format("(Couldn't evaluate [{0}])", propertyName);
				return new TokenEvaluationResults(null, null, null, ex, string.Format("(Couldn't evaluate [{0}])", propertyName));
			}
		}

        protected void OnTokenNotFound(TokenEvaluatorEventArgs e)
        {
            if (TokenNotFound != null)
                this.TokenNotFound(this, e);
        }

        private string DoStringConversion(object value)
        {
            if (Host == null)
                throw new InvalidOperationException("'Host' is null.");

            // create...
            ConvertToStringEventArgs e = new ConvertToStringEventArgs(value);
            this.Host.OnStringConversionNeeded(e);

            // return...
            return e.AsString;
        }

        private ITokenEvaluatorHost Host
        {
            get
            {
                return _host;
            }
        }
	}
}
