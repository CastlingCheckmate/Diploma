using System.Windows.Input;
using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.ViewModels.Base;

namespace Diploma.UI.ViewModels.Controls
{

    public class NumericUpDownViewModel : ControlViewModel
    {

        private int _minValue;
        private int _maxValue;
        private int _increment;
        private int _value;
        private ICommand _decrementCommand;
        private ICommand _incrementCommand;

        public int MinValue
        {
            get =>
                _minValue;

            set
            {
                _minValue = value;
                NotifyPropertyChanged(nameof(MinValue));
            }
        }

        public int MaxValue
        {
            get =>
                _maxValue;

            set
            {
                _maxValue = value;
                NotifyPropertyChanged(nameof(MaxValue));
            }
        }

        public int Increment
        {
            get =>
                _increment;

            set
            {
                if (value <= 0 || value > MaxValue - MinValue)
                {
                    return;
                }
                _increment = value;
                NotifyPropertyChanged(nameof(Increment));
            }
        }

        public int Value
        {
            get =>
                _value;

            set
            {
                if (value < MinValue)
                {
                    _value = MinValue;
                }
                else if (value > MaxValue)
                {
                    _value = MaxValue;
                }
                else
                {
                    _value = value;
                }
                NotifyPropertyChanged(nameof(Value));
            }
        }

        public ICommand DecrementCommand =>
            _decrementCommand ?? (_decrementCommand = new RelayCommand(_ => DecreaseValue()));

        public ICommand IncrementCommand =>
            _incrementCommand ?? (_incrementCommand = new RelayCommand(_ => IncreaseValue()));

        private void DecreaseValue()
        {
            Value -= Increment;
        }

        private void IncreaseValue()
        {
            Value += Increment;
        }

    }

}