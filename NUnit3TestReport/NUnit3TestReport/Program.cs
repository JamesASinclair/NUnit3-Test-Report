using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NUnit3TestReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                PrintUsage();
                Environment.Exit(1);
            }

            var files = GetFiles(args[0]);

            // Get properties from xml files
            var properties = GetProperties(files);

            // Merge properties with template file

            var template = GetEmbeddedResource("NUnit3TestReport.Template.html");
            var output = template.Replace("##FileCount##", files.Length.ToString());
            File.WriteAllText(args[1], output);
        }

        private static void PrintUsage()
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

        public static List<TemplateProperties> GetProperties(string[] files)
        {
            var properties = new List<TemplateProperties>();

            if (files != null && files.Length > 0)
            {
                foreach (var file in files)
                {
                    var xml = XElement.Load(file);

                    var templateproperties = new TemplateProperties
                    {
                        Assembly = Path.GetFileName(xml.Element("test-suite").Attribute("name").Value),
                        Result = xml.Element("test-suite").Attribute("result").Value,
                        Total = int.Parse(!string.IsNullOrEmpty(xml.Attribute("total").Value) ? xml.Attribute("total").Value : "0"),
                        Passed = int.Parse(!string.IsNullOrEmpty(xml.Attribute("passed").Value) ? xml.Attribute("passed").Value : "0"),
                        Failed = int.Parse(!string.IsNullOrEmpty(xml.Attribute("failed").Value) ? xml.Attribute("failed").Value : "0"),
                        Inconclusive = int.Parse(!string.IsNullOrEmpty(xml.Attribute("inconclusive").Value) ? xml.Attribute("inconclusive").Value : "0"),
                        Skipped = int.Parse(!string.IsNullOrEmpty(xml.Attribute("skipped").Value) ? xml.Attribute("skipped").Value : "0"),
                        Duration = decimal.Parse(!string.IsNullOrEmpty(xml.Attribute("duration").Value) ? xml.Attribute("duration").Value : "0")
                    };

                    properties.Add(templateproperties);
                }
            }

            return properties;
        }
    }

    public class TemplateProperties
    {
        public string Assembly { get; set; }
        public string Result { get; set; }
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public int Inconclusive { get; set; }
        public int Skipped { get; set; }
        public decimal Duration { get; set; }
    }
}
