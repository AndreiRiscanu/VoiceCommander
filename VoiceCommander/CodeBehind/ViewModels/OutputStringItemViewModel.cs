using VoiceCommander.ViewModels;

namespace VoiceCommander.CodeBehind.ViewModels
{
    public class OutputStringItemViewModel : BaseViewModel
    {
        public string Output { get; set; }

        public int FontSize { get; set; } = 12;

        public static OutputStringItemViewModel GetOutputStringInstance()
        {
            return OutputStringInstance;
        }

        private OutputStringItemViewModel() { }

        private static OutputStringItemViewModel OutputStringInstance = new OutputStringItemViewModel();
    }
}
