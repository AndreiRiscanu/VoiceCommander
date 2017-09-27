using System;
using System.IO;
using System.Windows;
using VoiceCommander.CodeBehind.ViewModels;
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
    }
}
