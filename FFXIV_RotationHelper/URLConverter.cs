using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public static class URLConverter
    {
        private static readonly string baseURL = "0123456789abcdefghijklmnopqrstuvwxyz";
        private static readonly int baseURLLength = baseURL.Length;

        private static readonly char urlSeparator = '/';
        private static readonly string ffxivRotationURL = "http://ffxivrotations.com/load.py?id={0}";

        public static string Convert(string url)
        {
            url = url.TrimEnd(urlSeparator).Split(urlSeparator).Last().ToLower();

            int c = 0, d = 1;
            for (int e = url.Length - 1; e >= 0; --e)
            {
                c += baseURL.IndexOf(url[e]) * d;
                d *= baseURLLength;
            }

            return string.Format(ffxivRotationURL, c);
        }
    }
}
