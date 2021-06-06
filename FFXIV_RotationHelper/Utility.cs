using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    class Utility
    {
        public static readonly char keySeparator = '\t';
        public static readonly char valueSeparator = '\n';

        public static string ObjectToString(object o)
        {
            return o == null ? string.Empty : o.ToString();
        }
    }
}
