using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Diploma.UI.Auxiliary.Commands
{
    public sealed class RelayMultiCommand : ICommand
    {

        private readonly IEnumerable<(RelayCommand, object)> _commands;
        private readonly RelayMultiCommandModes _mode;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public RelayMultiCommand(RelayMultiCommandModes mode, params (RelayCommand, object)[] commands)
        {
            _mode = mode;
            _commands = commands;
        }

        public bool CanExecute(object parameter)
        {
            if (_mode == RelayMultiCommandModes.WhenAnyCanExecute)
            {
                return _commands.Any(command => command.Item1.CanExecute(command.Item2));
            }
            if (_mode == RelayMultiCommandModes.WhenAllCanExecute)
            {
                return _commands.All(command => command.Item1.CanExecute(command.Item2));
            }
            return true;
        }

        public void Execute(object parameter)
        {
            foreach (var command in _commands)
            {
                command.Item1.Execute(command.Item2);
            }
        }
    }

    public enum RelayMultiCommandModes
    {
        WhenAnyCanExecute,
        WhenAllCanExecute
    }

}