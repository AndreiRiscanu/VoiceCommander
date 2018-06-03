using System.Threading.Tasks;
using System.Windows.Input;

namespace VoiceCommander.Commands
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
