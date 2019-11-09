using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public struct LogData
    {
        public bool IsByMe { get; private set; }
        public int Code { get; private set; }
        public int DBCode { get; private set; }
        public bool IsValid { get; private set; }
        private readonly DateTime time;

        private static DateTime latestLogTime = DateTime.MinValue;
        private static readonly Regex timeRegex = new Regex(@"\[([0-9.:]+)\]");
        
        public LogData(string[] log)
        {
            try
            {
                // 0  1                                 2        3     4
                // 21|2019-01-09T20:37:33.9620000+09:00|100D3D81|Ellie|B33|재장전|100D3D81|Ellie|3D|8000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|51144|51144|9480|9480|950|1000|118.3884|97.36858|0|51144|51144|9480|9480|950|1000|118.3884|97.36858|0||1e0d6c90c8a6fe6d77524cd0551b6894
                // 22|2019-01-09T20:38:14.4120000+09:00|100D3D81|Ellie|B30|자동포탑 룩|100D3D81|Ellie|32|80000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|42514|51144|9480|9480|980|1000|104.6286|98.71263|0|42514|51144|9480|9480|980|1000|104.6286|98.71263|0||74c560b31278b23ce2b9e32478b45ebc
                time = DateTime.Parse(log[1]);
                if (time.Equals(latestLogTime))
                    throw new Exception();

                uint casterCode = uint.Parse(log[2], NumberStyles.HexNumber);
                Code = int.Parse(log[4], NumberStyles.HexNumber);
                IsByMe = (PlayerData.Instance.Code == casterCode) || (PlayerData.Instance.PetCode == casterCode);
                DBCode = DB.ConvertCode(Code);

                IsValid = IsByMe;
                latestLogTime = time;
            }
            catch
            {
                time = DateTime.MinValue;
                IsByMe = false;
                Code = -1;
                DBCode = -1;

                IsValid = false;
            }
        }

        public LogData(string logLine)
        {
            try
            {
                string[] log = logLine.Split(':');

                // 0   1  2          3        4           5
                // [19:12:19.845] 15:100D3D81:Ellie      :B32:Split Shot:4000B9EB:나무인형:720003:ED00000:10:3588000:0:0:0:0:0:0:0:0:0:0:0:0:2778:2778:0:0:1000:1000:64.97395:129.2004:10.02:5062:52370:9480:3192:1000:1000:55.22277:126.8233:10.02:
                // [19:10:29.401] 16:100D3D81:Ellie      :B30:Rook Autoturret:100D3D81:Ellie:33:80000:0:0:0:0:0:0:0:0:0:0:0:0:0:0:5062:52370:9480:3192:1000:1000:55.22277:126.8233:10.02:5062:52370:9480:3192:1000:1000:55.22277:126.8233:10.02:
                // [19:12:22.057] 15:4001BEF7:자동포탑 룩:B4B:Volley Fire:4000B9EB:나무인형:720003:7020000:1C:B4B8000:0:0:0:0:0:0:0:0:0:0:0:0:2778:2778:0:0:1000:1000:64.97395:129.2004:10.02:52190:52190:12000:12000:1000:1000:55.22229:126.8177:10.02:
                time = DateTime.Parse(timeRegex.Match(logLine).Groups[1].Value);
                if (time.Equals(latestLogTime))
                    throw new Exception();

                uint casterCode = uint.Parse(log[3], NumberStyles.HexNumber);
                Code = int.Parse(log[5], NumberStyles.HexNumber);
                IsByMe = (PlayerData.Instance.Code == casterCode) || (PlayerData.Instance.PetCode == casterCode);
                DBCode = DB.ConvertCode(Code);

                IsValid = IsByMe;
                latestLogTime = time;
            }
            catch
            {
                time = DateTime.MinValue;
                IsByMe = false;
                Code = -1;
                DBCode = -1;

                IsValid = false;
            }
        }
    }
}
