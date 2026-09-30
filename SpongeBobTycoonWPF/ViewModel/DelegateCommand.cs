using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object?> _execute; //eltárolja azt ami meg lezs hívva click-nél, pl: HandleClick
        private readonly Predicate<object?>? _canExecute; //chosenfigurePos==(-1,-1), azaz meg hívódhat-e a step, ha nem akkor a selectFigure fog

        public event EventHandler? CanExecuteChanged; //gombok ki-be kapcsolása, pl. hogy ne tudjunk rossz helyre lépni (annak a logikája a step-ben van)

        public DelegateCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException("Command execution is disabled");
            }

            _execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

}
