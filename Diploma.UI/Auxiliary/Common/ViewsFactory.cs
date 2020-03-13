using System.Windows.Controls;

using Diploma.UI.Auxiliary.MessageBox;
using Diploma.UI.ViewModels.Windows;
using Diploma.UI.Views.Windows;

namespace Diploma.UI.Auxiliary.Common
{

    internal static class ViewsFactory
    {

        public static MainView CreateMainView()
        {
            var mainViewModel = new MainViewModel();
            var mainView = new MainView();
            // TODO: this is shitty
            var mainViewTabControl = (TabControl)mainView.FindName("_tabs");
            ViewBinder.Bind(mainView, mainViewModel);
            mainViewModel.TabsView = mainViewTabControl;
            return mainView;
        }

        public static MessageBoxView CreateMessageBoxView(MessageBoxTypes type, string header,
            string text, MessageBoxButtons buttons)
        {
            var messageBoxView = new MessageBoxView();
            var messageBoxViewModel = new MessageBoxViewModel(type, header, text, buttons);
            ViewBinder.Bind(messageBoxView, messageBoxViewModel);
            return messageBoxView;
        }

        public static HelpView CreateHelpView()
        {
            var helpView = new HelpView();
            var helpViewModel = new HelpViewModel();
            ViewBinder.Bind(helpView, helpViewModel);
            return helpView;
        }

        public static AboutView CreateAboutView()
        {
            var aboutView = new AboutView();
            var aboutViewModel = new AboutViewModel();
            ViewBinder.Bind(aboutView, aboutViewModel);
            return aboutView;
        }

        public static LanguageSelectionView CreateLanguageSelectionView()
        {
            var languageSelectionView = new LanguageSelectionView();
            var languageSelectionViewModel = new LanguageSelectionViewModel();
            ViewBinder.Bind(languageSelectionView, languageSelectionViewModel);
            return languageSelectionView;
        }

    }

}