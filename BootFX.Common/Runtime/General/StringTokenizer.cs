// BootFX - Application framework for .NET applications
// 
// File: StringTokenizer.cs
// Build: 5.0.61009.900
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.Text.RegularExpressions;
using System.Collections;
using BootFX.Common.Data;
using BootFX.Common.Management;

namespace BootFX.Common
{
	/// <summary>
	/// Summary description for StringTokeniser.
	/// </summary>
	public class StringTokenizer : ITokenEvaluatorHost
	{
		private const string EvalRegex = @"\${(?<name>[\w\.!]*)}";

        /// <summary>
        /// Defines the <c>SanitizeValue</c> event.
        /// </summary>
        // mbr - 2010-08-25 - added...
        public event SanitizeValueEventHandler SanitizeValue;

        public event ConvertToStringEventHandler StringConversionNeeded;
        public event TokenEvaluatorEventHandler TokenNotFound;

        public StringTokenizer()
		{
		}

        /// <summary>
        /// Raises the <c>SanitizeValue</c> event.
        /// </summary>
        protected virtual void OnSanitizeValue(SanitizeValueEventArgs e)
        {
            // raise...
            if (SanitizeValue != null)
                SanitizeValue(this, e);
        }

        public string Replace(string text, object context)
        {
            return this.Replace(text, context, EvaluateFlags.Normal);
        }

        // mbr - 2009-04-24 - added overload to allow passing of "ignore null values" check...
		public string Replace(string text, object context, EvaluateFlags flags)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			
			// check...
			if(text == null || text.Length == 0)
				return text;

			// repalce....
			Regex regex = new Regex(EvalRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
			EvalWorker worker = new EvalWorker(this, context, flags);
			return regex.Replace(text, new MatchEvaluator(worker.EvaluatePlaceholder));
		}

		private class EvalWorker : Loggable
		{
            private StringTokenizer Tokenizer;
			private TokenEvaluator Evaluator;
            private EvaluateFlags Flags;

			internal EvalWorker(StringTokenizer tokenizer, object context, EvaluateFlags flags)
			{
                this.Tokenizer = tokenizer;
                Flags = flags;
				Evaluator = new TokenEvaluator(context, tokenizer);
                this.Evaluator.TokenNotFound += new TokenEvaluatorEventHandler(Evaluator_TokenNotFound);
			}

            void Evaluator_TokenNotFound(object sender, TokenEvaluatorEventArgs e)
            {
                this.Tokenizer.OnTokenNotFound(e);
            }

			internal string EvaluatePlaceholder(Match match)
			{
				if(match == null)
					throw new ArgumentNullException("match");

				// replace...
				string propertyName = match.Groups["name"].Value;

                // get...
                if (!(string.IsNullOrEmpty(propertyName)))
                {
                    // mbr - 2010-08-25 - added an event...
                    string value = (string)this.Evaluator.Evaluate(propertyName, this.Flags | EvaluateFlags.ConvertResultToString);

                    // if...
                    if (this.Tokenizer.SanitizeValue != null)
                    {
                        SanitizeValueEventArgs e = new SanitizeValueEventArgs(value);
                        this.Tokenizer.OnSanitizeValue(e);

                        // reset...
                        value = e.Value;
                    }

                    // return....
                    return value;
                }
                else
                    return string.Format("(Null property: {0}, {1})", match.Index, match.Length);
            }
		}

        public void OnStringConversionNeeded(ConvertToStringEventArgs e)
        {
            if (this.StringConversionNeeded != null)
                this.StringConversionNeeded(this, e);
        }

        public MatchCollection GetMatches(string buf)
        {
            Regex regex = new Regex(EvalRegex);
            return regex.Matches(buf);
        }

        protected void OnTokenNotFound(TokenEvaluatorEventArgs e)
        {
            if (TokenNotFound != null)
                this.TokenNotFound(this, e);
        }
    }
}
