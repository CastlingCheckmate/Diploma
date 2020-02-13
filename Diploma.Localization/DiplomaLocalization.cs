using System;
using System.ComponentModel;
using System.Linq;

using Diploma.Localization.Languages;

namespace Diploma.Localization
{
	
	public sealed class DiplomaLocalization : IDiplomaLocalization, INotifyPropertyChanged
	{
		
		private static readonly Lazy<DiplomaLocalization> _instance;
		
		static DiplomaLocalization()
		{
			_instance = new Lazy<DiplomaLocalization>(() => new DiplomaLocalization());
		}
		
		public static DiplomaLocalization Instance =>
			_instance.Value;
		
		private readonly string[] _localizationPropertiesNames;
		private DiplomaLanguages _currentLanguage;
		private IDiplomaLocalization _currentLocalization;
		public event PropertyChangedEventHandler PropertyChanged;
		
		private DiplomaLocalization()
		{
			_localizationPropertiesNames = typeof(IDiplomaLocalization)
				.GetProperties()
				.Select(property => property.Name)
				.ToArray();
			CurrentLanguage = DiplomaLanguages.Russian;
		}
		
		public DiplomaLanguages CurrentLanguage
		{
			get =>
				_currentLanguage;
			
			set
			{
				if (CurrentLanguage == value)
				{
					return;
				}
				if (!Enum.IsDefined(typeof(DiplomaLanguages), value))
				{
					return;
				}
				_currentLanguage = value;
				switch (CurrentLanguage)
				{
					case DiplomaLanguages.English:
						_currentLocalization = DiplomaEnLocalization.Instance;
						break;
					case DiplomaLanguages.Russian:
						_currentLocalization = DiplomaRuLocalization.Instance;
						break;
				}
				NotifyPropertyChanged(_localizationPropertiesNames);
			}
		}
		
		public string DiplomaTitle =>
			_currentLocalization.DiplomaTitle;
		
		public string MainViewHeader =>
			_currentLocalization.MainViewHeader;
		
		public string HelpButtonToolTip =>
			_currentLocalization.HelpButtonToolTip;
		
		public string MinimizeButtonToolTip =>
			_currentLocalization.MinimizeButtonToolTip;
		
		public string CloseButtonToolTip =>
			_currentLocalization.CloseButtonToolTip;
		
		public string SimplexVerticesCount =>
			_currentLocalization.SimplexVerticesCount;
		
		public string VerticesGradesVector =>
			_currentLocalization.VerticesGradesVector;
		
		public string VerticesGradesVectorHint =>
			_currentLocalization.VerticesGradesVectorHint;
		
		public string Restore =>
			_currentLocalization.Restore;
		
		public string Clear =>
			_currentLocalization.Clear;
		
		public string VerticesGradesVectorCantBeRestored(object value0)
		{
			return _currentLocalization.VerticesGradesVectorCantBeRestored(
				value0);
		}
		
		public string Menu =>
			_currentLocalization.Menu;
		
		public string FileMenuItem =>
			_currentLocalization.FileMenuItem;
		
		public string LanguageSelectionMenuItem =>
			_currentLocalization.LanguageSelectionMenuItem;
		
		public string HelpMenuItem =>
			_currentLocalization.HelpMenuItem;
		
		public string AboutMenuItem =>
			_currentLocalization.AboutMenuItem;
		
		public string QuitMenuItem =>
			_currentLocalization.QuitMenuItem;
		
		public string OK =>
			_currentLocalization.OK;
		
		public string Cancel =>
			_currentLocalization.Cancel;
		
		public string Yes =>
			_currentLocalization.Yes;
		
		public string No =>
			_currentLocalization.No;
		
		public string Error =>
			_currentLocalization.Error;
		
		public string Information =>
			_currentLocalization.Information;
		
		public string Warning =>
			_currentLocalization.Warning;
		
		public string Language =>
			_currentLocalization.Language;
		
		public string LanguageSelectionHeader =>
			_currentLocalization.LanguageSelectionHeader;
		
		public string SelectLanguage =>
			_currentLocalization.SelectLanguage;
		
		public string EnglishLanguage =>
			_currentLocalization.EnglishLanguage;
		
		public string RussianLanguage =>
			_currentLocalization.RussianLanguage;
		
		public string LanguageChangedSuccessfully =>
			_currentLocalization.LanguageChangedSuccessfully;
		
		public string HelpHeader =>
			_currentLocalization.HelpHeader;
		
		public string HelpLines =>
			_currentLocalization.HelpLines;
		
		public string AboutHeader =>
			_currentLocalization.AboutHeader;
		
		public string AboutLines =>
			_currentLocalization.AboutLines;
		
		public string QuitHeader =>
			_currentLocalization.QuitHeader;
		
		public string QuitMessage =>
			_currentLocalization.QuitMessage;
		
		private void NotifyPropertyChanged(params string[] propertiesNames)
		{
			foreach (var propertyName in propertiesNames)
			{
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	
	}

}