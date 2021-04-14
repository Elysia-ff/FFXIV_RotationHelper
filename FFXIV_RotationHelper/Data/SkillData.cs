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
            return $"Idx : {Idx} // Name : {Name}";
        }
#endif
    }
}
