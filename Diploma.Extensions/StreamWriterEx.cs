using System.IO;
using System.Linq;

namespace Diploma.Extensions
{

    public static class StreamWriterEx
    {

        public static StreamWriter WriteEx(this StreamWriter streamWriter, string toWrite = "")
        {
            streamWriter.Write(toWrite);
            return streamWriter;
        }

        public static StreamWriter WriteLineEx(this StreamWriter streamWriter, string toWrite = "")
        {
            streamWriter.WriteLine(toWrite);
            return streamWriter;
        }

        public static StreamWriter WriteWithTabs(this StreamWriter streamWriter, int tabsCount, string toWrite)
        {
            streamWriter.Write($"{string.Concat(Enumerable.Repeat("\t", tabsCount))}{toWrite}");
            return streamWriter;
        }

        public static StreamWriter WriteLineWithTabs(this StreamWriter streamWriter, int tabsCount, string toWrite)
        {
            streamWriter.WriteLine($"{string.Concat(Enumerable.Repeat("\t", tabsCount))}{toWrite}");
            return streamWriter;
        }

    }

}