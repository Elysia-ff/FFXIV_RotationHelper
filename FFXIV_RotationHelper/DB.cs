using FFXIV_RotationHelper.StrongType;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public static class DB
    {
        private static readonly Dictionary<DBIdx, SkillData> data = new Dictionary<DBIdx, SkillData>();
        private static readonly Dictionary<GameIdx, DBIdx> gameToDb = new Dictionary<GameIdx, DBIdx>();
        private static readonly HashSet<DBIdx> ignoreSet = new HashSet<DBIdx>();

        public static bool IsLoaded { get; private set; }

        public static async Task LoadAsync()
        {
            await LoadDB();

            IsLoaded = true;
        }

        private static async Task LoadDB()
        {
            HttpWebRequest request = WebRequest.Create("https://ffxivrotations.com/db.json") as HttpWebRequest;
            using (HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse)
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string content = await streamReader.ReadToEndAsync();
                JObject jObject = JObject.Parse(content);
                
                if (jObject.TryGetValue("skills", out JToken skills))
                {
                    foreach (JProperty jData in skills.Children<JProperty>())
                    {
                        if (string.IsNullOrEmpty(jData.Value.Value<string>("deprecated")))
                        {
                            SkillData skillData = new SkillData(jData);
                            data.Add(skillData.DBIdx, skillData);

                            // TODO :: adjust duplicated game idxes 
                            if (gameToDb.ContainsKey(skillData.GameIdx))
                            {
                                // game : db

                                // 7423 : 802
                                // 7423 : 12021
                                // "Aetherpact"

                                // 167 : 167
                                // 167 : 12149
                                // "Energy Drain"
                            }
                            else
                            {
                                gameToDb.Add(skillData.GameIdx, skillData.DBIdx);
                            }
                        }
                    }
                }

                if (jObject.TryGetValue("misc", out JToken misc))
                {
                    foreach (JValue jValue in misc.Children<JValue>())
                    {
                        int idx = jValue.Value<int>();
                        ignoreSet.Add(new DBIdx(idx));
                    }
                }
            }

#if DEBUG
            Debug.WriteLine("Skill Count : " + data.Count);
#endif
        }

        public static List<SkillData> Get(List<DBIdx> sequence)
        {
            List<SkillData> list = new List<SkillData>();

            if (sequence == null || sequence.Count <= 0)
            {
                return list;
            }

            for (int i = 0; i < sequence.Count; ++i)
            {
                if (data.TryGetValue(sequence[i], out SkillData skillData))
                {
                    list.Add(skillData);
                }
            }
            
            return list;
        }

        public static DBIdx Convert(GameIdx gameIdx)
        {
            if (gameToDb.ContainsKey(gameIdx))
            {
                return gameToDb[gameIdx];
            }

            return (DBIdx)(int)gameIdx;
        }

        public static bool IsIgnoreSet(DBIdx dBIdx)
        {
            return ignoreSet.Contains(dBIdx);
        }

#if DEBUG
        public static List<SkillData> Find(string actionName)
        {
            try
            {
                actionName = actionName.Replace(" ", "").ToLower();

                List<SkillData> list = new List<SkillData>();
                foreach (KeyValuePair<DBIdx, SkillData> kv in data)
                {
                    string str = kv.Value.Name.Replace(" ", "").ToLower();
                    if (str.Equals(actionName))
                    {
                        list.Add(kv.Value);
                    }
                }

                return list;
            }
            catch
            {
                return new List<SkillData>();
            }
        }
#endif
    }
}
