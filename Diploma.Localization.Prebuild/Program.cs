using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using Diploma.Localization.Languages;

using static Diploma.Extensions.StreamWriterEx;

namespace Diploma.Localization.Prebuild
{

    internal sealed class Program
    {

        private static readonly string LocalizationValueUndefined = "LOCALIZATION_VALUE_UNDEFINED";
        private static readonly string InterfaceName = "IDiplomaLocalization";

        private static void Main()
        {
            var englishLocalization = ReadLocalization(DiplomaLanguages.English);
            var russianLocalization = ReadLocalization(DiplomaLanguages.Russian);
            var localizations = englishLocalization.Union(russianLocalization)
                .Select(item => item.Item1)
                .Distinct()
                .ToArray();
            // TODO: merge localizations
            WriteLocalizationInterface(englishLocalization);
            WriteLocalizationClass(DiplomaLanguages.English, englishLocalization, localizations);
            WriteLocalizationClass(DiplomaLanguages.Russian, russianLocalization, localizations);
            WriteCommonLocalizationClass(englishLocalization);
        }

        private static(string, string)[] ReadLocalization(DiplomaLanguages language)
        {
            var result = new List<(string, string)>();
            var localizationFilePath = $"{new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}" +
                $"\\Diploma.Localization\\{LanguagesToStringConverter.Convert(language, false)}\\Diploma.xml";
            var localizationFile = new XmlDocument();
            localizationFile.Load(localizationFilePath);
            foreach (XmlNode xmlNode in localizationFile.SelectNodes("/records/record"))
            {
                result.Add((xmlNode.Attributes["name"].Value, xmlNode.InnerText));
            }
            return result.ToArray();
        }

        private static void WriteLocalizationInterface((string, string)[] localizationsUnion)
        {
            var filePath = $"{new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}\\Diploma.Localization\\{InterfaceName}.cs";
            using (var localizationInterfaceWriter = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                localizationInterfaceWriter.WriteLineWithTabs(0, "namespace Diploma.Localization")
                    .WriteLineWithTabs(0, "{")
                    .WriteLineWithTabs(1, string.Empty)
                    .WriteLineWithTabs(1, $"public interface {InterfaceName}")
                    .WriteLineWithTabs(1, "{")
                    .WriteLineWithTabs(2, string.Empty);
                foreach (var item in localizationsUnion)
                {
                    var propertyOrMethodName = item.Item1;
                    var propertyOrMethodValue = item.Item2;
                    localizationInterfaceWriter.WriteLineWithTabs(2, $"string {propertyOrMethodName}");
                    var placeholdersCount = GetPlaceholdersCount(propertyOrMethodValue);
                    if (placeholdersCount == 0)
                    {
                        localizationInterfaceWriter.WriteLine("{ get; }");
                    }
                    else
                    {
                        localizationInterfaceWriter.Write("(");
                        for (var placeholder = 0; placeholder < placeholdersCount; placeholder++)
                        {
                            localizationInterfaceWriter.Write($"object value{placeholder}");
                            if (placeholder != placeholdersCount - 1)
                            {
                                localizationInterfaceWriter.Write(", ");
                            }
                        }
                        localizationInterfaceWriter.WriteLine(");");
                    }
                    localizationInterfaceWriter.WriteLineWithTabs(2, string.Empty);
                }
                localizationInterfaceWriter.WriteLineWithTabs(1, "}")
                    .WriteLineEx()
                    .WriteWithTabs(0, "}");
            }
        }

