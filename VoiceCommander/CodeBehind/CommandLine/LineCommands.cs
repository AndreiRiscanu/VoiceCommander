using System.IO;
using System.Windows;
using VoiceCommander.CodeBehind.ViewModels;
using VoiceCommander.ViewModels;

namespace VoiceCommander.CommandLine
{
    public abstract class LineCommands
    {

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

        public static void Clear(string[] parameters)
        {
            OutputStringItemViewModel.GetOutputStringInstance().output = "";
        }

        public static void ShowItemDetails(string[] parameters)
        {
            if (parameters != null)
            {
                if (Directory.Exists(parameters[0]))
                {
                    var item = new DirectoryInfo(@parameters[0]);

                    OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString();
                }
            }
            else
            {
                var item = new DirectoryInfo(DirectoryStructureViewModel.GetDirectoryStructureInstance().Items[0].FullPath);

                OutputStringItemViewModel.GetOutputStringInstance().output = item.Attributes.ToString();
            }
        }

    }
}
