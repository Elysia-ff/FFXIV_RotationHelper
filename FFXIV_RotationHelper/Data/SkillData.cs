using FFXIV_RotationHelper.StrongType;
using Newtonsoft.Json.Linq;

namespace FFXIV_RotationHelper
{
    public readonly struct SkillData
    {
        public readonly DBIdx DBIdx;
        public readonly string IconURL;
        public readonly GameIdx GameIdx;

#if DEBUG
        public readonly string Name;
#endif

        private static readonly string iconURLFormat = "https://ffxivrotations.com/icon/{0}.png";

        public SkillData(JProperty jProperty)
        {
            DBIdx = (DBIdx)int.Parse(jProperty.Name);

            string icon = jProperty.Value.Value<string>("icon");
            IconURL = string.Format(iconURLFormat, icon);


            GameIdx = new GameIdx(0);
            if (!DB.IsAdjustedIdx(DBIdx, ref GameIdx))
            {
                JToken cToken = jProperty.Value["c"];
                GameIdx = new GameIdx(cToken != null ? (int)cToken : (int)DBIdx);
            }

#if DEBUG
            Name = jProperty.Value.Value<string>("name");
#endif
        }

#if DEBUG
        public override string ToString()
        {
            return $"Idx : {DBIdx} // Name : {Name}";
        }
#endif
    }
}
