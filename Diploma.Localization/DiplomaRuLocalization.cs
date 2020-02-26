using System;

namespace Diploma.Localization
{
	
	internal sealed class DiplomaRuLocalization : IDiplomaLocalization
	{
		
		private static readonly Lazy<DiplomaRuLocalization> _instance;
		
		static DiplomaRuLocalization()
		{
			_instance = new Lazy<DiplomaRuLocalization>(() => new DiplomaRuLocalization());
		}
		
		public static DiplomaRuLocalization Instance =>
			_instance.Value;
		
		private DiplomaRuLocalization()
		{
			
		}
		
		private readonly string _diplomaTitle = "Редукционный алгоритм восстановления n-комплексов из векторов степеней вершин";
		public string DiplomaTitle =>
			_diplomaTitle;
		
		private readonly string _mainViewHeader = "Диплом";
		public string MainViewHeader =>
			_mainViewHeader;
		
		private readonly string _helpButtonToolTip = "Помощь [F1]";
		public string HelpButtonToolTip =>
			_helpButtonToolTip;
		
		private readonly string _minimizeButtonToolTip = "Свернуть [Shift + _]";
		public string MinimizeButtonToolTip =>
			_minimizeButtonToolTip;
		
		private readonly string _closeButtonToolTip = "Выход [Esc]";
		public string CloseButtonToolTip =>
			_closeButtonToolTip;
		
		private readonly string _simplexVerticesCount = "Количество вершин симплекса:";
		public string SimplexVerticesCount =>
			_simplexVerticesCount;
		
		private readonly string _verticesGradesVector = "Вектор степеней вершин:";
		public string VerticesGradesVector =>
			_verticesGradesVector;
		
		private readonly string _verticesGradesVectorHint = "Введите степени вершин из вектора степеней вершин через пробел";
		public string VerticesGradesVectorHint =>
			_verticesGradesVectorHint;
		
		private readonly string _restore = "Восстановить";
		public string Restore =>
			_restore;
		
		private readonly string _clear = "Очистить";
		public string Clear =>
			_clear;
		
		private readonly string _verticesGradesVectorCantBeRestored = "Вектор степеней вершин не может быть реализован в {0}-комплекс!";
		public string VerticesGradesVectorCantBeRestored(object value0)
		{
			return string.Format(_verticesGradesVectorCantBeRestored
				, value0);
		}
		
		private readonly string _saveResult = "Сохранить?";
		public string SaveResult =>
			_saveResult;
		
		private readonly string _menu = "Меню";
		public string Menu =>
			_menu;
		
		private readonly string _fileMenuItem = "Файл";
		public string FileMenuItem =>
			_fileMenuItem;
		
		private readonly string _languageSelectionMenuItem = "Выбор языка";
		public string LanguageSelectionMenuItem =>
			_languageSelectionMenuItem;
		
		private readonly string _helpMenuItem = "Помощь";
		public string HelpMenuItem =>
			_helpMenuItem;
		
		private readonly string _aboutMenuItem = "О программе";
		public string AboutMenuItem =>
			_aboutMenuItem;
		
		private readonly string _quitMenuItem = "Выход";
		public string QuitMenuItem =>
			_quitMenuItem;
		
		private readonly string _oK = "OK";
		public string OK =>
			_oK;
		
		private readonly string _cancel = "Отмена";
		public string Cancel =>
			_cancel;
		
		private readonly string _yes = "Да";
		public string Yes =>
			_yes;
		
		private readonly string _no = "Нет";
		public string No =>
			_no;
		
		private readonly string _error = "Ошибка";
		public string Error =>
			_error;
		
		private readonly string _information = "Информация";
		public string Information =>
			_information;
		
		private readonly string _warning = "Предупреждение";
		public string Warning =>
			_warning;
		
		private readonly string _language = "Язык";
		public string Language =>
			_language;
		
		private readonly string _languageSelectionHeader = "Выбор языка";
		public string LanguageSelectionHeader =>
			_languageSelectionHeader;
		
		private readonly string _selectLanguage = "Выберите язык";
		public string SelectLanguage =>
			_selectLanguage;
		
		private readonly string _englishLanguage = "Английский";
		public string EnglishLanguage =>
			_englishLanguage;
		
		private readonly string _russianLanguage = "Русский";
		public string RussianLanguage =>
			_russianLanguage;
		
		private readonly string _languageChangedSuccessfully = "Язык успешно изменён!";
		public string LanguageChangedSuccessfully =>
			_languageChangedSuccessfully;
		
		private readonly string _helpHeader = "Diploma::Помощь";
		public string HelpHeader =>
			_helpHeader;
		
		private readonly string _helpLines = "";
		public string HelpLines =>
			_helpLines;
		
		private readonly string _aboutHeader = "Diploma::О программе";
		public string AboutHeader =>
			_aboutHeader;
		
		private readonly string _aboutLines = "";
		public string AboutLines =>
			_aboutLines;
		
		private readonly string _quitHeader = "Diploma::Выход";
		public string QuitHeader =>
			_quitHeader;
		
		private readonly string _quitMessage = "Вы действительно хотите выйти?";
		public string QuitMessage =>
			_quitMessage;
		
	}

}