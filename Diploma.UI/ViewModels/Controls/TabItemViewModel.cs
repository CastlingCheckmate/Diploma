using System;
using System.Collections.ObjectModel;
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
using Diploma.UI.Views.Controls;
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
        private ICommand _clearCommand;
        private ICommand _actionCommand;
        private readonly NumericUpDownViewModel _simplexVerticesCountDataContext;
        private string _header = "123";
        private ObservableCollection<TabItemViewModel> _tabs;
        public string Header =>
            _header;

        public TabItemViewModel(ObservableCollection<TabItemViewModel> tabs, TabControl view, bool isAddNewTabTabItem)
        {
            _tabs = tabs;
            _tabsView = view;
            
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
            var ind = _tabs.IndexOf(this);
            HypergraphViewModel = new HypergraphViewModel(restoredHypergraph, (e, e1) => { });
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
                _tabs.Insert(_tabs.Count - 1, new TabItemViewModel(_tabs, _tabsView, false));
            }
            else
            {
                if (HypergraphViewModel != null || !string.IsNullOrEmpty(VectorString))
                {
                    var result = MessageBox.Show(MessageBoxTypes.Question, "?", DiplomaLocalization.Instance.SaveResult, MessageBoxButtons.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        // TODO: saving to local file / database
                        MessageBox.Show(MessageBoxTypes.Information, string.Empty, "// TODO: implement saving", MessageBoxButtons.Ok);
                    }
                }
                _tabs.Remove(this);
            }
        }

    }

}