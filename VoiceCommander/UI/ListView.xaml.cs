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
using System.Windows.Media;
using VoiceCommander.CodeBehind.ViewModels;
using VoiceCommander.CommandLine;
using VoiceCommander.Data;
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

                    // Only go back if we're not in root
                    if (firstItem.Name == "..")
                        firstItem.EnterCommand.Execute(null);
                    break;
            }
        }

        private TextBlock ConvertElement(object sender)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                ContextMenu parentContextMenu = menuItem.CommandParameter as ContextMenu;
                if (parentContextMenu != null)
                {
                    TextBlock textBlock = parentContextMenu.PlacementTarget as TextBlock;
                    return textBlock;
                }
            }

            return null;
        }


        private void Delete(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = ConvertElement(sender);

            if (textBlock == null)
            {
                return;
            }
            
            foreach (var item in DirectoryStructureViewModel.GetDirectoryStructureInstance().Items)
            {
                if (item.Name == textBlock.Text)
                {
                    if (item.Type == DirectoryItemType.Drive)
                    {
                        OutputStringItemViewModel.GetOutputStringInstance().Output = "Cannot delete drive";

                        return;
                    }

                    if (item.Type == DirectoryItemType.File)
                    {
                        Command.Execute("rmfile", new string[] { item.FullPath });
                    } else if (item.Type == DirectoryItemType.Folder)
                    {
                        MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this folder?", "Delete Folder", MessageBoxButton.OKCancel, MessageBoxImage.Warning);

                        if (result == MessageBoxResult.Cancel)
                        {
                            return;
                        }

                        Command.Execute("rmdir", new string[] { item.FullPath, "-r" });
                    }

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();

                    return;
                }
            }
        }
    }
}
