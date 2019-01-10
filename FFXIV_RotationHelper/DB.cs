using CsvHelper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static Dictionary<string, string> table;
        public static bool IsLoaded { get; private set; }

        private static readonly Regex skillRegex = new Regex(@"\d+:{icon:[^}]*}");

        public static async Task LoadAsync()
        {
            #region Table
            using (StringReader reader = new StringReader(Properties.Resources.ActionTable))
            using (CsvReader csv = new CsvReader(reader))
            {
                table = new Dictionary<string, string>();
                await csv.ReadAsync();
                while (await csv.ReadAsync())
                {
                    table.Add(csv.Context.Record[0], csv.Context.Record[1]);
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
#if DEBUG
                    Debug.WriteLine(skillData.Idx + "," + skillData.Name);
#endif
                }
            }

            
            return list;
        }

        public static string ConvertCode(string code)
        {
            if (table.ContainsKey(code))
                return table[code];

            return code;
        }
    }
}
