using System.IO;
using System.Linq;

namespace Diploma.Extensions
{

    public static class StreamWriterEx
    {

        public static void WriteWithTabs(this StreamWriter streamWriter, int tabsCount, string toWrite)
        {
            streamWriter.Write($"{string.Concat(Enumerable.Repeat("\t", tabsCount))}{toWrite}");
        }

        public static void WriteLineWithTabs(this StreamWriter streamWriter, int tabsCount, string toWrite)
        {
            streamWriter.WriteLine($"{string.Concat(Enumerable.Repeat("\t", tabsCount))}{toWrite}");
        }

    }

}