// BootFX - Application framework for .NET applications
// 
// File: DelimitedDataReader.cs
// Build: 5.2.10321.2307
// 
// An open source project by Matthew Reynolds (@mbrit).  
// Copyright 2001-2018 Matthew Reynolds, Red Piranha Labs Limited,
// Elixia Solutions Limited.  All Rights Reserved.
//
// Licensed under the MIT license.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BootFX.Common.Data.Text
{
    public abstract class DelimitedDataReader : TextDataReader
    {
        /// <summary>
        /// Private value to support the <see cref="Delimiter">Delimiter</see> property.
        /// </summary>
        private char _delimiter;

        protected DelimitedDataReader(TextReader reader, bool firstRowIsHeader, char delimiter)
			: base(reader, firstRowIsHeader)
		{
            _delimiter = delimiter;
		}

        protected override string[] ReadValuesFromStream()
        {
            // get...
            string buf = this.ReadLine();

            // mbr - 10-05-2008 - changed to handle zero-length...
            if (buf == null)
                return null;

            // walk...
            ArrayList results = new ArrayList();
            StringBuilder builder = null;
            bool inQuotes = false;
            for (int index = 0; index < buf.Length; index++)
            {
                char c = buf[index];

                // get...
                if (c == '\"')
                {
                    // if we're not in quotes, change it...
                    if (!(inQuotes))
                        inQuotes = true;
                    else
                    {
                        // ok, peek - is the next char a quote?
                        if (index < buf.Length - 1)
                        {
                            if (buf[index + 1] == '\"')
                            {
                                // add and bump...
                                builder.Append("\"");
                                index++;
                            }
                            else
                                inQuotes = false;
                        }
                        else
                            inQuotes = false;
                    }
                }
                else if (c == this.Delimiter && !(inQuotes))
                {
                    // clear...
                    if (builder != null)
                    {
                        results.Add(builder.ToString());
                        builder = new StringBuilder();
                    }
                }
                else
                {
                    if (builder == null)
                        builder = new StringBuilder();
                    builder.Append(c);
                }
            }

            // set...
            if (builder != null)
                results.Add(builder.ToString());

            // return...
            string[] values = (string[])results.ToArray(typeof(string));

            // mbr - 10-05-2008 - if the zeroth value is empty, call that the end of the file...
            if (values.Length == 0)
                return null;

            // mbr - 25-06-2008 - trim the values...
            for (int index = 0; index < values.Length; index++)
            {
                if (values[index] != null)
                    values[index] = values[index].Trim();
            }

            // ok...
            return values;
        }

        /// <summary>
        /// Gets the Delimiter value.
        /// </summary>
        private char Delimiter
        {
            get
            {
                return _delimiter;
            }
        }
    }
}
