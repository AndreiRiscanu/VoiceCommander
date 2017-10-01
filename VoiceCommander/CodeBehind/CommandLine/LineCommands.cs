using System;
using System.IO;
using System.Windows;
using VoiceCommander.CodeBehind.ViewModels;
using VoiceCommander.Helpers;
using VoiceCommander.ViewModels;

namespace VoiceCommander.CommandLine
{
    public abstract class LineCommands
    {
        #region Public Methods

        /// <summary>
        /// Count the total number of elements in the current folder
        /// </summary>
        /// <param name="parameters"></param>
        public static void CountElements(string[] parameters)
        {
            var items = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items;

            // The parent directory isn't taken into account
            if (items[0].Name == "..")
            {
                OutputStringItemViewModel.GetOutputStringInstance().output = (items.Count - 1).ToString();

                return;
            }

            OutputStringItemViewModel.GetOutputStringInstance().output = items.Count.ToString();
        }

        /// <summary>
        /// Clear the output TextBox
        /// </summary>
        /// <param name="parameters"></param>
        public static void Clear(string[] parameters)
        {
            OutputStringItemViewModel.GetOutputStringInstance().output = "";
        }

        /// <summary>
        /// Show details about a selected file or folder
        /// </summary>
        /// <param name="parameters"></param>
        public static void ShowItemDetails(string[] parameters)
        {
            if (parameters != null)
            {
                if (GetInfo(@parameters[0]))
                    return;

                GetInfo(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + @parameters[0]);
            }
            // Show info about the parent directory, if we're not in the root
            else
            {
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = "root";

                    return;
                }

                var item = new DirectoryInfo(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].FullPath);

                OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString() + "\nName: " + item.Name + "\nLast access time: " + item.LastAccessTime;
            }
        }

        /// <summary>
        /// Change the font size of the output TextBox
        /// </summary>
        /// <param name="parameters"></param>
        public static void ChangeFontSize(string[] parameters)
        {
            // Check if there is an argument given and that it has less than 4 characters
            if (parameters != null && parameters[0].Length < 4)
            {
                // Check if there are only numbers in the given size
                foreach (char c in parameters[0])
                {
                    if (c < '0' || c > '9')
                        return ;
                }

                var size = Int32.Parse(parameters[0]);

                // Font size shouldn't be too small or too big
                if (size > 9 && size < 101)
                    OutputStringItemViewModel.GetOutputStringInstance().fontSize = size;
            }
        }

        /// <summary>
        /// Change the location of the user
        /// </summary>
        /// <param name="parameters"></param>
        public static void ChangeLocation(string[] parameters)
        {
            if (parameters == null)
                return;

            // Go back
            if (parameters[0] == "..")
            {
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name == "..")
                {
                    var parentPath = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].EnterCommand;

                    parentPath.Execute(null);
                }
            }
            // If the full path to the directory is given, get it's items and populate the view
            else if (Directory.Exists(@parameters[0]))
            {
                DirectoryStructure.numberOfItems = 0;

                PopulateGivenPath(@parameters[0]);
            }
            // Else, check if the path given is a continuation of the current path 
            else
            {
                var path = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + "\\" + @parameters[0];

                if (Directory.Exists(path))
                {
                    DirectoryStructure.numberOfItems = 0;

                    PopulateGivenPath(path);
                }
            }
        }

        /// <summary>
        /// Display the content of a file to the user
        /// </summary>
        /// <param name="parameters"></param>
        public static void ReadContent(string[] parameters) 
        {
            if (parameters == null)
            {
                return;
            }

            // Check if the full path to the file was given
            if (File.Exists(@parameters[0]))
            {
                ReadFileContent(@parameters[0]);
            }
            // Else, check if the file cand be found in the current folder
            else if (File.Exists(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]))
            {
                ReadFileContent(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]);
            }
        }

        /// <summary>
        /// Create a file with the specified name, at the specified path
        /// </summary>
        /// <param name="parameters"></param>
        public static void CreateFile(string[] parameters)
        {
            // No file name given
            if (parameters == null)
            {
                OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.NONAME;

                return;
            }

            // Create the file at the given path
            if (parameters.Length > 1)
            {
                #region Path Given

                // The name given contains illegal characters
                if (StringContainsIllegalCharacters(parameters[1]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (Directory.Exists(@parameters[0]))
                    if (!File.Exists(@parameters[0] + '\\' + parameters[1]))
                    {
                        // File can be created
                        File.Create(@parameters[0] + '\\' + parameters[1]);

                        OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATED;

                        DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                    }
                    else
                        // A file with this name already exists
                        OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.EXISTSERROR;
                else
                    // Path given is inexistent
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.PATHERROR;

            #endregion
            }
            else
            {
                #region Current folder

                // We cannot create a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.ROOT;

                    return;
                }

                // The name given contains illegal characters
                if (StringContainsIllegalCharacters(parameters[0]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (!File.Exists(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]))
                {
                    File.Create(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATED;

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.EXISTSERROR;

                #endregion
            }
        }

        /// <summary>
        /// Delete a given file from the specified path
        /// </summary>
        /// <param name="parameters"></param>
        public static void DeleteFile(string[] parameters)
        {
            if (parameters[0] == null)
                return;

            // Full path to the file was given
            if (File.Exists(@parameters[0]))
            {
                #region Path Given

                File.Delete(@parameters[0]);

                OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETED;

                DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();

                #endregion
            }
            else
            {
                #region Current Folder

                // We cannot delete a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETIONERROR + OutputStrings.ROOT;

                    return;
                }

                var filePath = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0];

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETED;

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETIONERROR + OutputStrings.DOESNTEXISTERROR;

                #endregion
            }
        }

        /// <summary>
        /// Create a folder with the specified name, at the specified path
        /// </summary>
        /// <param name="parameters"></param>
        public static void CreateFolder(string[] parameters)
        {
            if (parameters == null)
            {
                OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.NONAME;

                return;
            }

            // Create the folder at the given path
            if (parameters.Length > 1)
            {
                #region Path Given

                if (StringContainsIllegalCharacters(parameters[1]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (Directory.Exists(@parameters[0]))
                    if (!Directory.Exists(@parameters[0] + '\\' + parameters[1]))
                    {
                        // Folder can be created
                        Directory.CreateDirectory(@parameters[0] + '\\' + parameters[1]);

                        OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATED;

                        DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                    }
                    else
                        // A folder with this name already exists
                        OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.EXISTSERROR;
                else
                    // Path given is inexistent
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.PATHERROR;

                #endregion
            }
            else
            {
                #region Current Folder

                // We cannot create a folder if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.ROOT;

                    return;
                }

                if (StringContainsIllegalCharacters(parameters[0]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (!Directory.Exists(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]))
                {
                    Directory.CreateDirectory(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATED;

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.EXISTSERROR;

                #endregion
            }
        }

        /// <summary>
        /// Delete a given folder from the specified path
        /// </summary>
        /// <param name="parameters"></param>
        public static void DeleteFolder(string[] parameters)
        {
            if (parameters[0] == null)
                return;

            // Full path to the file was given
            if (Directory.Exists(@parameters[0]))
            {
                #region Path Given

                if (parameters.Length > 1 && parameters[1] == "-r")
                    Directory.Delete(@parameters[0], true);
                else if (Directory.GetFiles(@parameters[0]).Length < 0)
                    Directory.Delete(@parameters[0]);
                else
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.FOLDERNOTEMPTY;

                    return;
                }

                OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERDELETED;

                DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();

                #endregion
            }
            else
            {
                #region Current Folder

                // We cannot delete a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.ROOT;

                    return;
                }

                var filePath = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + @parameters[0];

                if (Directory.Exists(filePath))
                {
                    // Delete recursively
                    if (parameters.Length > 1 && parameters[1] == "-r")
                        Directory.Delete(filePath, true);
                    // Delete empty folder
                    else if (Directory.GetFiles(filePath).Length < 0)
                        Directory.Delete(filePath);
                    // Specified folder is not empty
                    else
                    {
                        OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.FOLDERNOTEMPTY;

                        return;
                    }
                    
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERDELETED;

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.DOESNTEXISTERROR;

                #endregion
            }
        }

        #endregion


        #region Private Methods

        private static bool GetInfo(string path)
        {
            // Check if the directory exists and show its info
            if (Directory.Exists(path))
            {
                var item = new DirectoryInfo(path);

                OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString() + "\nName: " + item.Name + "\nLast access time: " + item.LastAccessTime;

                return true;
            }
            // Else, check if the file exists and show its info
            else if (File.Exists(path))
            {
                var item = new FileInfo(path);

                OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString() + "\nName: " + item.Name + "\nLast access time: " + item.LastAccessTime + "\nSize: " + item.Length;

                return true;
            }

            return false;
        }

        private static void ReadFileContent(string path)
        {
            // Get the size of the file in bytes
            Int64 fileSize = new FileInfo(path).Length;

            // Don't open files too big, might be a picture or a movie
            if (fileSize < 500000)
                OutputStringItemViewModel.GetOutputStringInstance().output = File.ReadAllText(path, System.Text.Encoding.UTF8);
            else
                OutputStringItemViewModel.GetOutputStringInstance().output = "Could not read: File size too big.";
        }

        private static void PopulateGivenPath(string path)
        {
            var items = DirectoryStructure.GetDirectoryContents(path);

            DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Clear();

            DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(new DirectoryItemViewModel(path, Data.DirectoryItemType.Folder));

            foreach (var item in items)
            {
                DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(new DirectoryItemViewModel(item.FullPath, item.Type));
            }
        }

        private static bool StringContainsIllegalCharacters(string sentence)
        {
            foreach (char c in sentence)
            {
                if (c == '\\' || c == '/')
                    return true;
            }

            return false;
        }

        #endregion
    }
}
