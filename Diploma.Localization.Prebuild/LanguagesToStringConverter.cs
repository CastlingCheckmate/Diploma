using System;

using Diploma.Localization.Languages;

namespace Diploma.Localization.Prebuild
{

    internal static class LanguagesToStringConverter
    {

        internal static string Convert(DiplomaLanguages language, bool isFullName = true, bool isFirstLetterUppercase = true)
        {
            switch (language)
            {
                case DiplomaLanguages.English:
                    return isFullName ? isFirstLetterUppercase ? "English" : "english" : isFirstLetterUppercase ? "En" : "en";
                case DiplomaLanguages.Russian:
                    return isFullName ? isFirstLetterUppercase ? "Russian" : "russian" : isFirstLetterUppercase ? "Ru" : "ru";
                default:
                    throw new NotSupportedException();
            }
        }

    }

}