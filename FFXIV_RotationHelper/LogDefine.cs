using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIV_RotationHelper
{
    public static class LogDefine
    {
        public enum Type
        {
            ChangePrimaryPlayer = 2,
            AddCombatant = 3,
            Ability = 21,
            AOEAbility = 22,
        }
    }
}
