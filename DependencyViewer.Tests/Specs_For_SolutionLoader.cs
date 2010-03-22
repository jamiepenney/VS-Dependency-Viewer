using System;
using System.IO;
using DependencyViewer.Common;
using DependencyViewer.Tests;
using NUnit.Framework;

namespace Specs_For_SolutionLoader
{
    internal static class Helper
    {
        internal static Solution GetLoader(string filename)
        {
            return new Solution(File.ReadAllText(filename), filename);
        }
    }

	[TestFixture]
	public class When_Given_A_Solution_With_A_Single_Project : TestBase
	{
		[Test]
		public void Returns_A_Single_ProjectLoader()
		{
            var filename = GetSolutionFilename("1proj_solution.txt");
		    Solution loader = Helper.GetLoader(filename);
			loader.LoadProjects();

			Assert.That(loader.Projects, Has.Count.EqualTo(1));
			Assert.That(loader.Projects[0].ProjectIdentifier, Is.EqualTo(new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF}")));
			Assert.That(loader.Projects[0].ProjectReferences, Is.Empty);
		}
	}

	[TestFixture]
	public class When_Given_A_Solution_With_Two_Projects : TestBase
	{
		[Test]
		public void Returns_Both_ProjectLoaders()
		{
            var filename = GetSolutionFilename("2proj_solution.txt");
            Solution loader = Helper.GetLoader(filename);
			loader.LoadProjects();

			Assert.That(loader.Projects, Has.Count.EqualTo(2));
			Assert.That(loader.Projects[0].ProjectIdentifier, Is.EqualTo(new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF}")));
			Assert.That(loader.Projects[0].ProjectReferences, Is.Empty);

			Assert.That(loader.Projects[1].ProjectIdentifier, Is.EqualTo(new Guid("{AAAAAAAA-1FC6-4905-9426-AED7F6FDB6AE}")));
			Assert.That(loader.Projects[1].ProjectReferences, Has.Count.EqualTo(1));
			Assert.That(loader.Projects[1].ProjectReferences[0], Is.EqualTo(loader.Projects[0].ProjectIdentifier));
		}
	}
}
