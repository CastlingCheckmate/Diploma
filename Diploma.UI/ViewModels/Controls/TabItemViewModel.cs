using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Diploma.Hypergraph;
using Diploma.Localization;
using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.Auxiliary.MessageBox;
using Diploma.UI.ViewModels.Base;
using Diploma.UI.ViewModels.Hypergraph;

using MessageBox = Diploma.UI.Auxiliary.MessageBox.MessageBox;

namespace Diploma.UI.ViewModels.Controls
{

    public class TabItemViewModel : ControlViewModel
    {

        private bool _isAddNewTabTabItem;
        private string _vectorString;
        private HypergraphViewModel _hypergraphViewModel;
        private TabControl _tabsView;
        private ICommand _restoreCommand;
        private ICommand _saveCommand;
        private ICommand _clearCommand;
        private ICommand _actionCommand;
        private readonly NumericUpDownViewModel _simplexVerticesCountDataContext;
        private string _filePath;
        
        private ObservableCollection<TabItemViewModel> _tabs;
        
        public string Header =>
            _filePath is null ? $"new {_tabs.IndexOf(this) + 1}" : Path.GetFileName(_filePath);

        public TabItemViewModel(ObservableCollection<TabItemViewModel> tabs, TabControl view, bool isAddNewTabTabItem, string filePath = null)
        {
            _tabs = tabs;
            _tabsView = view;
            _filePath = filePath;
            IsAddNewTabTabItem = isAddNewTabTabItem;
            _simplexVerticesCountDataContext = new NumericUpDownViewModel() { MinValue = 2, MaxValue = 20, Increment = 1, Value = 2 };
        }

        public NumericUpDownViewModel SimplexVerticesCountDataContext =>
            _simplexVerticesCountDataContext;

        public bool IsAddNewTabTabItem
        {
            get =>
                _isAddNewTabTabItem;

            set
            {
                _isAddNewTabTabItem = value;
                NotifyPropertyChanged(nameof(IsAddNewTabTabItem));
            }
        }

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

        public HypergraphViewModel HypergraphViewModel
        {
            get =>
                _hypergraphViewModel;

            set
            {
                _hypergraphViewModel = value;
                NotifyPropertyChanged(nameof(HypergraphViewModel));
            }
        }

        public ICommand RestoreCommand =>
            _restoreCommand ?? (_restoreCommand = new RelayCommand(_ => Restore(),
                _ => IsValidVectorString));

        public ICommand SaveCommand =>
            _saveCommand ?? (_saveCommand = new RelayCommand(_ => Save(),
                _ => HypergraphViewModel != null));

        public ICommand ClearCommand =>
            _clearCommand ?? (_clearCommand = new RelayCommand(_ => Clear(),
                _ => HypergraphViewModel != null));

        public ICommand ActionCommand =>
            _actionCommand ?? (_actionCommand = new RelayCommand(_ => Action()));

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
            HypergraphViewModel = new HypergraphViewModel(restoredHypergraph);
        }

        private void Save()
        {
            var saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.CurrentDirectory,
                OverwritePrompt = true,
                Title = "Saving hypergraph to file...",
                Filter = "Hypergraph file (*.hypg)|*.hypg"
            };
            if (saveFileDialog.ShowDialog() is false)
            {
                return;
            }
            using (var writer = new StreamWriter(new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)))
            {
                // TODO: serialize hypergraph
            }
            _filePath = saveFileDialog.FileName;
        }

        private void Clear()
        {
            VectorString = string.Empty;
            HypergraphViewModel?.Dispose();
            HypergraphViewModel = null;
        }

        private void Action()
        {
            if (IsAddNewTabTabItem)
            {
                var newTabItemViewModel = new TabItemViewModel(_tabs, _tabsView, false);
                _tabs.Insert(_tabs.Count - 1, newTabItemViewModel);
                _tabsView.SelectedIndex = _tabs.IndexOf(newTabItemViewModel);
            }
            else
            {
                if (!(HypergraphViewModel is null))
                {
                    var saveToFile = MessageBox.Show(MessageBoxTypes.Question, "?", DiplomaLocalization.Instance.SaveResult, MessageBoxButtons.YesNo);
                    if (saveToFile == MessageBoxResult.None)
                    {
                        return;
                    }
                    else if (saveToFile == MessageBoxResult.Yes)
                    {
                        Save();
                    }
                }
                _tabs.Remove(this);
            }
        }


    }

}