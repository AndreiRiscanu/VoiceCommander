using System.IO;
using System.Linq;
using System.Windows.Input;
using VoiceCommander.Commands;
using VoiceCommander.Data;

namespace VoiceCommander.ViewModels
{
    public class DirectoryItemViewModel : BaseViewModel
    {
        #region Public

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
        public string Name
        {
            get
            {
                // If it's the first item, set it to be the "go back" component
                return this.Type == DirectoryItemType.Drive ?
                    this.FullPath : this.FullPath == DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.First().FullPath ?
                    ".." : DirectoryStructure.GetFileFolderName(this.FullPath);
            }
        }
        
        public ICommand EnterCommand { get; set; }

        /// <summary>
        /// Used to show the full path to the current item
        /// </summary>
        public string GetParent
        {
            get
            {
                // The View looks at the first element and if it's not the Go Back folder (".."), it means we're in the root
                if (this.Name == "..")
                {
                    return this.FullPath;
                }

                return null;
            }
        }

        #endregion

        #region Constructor

        public DirectoryItemViewModel(string fullPath, DirectoryItemType type)
        {
            this.EnterCommand = new RelayCommand(Enter);

            this.FullPath = fullPath;
            this.Type = type;
        }

        #endregion

        #region Private

        /// <summary>
        /// The function that will be called when an item is double-clicked
        /// </summary>
        private void Enter()
        {
            // We cannot enter a file
            if (this.Type == DirectoryItemType.File)
                return;

            DirectoryInfo parentInfo = new DirectoryInfo(this.FullPath);

            parentInfo = parentInfo.Parent;

            #region Go Back

            // If the user double-clicks the "go back" item...
            if (this.Name == "..")
            {
                // Check so that the parent folder isn't the root and then set the path to it
                if (parentInfo != null)
                    this.FullPath = parentInfo.FullName;
                // Else, we need to populate with the users drives
                else
                {
                    // Remove the current items shown
                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Clear();

                    var drives = DirectoryStructure.GetLogicalDrives();

                    foreach (var child in drives.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)))
                    {
                        DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(child);
                    }

                    return;
                }
            }

            #endregion

            #region Enter Folder

            DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Clear();
            
            // We need to add the Go Back ("..") item first
            var parent = new DirectoryItemViewModel(this.FullPath, DirectoryItemType.Folder);
            DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(parent);
            
            var children = DirectoryStructure.GetDirectoryContents(this.FullPath);

            // Populate the View with the folder's content
            foreach (var child in children.Select(content => new DirectoryItemViewModel(content.FullPath, content.Type)))
            {
                DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(child);
            }

            #endregion
        }

        #endregion
    }
}
