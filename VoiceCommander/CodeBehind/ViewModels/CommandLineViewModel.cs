using System;
using System.Linq;
using System.Windows.Input;
using VoiceCommander.CommandLine;
using VoiceCommander.Commands;

namespace VoiceCommander.ViewModels
{
    class CommandLineViewModel : BaseViewModel
    {
        public ICommand ExecuteCommand { get; set; }
        
        public CommandLineViewModel()
        {
            this.ExecuteCommand = new ExecuteCommand(this.GetCommand);
        }

        /// <summary>
        /// Execute the command, if it exists
        /// </summary>
        /// <param name="command"></param>
        private void GetCommand(string command)
        {
            if (command == null)
                return;

            // Split the command from the arguments, removing any white spaces
            string[] param = (command as string).Split(new Char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            

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
