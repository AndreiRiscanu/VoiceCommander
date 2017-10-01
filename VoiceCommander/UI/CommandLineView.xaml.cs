using System.Windows.Controls;
using System.Windows.Input;
using VoiceCommander.ViewModels;

namespace VoiceCommander.UI
{
    /// <summary>
    /// Interaction logic for CommandLineView.xaml
    /// </summary>
    public partial class CommandLineView : UserControl
    {
        public CommandLineView()
        {
            InitializeComponent();

            this.DataContext = new CommandLineViewModel();

            CommandLine.Register reg = new CommandLine.Register();
        }
    }
}
