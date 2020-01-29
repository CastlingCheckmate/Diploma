using System.Windows.Input;

using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.ViewModels.Base;

namespace Diploma.UI.ViewModels.Windows
{

    public sealed class HelpViewModel : WindowViewModel
    {

        private ICommand _okCommand;

        public ICommand OkCommand =>
            _okCommand ?? (_okCommand = new RelayCommand(o => Ok()));

        private void Ok()
        {
            View?.Close();
        }

    }

}