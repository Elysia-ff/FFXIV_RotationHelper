using System.Collections.Generic;

namespace FFXIV_RotationHelper
{
    public static class Command
    {
        private static readonly Dictionary<string, Method> commands = new Dictionary<string, Method>();

        public static void Bind(string commandName, Method method)
        {
            if (commands.ContainsKey(commandName))
            {
                return;
            }

            commands.Add(commandName, method);
        }

        public static void Execute(string commandName)
        {
            if (commands.TryGetValue(commandName, out Method method))
            {
                method.Run();
            }
        }
    }
}
