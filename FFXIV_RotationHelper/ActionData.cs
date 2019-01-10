using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public class ActionData
    {
        public string Code { get; private set; }
        public int DBIdx { get; private set; }
    }
}
