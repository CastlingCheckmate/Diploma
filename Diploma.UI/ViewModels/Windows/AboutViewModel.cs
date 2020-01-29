using System.Windows.Input;

using Diploma.UI.Auxiliary.Commands;
using Diploma.UI.ViewModels.Base;

namespace Diploma.UI.ViewModels.Windows
{

    public sealed class AboutViewModel : WindowViewModel
    {

        private ICommand _okCommand;

        public ICommand OkCommand =>
            _okCommand ?? (_okCommand = new RelayCommand(o => Ok(), o => true));

        private void Ok()
        {
            View?.Close();
        }

    }

}