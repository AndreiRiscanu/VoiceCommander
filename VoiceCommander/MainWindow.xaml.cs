using System.Windows;
using VoiceCommander.CodeBehind;
using VoiceCommander.ViewModels;

namespace VoiceCommander
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SpeechFunctionality.InitializeSpeechFunctionality();

            this.DataContext = DirectoryStructureViewModel.GetDirectoryStructureInstance();
        }
    }
}
