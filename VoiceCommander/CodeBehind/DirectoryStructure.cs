using System.Collections.Generic;
using System.Linq;
using System.IO;
using VoiceCommander.Data;
using VoiceCommander.ViewModels;

namespace VoiceCommander
{
    public static class DirectoryStructure
    {
        public static int numberOfItems = 0;

        /// <summary>
        /// Gets the logical drives on the computer
        /// </summary>
        /// <returns></returns>
        public static List<DirectoryItem> GetLogicalDrives()
        {
            // Get every logical drive on the machine
            return Directory.GetLogicalDrives().Select(drive => new DirectoryItem { FullPath = drive, Type = DirectoryItemType.Drive }).ToList();
        }

        public static List<DirectoryItem> GetDirectoryContents(string fullPath)
        {
            var items = new List<DirectoryItem>();

            #region Get Directories

            // Try and get directories from the folder ignoring any issues doing so
            try
            {
                var directories = Directory.GetDirectories(fullPath);
                
                if (directories.Length > 0)
                {
                    items.AddRange(directories.Select(dir => new DirectoryItem { FullPath = dir, Type = DirectoryItemType.Folder }));
                }
            }
            catch { }

            #endregion

            #region Get Files

            // Try and get files from the folder ignoring any issues doing so
            try
            {
                var files = Directory.GetFiles(fullPath);

                if (files.Length > 0)
                {
                    items.AddRange(files.Select(dir => new DirectoryItem { FullPath = dir, Type = DirectoryItemType.File }));
                }
            }
            catch { }

            #endregion

            return items;
        }

        #region Helpers

        public static string GetFileFolderName(string path)
        {
            // If we have no path, return empty
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            // Make all slashes backslashes
            var normalizedPath = path.Replace('/', '\\');

            // Find the last backslash in the path
            var lastIndex = normalizedPath.LastIndexOf('\\');

            // If we don't find a backslash return the path itself
            if (lastIndex <= 0)
                return path;

            // Return the name after the last backslash
            return path.Substring(lastIndex + 1);
        }

        public static string getElementAtPosition(int pos)
        {
            return DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[pos].FullPath;
        }

        #endregion
    }
}
