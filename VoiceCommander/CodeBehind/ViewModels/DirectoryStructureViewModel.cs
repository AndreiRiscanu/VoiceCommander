using System.Linq;
using System.Collections.ObjectModel;
using VoiceCommander.Data;

namespace VoiceCommander.ViewModels
{
    public class DirectoryStructureViewModel : BaseViewModel
    {
        #region Public Properties

        public static ObservableCollection<DirectoryItemViewModel> Items { get; set; }

        #endregion

        #region Constructor

        public DirectoryStructureViewModel()
        {
            var children = DirectoryStructure.GetLogicalDrives();

            Items = new ObservableCollection<DirectoryItemViewModel>(children.Select(drive => new DirectoryItemViewModel(drive.FullPath, DirectoryItemType.Drive)));
        }

        #endregion
    }
}
