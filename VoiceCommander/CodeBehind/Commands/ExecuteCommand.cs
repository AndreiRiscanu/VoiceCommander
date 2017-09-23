using System;
using System.Linq;
using System.Windows.Input;
using VoiceCommander.CommandLine;

namespace VoiceCommander.Commands
{
    class ExecuteCommand : ICommand
    {
        #region Private Members

        /// <summary>
        /// The action to run
        /// </summary>
        private Action<string> mAction;

        #endregion

        #region Public Events

        /// <summary>
        /// The event that's fired when the <see cref="CanExecute(object)"/>  value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        public ExecuteCommand(Action<string> action)
        {
            mAction = action;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the command Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            mAction(parameter as string);
        }

        #endregion
    }
}
