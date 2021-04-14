using FFXIV_RotationHelper.StrongType;
using System.Globalization;

namespace FFXIV_RotationHelper
{
    public readonly struct LogData
    {
        public readonly bool IsByMe;
        public readonly GameIdx GameIdx;
        public readonly DBIdx DBIdx;
        public readonly bool IsValid;
        
        public LogData(string[] log)
        {
            try
            {
                // 0    1                                   2          3       4
                // 21 | 2019-01-09T20:37:33.9620000+09:00 | 100D3D81 | Ellie | B33 | 재장전|100D3D81|Ellie|3D|8000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|51144|51144|9480|9480|950|1000|118.3884|97.36858|0|51144|51144|9480|9480|950|1000|118.3884|97.36858|0||1e0d6c90c8a6fe6d77524cd0551b6894
                // 22 | 2019-01-09T20:38:14.4120000+09:00 | 100D3D81 | Ellie | B30 | 자동포탑 룩|100D3D81|Ellie|32|80000|0|0|0|0|0|0|0|0|0|0|0|0|0|0|42514|51144|9480|9480|980|1000|104.6286|98.71263|0|42514|51144|9480|9480|980|1000|104.6286|98.71263|0||74c560b31278b23ce2b9e32478b45ebc
                uint casterCode = uint.Parse(log[2], NumberStyles.HexNumber);
                GameIdx = (GameIdx)int.Parse(log[4], NumberStyles.HexNumber);
                IsByMe = (PlayerData.Code == casterCode) || (PlayerData.PetCode == casterCode);
                DBIdx = DB.Convert(GameIdx);

                IsValid = IsByMe;
            }
            catch
            {
                IsByMe = false;
                GameIdx = new GameIdx(-1);
                DBIdx = new DBIdx(-1);

                IsValid = false;
            }
        }
    }
}
