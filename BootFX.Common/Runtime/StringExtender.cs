using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BootFX.Common
{
    public static class StringExtender
    {
        public static IEnumerable<string> SplitAndTrim(this string buf, params char[] separator)
        {
            return CleanupSplitStrings(buf.Split(separator));
        }

        private static IEnumerable<string> CleanupSplitStrings(IEnumerable<string> bufs)
        {
            var results = new List<string>();
            foreach (var buf in bufs)
            {
                var useBuf = buf.Trim();
                if (useBuf.Length > 0)
                    results.Add(useBuf);
            }
            return results;
        }

        public static string RemoveFromStart(this string buf, string toFind)
        {
            while (buf.StartsWith(toFind))
                buf = buf.Substring(toFind.Length);
            return buf;
        }

        public static string RemoveFromEnd(this string buf, string toFind)
        {
            while (buf.EndsWith(toFind))
                buf = buf.Substring(0, buf.Length - toFind.Length);
            return buf;
        }

        public static string RemoveFromStartAndEnd(this string buf, string toFind)
        {
            buf = buf.RemoveFromStart(toFind);
            buf = buf.RemoveFromEnd(toFind);
            return buf;
        }
    }
}
