using CsvHelper;
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

        private static readonly Regex skillRegex = new Regex(@"\d+:{icon:[^}]*}");

        public static async Task LoadAsync()
        {
            #region Table
            using (StringReader reader = new StringReader(Properties.Resources.ActionTable))
            using (CsvReader csv = new CsvReader(reader))
            {
                table = new Dictionary<int, int>();
                await csv.ReadAsync();
                while (await csv.ReadAsync())
                {
                    try
                    {
                        string[] records = csv.Context.Record;
                        int code = int.Parse(records[0], NumberStyles.HexNumber);
                        int dbCode = int.Parse(records[1]);
                        table.Add(code, dbCode);
                    }
                    catch
                    {
                    }
                }
            }
            #endregion

            #region DB
            HttpWebRequest request = WebRequest.Create("http://ffxivrotations.com/db3.js") as HttpWebRequest;
            HttpWebResponse response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null) as HttpWebResponse;
            StreamReader streamReader = new StreamReader(response.GetResponseStream());
            string content = await streamReader.ReadToEndAsync();

            data = new Dictionary<int, SkillData>();
            MatchCollection mc = skillRegex.Matches(content);
            foreach (Match match in mc)
            {
                SkillData skillData = new SkillData();
                skillData.Load(match.Value);

                data.Add(skillData.Idx, skillData);
            }
            #endregion

            IsLoaded = true;
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
