using System.Linq;
using System.Collections.ObjectModel;
using VoiceCommander.Data;

namespace VoiceCommander.ViewModels
{
    public class DirectoryStructureViewModel : BaseViewModel
    {
        #region Public

        public ObservableCollection<DirectoryItemViewModel> Items { get; set; }

        /// <summary>
        /// Return the only instance of this class (Singleton)
        /// </summary>
        /// <returns>DirectoryStructureViewModel</returns>
        public static DirectoryStructureViewModel GetDirectoryStructureInstance()
        {
            return DirectoryStructureInstance;
        }

        /// <summary>
        /// Populate the ListView with the user's drives
        /// </summary>
        public void PopulateWithDrives()
        {
            var children = DirectoryStructure.GetLogicalDrives();

            Items = new ObservableCollection<DirectoryItemViewModel>(children.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)));
        }

        /// <summary>
        /// Refresh the view
        /// </summary>
        public void Refresh()
        {
            if (Items[0].Name == "..")
            {
                var first = Items[0];

                Items.Clear();

                DirectoryStructure.numberOfItems = 1;

                // We need to add the Go Back ("..") item first
                DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(first);

                var children = DirectoryStructure.GetDirectoryContents(Items[0].GetParent);

                // Populate the View with the folder's content
                foreach (var child in children.Select(content => new DirectoryItemViewModel(content.FullPath, content.Type)))
                {
                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(child);
                }
            }
        }

        #endregion

        #region Private

        private static DirectoryStructureViewModel DirectoryStructureInstance = new DirectoryStructureViewModel();

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor is private so that the class cannot be instantiated but once
        /// </summary>
        private DirectoryStructureViewModel()
        {
            PopulateWithDrives();
        }

        #endregion
    }
}
