using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public static class DB
    {
        private static Dictionary<int, SkillData> data;
        private static Dictionary<int, int> table;
        public static bool IsLoaded { get; private set; }

        public static async Task LoadAsync()
        {
            await LoadTable();
            await LoadDB();

            IsLoaded = true;
        }

        private static async Task LoadTable()
        {
            HttpWebRequest request = WebRequest.Create("https://raw.githubusercontent.com/Elysia-ff/FFXIV_RotationHelper-resources/master/Output/ActionTable/ActionTable.csv") as HttpWebRequest;
            using (HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse)
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string content = await streamReader.ReadToEndAsync();

                using (StringReader reader = new StringReader(content))
                using (CsvReader csv = new CsvReader(reader))
                {
                    table = new Dictionary<int, int>();
                    await csv.ReadAsync();
                    while (await csv.ReadAsync())
                    {
                        try
                        {
                            string[] records = csv.Context.Record;
                            int code = int.Parse(records[0]);
                            int dbCode = int.Parse(records[1]);

                            if (code != dbCode && !table.ContainsKey(code))
                                table.Add(code, dbCode);
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private static async Task LoadDB()
        {
            HttpWebRequest request = WebRequest.Create("https://raw.githubusercontent.com/Elysia-ff/FFXIV_RotationHelper-resources/master/Output/DB/db.json") as HttpWebRequest;
            using (HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse)
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
            {
                string content = await streamReader.ReadToEndAsync();

                List<SkillData> dataList = JsonConvert.DeserializeObject<List<SkillData>>(content);
                data = new Dictionary<int, SkillData>();
                for (int i = 0; i < dataList.Count; ++i)
                {
                    data.Add(dataList[i].Idx, dataList[i]);
                }
            }

#if DEBUG
            Debug.WriteLine("Skill Count : " + data.Count);
#endif
        }

        public static List<SkillData> Get(List<int> sequence)
        {
            List<SkillData> list = new List<SkillData>();

            if (sequence == null || sequence.Count <= 0)
                return list;

            SkillData skillData = null;
            for (int i = 0; i < sequence.Count; ++i)
            {
                if (data.TryGetValue(sequence[i], out skillData))
                {
                    list.Add(skillData);
                }
            }
            
            return list;
        }

        public static int ConvertCode(int code)
        {
            if (table.ContainsKey(code))
                return table[code];

            return code;
        }

#if DEBUG
        public static List<SkillData> Find(string actionName)
        {
            try
            {
                actionName = actionName.Replace(" ", "").ToLower();

                List<SkillData> list = new List<SkillData>();
                foreach (KeyValuePair<int, SkillData> kv in data)
                {
                    string str = kv.Value.Name.Replace(" ", "").ToLower();
                    if (str.Equals(actionName))
                        list.Add(kv.Value);
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
