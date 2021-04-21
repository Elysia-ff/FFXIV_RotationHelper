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
    using ActionTable = Dictionary<string, Dictionary<GameIdx, List<DBIdx>>>;

    public static class DB
    {
        /// <summary>
        /// DB loaded from https://ffxivrotations.com/db.json
        /// </summary>
        private static readonly Dictionary<string, Dictionary<DBIdx, SkillData>> data = new Dictionary<string, Dictionary<DBIdx, SkillData>>();

        /// <summary>
        /// Is used to find DB using GameIdx
        /// </summary>
        private static readonly ActionTable actionTable = new ActionTable();

        /// <summary>
        /// Stores DBIdxes which is not supported (e.g. potions)
        /// </summary>
        private static readonly HashSet<DBIdx> ignoreSet = new HashSet<DBIdx>();

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
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.Quote = '\"';

                    await csv.ReadAsync();
                    while (await csv.ReadAsync())
                    {
                        string[] records = csv.Context.Record;
                        string className = records[0];
                        if (!actionTable.ContainsKey(className))
                        {
                            actionTable.Add(className, new Dictionary<GameIdx, List<DBIdx>>());
                        }

                        GameIdx gameIdx = (GameIdx)int.Parse(records[2]);
                        if (!actionTable[className].ContainsKey(gameIdx))
                        {
                            actionTable[className].Add(gameIdx, new List<DBIdx>());
                        }

                        DBIdx dbIdx = (DBIdx)int.Parse(records[3]);
                        actionTable[className][gameIdx].Add(dbIdx);
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
                JToken skills = jObject.GetValue("skills");
                JToken classes = jObject.GetValue("classes");

                foreach (JProperty classProperty in classes.Children<JProperty>())
                {
                    string discipline = classProperty.Value.Value<string>("discipline");
                    if (discipline != "war" && discipline != "magic")
                    {
                        continue;
                    }

                    string className = classProperty.Name;
                    if (!data.ContainsKey(className))
                    {
                        data.Add(className, new Dictionary<DBIdx, SkillData>());
                    }

                    foreach (JProperty skillProperty in classProperty.Value.Children<JProperty>())
                    {
                        if (skillProperty.Value.Type != JTokenType.Array)
                        {
                            continue;
                        }

                        foreach (int idx in skillProperty.Value.Values<int>())
                        {
                            JObject skillObject = skills.Value<JObject>(idx.ToString());
                            if (string.IsNullOrEmpty(skillObject.Value<string>("deprecated")))
                            {
                                DBIdx dbIdx = (DBIdx)idx;
                                SkillData skillData = new SkillData(dbIdx, skillObject);
                                data[className].Add(dbIdx, skillData);
                            }
                        }
                    }
                }

                JToken misc = jObject.GetValue("misc");
                foreach (JValue jValue in misc.Children<JValue>())
                {
                    int idx = jValue.Value<int>();
                    ignoreSet.Add((DBIdx)idx);
                }
            }

#if DEBUG
            Debug.WriteLine("Skill Count : " + data.Count);
#endif
        }

        public static List<SkillData> Get(RotationData rotationData)
        {
            if (!data.ContainsKey(rotationData.Class) || rotationData.Sequence == null || rotationData.Sequence.Count <= 0)
            {
                return new List<SkillData>();
            }

            List<SkillData> list = new List<SkillData>();
            for (int i = 0; i < rotationData.Sequence.Count; ++i)
            {
                if (data[rotationData.Class].TryGetValue(rotationData.Sequence[i], out SkillData skillData))
                {
                    list.Add(skillData);
                }
            }
            
            return list;
        }

        public static bool IsSameAction(string className, GameIdx gameIdx, DBIdx dBIdx)
        {
            if (actionTable.ContainsKey(className) && actionTable[className].ContainsKey(gameIdx))
            {
                List<DBIdx> idxes = actionTable[className][gameIdx];
                for (int i = 0; i < idxes.Count; i++)
                {
                    if (idxes[i] == dBIdx)
                    {
                        return true;
                    }
                }
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
                foreach (var d in data)
                {
                    foreach (KeyValuePair<DBIdx, SkillData> kv in data[d.Key])
                    {
                        string str = kv.Value.Name.Replace(" ", "").ToLower();
                        if (str.Equals(actionName))
                        {
                            list.Add(kv.Value);
                        }
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
