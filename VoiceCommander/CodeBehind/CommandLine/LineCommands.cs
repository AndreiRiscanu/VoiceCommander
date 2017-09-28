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

        /// <summary>
        /// Count the total number of elements in the current folder
        /// </summary>
        /// <param name="parameters"></param>
        public static void CountElements(string[] parameters)
        {
            var items = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items;

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
                if (Directory.Exists(@parameters[0]))
                {
                    var item = new DirectoryInfo(@parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString() + "\nName: " + item.Name + "\nLast access time: " + item.LastAccessTime;
                }
                else if (File.Exists(parameters[0]))
                {
                    var item = new FileInfo(@parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString() + "\nName: " + item.Name + "\nLast access time: " + item.LastAccessTime + "\nSize: " + item.Length;
                }
            }
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
            else
                // If the directory exists, get it's items and populate the view
                if (Directory.Exists(@parameters[0]))
                {
                    var items = DirectoryStructure.GetDirectoryContents(@parameters[0]);

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Clear();

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(new DirectoryItemViewModel(@parameters[0], Data.DirectoryItemType.Folder));

                    foreach (var item in items)
                    {
                        DirectoryStructureViewModel.GetDirectoryStructureInstance().Items.Add(new DirectoryItemViewModel(item.FullPath, item.Type));
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

            if (File.Exists(@parameters[0]))
            {
                // Get the size of the file in bytes
                Int64 fileSize = new FileInfo(@parameters[0]).Length;

                // Don't open files too big, might be a picture or a movie
                if (fileSize < 500000)
                    OutputStringItemViewModel.GetOutputStringInstance().output = File.ReadAllText(@parameters[0], System.Text.Encoding.UTF8);
                else
                    OutputStringItemViewModel.GetOutputStringInstance().output = "Could not read: File size too big.";
            }
        }

        /// <summary>
        /// Create a file with the specified name, at the specified path
        /// </summary>
        /// <param name="parameters"></param>
        public static void CreateFile(string[] parameters)
        {
            if (parameters == null)
            {
                OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILECREATIONERROR + OutputStrings.NONAME;

                return;
            }

            // Create the file at the given path
            if (parameters.Length > 1)
            {
                #region Path Given

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
                File.Delete(@parameters[0]);

                OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETED;

                DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
            }
            else
            {
                // We cannot delete a file if we are in the root
                if (DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].Name != "..")
                {
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETIONERROR + OutputStrings.ROOT;

                    return;
                }

                var filePath = DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].GetParent;

                if (File.Exists(filePath + '\\' + parameters[0]))
                {
                    File.Delete(filePath + '\\' + parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETED;

                    DirectoryStructureViewModel.GetDirectoryStructureInstance().Refresh();
                }
                else
                    OutputStringItemViewModel.GetOutputStringInstance().output = OutputStrings.FILEDELETIONERROR + OutputStrings.DOESNTEXISTERROR;
            }
        }

        #region Private Methods

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
