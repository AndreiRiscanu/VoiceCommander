using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using VoiceCommander.CodeBehind.ViewModels;
using VoiceCommander.CommandLine;
using VoiceCommander.Commands;
using VoiceCommander.Helpers;

namespace VoiceCommander.ViewModels
{
    class CommandLineViewModel : BaseViewModel
    {
        #region Public Members

        public ICommand ExecuteCommand { get; set; }

        public string CurrentCommand { get; set; } = "";
        
        public CommandLineViewModel()
        {
            this.ExecuteCommand = new ExecuteCommand(this.GetCommand);

            CommandHistory = new List<string>();
        }

        #endregion Public Members

        #region Private Members

        private List<string> CommandHistory { get; set; }

        private int CommandIndex = 0;

        /// <summary>
        /// Execute the command, if it exists
        /// </summary>
        /// <param name="currentCommand"></param>
        private void GetCommand(string parameter)
        {
            #region Switch Command
            
            if (parameter == "Up")
            {
                // Don't overflow
                if (CommandIndex < CommandHistory.Count)
                {
                    // Move index to previous command
                    CommandIndex += CommandIndex == CommandHistory.Count ? 0 : 1;

                    CurrentCommand = CommandHistory[CommandHistory.Count - CommandIndex];
                }

                return;
            }
            else if (parameter == "Down")
            {
                if (CommandIndex > 0)
                {
                    // Move index to next command
                    CommandIndex -= CommandIndex == 1 ? 0 : 1;

                    CurrentCommand = CommandHistory[CommandHistory.Count - CommandIndex];
                }

                return;
            }

            #endregion

            if (parameter == "Paste")
            {
                CurrentCommand += DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent;

                return;
            } else if (parameter == "Help")
            {
                Command.Execute("help", null);

                return;
            }

            if (CurrentCommand == null || CurrentCommand == "")
                return;
            
            // Split the command from the arguments, removing any white spaces
            string[] param = CurrentCommand.Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            ManageCommands();

            // If the command exists in the Dictionary
            if (Command.GetCommand(param[0] as string) != null)
            {
                // And at least one parameter was given
                if (param.Length > 1)
                {
                    for (int i = 1; i < param.Length; ++i)
                    {
                        param[i] = ConvertToPath(param[i]);
                    }
                    
                    // Execute the command with the given parameters
                    Command.Execute(param[0], param.Skip(1).ToArray());

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();

                    return;
                }

                // Else, execute the command alone
                Command.Execute(param[0], null);

                DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
            }
            else
                CodeBehind.ViewModels.OutputStringItemViewModel.GetOutputStringInstance().Output = "Invalid Command";
        }

        /// <summary>
        /// Convert the number given as parameter to the path of the item at that position, or return the path unchanged
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string ConvertToPath(string path)
        {
            var numberOfDigitsInItemsLength = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Count.ToString().Length;

            if (path.ElementAt(0) == '\\' && path.Length > 1 && path.Length <= numberOfDigitsInItemsLength + 1)
            {
                var itemNumber = path.Substring(1);

                // Check if there are only numbers in the given size
                foreach (char c in itemNumber)
                {
                    if (c < '0' || c > '9')
                        return path;
                }

                var element = Int32.Parse(itemNumber);

                // Get the element at the specified position
                if (element < DirectoryStructure.numberOfItems)
                    return DirectoryStructure.getElementAtPosition(element);
            }

            return path;
        }

        /// <summary>
        /// Add the new command to the history of commands
        /// </summary>
        private void ManageCommands()
        {
            CommandHistory.Add(CurrentCommand);
            CommandIndex = 0;
            CurrentCommand = "";

            // Save only the last 20 commands
            if (CommandHistory.Count > 20)
            {
                CommandHistory.RemoveAt(0);
            }
        }

        #endregion
    }
}
