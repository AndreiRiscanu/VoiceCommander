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

        // TODO: find a way to replace this with a Command so it respects the MVVM design pattern
        private void PastePath(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                if (Keyboard.IsKeyDown(Key.F))
                {
                    var textbox = sender as TextBox;

                    textbox.AppendText(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent);
                    textbox.CaretIndex = textbox.Text.Length;
                }
            }
        }
    }
}
