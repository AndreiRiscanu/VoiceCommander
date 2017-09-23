using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
