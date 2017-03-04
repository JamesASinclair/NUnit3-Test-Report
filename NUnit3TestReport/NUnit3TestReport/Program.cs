using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NUnit3TestReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                var info = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

                Console.WriteLine($@"{info.InternalName}
Version {info.ProductVersion}
{info.LegalCopyright}

{info.Comments}

Usage:
    {info.InternalName} [input-path-with-wildcards] [output-path]
Examples:
    Single input file: {info.InternalName} TestResults.xml TestResult.html
    Folder:            {info.InternalName} C:\Folder\* TestResult.html
    File pattern:      {info.InternalName} C:\Folder\*.xml TestResult.html");
                Environment.Exit(1);
            }

            var files = GetFiles(args[0]);


            var template = GetEmbeddedResource("NUnit3TestReport.Template.html");
            var output = template.Replace("##FileCount##", files.Length.ToString());
            File.WriteAllText(args[1], output);
        }

        public static string[] GetFiles(string filePattern)
        {
            var folder = Path.GetDirectoryName(filePattern);
            var searchPattern = Path.GetFileName(filePattern);
            return Directory.GetFiles(folder, searchPattern);
        }

        public static string GetEmbeddedResource(string fullyQualifiedName)
        {
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(fullyQualifiedName)))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
