using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

using Diploma.UI.ViewModels.Base;

namespace Diploma.UI.Auxiliary.Common
{

    internal static class ViewBinder
    {

        private static IDictionary<ControlViewModel, Control> _binded;

        static ViewBinder()
        {
            _binded = new Dictionary<ControlViewModel, Control>();
        }

        public static void Bind(Control view, ControlViewModel viewModel)
        {
            view.DataContext = viewModel;
            viewModel.OnBind();
            _binded.Add(viewModel, view);
        }

        public static void Unbind(Control view, ControlViewModel viewModel)
        {
            if (!ReferenceEquals(view.DataContext, viewModel))
            {
                return;
            }
            if (!_binded.ContainsKey(viewModel))
            {
                return;
            }
            _binded.Remove(viewModel);
            view.DataContext = null;
            viewModel.OnUnbind();
        }

        public static Control FindView(ControlViewModel viewModel)
        {
            if (!_binded.ContainsKey(viewModel))
            {
                return null;
            }
            return _binded[viewModel];
        }

    }

}