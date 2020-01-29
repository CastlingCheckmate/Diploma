using Diploma.UI.ViewModels.Controls;

namespace Diploma.UI
{

    public sealed class MainViewSettingsModel
    {

        private readonly CheckBoxMenuItemViewModel _fileMenuItemViewModel;
        private readonly CheckBoxMenuItemViewModel _languageSelectionItemViewModel;
        private readonly CheckBoxMenuItemViewModel _helpItemViewModel;
        private readonly CheckBoxMenuItemViewModel _aboutItemViewModel;

        public MainViewSettingsModel()
        {
            _fileMenuItemViewModel = new CheckBoxMenuItemViewModel(new CheckBoxMenuItemModel()
                { IsChecked = false, IsVisible = true, IsEnabled = true });
            _languageSelectionItemViewModel = new CheckBoxMenuItemViewModel(new CheckBoxMenuItemModel()
                { IsChecked = false, IsVisible = true, IsEnabled = true });
            _helpItemViewModel = new CheckBoxMenuItemViewModel(new CheckBoxMenuItemModel()
                { IsChecked = false, IsVisible = true, IsEnabled = true });
            _aboutItemViewModel = new CheckBoxMenuItemViewModel(new CheckBoxMenuItemModel()
                { IsChecked = false, IsVisible = true, IsEnabled = true });
        }

        public CheckBoxMenuItemViewModel FileMenuItemViewModel =>
            _fileMenuItemViewModel;

        public CheckBoxMenuItemViewModel LanguageSelectionItemViewModel =>
            _languageSelectionItemViewModel;

        public CheckBoxMenuItemViewModel HelpItemViewModel =>
            _helpItemViewModel;

        public CheckBoxMenuItemViewModel AboutItemViewModel =>
            _aboutItemViewModel;

    }

}