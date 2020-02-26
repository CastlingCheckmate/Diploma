using Diploma.UI.ViewModels.Windows;
using System.Windows;

namespace Diploma.UI.Views.Windows
{

    public partial class MainView : Window
    {

        public MainView()
        {
            InitializeComponent();
        }

        public MainView(MainViewModel mainViewModel)
            : this()
        {
            mainViewModel.TabsView = _tabs;
        }

    }

}