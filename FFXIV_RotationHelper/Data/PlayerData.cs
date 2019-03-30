using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                //     0  1                                 2        3
                // Me  02|2019-01-12T23:31:21.3100000+09:00|100d3d81|Ellie|bd34d5f741c6d8d6721c51699ae8aafc
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
                    return false;

                //     0  1                                 2        3           4 5  6    7    8
                // Pet 03|2019-01-13T00:55:58.3230000+09:00|40019742|요정 에오스|0|46|a8d8|2ee0|100d3d81|918bd70cb1e0ccbed68d3b600ac8dd18
                uint summonerCode = uint.Parse(log[8], NumberStyles.HexNumber);
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
                    return false;

                //     0  1                                 2        3           4 5  6    7    8
                // Pet 04|2019-03-29T21:54:57.2570000+09:00|40010d5e|자동포탑 룩|0|46|ca05|2ee0|100d3d81|0||25deed966a547a062f45df5388bc6a1c
                uint summonerCode = uint.Parse(log[8], NumberStyles.HexNumber);
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
