using System;
using System.Text.RegularExpressions;

namespace CrawlerForDIGitEx
{
    public static class StringExtension
    {
        public static String EnQuotes(this String sOriginal)
        {
            const string quote = "\"";
            return quote + sOriginal + quote;
        }
         
        public static string InserDelimeter(this String sOriginal)
        {
            return sOriginal + ",";
        }

        public static bool IsInteger(this String sOriginal)
        {
            Int32 iNum = 0;
            return Int32.TryParse(sOriginal, out iNum);
        }

        public static String TrimAll(this String sOriginal, String sPattern)
        {
            return Regex.Replace(sOriginal, sPattern, "").Trim();
        }

        public static String TrimAll(this String sOriginal, String sPattern, String sReplace)
        {
            return Regex.Replace(sOriginal, sPattern, sReplace).Trim();
        }
    }
}
