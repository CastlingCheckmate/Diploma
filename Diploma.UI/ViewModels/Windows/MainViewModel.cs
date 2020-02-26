using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Diploma.Localization;
using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.Auxiliary.Common;
using Diploma.UI.Auxiliary.MessageBox;
using Diploma.UI.ViewModels.Base;
using Diploma.UI.ViewModels.Controls;

using MessageBox = Diploma.UI.Auxiliary.MessageBox.MessageBox;

namespace Diploma.UI.ViewModels.Windows
{

    public class MainViewModel : WindowViewModel
    {

        private readonly MainViewSettingsModel _settings;
        private WindowState _state;
        private ICommand _showLanguageSelectionCommand;
        private ICommand _showHelpCommand;
        private ICommand _showAboutCommand;
        private ICommand _minimizeCommand;
        private ICommand _quitCommand;
        private int _selectedTabIndex;

        private ObservableCollection<TabItemViewModel> _tabs;

        public MainViewModel()
        {
            _settings = new MainViewSettingsModel();
            State = WindowState.Maximized;
            Tabs = new ObservableCollection<TabItemViewModel>();
            Tabs.Add(new TabItemViewModel(Tabs, false));
            Tabs.Add(new TabItemViewModel(Tabs, true));
        }

        private TabControl _tabsView;
        public TabControl TabsView
        {
            set =>
                _tabsView = value;
        }

        public MainViewSettingsModel Settings =>
            _settings;

        public int SelectedTabIndex
        {
            get =>
                _selectedTabIndex;

            set
            {
                if (value == Tabs.Count - 1)
                {
                    _tabsView.ItemsSource = null;
                    Tabs.Insert(Tabs.Count - 1, new TabItemViewModel(Tabs, false));
                    _tabsView.ItemsSource = Tabs;
                }
                _selectedTabIndex = value;
                NotifyPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        public ObservableCollection<TabItemViewModel> Tabs
        {
            get =>
                _tabs;

            private set
            {
                _tabs = value;
                NotifyPropertyChanged(nameof(Tabs));
            }
        }

        public WindowState State
        {
            get =>
                _state;

            set
            {
                _state = value;
                NotifyPropertyChanged(nameof(State));
            }
        }

        public ICommand ShowLanguageSelectionCommand =>
            _showLanguageSelectionCommand ??
                (_showLanguageSelectionCommand = new RelayCommand(_ => ShowLanguageSelection()));

        public ICommand ShowHelpCommand =>
            _showHelpCommand ?? (_showHelpCommand = new RelayCommand(_ => ShowHelp()));

        public ICommand ShowAboutCommand =>
            _showAboutCommand ?? (_showAboutCommand = new RelayCommand(_ => ShowAbout()));

        public ICommand QuitCommand =>
            _quitCommand ?? (_quitCommand = new RelayCommand(_ => Quit()));

        public ICommand MinimizeCommand =>
            _minimizeCommand ?? (_minimizeCommand = new RelayCommand(_ => Minimize()));

        private void ShowLanguageSelection()
        {
            var languageSelectionView = ViewsFactory.CreateLanguageSelectionView();
            languageSelectionView.ShowDialog();
            ViewBinder.Unbind(languageSelectionView, (ControlViewModel)languageSelectionView.DataContext);
        }

        private void ShowHelp()
        {
            var helpView = ViewsFactory.CreateHelpView();
            helpView.ShowDialog();
            ViewBinder.Unbind(helpView, (ControlViewModel)helpView.DataContext);
        }

        private void ShowAbout()
        {
            var aboutView = ViewsFactory.CreateAboutView();
            aboutView.ShowDialog();
            ViewBinder.Unbind(aboutView, (ControlViewModel)aboutView.DataContext);
        }

        private void Quit()
        {
            var result = MessageBox.Show(MessageBoxTypes.Question, DiplomaLocalization.Instance.QuitHeader,
                DiplomaLocalization.Instance.QuitMessage, MessageBoxButtons.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            View?.Close();
            ViewBinder.Unbind(View, this);
        }

        private void Minimize()
        {
            State = WindowState.Minimized;
        }

    }

}