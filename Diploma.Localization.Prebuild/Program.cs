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
                localizationInterfaceWriter.WriteLineWithTabs(0, "namespace Diploma.Localization");
                localizationInterfaceWriter.WriteLineWithTabs(0, "{");
                localizationInterfaceWriter.WriteLineWithTabs(1, string.Empty);
                localizationInterfaceWriter.WriteLineWithTabs(1, $"public interface {InterfaceName}");
                localizationInterfaceWriter.WriteLineWithTabs(1, "{");
                localizationInterfaceWriter.WriteLineWithTabs(2, string.Empty);
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
                localizationInterfaceWriter.WriteLineWithTabs(1, "}");
                localizationInterfaceWriter.WriteLine();
                localizationInterfaceWriter.WriteWithTabs(0, "}");
            }
        }

        private static void WriteLocalizationClass(DiplomaLanguages language, (string, string)[] localization, string[] localizationsUnion)
        {
            var className = $"Diploma{LanguagesToStringConverter.Convert(language, false)}Localization";
            var filePath = $"{new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}\\Diploma.Localization\\{className}.cs";
            using (var localizationClassWriter = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                localizationClassWriter.WriteLineWithTabs(0, "using System;");
                localizationClassWriter.WriteLineWithTabs(0, string.Empty);
                localizationClassWriter.WriteLineWithTabs(0, "namespace Diploma.Localization");
                localizationClassWriter.WriteLineWithTabs(0, "{");
                localizationClassWriter.WriteLineWithTabs(1, string.Empty);
                localizationClassWriter.WriteLineWithTabs(1, $"internal sealed class {className} : {InterfaceName}");
                localizationClassWriter.WriteLineWithTabs(1, "{");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, $"private static readonly Lazy<{className}> _instance;");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, $"static {className}()");
                localizationClassWriter.WriteLineWithTabs(2, "{");
                localizationClassWriter.WriteLineWithTabs(3, $"_instance = new Lazy<{className}>(() => new {className}());");
                localizationClassWriter.WriteLineWithTabs(2, "}");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, $"public static {className} Instance =>");
                localizationClassWriter.WriteLineWithTabs(3, "_instance.Value;");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, $"private {className}()");
                localizationClassWriter.WriteLineWithTabs(2, "{");
                localizationClassWriter.WriteLineWithTabs(3, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "}");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
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
                        localizationClassWriter.WriteLine(" =>");
                        localizationClassWriter.WriteLineWithTabs(3, $"{fieldName};");
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
                        localizationClassWriter.WriteLine(")");
                        localizationClassWriter.WriteLineWithTabs(2, "{");
                        localizationClassWriter.WriteLineWithTabs(3, $"return string.Format({fieldName}");
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
                localizationClassWriter.WriteLineWithTabs(1, "}");
                localizationClassWriter.WriteLine();
                localizationClassWriter.WriteWithTabs(0, "}");
            }
        }

        private static void WriteCommonLocalizationClass((string, string)[] localization)
        {
            var filePath = $"{new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.FullName}" +
                $"\\Diploma.Localization\\DiplomaLocalization.cs";
            using (var localizationClassWriter = new StreamWriter(new FileStream(filePath, FileMode.Create)))
            {
                localizationClassWriter.WriteLineWithTabs(0, "using System;");
                localizationClassWriter.WriteLineWithTabs(0, "using System.ComponentModel;");
                localizationClassWriter.WriteLineWithTabs(0, "using System.Linq;");
                localizationClassWriter.WriteLineWithTabs(0, string.Empty);
                localizationClassWriter.WriteLineWithTabs(0, "using Diploma.Localization.Languages;");
                localizationClassWriter.WriteLineWithTabs(0, string.Empty);
                localizationClassWriter.WriteLineWithTabs(0, "namespace Diploma.Localization");
                localizationClassWriter.WriteLineWithTabs(0, "{");
                localizationClassWriter.WriteLineWithTabs(1, string.Empty);
                localizationClassWriter.WriteLineWithTabs(1, $"public sealed class DiplomaLocalization : {InterfaceName}, INotifyPropertyChanged");
                localizationClassWriter.WriteLineWithTabs(1, "{");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "private static readonly Lazy<DiplomaLocalization> _instance;");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "static DiplomaLocalization()");
                localizationClassWriter.WriteLineWithTabs(2, "{");
                localizationClassWriter.WriteLineWithTabs(3, "_instance = new Lazy<DiplomaLocalization>(() => new DiplomaLocalization());");
                localizationClassWriter.WriteLineWithTabs(2, "}");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "public static DiplomaLocalization Instance =>");
                localizationClassWriter.WriteLineWithTabs(3, "_instance.Value;");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "private readonly string[] _localizationPropertiesNames;");
                localizationClassWriter.WriteLineWithTabs(2, "private DiplomaLanguages _currentLanguage;");
                localizationClassWriter.WriteLineWithTabs(2, "private IDiplomaLocalization _currentLocalization;");
                localizationClassWriter.WriteLineWithTabs(2, "public event PropertyChangedEventHandler PropertyChanged;");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "private DiplomaLocalization()");
                localizationClassWriter.WriteLineWithTabs(2, "{");
                localizationClassWriter.WriteLineWithTabs(3, "_localizationPropertiesNames = typeof(IDiplomaLocalization)");
                localizationClassWriter.WriteLineWithTabs(4, ".GetProperties()");
                localizationClassWriter.WriteLineWithTabs(4, ".Select(property => property.Name)");
                localizationClassWriter.WriteLineWithTabs(4, ".ToArray();");
                localizationClassWriter.WriteLineWithTabs(3, "CurrentLanguage = DiplomaLanguages.Russian;");
                localizationClassWriter.WriteLineWithTabs(2, "}");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                localizationClassWriter.WriteLineWithTabs(2, "public DiplomaLanguages CurrentLanguage");
                localizationClassWriter.WriteLineWithTabs(2, "{");
                localizationClassWriter.WriteLineWithTabs(3, "get =>");
                localizationClassWriter.WriteLineWithTabs(4, "_currentLanguage;");
                localizationClassWriter.WriteLineWithTabs(3, string.Empty);
                localizationClassWriter.WriteLineWithTabs(3, "set");
                localizationClassWriter.WriteLineWithTabs(3, "{");
                localizationClassWriter.WriteLineWithTabs(4, "if (CurrentLanguage == value)");
                localizationClassWriter.WriteLineWithTabs(4, "{");
                localizationClassWriter.WriteLineWithTabs(5, "return;");
                localizationClassWriter.WriteLineWithTabs(4, "}");
                localizationClassWriter.WriteLineWithTabs(4, "if (!Enum.IsDefined(typeof(DiplomaLanguages), value))");
                localizationClassWriter.WriteLineWithTabs(4, "{");
                localizationClassWriter.WriteLineWithTabs(5, "return;");
                localizationClassWriter.WriteLineWithTabs(4, "}");
                localizationClassWriter.WriteLineWithTabs(4, "_currentLanguage = value;");
                localizationClassWriter.WriteLineWithTabs(4, "switch (CurrentLanguage)");
                localizationClassWriter.WriteLineWithTabs(4, "{");
                foreach (DiplomaLanguages language in Enum.GetValues(typeof(DiplomaLanguages)))
                {
                    localizationClassWriter.WriteLineWithTabs(5, $"case DiplomaLanguages.{LanguagesToStringConverter.Convert(language)}:");
                    localizationClassWriter.WriteLineWithTabs(6, $"_currentLocalization = Diploma{LanguagesToStringConverter.Convert(language, false)}Localization.Instance;");
                    localizationClassWriter.WriteLineWithTabs(6, "break;");
                }
                localizationClassWriter.WriteLineWithTabs(4, "}");
                localizationClassWriter.WriteLineWithTabs(4, "NotifyPropertyChanged(_localizationPropertiesNames);");
                localizationClassWriter.WriteLineWithTabs(3, "}");
                localizationClassWriter.WriteLineWithTabs(2, "}");
                localizationClassWriter.WriteLineWithTabs(2, string.Empty);
                foreach (var item in localization)
                {
                    var propertyOrMethodName = item.Item1;
                    var propertyOrMethodValue = item.Item2;
                    localizationClassWriter.WriteWithTabs(2, $"public string {propertyOrMethodName}");
                    var placeholdersCount = GetPlaceholdersCount(propertyOrMethodValue);
                    if (placeholdersCount == 0)
                    {
                        localizationClassWriter.WriteLine(" =>");
                        localizationClassWriter.WriteLineWithTabs(3, $"_currentLocalization.{propertyOrMethodName};");
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
                        localizationClassWriter.WriteLine(")");
                        localizationClassWriter.WriteLineWithTabs(2, "{");
                        localizationClassWriter.WriteLineWithTabs(3, $"return _currentLocalization.{propertyOrMethodName}(");
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
                localizationClassWriter.WriteLineWithTabs(2, "private void NotifyPropertyChanged(params string[] propertiesNames)");
                localizationClassWriter.WriteLineWithTabs(2, "{");
                localizationClassWriter.WriteLineWithTabs(3, "foreach (var propertyName in propertiesNames)");
                localizationClassWriter.WriteLineWithTabs(3, "{");
                localizationClassWriter.WriteLineWithTabs(4, "PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));");
                localizationClassWriter.WriteLineWithTabs(3, "}");
                localizationClassWriter.WriteLineWithTabs(2, "}");
                localizationClassWriter.WriteLineWithTabs(1, string.Empty);
                localizationClassWriter.WriteLineWithTabs(1, "}");
                localizationClassWriter.WriteLine();
                localizationClassWriter.WriteWithTabs(0, "}");
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