using System.Windows;

namespace Diploma.UI.ViewModels.Base
{

    public class WindowViewModel : ControlViewModel
    {

        protected WindowViewModel()
            : base()
        {

        }

        public new Window View =>
            (Window)base.View;

    }

}