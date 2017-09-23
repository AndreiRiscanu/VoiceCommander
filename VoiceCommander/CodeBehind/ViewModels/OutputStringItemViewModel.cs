using VoiceCommander.ViewModels;

namespace VoiceCommander.CodeBehind.ViewModels
{
    public class OutputStringItemViewModel : BaseViewModel
    {
        public string output { get; set; }

        public static OutputStringItemViewModel OutputStringInstance = new OutputStringItemViewModel();

        public static OutputStringItemViewModel GetOutputStringInstance()
        {
            return OutputStringInstance;
        }

        private OutputStringItemViewModel() { }
    }
}
