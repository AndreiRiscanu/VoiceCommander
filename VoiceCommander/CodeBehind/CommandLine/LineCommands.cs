using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using VoiceCommander.CodeBehind.ViewModels;
using VoiceCommander.Helpers;
using VoiceCommander.ViewModels;

namespace VoiceCommander.CommandLine
{
    public class LineCommands
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
                OutputStringItemViewModel.GetOutputStringInstance().Output = (items.Count - 1).ToString();

                return;
            }

            OutputStringItemViewModel.GetOutputStringInstance().Output = items.Count.ToString();
        }

        /// <summary>
        /// Clear the output TextBox
        /// </summary>
        /// <param name="parameters"></param>
        public static void Clear(string[] parameters)
        {
            OutputStringItemViewModel.GetOutputStringInstance().Output = "";
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
                    OutputStringItemViewModel.GetOutputStringInstance().Output = "root";

                    return;
                }

                var item = new DirectoryInfo(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].FullPath);

                OutputStringItemViewModel.GetOutputStringInstance().Output = item.Attributes.ToString()
                    + "\nName: " + item.Name + "\nLast access time: " + item.LastAccessTime;
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
                if (size >= 10 && size <= 100)
                    OutputStringItemViewModel.GetOutputStringInstance().FontSize = size;
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

            DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
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
                OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.NONAME;

                return;
            }

            // Create the file at the given path
            if (parameters.Length > 1)
            {
                #region Path Given

                // The name given contains illegal characters
                if (StringContainsIllegalCharacters(parameters[1]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (Directory.Exists(@parameters[0]))
                    if (!File.Exists(@parameters[0] + '\\' + parameters[1]))
                    {
                        // File can be created
                        File.Create(@parameters[0] + '\\' + parameters[1]);

                        OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATED;
                    }
                    else
                        // A file with this name already exists
                        OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.EXISTSERROR;
                else
                    // Path given is inexistent
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.PATHERROR;

            #endregion
            }
            else
            {
                #region Current folder

                // We cannot create a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.ROOT;

                    return;
                }

                // The name given contains illegal characters
                if (StringContainsIllegalCharacters(parameters[0]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (!File.Exists(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]))
                {
                    File.Create(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATED;
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILECREATIONERROR + OutputStrings.EXISTSERROR;

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

                OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILEDELETED;

                #endregion
            }
            else
            {
                #region Current Folder

                // We cannot delete a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILEDELETIONERROR + OutputStrings.ROOT;

                    return;
                }

                var filePath = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0];

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);

                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILEDELETED;
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FILEDELETIONERROR + OutputStrings.DOESNTEXISTERROR;

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
                OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.NONAME;

                return;
            }

            // Create the folder at the given path
            if (parameters.Length > 1)
            {
                #region Path Given

                if (StringContainsIllegalCharacters(parameters[1]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (Directory.Exists(@parameters[0]))
                    if (!Directory.Exists(@parameters[0] + '\\' + parameters[1]))
                    {
                        // Folder can be created
                        Directory.CreateDirectory(@parameters[0] + '\\' + parameters[1]);

                        OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATED;
                    }
                    else
                        // A folder with this name already exists
                        OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.EXISTSERROR;
                else
                    // Path given is inexistent
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.PATHERROR;

                #endregion
            }
            else
            {
                #region Current Folder

                // We cannot create a folder if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.ROOT;

                    return;
                }

                if (StringContainsIllegalCharacters(parameters[0]))
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.ILLEGALCHARACTERS;

                    return;
                }

                if (!Directory.Exists(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]))
                {
                    Directory.CreateDirectory(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATED;
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERCREATIONERROR + OutputStrings.EXISTSERROR;

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
                else if (Directory.GetFiles(@parameters[0]).Length == 0 && Directory.GetDirectories(parameters[0]).Length == 0)
                    Directory.Delete(@parameters[0]);
                else
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.FOLDERNOTEMPTY;

                    return;
                }

                OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERDELETED;

                #endregion
            }
            else
            {
                #region Current Folder

                // We cannot delete a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.ROOT;

                    return;
                }

                var filePath = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + @parameters[0];

                if (Directory.Exists(filePath))
                {
                    // Delete recursively
                    if (parameters.Length > 1 && parameters[1] == "-r")
                        Directory.Delete(filePath, true);
                    // Delete empty folder
                    else if (Directory.GetFiles(filePath).Length == 0)
                        Directory.Delete(filePath);
                    // Specified folder is not empty
                    else
                    {
                        OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.FOLDERNOTEMPTY;

                        return;
                    }
                    
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERDELETED;
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().Output = OutputStrings.FOLDERDELETIONERROR + OutputStrings.DOESNTEXISTERROR;

                #endregion
            }
        }

        public static void CopyFile(string[] parameters)
        {
            if (parameters.Length < 2)
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "Not enough arguments given; should be: <copy_file_name> <new_file_name>";

                return;
            }

            var newPath = parameters[1];

            // Check if the path given as new is valid
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "The path given is not valid";

                return;
            }

            if (!FileCopied(parameters[0], newPath))
            {
                var path = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0];

                // Only if the first argument is an existent file
                if (!FileCopied(path, newPath))
                    OutputStringItemViewModel.GetOutputStringInstance().Output = "File to be moved or new path given is invalid";
            }
        }

        /// <summary>
        /// Rename a given file
        /// </summary>
        /// <param name="parameters"></param>
        public static void MoveFile(string[] parameters)
        {
            if (parameters.Length < 2)
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "Not enough arguments given; should be: <old_file_name> <new_file_name>";

                return;
            }

            var newPath = parameters[1];

            // The second argument shouldn't end with a '\' because it would mean that a path with no file name to be created was given
            if (newPath.EndsWith("\\"))
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "No file name given";

                return;
            }

            // Check if the path given as new is valid
            if (!Directory.Exists(Path.GetDirectoryName(newPath)))
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "The new path given is not valid";

                return;
            }

            // The new file name can't match an existing file or folder name
            if (File.Exists(newPath))
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "There already exists a file with that name";

                return;
            }

            if (!Directory.Exists(newPath) || File.Exists(Path.Combine(newPath, Path.GetFileName(parameters[0]))))
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "There already exists a file with that name";

                return;
            }

            if (!FileMoved(parameters[0], newPath))
            {
                var path = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent + '\\' + parameters[0];

                // Only if the first argument is an existent file
                if (!FileMoved(path, newPath))
                    OutputStringItemViewModel.GetOutputStringInstance().Output = "File to be copied or new path given is invalid";
            }
        }

        /// <summary>
        /// Find a file or folder by partially giving its name
        /// </summary>
        /// <param name="parameters"></param>
        public static void FindFileOrFolder(string[] parameters)
        {
            if (parameters == null || parameters.Length < 2)
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "Not enough arguments given";

                return;
            }

            if (!parameters[0].Contains("\\") && parameters[1].Contains("\\"))
            {
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent == null)
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = "Please select a drive in which to search";

                    return;
                }
            }

            string[] paths;
            try
            {
                if (parameters.Length == 3 && parameters[1] == "-r")
                {
                    paths =
                        Directory.GetFiles(parameters[0], parameters[2], System.IO.SearchOption.AllDirectories);
                }
                else
                {
                    paths =
                        Directory.GetFiles(parameters[0], parameters[1]);
                }
            } catch (Exception e)
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "Permission to folder denied";

                return;
            }

            OutputStringItemViewModel.GetOutputStringInstance().Output = "";

            foreach (var path in paths)
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output += path + "\n";
            }
        }

        public static void ShowHelp(string[] parameters)
        {
            try
            {
                Process.Start("Help.chm", null);
            }
            catch (Exception e)
            {
                OutputStringItemViewModel.GetOutputStringInstance().Output = "Could not open help file; " +
                    "it could be missing or you don't have permission to open it.";
            }
        }

        public static void MaximizeWindow(string[] parameters)
        {
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
        }

        public static void MinimizeWindow(string[] parameters)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public static void NormalWindow(string[] parameters)
        {
            Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        public static void Exit(string[] parameters)
        {
            Application.Current.Shutdown();
        }

        #endregion


        #region Private Methods

        private static bool FileMoved(string path, string newPath)
        {
            if (File.Exists(path))
            {
                // File can be moved safely
                try
                {
                    File.Move(path, Path.Combine(newPath, Path.GetFileName(path)));
                }
                catch (Exception e)
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = "File could not be copied; there may already be a file with this name.";

                    return false;
                }

                OutputStringItemViewModel.GetOutputStringInstance().Output = "File moved";

                return true;
            }

            return false;
        }

        private static bool FileCopied(string toCopy, string toPaste)
        {
            if (File.Exists(toCopy))
            {
                string Paste = Path.Combine(toPaste, Path.GetFileName(toCopy));
                // File can be moved safely
                try
                {
                    File.Copy(toCopy, Paste);
                }
                catch (Exception e)
                {
                    OutputStringItemViewModel.GetOutputStringInstance().Output = 
                        "File could not be copied; path invalid or a file with that name already exists";
                }

                OutputStringItemViewModel.GetOutputStringInstance().Output = "File copied";

                return true;
            }

            return false;
        }

        private static bool GetInfo(string path)
        {
            // Check if the directory exists and show its info
            if (Directory.Exists(path))
            {
                var Item = new DirectoryInfo(path);
                long TotalSize = 0;
                string[] Files = null;
                try
                {
                    Files = Directory.GetFiles(path, "*.*");
                }
                catch (Exception e)
                {
                    return true;
                }

                foreach (string name in Files)
                {
                    FileInfo info = new FileInfo(name);
                    TotalSize += info.Length;
                }

                OutputStringItemViewModel.GetOutputStringInstance().Output = Item.Attributes.ToString()
                    + "\nName: " + Item.Name + "\nSize: " + (((TotalSize / 1024.0) / 1024.0) / 1024.0).ToString("0.00")
                    + " GB\nLast access time: " + Item.LastAccessTime;

                return true;
            }
            // Else, check if the file exists and show its info
            else if (File.Exists(path))
            {
                var item = new FileInfo(path);

                OutputStringItemViewModel.GetOutputStringInstance().Output = item.Attributes.ToString()
                    + "\nName: " + item.Name + "\nSize: " + (((item.Length / 1024.0) / 1024.0) / 1024.0).ToString("0.00")
                    + "GB\nLast access time: " + item.LastAccessTime;

                return true;
            }

            return false;
        }

        private static void ReadFileContent(string path)
        {
            // Get the size of the file in bytes
            Int64 fileSize = new FileInfo(path).Length;

            // Don't open files too big, might be a picture or a movie
            if (fileSize < 200000)
                OutputStringItemViewModel.GetOutputStringInstance().Output = File.ReadAllText(path, System.Text.Encoding.UTF8);
            else
                OutputStringItemViewModel.GetOutputStringInstance().Output = "Could not read: File size too big.";
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
