using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using VoiceCommander.CodeBehind;
using VoiceCommander.CommandLine;
using VoiceCommander.ViewModels;

namespace VoiceCommander.UI
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        public MenuView()
        {
            InitializeComponent();
        }
        
        private void ShowHelp(object sender, RoutedEventArgs e)
        {
            Command.Execute("help", null);
        }

        private void DisableEnableVoiceRec(object sender, RoutedEventArgs e)
        {
            MenuItem Item = (MenuItem)sender;

            if ((string)Item.Header == "Disable voice recognition")
            {
                SpeechFunctionality.DeactivateSpeechRecognition();

                Item.Header = "Enable voice recognition";
            }
            else
            {
                SpeechFunctionality.InitializeSpeechFunctionality();

                Item.Header = "Disable voice recognition";
            }
        }
    }
}
