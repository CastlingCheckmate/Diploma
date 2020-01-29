using Diploma.UI.Auxiliary.MessageBox;
using Diploma.UI.ViewModels.Windows;
using Diploma.UI.Views.Windows;

namespace Diploma.UI.Auxiliary.Common
{

    internal static class ViewsFactory
    {

        public static MainView CreateMainView()
        {
            var mainView = new MainView();
            var mainViewViewModel = new MainViewModel();
            ViewBinder.Bind(mainView, mainViewViewModel);
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
            var helpViewViewModel = new HelpViewModel();
            ViewBinder.Bind(helpView, helpViewViewModel);
            return helpView;
        }

        public static AboutView CreateAboutView()
        {
            var aboutView = new AboutView();
            var aboutViewViewModel = new AboutViewModel();
            ViewBinder.Bind(aboutView, aboutViewViewModel);
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