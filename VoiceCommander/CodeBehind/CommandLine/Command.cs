using System;
using System.Collections.Generic;

namespace VoiceCommander.CommandLine
{
    abstract class Command
    {
        private static readonly IDictionary<string, Action<string[]>> _commands = new Dictionary<string, Action<string[]>>();

        public static void Register(string commandName, Action<string[]> action)
        {
            _commands.Add(commandName, action);
        }

        public static Action<string[]> GetCommand(string commandName)
        {
            if (_commands.ContainsKey(commandName))
                return _commands[commandName];

            return null;
        }

        public static void Execute(string commandName, string[] parameters)
        {
            GetCommand(commandName)(parameters);
        }
    }
}
