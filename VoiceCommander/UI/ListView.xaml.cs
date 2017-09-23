using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using VoiceCommander.ViewModels;

namespace VoiceCommander.UI
{
    /// <summary>
    /// Interaction logic for ListView.xaml
    /// </summary>
    public partial class ListView : UserControl
    {
        public ListView()
        {
            InitializeComponent();
        }

        public void ManageFiles(object sender, KeyEventArgs e)
        {
            var vm = (sender as ListViewItem).Content as DirectoryItemViewModel;

           switch(e.Key)
            {
                case Key.Enter:
                    vm.EnterCommand.Execute(null);
                    break;

                case Key.Back:
                    // Get the first item in the ListView
                    var firstItem = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.First();

                    // Only go back if we're not in the root
                    if (firstItem.Name == "..")
                        firstItem.EnterCommand.Execute(null);
                    break;
            }
        }
    }
}
