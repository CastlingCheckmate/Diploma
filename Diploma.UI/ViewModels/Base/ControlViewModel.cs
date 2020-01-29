using System.ComponentModel;
using System.Windows.Controls;

using Diploma.UI.Auxiliary.Common;

namespace Diploma.UI.ViewModels.Base
{

    public abstract class ControlViewModel : INotifyPropertyChanged
    {

        protected ControlViewModel()
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnBind()
        {

        }

        public virtual void OnUnbind()
        {

        }

        protected Control View =>
            ViewBinder.FindView(this);


        protected void NotifyPropertyChanged(params string[] propertiesNames)
        {
            foreach (var propertyName in propertiesNames)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }

}