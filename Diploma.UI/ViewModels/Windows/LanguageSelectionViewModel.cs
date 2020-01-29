using System;
using System.Windows;
using System.Windows.Input;

using Diploma.Localization;
using Diploma.Localization.Languages;
using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.Auxiliary.MessageBox;
using Diploma.UI.ViewModels.Base;

using MessageBox = Diploma.UI.Auxiliary.MessageBox.MessageBox;

namespace Diploma.UI.ViewModels.Windows
{

    internal sealed class LanguageSelectionViewModel : WindowViewModel
    {

        private DiplomaLanguages _selectedLanguage;
        private ICommand _applyLanguageChangeCommand;
        private ICommand _cancelCommand;

        public override void OnBind()
        {
            base.OnBind();
            _selectedLanguage = DiplomaLocalization.Instance.CurrentLanguage;
        }

        public ICommand ApplyLanguageChangeCommand =>
            _applyLanguageChangeCommand ?? (_applyLanguageChangeCommand = new RelayCommand(
                _ => ApplyLanguageChange(), _ => IsLanguageChanged));

        public ICommand CancelCommand =>
            _cancelCommand ?? (_cancelCommand = new RelayCommand(o => Cancel()));

        public object SelectedLanguage
        {
            get =>
                _selectedLanguage;

            set
            {
                if (value is null || !(value is DiplomaLanguages) || !Enum.IsDefined(typeof(DiplomaLanguages), value))
                {
                    return;
                }
                _selectedLanguage = (DiplomaLanguages)value;
                NotifyPropertyChanged(nameof(IsLanguageChanged));
            }
        }

        public bool IsLanguageChanged =>
            _selectedLanguage != DiplomaLocalization.Instance.CurrentLanguage;

        private void ApplyLanguageChange()
        {
            DiplomaLocalization.Instance.CurrentLanguage = _selectedLanguage;
            NotifyPropertyChanged(nameof(IsLanguageChanged));
            // TODO: if View is null
            View.Visibility = Visibility.Hidden;
            MessageBox.Show(MessageBoxTypes.Information, DiplomaLocalization.Instance.Information,
                DiplomaLocalization.Instance.LanguageChangedSuccessfully, MessageBoxButtons.Ok);
            Cancel();
        }

        private void Cancel()
        {
            View?.Close();
        }

    }

}