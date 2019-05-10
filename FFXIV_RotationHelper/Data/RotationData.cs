using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public class RotationData
    {
        public string URL { get; private set; } = string.Empty;

        [JsonProperty("hits")]
        public int Hits { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("sequence")]
        public string SequenceStr { get; set; }

        public List<int> Sequence { get; private set; }

        public void Initialize(string url)
        {
            URL = url;
            Sequence = SequenceStr.Split(',').ToList().ConvertAll((s) => int.Parse(s));
        }
    }
}
