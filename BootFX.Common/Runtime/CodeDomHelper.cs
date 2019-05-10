using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BootFX.Common
{
    public static class CodeDomHelper
    {
        private static Regex _idRegex;
        private static Regex _acronymRegex;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static CodeDomHelper()
        {
            _idRegex = new Regex(@"(?<pre>\w*)id", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            _acronymRegex = new Regex(@"(?<acronym>[A-Z]{3,})", RegexOptions.Singleline);
        }

        private static Regex AcronymRegex
        {
            get
            {
                // returns the value...
                return _acronymRegex;
            }
        }

        private static Regex IdRegex
        {
            get
            {
                // returns the value...
                return _idRegex;
            }
        }

        /// <summary>
        /// Suggests a camel case name.
        /// </summary>
        /// <param name="name"></param>
        public static string GetCamelName(string name)
        {
            // get...
            name = GetPascalName(name);
            if (name == null || name.Length == 0)
                return name;

            // return...
            return name.Substring(0, 1).ToLower() + name.Substring(1);
        }

        /// <summary>
        /// Suggests a Pascal case name.
        /// </summary>
        /// <param name="name"></param>
        public static string GetPascalName(string name)
        {
            if (name == null || name.Length == 0)
                return string.Empty;

            try
            {
                // is the name all upper case or all lower case?
                int numUpper = 0;
                int numLower = 0;
                int numNonChar = 0;
                foreach (char c in name)
                {
                    if (char.IsLetter(c))
                    {
                        if (char.IsUpper(c))
                            numUpper++;
                        else
                            numLower++;
                    }
                    else
                        numNonChar++;
                }

                // check...
                if ((numUpper == name.Length - numNonChar) || (numLower == name.Length - numNonChar))
                    return GetPascalNameForSingleCaseName(name);
                else
                    return GetPascalNameForMixedCaseName(name);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Failed to get Pascal version of '{0}'.", name), ex);
            }
        }

        /// <summary>
        /// Gets the Pascal case name for mixed case name, e.g. XMLDocument.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetPascalNameForMixedCaseName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentOutOfRangeException("'name' is zero-length.");

            // match them...
            var context = new AcronymContext();
            context.Name = name;
            name = AcronymRegex.Replace(name, new MatchEvaluator(context.Evaluate));

            // set the first upper case...
            name = name.Substring(0, 1).ToUpper() + name.Substring(1);
            return name;
        }

        /// <summary>
        /// Context for use with <see cref="GetPascalNameForMixedCaseName"></see>.
        /// </summary>
        private class AcronymContext
        {
            public string Name;

            /// <summary>
            /// Evaluates a match.
            /// </summary>
            /// <param name="match"></param>
            /// <returns></returns>
            public string Evaluate(Match match)
            {
                if (match == null)
                    throw new ArgumentNullException("match");

                // eval...
                string value = match.Value;
                if (match.Index + match.Length == this.Name.Length)
                {
                    value = value.Substring(0, 1).ToUpper() + value.Substring(1, value.Length - 1).ToLower();
                }
                else
                {
                    value = value.Substring(0, 1).ToUpper() + value.Substring(1, value.Length - 2).ToLower() + value.Substring(value.Length - 1);
                }

                // return...
                return value;
            }
        }

        /// <summary>
        /// Gets the Pascal case name for ALL UPPER or all lower names.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetPascalNameForSingleCaseName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentOutOfRangeException("'name' is zero-length.");

            // return...
            name = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

            // look for an id match...
            Match match = IdRegex.Match(name);
            if (match.Success)
            {
                // mangle the ID...
                name = match.Groups["pre"].Value + "Id";
            }

            // return...
            return name;
        }

        public static string SanitizeName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentOutOfRangeException("'name' is zero-length.");

            // walk...
            StringBuilder builder = new StringBuilder();
            foreach (char c in name)
            {
                if (char.IsLetterOrDigit(c))
                    builder.Append(c);
                else
                    builder.Append("_");
            }

            // get...
            name = builder.ToString();
            if (name == null)
                throw new InvalidOperationException("'name' is null.");
            if (name.Length == 0)
                throw new InvalidOperationException("'name' is zero-length.");

            // check...
            if (char.IsDigit(name[0]))
                name = "_" + name;

            // return...
            return name;
        }
    }
}
