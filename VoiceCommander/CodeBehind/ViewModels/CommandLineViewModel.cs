using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using VoiceCommander.CommandLine;
using VoiceCommander.Commands;

namespace VoiceCommander.ViewModels
{
    class CommandLineViewModel : BaseViewModel
    {
        public ICommand ExecuteCommand { get; set; }

        public string currentCommand { get; set; } = "";

        public CommandLineViewModel()
        {
            this.ExecuteCommand = new ExecuteCommand(this.GetCommand);

            commandHistory = new List<string>();
        }

        private List<string> commandHistory { get; set; }

        private int commandIndex = 0;

        /// <summary>
        /// Execute the command, if it exists
        /// </summary>
        /// <param name="currentCommand"></param>
        private void GetCommand(string parameter)
        {
            #region Switch Command

            if (parameter != null)
            {
                if (parameter == "Up")
                {
                    // Don't overflow
                    if (commandIndex < commandHistory.Count)
                    {
                        // Move index to previous command
                        commandIndex += commandIndex == commandHistory.Count ? 0 : 1;

                        currentCommand = commandHistory[commandHistory.Count - commandIndex];
                    }

                    return;
                }
                else if (parameter == "Down")
                {
                    if (commandIndex > 0)
                    {
                        // Move index to next command
                        commandIndex -= commandIndex == 1 ? 0 : 1;

                        currentCommand = commandHistory[commandHistory.Count - commandIndex];
                    }

                    return;
                }
            }

            #endregion

            if (currentCommand == null || currentCommand == "")
                return;
            
            // Split the command from the arguments, removing any white spaces
            string[] param = currentCommand.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            commandHistory.Add(currentCommand);

            currentCommand = "";

            // Save only the last 20 commands
            if (commandHistory.Count > 20)
            {
                commandHistory.RemoveAt(0);
            }

            // If the command exists in the Dictionary
            if (Command.GetCommand(param[0] as string) != null)
            {
                // And at least one parameter was given
                if (param.Length > 1)
                {
                    // Execute the command with the given parameters
                    Command.Execute(param[0], param.Skip(1).ToArray());

                    return;
                }

                // Else, execute the command alone
                Command.Execute(param[0], null);
            }
        }
    }
}
