using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FFXIV_RotationHelper
{
    public class SkillData
    {
        [JsonProperty("idx")]
        public int Idx { get; set; }

        [JsonProperty("icon")]
        public string IconURL { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

#if DEBUG
        public override string ToString()
        {
            return string.Format("Idx:{0,-10}Name:{1}", Idx, Name);
        }
#endif
    }
}
