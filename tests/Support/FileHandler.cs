using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Tests.Support
{
    public static class FileHandler
    {
        private static string _projectDirectory =
            Path.GetFullPath($"{Directory.GetParent(Environment.CurrentDirectory).Parent.FullName}../../../");

        public static JObject ReadFileAsDictionary(string resourceToBeRead)
        {
            var whereTheFileIs = $"{_projectDirectory}/{resourceToBeRead}";
            var text = File.ReadAllText(whereTheFileIs);
            return JObject.Parse(text);
        }

        public static IEnumerable<string> EnumerableFromFile(string resourceToBeRead)
        {
            var whereTheFileIs = $"{_projectDirectory}/{resourceToBeRead}";
            using StreamReader reader = new(whereTheFileIs);
            string line;

            while ((line = reader.ReadLine()) is not null)
            {
                yield return line;
            }
        }
    }
}