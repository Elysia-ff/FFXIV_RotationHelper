namespace FFXIV_RotationHelper
{
    public static class LogDefine
    {
        public enum Type
        {
            Chat = 0,
            ChangePrimaryPlayer = 2,
            AddCombatant = 3,
            RemoveCombatant = 4,
            Ability = 21,
            AOEAbility = 22,
        }

        public static readonly string ECHO_CHAT_CODE = "0038";
    }
}
