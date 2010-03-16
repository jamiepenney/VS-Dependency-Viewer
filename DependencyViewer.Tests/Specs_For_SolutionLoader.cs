using System;
using System.IO;
using DependencyViewer.Common;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace DependencyViewer.Tests
{
	[TestFixture]
	public class When_Given_A_Solution_With_A_Single_Project
	{
		[Test]
		public void Returns_A_Single_ProjectLoader()
		{
			var loader = new Solution(File.ReadAllText("1proj_solution.txt"), "1proj_solution.txt");
			loader.LoadProjects();

			Assert.That(loader.Projects, Has.Count(1));
			Assert.That(loader.Projects[0].ProjectIdentifier(), Is.EqualTo(new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF}")));
			Assert.That(loader.Projects[0].ProjectReferences(), Is.Empty);
		}
	}

	[TestFixture]
	public class When_Given_A_Solution_With_Two_Projects
	{
		[Test]
		public void Returns_Both_ProjectLoaders()
		{
			var loader = new Solution(File.ReadAllText("2proj_solution.txt"), "2proj_solution.txt");
			loader.LoadProjects();

			Assert.That(loader.Projects, Has.Count(2));
			Assert.That(loader.Projects[0].ProjectIdentifier(), Is.EqualTo(new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF}")));
			Assert.That(loader.Projects[0].ProjectReferences(), Is.Empty);

			Assert.That(loader.Projects[1].ProjectIdentifier(), Is.EqualTo(new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AE}")));
			Assert.That(loader.Projects[1].ProjectReferences(), Has.Count(1));
			Assert.That(loader.Projects[1].ProjectReferences()[0], Is.EqualTo(loader.Projects[0].ProjectIdentifier()));
		}
	}
}
