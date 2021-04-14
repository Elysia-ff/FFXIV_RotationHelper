using System.Globalization;

namespace FFXIV_RotationHelper
{
    public class PlayerData
    {
        #region Singleton
        private static PlayerData instance;
        public static PlayerData Instance => instance ?? (instance = new PlayerData());

        public static void Free()
        {
            instance = null;
        }
        #endregion

        public uint Code { get; private set; } = 0;
        public string Name { get; private set; } = string.Empty;

        public uint PetCode { get; private set; } = 0;
        public string PetName { get; private set; } = string.Empty;

        public bool SetPlayer(string[] log)
        {
            try
            {
                //     0    1                                   2          3
                // Me  02 | 2019-01-12T23:31:21.3100000+09:00 | 100d3d81 | Ellie | bd34d5f741c6d8d6721c51699ae8aafc
                Code = uint.Parse(log[2], NumberStyles.HexNumber);
                Name = log[3];

                return true;
            }
            catch
            {
            }

            return false;
        }

        public bool SetPet(string[] log)
        {
            try
            {
                if (Code <= 0)
                {
                    return false;
                }

                //     0    1                                   2          3     4   5    6
                // Pet 03 | 2019-07-07T00:17:44.7630000+02:00 | 4000ad7d | Eos | 0 | 4f | 100d3d81 |0||1398|1008|67129|67129|10000|10000|0|0|-40.38065|19.00028|84.19518|2.220902||d2655803925a7acc840d57a7b9be2f8c
                uint summonerCode = uint.Parse(log[6], NumberStyles.HexNumber);
                if (Code == summonerCode)
                {
                    PetCode = uint.Parse(log[2], NumberStyles.HexNumber);
                    PetName = log[3];

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public bool RemovePet(string[] log)
        {
            try
            {
                if (Code <= 0 || PetCode <= 0)
                {
                    return false;
                }

                //     0    1                                   2          3        4   5    6
                // Pet 03 | 2019-07-07T00:17:46.1030000+02:00 | 4000ad62 | Selene | 0 | 50 | 100d3d81 |0||1399|1009|69948|69948|10000|10000|0|0|-49.31417|23.3655|84.19518|-1.111434||e1f498150dadecce15670dc1a92b8287

                uint summonerCode = uint.Parse(log[6], NumberStyles.HexNumber);
                uint logCode = uint.Parse(log[2], NumberStyles.HexNumber);
                if (Code == summonerCode && PetCode == logCode)
                {
                    PetCode = 0;
                    PetName = string.Empty;

                    return true;
                }
            }
            catch
            {
            }

            return false;
        }
    }
}
