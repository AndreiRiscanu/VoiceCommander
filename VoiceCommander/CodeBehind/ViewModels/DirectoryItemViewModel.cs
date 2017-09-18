using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using VoiceCommander.Data;
using VoiceCommander.ViewModels;

namespace VoiceCommander.ViewModels
{
    public class DirectoryItemViewModel : BaseViewModel
    {
        #region Public Functions

        /// <summary>
        /// The type of this item (drive, file, folder)
        /// </summary>
        public DirectoryItemType Type { get; set; }

        /// <summary>
        /// The absolute path to this item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The name of this discovery item
        /// </summary>
        public string Name { get { return this.Type == DirectoryItemType.Drive ? this.FullPath : DirectoryStructure.GetFileFolderName(this.FullPath); } }

        /// <summary>
        /// A list of all the children contained inside this item
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Children { get; set; }

        /// <summary>
        /// Indicates if this item can be entered
        /// </summary>
        public bool CanEnter { get { return this.Type != DirectoryItemType.File; } }

        public ICommand EnterCommand { get; set; }

        #endregion

        #region Constructor

        public DirectoryItemViewModel(string fullPath, DirectoryItemType type)
        {
            this.EnterCommand = new RelayCommand(Enter);

            this.FullPath = fullPath;
            this.Type = type;
        }

        #endregion

        #region Helper Functions

        private void Enter()
        {
            // We cannot enter a file
            if (this.Type == DirectoryItemType.File)
                return;

            // Get all the children from the selected directory
            var children = DirectoryStructure.GetDirectoryContents(this.FullPath);

            // Remove the current items shown
            DirectoryStructureViewModel.Items.Clear();

            // Used to get back to the selected drive's root
            // TODO: find a way to go back in the listview
            var root = new DirectoryItemViewModel(Path.GetPathRoot(this.FullPath), DirectoryItemType.Folder);
            DirectoryStructureViewModel.Items.Add(root);

            // Add all files/folders from the selected directory to the view
            foreach (var child in children.Select(content => new DirectoryItemViewModel(content.FullPath, content.Type)))
            {
                DirectoryStructureViewModel.Items.Add(child);
            }
        }

        #endregion
    }
}
