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
        public enum ReportType { TestReport, ConsoleReport, TimingReport};

        public static void Main(string[] args)
        {
            var showHelp = false;
            string inputFilePattern = null;
            string outputFile = null;
            var reportType = ReportType.TestReport;

            var optionSet = new OptionSet()
            {
                {"f=", "The input file pattern", v => inputFilePattern = v},
                {"o=", "The output file", v => outputFile = v},
                {"testreport|t", "Generate the test report", v => reportType = ReportType.TestReport },
                {"consolereport|c", "Generate Console Report", v => reportType = ReportType.ConsoleReport },
                {"timingreport|d", "Generate Timing Report", v => reportType = ReportType.TimingReport },
                { "?|h|help", "Show help", v => showHelp = v != null },
            };

            try
            {
                optionSet.Parse(args);
            }
            catch (OptionException e)
            {
                PrintUsage(e.Message);
                Environment.Exit(1);
            }

            if (showHelp)
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

            IReportBuilder reportBuilder;
            switch (reportType)
            {
                case ReportType.TestReport:
                    reportBuilder = new TestReport();
                    break;
                case ReportType.ConsoleReport:
                    reportBuilder = new ConsoleOutputReport();
                    break;
                case ReportType.TimingReport:
                    reportBuilder = new TimingReport();
                    break;
                default:
                    reportBuilder = new TestReport();
                    break;
            }

            var output = reportBuilder.Build(testRuns);
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
    {info.InternalName} -f <input-path-with-wildcards> -o <output-path> [-testreport|-consolereport|-timingreport]
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
