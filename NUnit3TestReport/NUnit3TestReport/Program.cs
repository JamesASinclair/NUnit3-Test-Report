using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Mono.Options;

namespace NUnit3TestReport
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var show_help = false;
            string inputFilePattern = null;
            string outputFile = null;

            var p = new OptionSet()
            {
                {"f=", "The input file pattern", v => inputFilePattern = v},
                {"o=", "The output file", v => outputFile = v},
                { "?|h|help", "Show help", v => show_help = v != null },
            };

            try
            {
                p.Parse(args);
            }
            catch (OptionException e)
            {
                PrintUsage(e.Message);
                Environment.Exit(1);
            }

            if (show_help)
            {
                PrintUsage();
                Environment.Exit(0);
            }

            if (inputFilePattern == null)
            {
                PrintUsage("Missing required option -f.");
                Environment.Exit(1);
            }

            if (outputFile == null)
            {
                PrintUsage("Missing required option -o.");
                Environment.Exit(1);
            }

            var testRuns = new FileParser().Parse(inputFilePattern);
            var output = new TestReport().Build(testRuns);

            //var output = new ConsoleOutputReport().Build(testRuns);
            File.WriteAllText(outputFile, output);
        }

        private static void PrintUsage(string errorMessage = null)
        {
            var info = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

            Console.WriteLine($@"{info.InternalName}
Version {info.ProductVersion}
{info.LegalCopyright}

{info.Comments}

Usage:
    {info.InternalName} -f [input-path-with-wildcards] -o [output-path]
Examples:
    Single input file: {info.InternalName} -f TestResults.xml -o TestResult.html
    Folder:            {info.InternalName} -f C:\Folder\* -o TestResult.html
    File pattern:      {info.InternalName} -f C:\Folder\*.xml -o TestResult.html");

            if (errorMessage != null)
            {
                Console.WriteLine($@"Error:
    {errorMessage}");
            }
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
