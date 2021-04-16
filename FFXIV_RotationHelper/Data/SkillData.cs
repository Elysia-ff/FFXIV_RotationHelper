using FFXIV_RotationHelper.StrongType;
using Newtonsoft.Json.Linq;

namespace FFXIV_RotationHelper
{
    public readonly struct SkillData
    {
        public readonly DBIdx DBIdx;
        public readonly string IconURL;

#if DEBUG
        public readonly string Name;
#endif

        private static readonly string iconURLFormat = "https://ffxivrotations.com/icon/{0}.png";

        public SkillData(DBIdx dbIdx, JObject jObject)
        {
            DBIdx = dbIdx;

            string icon = jObject.Value<string>("icon");
            IconURL = string.Format(iconURLFormat, icon);

#if DEBUG
            Name = jObject.Value<string>("name");
#endif
        }

#if DEBUG
        public override string ToString()
        {
            return $"DBIdx : {(int)DBIdx} // Name : {Name}";
        }
#endif
    }
}
