using System;
using System.IO;
using System.Linq;
using DependencyViewer.Common.Loaders;
using DependencyViewer.Tests;
using NUnit.Framework;

namespace Specs_For_SolutionLoader
{
	[TestFixture]
    public class When_Given_A_Solution_With_A_Single_Project_Called_WindowsGame1 : TestBase
	{
		[Test]
		public void Projects_Contains_A_Single_Project()
		{
			Assert.That(loader.Projects, Has.Count.EqualTo(1));
		}

        [Test]
        public void Projects_Contains_The_WindowsGame1_Project_Guid()
        {
            Assert.That(loader.Projects.Any(p => p.ProjectIdentifier == projectGuid));
        }

        [Test]
        public void GetProject_Returns_The_Correct_ProjectLoader()
        {
            Assert.That(loader.GetProject(projectGuid).Name, Is.EqualTo("WindowsGame1"));
        }

        [SetUp]
        public void Setup()
        {
            var filename = GetSolutionFilename("1proj_solution.txt");
            loader = Helper.GetLoader(filename);
            loader.LoadProjects();
        }

        private readonly Guid projectGuid = new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF}");
	    private SolutionLoader loader;
	}

	[TestFixture]
	public class When_Given_A_Solution_With_Two_Projects_Named_WindowsGame1_And_WindowsGame2 : TestBase
	{
		[Test]
		public void Projects_Contains_2_Projects()
		{
			Assert.That(loader.Projects, Has.Count.EqualTo(2));
		}

        [Test]
        public void Projects_Contains_WindowsGame1()
        {
            Assert.That(loader.Projects.Any(p => p.ProjectIdentifier == project1Guid));
        }

        [Test]
        public void Projects_Contains_WindowsGame2()
        {
            Assert.That(loader.Projects.Any(p => p.ProjectIdentifier == project2Guid));
        }

        [Test]
        public void GetProject_Returns_The_Correct_ProjectLoader_When_Passed_WindowsGame1s_Guid()
        {
            Assert.That(loader.GetProject(project1Guid).Name, Is.EqualTo("WindowsGame1"));
        }

        [Test]
        public void GetProject_Returns_The_Correct_ProjectLoader_When_Passed_WindowsGame2s_Guid()
        {
            Assert.That(loader.GetProject(project2Guid).Name, Is.EqualTo("WindowsGame2"));
        }

        [SetUp]
        public void Setup()
        {
            var filename = GetSolutionFilename("2proj_solution.txt");
            loader = Helper.GetLoader(filename);
            loader.LoadProjects();
        }

	    private SolutionLoader loader;

	    private readonly Guid project1Guid = new Guid("{DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF}");
	    private readonly Guid project2Guid = new Guid("{AAAAAAAA-1FC6-4905-9426-AED7F6FDB6AE}");
	}

    [TestFixture]
    public class When_Given_A_Valid_Solution : TestBase
    {
        [Test]
        public void SolutionName_Returns_The_Filename()
        {
            Assert.That(loader.SolutionName, Is.EqualTo("1proj_solution.txt"));
        }

        [SetUp]
        public void Setup()
        {
            var filename = GetSolutionFilename("1proj_solution.txt");
            loader = Helper.GetLoader(filename);
            loader.LoadProjects();
        }

        private SolutionLoader loader;
    }

    [TestFixture]
    public class When_Given_Invalid_Solution_Text : TestBase
    {
        [Test]
        [ExpectedException(typeof(LoaderException))]
        public void LoadProjects_Throws_A_LoaderException()
        {
            loader.LoadProjects();
        }

        [Test]
        public void The_Exception_Has_A_Message()
        {
            try
            {
                loader.LoadProjects();
                Assert.Fail("Exception not thrown");
            }
            catch (LoaderException e)
            {
                Assert.That(e.Message, Is.Not.Null.Or.Empty);
            }
        }

        [SetUp]
        public void Setup()
        {
            var filename = GetSolutionFilename("1proj_solution.txt");
            loader = new SolutionLoader("invalid text but valid filename", filename);
        }

        private SolutionLoader loader;
    }

    [TestFixture]
    public class When_Given_A_Solution_With_A_Non_Existant_Project : TestBase
    {
        [Test]
        [ExpectedException(typeof(LoaderException))]
        public void LoadProjects_Throws_A_LoaderException()
        {
            loader.LoadProjects();
        }

        [Test]
        public void The_Exception_Message_Contains_The_Filename()
        {
            try
            {
                loader.LoadProjects();
                Assert.Fail("Exception not thrown");
            }
            catch (LoaderException e)
            {
                StringAssert.Contains("ProjectThatDoesNotExist", e.Message);
            }
        }

        [SetUp]
        public void Setup()
        {
            var filename = GetSolutionFilename("solution_bad_proj_filename.txt");
            loader = Helper.GetLoader(filename);
        }

        private SolutionLoader loader;
    }

    [TestFixture]
    public class When_Given_A_Solution_With_A_Non_CSProj_Project : TestBase
    {
        [Test]
        public void It_Ignores_That_Project()
        {
            Assert.That(loader.Projects.Any(p => p.Name == "TestProject"), Is.False);
        }

        [SetUp]
        public void Setup()
        {
            var filename = GetSolutionFilename("solution_non_csproj_project.txt");
            loader = Helper.GetLoader(filename);
            loader.LoadProjects();
        }

        private SolutionLoader loader;
    }

    internal static class Helper
    {
        internal static SolutionLoader GetLoader(string filename)
        {
            return new SolutionLoader(File.ReadAllText(filename), filename);
        }
    }
}
