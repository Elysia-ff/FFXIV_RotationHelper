using FFXIV_RotationHelper.StrongType;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace FFXIV_RotationHelper
{
    public class RotationData
    {
        public string URL { get; private set; } = string.Empty;

        //[JsonProperty("hits")]
        //public int Hits { get; set; }

        [JsonProperty("class")]
        public string Class { get; set; }

        [JsonProperty("sequence")]
        public string SequenceStr { get; set; }

        public List<DBIdx> Sequence { get; private set; }

        public void Initialize(string url)
        {
            URL = url;
            Sequence = SequenceStr?.Split(',')
                .ToList()
                .ConvertAll((s) => (DBIdx)int.Parse(s))
                .Where((i) => !DB.IsIgnoreSet(i))
                .ToList();
        }
    }
}