        private static void WriteLocalizationClass(DiplomaLanguages language, (string, string)[] localization, string[] localizationsUnion)
        {
            var className = $"Diploma{LanguagesToStringConverter.Convert(language, false)}Localization";
            var filePath = $"{new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}\\Diploma.Localization\\{className}.cs";
            using (var localizationClassWriter = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                localizationClassWriter.WriteLineWithTabs(0, "using System;")
                    .WriteLineWithTabs(0, string.Empty)
                    .WriteLineWithTabs(0, "namespace Diploma.Localization")
                    .WriteLineWithTabs(0, "{")
                    .WriteLineWithTabs(1, string.Empty)
                    .WriteLineWithTabs(1, $"internal sealed class {className} : {InterfaceName}")
                    .WriteLineWithTabs(1, "{")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, $"private static readonly Lazy<{className}> _instance;")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, $"static {className}()")
                    .WriteLineWithTabs(2, "{")
                    .WriteLineWithTabs(3, $"_instance = new Lazy<{className}>(() => new {className}());")
                    .WriteLineWithTabs(2, "}")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, $"public static {className} Instance =>")
                    .WriteLineWithTabs(3, "_instance.Value;")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, $"private {className}()")
                    .WriteLineWithTabs(2, "{")
                    .WriteLineWithTabs(3, string.Empty)
                    .WriteLineWithTabs(2, "}")
                    .WriteLineWithTabs(2, string.Empty);
                foreach (var item in localizationsUnion)
                {
                    var propertyOrMethodName = item;
                    var fieldName = $"_{char.ToLower(propertyOrMethodName[0])}{propertyOrMethodName.Substring(1)}";
                    var fieldValue = localization.Where(_item => _item.Item1 == item)
                        .DefaultIfEmpty((null, LocalizationValueUndefined))
                        .Single()
                        .Item2;
                    localizationClassWriter.WriteLineWithTabs(2, $"private readonly string {fieldName} = \"{fieldValue}\";");
                    localizationClassWriter.WriteWithTabs(2, $"public string {propertyOrMethodName}");
                    var placeholdersCount = GetPlaceholdersCount(fieldValue);
                    if (placeholdersCount == 0)
                    {
                        localizationClassWriter.WriteLineEx(" =>")
                            .WriteLineWithTabs(3, $"{fieldName};");
                    }
                    else
                    {
                        localizationClassWriter.Write("(");
                        for (var placeholder = 0; placeholder < placeholdersCount; placeholder++)
                        {
                            localizationClassWriter.Write($"object value{placeholder}");
                            if (placeholder != placeholdersCount - 1)
                            {
                                localizationClassWriter.Write(", ");
                            }
                        }
                        localizationClassWriter.WriteLineEx(")")
                            .WriteLineWithTabs(2, "{")
                            .WriteLineWithTabs(3, $"return string.Format({fieldName}");
                        for (var placeholder = 0; placeholder < placeholdersCount; placeholder++)
                        {
                            localizationClassWriter.WriteWithTabs(4, $", value{placeholder}");
                            if (placeholder != placeholdersCount - 1)
                            {
                                localizationClassWriter.WriteLine();
                            }
                            else
                            {
                                localizationClassWriter.WriteLine(");");
                            }
                        }
                        localizationClassWriter.WriteLineWithTabs(2, "}");
                    }
                    localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                }
                localizationClassWriter.WriteLineWithTabs(1, "}")
                    .WriteLineEx()
                    .WriteWithTabs(0, "}");
            }
        }

        private static void WriteCommonLocalizationClass((string, string)[] localization)
        {
            var filePath = $"{new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}" +
                $"\\Diploma.Localization\\DiplomaLocalization.cs";
            using (var localizationClassWriter = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                localizationClassWriter.WriteLineWithTabs(0, "using System;")
                    .WriteLineWithTabs(0, "using System.ComponentModel;")
                    .WriteLineWithTabs(0, "using System.Linq;")
                    .WriteLineWithTabs(0, string.Empty)
                    .WriteLineWithTabs(0, "using Diploma.Localization.Languages;")
                    .WriteLineWithTabs(0, string.Empty)
                    .WriteLineWithTabs(0, "namespace Diploma.Localization")
                    .WriteLineWithTabs(0, "{")
                    .WriteLineWithTabs(1, string.Empty)
                    .WriteLineWithTabs(1, $"public sealed class DiplomaLocalization : {InterfaceName}, INotifyPropertyChanged")
                    .WriteLineWithTabs(1, "{")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, "private static readonly Lazy<DiplomaLocalization> _instance;")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, "static DiplomaLocalization()")
                    .WriteLineWithTabs(2, "{")
                    .WriteLineWithTabs(3, "_instance = new Lazy<DiplomaLocalization>(() => new DiplomaLocalization());")
                    .WriteLineWithTabs(2, "}")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, "public static DiplomaLocalization Instance =>")
                    .WriteLineWithTabs(3, "_instance.Value;")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, "private readonly string[] _localizationPropertiesNames;")
                    .WriteLineWithTabs(2, "private DiplomaLanguages _currentLanguage;")
                    .WriteLineWithTabs(2, "private IDiplomaLocalization _currentLocalization;")
                    .WriteLineWithTabs(2, "public event PropertyChangedEventHandler PropertyChanged;")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, "private DiplomaLocalization()")
                    .WriteLineWithTabs(2, "{")
                    .WriteLineWithTabs(3, "_localizationPropertiesNames = typeof(IDiplomaLocalization)")
                    .WriteLineWithTabs(4, ".GetProperties()")
                    .WriteLineWithTabs(4, ".Select(property => property.Name)")
                    .WriteLineWithTabs(4, ".ToArray();")
                    .WriteLineWithTabs(3, "CurrentLanguage = DiplomaLanguages.Russian;")
                    .WriteLineWithTabs(2, "}")
                    .WriteLineWithTabs(2, string.Empty)
                    .WriteLineWithTabs(2, "public DiplomaLanguages CurrentLanguage")
                    .WriteLineWithTabs(2, "{")
                    .WriteLineWithTabs(3, "get =>")
                    .WriteLineWithTabs(4, "_currentLanguage;")
                    .WriteLineWithTabs(3, string.Empty)
                    .WriteLineWithTabs(3, "set")
                    .WriteLineWithTabs(3, "{")
                    .WriteLineWithTabs(4, "if (CurrentLanguage == value)")
                    .WriteLineWithTabs(4, "{")
                    .WriteLineWithTabs(5, "return;")
                    .WriteLineWithTabs(4, "}")
                    .WriteLineWithTabs(4, "if (!Enum.IsDefined(typeof(DiplomaLanguages), value))")
                    .WriteLineWithTabs(4, "{")
                    .WriteLineWithTabs(5, "return;")
                    .WriteLineWithTabs(4, "}")
                    .WriteLineWithTabs(4, "_currentLanguage = value;")
                    .WriteLineWithTabs(4, "switch (CurrentLanguage)")
                    .WriteLineWithTabs(4, "{");
                foreach (DiplomaLanguages language in Enum.GetValues(typeof(DiplomaLanguages)))
                {
                    localizationClassWriter.WriteLineWithTabs(5, $"case DiplomaLanguages.{LanguagesToStringConverter.Convert(language)}:")
                        .WriteLineWithTabs(6, $"_currentLocalization = Diploma{LanguagesToStringConverter.Convert(language, false)}Localization.Instance;")
                        .WriteLineWithTabs(6, "break;");
                }
                localizationClassWriter.WriteLineWithTabs(4, "}")
                    .WriteLineWithTabs(4, "NotifyPropertyChanged(_localizationPropertiesNames);")
                    .WriteLineWithTabs(3, "}")
                    .WriteLineWithTabs(2, "}")
                    .WriteLineWithTabs(2, string.Empty);
                foreach (var item in localization)
                {
                    var propertyOrMethodName = item.Item1;
                    var propertyOrMethodValue = item.Item2;
                    localizationClassWriter.WriteWithTabs(2, $"public string {propertyOrMethodName}");
                    var placeholdersCount = GetPlaceholdersCount(propertyOrMethodValue);
                    if (placeholdersCount == 0)
                    {
                        localizationClassWriter.WriteLineEx(" =>")
                            .WriteLineWithTabs(3, $"_currentLocalization.{propertyOrMethodName};");
                    }
                    else
                    {
                        localizationClassWriter.Write("(");
                        for (var placeholder = 0; placeholder < placeholdersCount; placeholder++)
                        {
                            localizationClassWriter.Write($"object value{placeholder}");
                            if (placeholder != placeholdersCount - 1)
                            {
                                localizationClassWriter.Write(", ");
                            }
                        }
                        localizationClassWriter.WriteLineEx(")")
                            .WriteLineWithTabs(2, "{")
                            .WriteLineWithTabs(3, $"return _currentLocalization.{propertyOrMethodName}(");
                        for (var placeholder = 0; placeholder < placeholdersCount; placeholder++)
                        {
                            localizationClassWriter.WriteWithTabs(4, $"value{placeholder}");
                            if (placeholder != placeholdersCount - 1)
                            {
                                localizationClassWriter.WriteLine(",");
                            }
                            else
                            {
                                localizationClassWriter.WriteLine(");");
                            }
                        }
                        localizationClassWriter.WriteLineWithTabs(2, "}");
                    }
                    localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                }
                localizationClassWriter.WriteLineWithTabs(2, "private void NotifyPropertyChanged(params string[] propertiesNames)")
                    .WriteLineWithTabs(2, "{")
                    .WriteLineWithTabs(3, "foreach (var propertyName in propertiesNames)")
                    .WriteLineWithTabs(3, "{")
                    .WriteLineWithTabs(4, "PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));")
                    .WriteLineWithTabs(3, "}")
                    .WriteLineWithTabs(2, "}")
                    .WriteLineWithTabs(1, string.Empty)
                    .WriteLineWithTabs(1, "}")
                    .WriteLineEx()
                    .WriteWithTabs(0, "}");
            }
        }

        private static int GetPlaceholdersCount(string localization)
        {
            int result = 0;
            while (true)
            {
                if (!localization.Contains($"{{{result}}}"))
                {
                    return result;
                }
                result++;
            }
        }

    }

}