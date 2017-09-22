using System.Windows;
using System.Windows.Input;
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

        private void GetCommand()
        {
            MessageBox.Show("works");
        }
    }
}
