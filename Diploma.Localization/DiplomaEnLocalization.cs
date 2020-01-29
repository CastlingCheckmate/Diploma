using System;

namespace Diploma.Localization
{
	
	internal sealed class DiplomaEnLocalization : IDiplomaLocalization
	{
		
		private static readonly Lazy<DiplomaEnLocalization> _instance;
		
		static DiplomaEnLocalization()
		{
			_instance = new Lazy<DiplomaEnLocalization>(() => new DiplomaEnLocalization());
		}
		
		public static DiplomaEnLocalization Instance =>
			_instance.Value;
		
		private DiplomaEnLocalization()
		{
			
		}
		
		private readonly string _diplomaTitle = "(en)Редукционный алгоритм восстановления n-комплексов по вектору степеней вершин";
		public string DiplomaTitle =>
			_diplomaTitle;
		
		private readonly string _mainViewHeader = "Diploma";
		public string MainViewHeader =>
			_mainViewHeader;
		
		private readonly string _helpButtonToolTip = "Help [F1]";
		public string HelpButtonToolTip =>
			_helpButtonToolTip;
		
		private readonly string _minimizeButtonToolTip = "Minimize [Shift + _]";
		public string MinimizeButtonToolTip =>
			_minimizeButtonToolTip;
		
		private readonly string _closeButtonToolTip = "Quit [Esc]";
		public string CloseButtonToolTip =>
			_closeButtonToolTip;
		
		private readonly string _simplexVerticesCount = "Simplex vertices count:";
		public string SimplexVerticesCount =>
			_simplexVerticesCount;
		
		private readonly string _verticesGradesVector = "Vertices grades vector:";
		public string VerticesGradesVector =>
			_verticesGradesVector;
		
		private readonly string _restore = "Restore";
		public string Restore =>
			_restore;
		
		private readonly string _verticesGradesVectorCantBeRestored = "Vertices grades vector can't be restored into {0}-complex!";
		public string VerticesGradesVectorCantBeRestored(object value0)
		{
			return string.Format(_verticesGradesVectorCantBeRestored
				, value0);
		}
		
		private readonly string _menu = "Menu";
		public string Menu =>
			_menu;
		
		private readonly string _fileMenuItem = "File";
		public string FileMenuItem =>
			_fileMenuItem;
		
		private readonly string _languageSelectionMenuItem = "Language selection";
		public string LanguageSelectionMenuItem =>
			_languageSelectionMenuItem;
		
		private readonly string _helpMenuItem = "Help";
		public string HelpMenuItem =>
			_helpMenuItem;
		
		private readonly string _aboutMenuItem = "About";
		public string AboutMenuItem =>
			_aboutMenuItem;
		
		private readonly string _quitMenuItem = "Quit";
		public string QuitMenuItem =>
			_quitMenuItem;
		
		private readonly string _oK = "OK";
		public string OK =>
			_oK;
		
		private readonly string _cancel = "Cancel";
		public string Cancel =>
			_cancel;
		
		private readonly string _yes = "Yes";
		public string Yes =>
			_yes;
		
		private readonly string _no = "No";
		public string No =>
			_no;
		
		private readonly string _error = "Error";
		public string Error =>
			_error;
		
		private readonly string _information = "Information";
		public string Information =>
			_information;
		
		private readonly string _warning = "Warning";
		public string Warning =>
			_warning;
		
		private readonly string _language = "Language";
		public string Language =>
			_language;
		
		private readonly string _languageSelectionHeader = "Language selection";
		public string LanguageSelectionHeader =>
			_languageSelectionHeader;
		
		private readonly string _selectLanguage = "Select language";
		public string SelectLanguage =>
			_selectLanguage;
		
		private readonly string _englishLanguage = "English";
		public string EnglishLanguage =>
			_englishLanguage;
		
		private readonly string _russianLanguage = "Russian";
		public string RussianLanguage =>
			_russianLanguage;
		
		private readonly string _languageChangedSuccessfully = "Language changed successfully!";
		public string LanguageChangedSuccessfully =>
			_languageChangedSuccessfully;
		
		private readonly string _helpHeader = "Diploma::Help";
		public string HelpHeader =>
			_helpHeader;
		
		private readonly string _helpLines = "";
		public string HelpLines =>
			_helpLines;
		
		private readonly string _aboutHeader = "Diploma::About";
		public string AboutHeader =>
			_aboutHeader;
		
		private readonly string _aboutLines = "";
		public string AboutLines =>
			_aboutLines;
		
		private readonly string _quitHeader = "Diploma::Quit";
		public string QuitHeader =>
			_quitHeader;
		
		private readonly string _quitMessage = "Are You really want to quit?";
		public string QuitMessage =>
			_quitMessage;
		
	}

}