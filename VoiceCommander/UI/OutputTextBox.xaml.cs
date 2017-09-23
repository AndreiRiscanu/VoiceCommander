using System;
using System.Collections.Generic;
using System.Windows.Controls;
using VoiceCommander.CodeBehind.ViewModels;

namespace VoiceCommander.UI
{
    /// <summary>
    /// Interaction logic for OutputTextBox.xaml
    /// </summary>
    public partial class OutputTextBox : UserControl
    {
        public OutputTextBox()
        {
            InitializeComponent();

            this.DataContext = OutputStringItemViewModel.GetOutputStringInstance();
        }
    }
}
