using System.Globalization;

namespace FFXIV_RotationHelper
{
    public struct LogData
    {
        public bool IsByMe { get; private set; }
        public int Code { get; private set; }
        public int DBCode { get; private set; }
        public bool IsValid { get; private set; }
        
        public LogData(string[] log)
        {
            try
            {
                // 0    1                                   2          3       4
                // 21 | 2019-01-09T20:37:33.9620000+09:00 | 100D3D81 | Ellie | B33 | 재장전|100D3D81|Ellie|3D|8000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|51144|51144|9480|9480|950|1000|118.3884|97.36858|0|51144|51144|9480|9480|950|1000|118.3884|97.36858|0||1e0d6c90c8a6fe6d77524cd0551b6894
                // 22 | 2019-01-09T20:38:14.4120000+09:00 | 100D3D81 | Ellie | B30 | 자동포탑 룩|100D3D81|Ellie|32|80000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|42514|51144|9480|9480|980|1000|104.6286|98.71263|0|42514|51144|9480|9480|980|1000|104.6286|98.71263|0||74c560b31278b23ce2b9e32478b45ebc
                uint casterCode = uint.Parse(log[2], NumberStyles.HexNumber);
                Code = int.Parse(log[4], NumberStyles.HexNumber);
                IsByMe = (PlayerData.Instance.Code == casterCode) || (PlayerData.Instance.PetCode == casterCode);
                DBCode = DB.ConvertCode(Code);

                IsValid = IsByMe;
            }
            catch
            {
                IsByMe = false;
                Code = -1;
                DBCode = -1;

                IsValid = false;
            }
        }
    }
}
