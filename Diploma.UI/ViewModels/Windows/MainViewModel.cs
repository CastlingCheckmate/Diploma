using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Diploma.Hypergraph;
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
        private string _vectorString;
        private readonly MainViewSettingsModel _settings;
        private readonly NumericUpDownViewModel _simplexVerticesCountDataContext;
        private WindowState _state;
        private HypergraphModel _hypergraph;
        private ICommand _showLanguageSelectionCommand;
        private ICommand _showHelpCommand;
        private ICommand _showAboutCommand;
        private ICommand _minimizeCommand;
        private ICommand _quitCommand;
        private ICommand _restoreCommand;
        private ICommand _clearCommand;

        public MainViewModel()
        {
            _settings = new MainViewSettingsModel();
            _simplexVerticesCountDataContext = new NumericUpDownViewModel() { MinValue = 2, MaxValue = 20, Increment = 1, Value = 2 };
            State = WindowState.Maximized;
        }

        public MainViewSettingsModel Settings =>
            _settings;

        public NumericUpDownViewModel SimplexVerticesCountDataContext =>
            _simplexVerticesCountDataContext;

        public string VectorString
        {
            get =>
                _vectorString;

            set
            {
                _vectorString = value;
                NotifyPropertyChanged(nameof(VectorString), nameof(IsValidVectorString));
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

        public HypergraphModel Hypergraph
        {
            get =>
                _hypergraph;

            set
            {
                _hypergraph = value;
                NotifyPropertyChanged(nameof(Hypergraph));
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

        public ICommand RestoreCommand =>
            _restoreCommand ?? (_restoreCommand = new RelayCommand(_ => Restore(),
                _ => IsValidVectorString));

        public ICommand ClearCommand =>
            _clearCommand ?? (_clearCommand = new RelayCommand(_ => Clear(),
                _ => Hypergraph != null));

        public bool IsValidVectorString
        {
            get
            {
                if (string.IsNullOrEmpty(VectorString))
                {
                    return false;
                }
                var gradesStrings = VectorString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var gradeString in gradesStrings)
                {
                    if (!int.TryParse(gradeString, out int grade) || grade <= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

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

        private void Restore()
        {
            VectorString = string.Join(" ", VectorString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            var verticesGradesVector = VectorString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(vertexGradeString => int.Parse(vertexGradeString))
                .ToArray();
            var restoredHypergraph = ReductionAlgorithm.ReductionRecovery(verticesGradesVector, SimplexVerticesCountDataContext.Value);
            if (restoredHypergraph is null)
            {
                MessageBox.Show(MessageBoxTypes.Error, DiplomaLocalization.Instance.Error,
                    DiplomaLocalization.Instance.VerticesGradesVectorCantBeRestored(SimplexVerticesCountDataContext.Value - 1), MessageBoxButtons.Ok);
                return;
            }
            Hypergraph = restoredHypergraph;
        }

        private void Clear()
        {
            VectorString = string.Empty;
            Hypergraph = null;
        }

    }

}