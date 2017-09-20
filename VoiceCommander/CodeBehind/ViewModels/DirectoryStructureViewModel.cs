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
            var children = DirectoryStructure.GetLogicalDrives();

            Items = new ObservableCollection<DirectoryItemViewModel>(children.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)));
        }

        #endregion
    }
}
