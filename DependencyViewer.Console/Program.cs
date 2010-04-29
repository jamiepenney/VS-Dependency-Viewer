using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using DependencyViewer.Common;
using DependencyViewer.Common.Loaders;
using DependencyViewer.Common.Model;
using DependencyViewer.Common.Rules;
using Mono.Options;

namespace DependencyViewer.Console
{
    class Program
    {
        static string _solutionName = "";
        static readonly List<string> AssemblyNames = new List<string>();
        static readonly List<string> AssemblyFolders = new List<string>();
        static bool _showHelp;

        static int Main(string[] args)
        {
            var p = ParseArgs(args);
            if (p == null) return 0;

            if (_showHelp)
            {
                ShowHelp(p);
                return 0;
            }

            if (AssemblyNames.Count + AssemblyFolders.Count == 0)
            {
                System.Console.WriteLine("No rules to run. Add an assembly or folder to search using -a or -f");
                return 0;
            }

            return Run();
        }

        private static int Run()
        {
            var solution = SetupSolution();
            var runner = SetupRunner();

            var results = runner.RunRules(solution);

            return results.AllRulesPassed ? 0 : 1;
        }

        private static Solution SetupSolution()
        {
            var solutionLoader = new SolutionLoader(File.ReadAllText(_solutionName), _solutionName);
            return new Solution(solutionLoader);
        }

        private static RuleRunner SetupRunner()
        {
            var runner = new RuleRunner();
            Compose(runner);
            return runner;
        }

        static void Compose(RuleRunner runner)
        {
            var catalog = new AggregateCatalog();

            foreach(var directory in AssemblyFolders)
            {
                catalog.Catalogs.Add(new DirectoryCatalog(directory));
            }

            foreach(var filename in AssemblyNames)
            {
                var assembly = Assembly.LoadFile(filename);
                catalog.Catalogs.Add(new AssemblyCatalog(assembly));
            }

            var container = new CompositionContainer(catalog);
            container.ComposeParts(runner);
        }

        private static OptionSet ParseArgs(IEnumerable<string> args)
        {
            var p = new OptionSet
                        {
                            {
                                "sln=", "the solution file to process",
                                v =>
                                    {
                                        if(v == null) throw new OptionException("Missing solution file", "sln");
                                        if(File.Exists(v) == false) throw new OptionException("Solution file does not exist", "sln");
                                        _solutionName = v;
                                    }
                                },
                            {
                                "a|assembly",
                                "the path to an assembly that contains a solution processor",
                                v => AssemblyNames.Add(v)
                                },
                            {
                                "f|folder", "the path to a folder that contains solution processor assemblies",
                                v => AssemblyFolders.Add(v)
                                },
                            {
                                "h|help", "show this message and exit",
                                v => _showHelp = v != null
                                },
                        };

            try
            {
                p.Parse(args);
            }
            catch(OptionException e)
            {
                System.Console.WriteLine("DependencyViewer.Console:");
                System.Console.WriteLine(e.Message);
                return null;
            }

            return p;
        }

        static void ShowHelp(OptionSet p)
        {
            System.Console.WriteLine("Usage: DependencyViewer.Console.exe [OPTIONS]+");
            System.Console.WriteLine("Runs a set of rules against a Visual Studio Solution.");
            System.Console.WriteLine("Returns: 0 if all rules passed, otherwise 1");
            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }
    }
}
