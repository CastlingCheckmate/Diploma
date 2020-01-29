using Diploma.UI.ViewModels.Base;

namespace Diploma.UI.ViewModels.Controls
{

    public sealed class CheckBoxMenuItemViewModel : ControlViewModel
    {

        private readonly CheckBoxMenuItemModel _model;

        public CheckBoxMenuItemViewModel(CheckBoxMenuItemModel model)
        {
            _model = model;
        }

        public bool IsVisible
        {
            get =>
                _model.IsVisible;

            set
            {
                if (_model.IsVisible == value)
                {
                    return;
                }
                _model.IsVisible = value;
                NotifyPropertyChanged(nameof(IsVisible));
            }
        }

        public bool IsChecked
        {
            get =>
                _model.IsChecked;

            set
            {
                if (_model.IsChecked == value)
                {
                    return;
                }
                _model.IsChecked = value;
                NotifyPropertyChanged(nameof(IsChecked));
            }
        }

    }

}