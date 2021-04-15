using CsvHelper;
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
        /// <summary>
        /// DB loaded from https://ffxivrotations.com/db.json
        /// </summary>
        private static readonly Dictionary<DBIdx, SkillData> data = new Dictionary<DBIdx, SkillData>();

        /// <summary>
        /// Is used to find DB using GameIdx
        /// </summary>
        private static readonly Dictionary<GameIdx, DBIdx> gameToDb = new Dictionary<GameIdx, DBIdx>();

        /// <summary>
        /// Stores DBIdxes which is not supported (e.g. potions)
        /// </summary>
        private static readonly HashSet<DBIdx> ignoreSet = new HashSet<DBIdx>();

        /// <summary>
        /// To deal with DB data corruption, stores specified DBIdx, GameIdx pair
        /// </summary>
        private static readonly Dictionary<DBIdx, GameIdx> adjustTable = new Dictionary<DBIdx, GameIdx>();

        public static bool IsLoaded { get; private set; }

        public static async Task LoadAsync()
        {
            await LoadAdjustTable();
            await LoadDB();

            IsLoaded = true;
        }

        private static async Task LoadAdjustTable()
        {
            HttpWebRequest request = WebRequest.Create("https://raw.githubusercontent.com/Elysia-ff/FFXIV_RotationHelper-resources/master/Output/ActionTable/ActionTable.csv") as HttpWebRequest;
            using (HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse)
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string content = await streamReader.ReadToEndAsync();

                using (StringReader reader = new StringReader(content))
                using (CsvReader csv = new CsvReader(reader))
                {
                    await csv.ReadAsync();
                    while (await csv.ReadAsync())
                    {
                        string[] records = csv.Context.Record;
                        int dbIdx = int.Parse(records[0]);
                        int gameIdx = int.Parse(records[1]);

                        adjustTable.Add(new DBIdx(dbIdx), new GameIdx(gameIdx));
                    }
                }
            }
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

                            if (!data.ContainsKey(skillData.DBIdx))
                            {
                                data.Add(skillData.DBIdx, skillData);
                            }
#if DEBUG
                            else
                            {
                                throw new System.ArgumentException($"({(int)skillData.DBIdx}) key already exists in data");
                            }
#endif

                            if (!gameToDb.ContainsKey(skillData.GameIdx))
                            {
                                gameToDb.Add(skillData.GameIdx, skillData.DBIdx);
                            }
#if DEBUG
                            else
                            {
                                throw new System.ArgumentException($"({(int)skillData.GameIdx}) key already exists in gameToDb");
                            }
#endif
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

        public static bool IsAdjustedIdx(DBIdx dbIdx, ref GameIdx output)
        {
            if (adjustTable.ContainsKey(dbIdx))
            {
                output = adjustTable[dbIdx];
                return true;
            }

            return false;
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
