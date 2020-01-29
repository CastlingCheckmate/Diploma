using System.Windows;

using Diploma.UI.Auxiliary.Common;
using Diploma.UI.ViewModels.Windows;

namespace Diploma.UI.Auxiliary.MessageBox
{

    internal static class MessageBox
    {

        public static MessageBoxResult Show(MessageBoxTypes type, string header, string text, MessageBoxButtons buttons)
        {
            var messageBoxView = ViewsFactory.CreateMessageBoxView(type, header, text, buttons);
            var messageBoxViewModel = (MessageBoxViewModel)messageBoxView.DataContext;
            messageBoxView.ShowDialog();
            var result = messageBoxViewModel.Result;
            ViewBinder.Unbind(messageBoxView, messageBoxViewModel);
            return result;
        }

    }

}