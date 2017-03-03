using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        }
    }
}
