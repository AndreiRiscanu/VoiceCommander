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

        private void GetCommand(string command)
        {
            if (Command.GetCommand(command as string) != null)
            {
                string[] param = (command as string).Split(new Char[] { ' ' });

                if (param.Length > 1)
                {
                    Command.Execute(param[0], param.Skip(1).ToArray());

                    return;
                }

                Command.Execute(command, null);
            }
        }
    }
}
