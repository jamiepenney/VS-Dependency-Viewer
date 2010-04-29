using System;
using System.Linq;
using DependencyViewer.Common.Loaders;
using DependencyViewer.Common.Model;
using DependencyViewer.Tests.Model;
using NUnit.Framework;
using Rhino.Mocks;

namespace Specs_For_Solution
{
    public class When_The_Solution_Is_Initialised_From_A_SolutionLoader
    {
        [Test]
        public void The_SolutionName_Is_Set()
        {
            Assert.That(solution.SolutionName, Is.EqualTo("Solution"));
        }

        [Test]
        public void The_Projects_Are_Set()
        {
            Assert.That(solution.Projects.Any(p => p.ProjectIdentifier == projectIdentifier), Is.True);
        }

        [Test]
        public void GetProject_Returns_A_Project_For_A_ProjectID_That_Exists()
        {
            Assert.That(solution.GetProject(projectIdentifier), Is.Not.Null);
        }

        [Test]
        public void GetProject_Returns_Null_For_A_ProjectID_That_Does_Not_Exist()
        {
            Assert.That(solution.GetProject(Guid.Empty), Is.Null);
        }

        [Test]
        public void HasProject_Returns_True_For_A_Project_That_Exists()
        {
            Assert.That(solution.HasProject(projectIdentifier), Is.True);
        }

        [Test]
        public void HasProject_Returns_False_For_A_Project_That_Does_Not_Exist()
        {
            Assert.That(solution.HasProject(Guid.Empty), Is.False);
        }

        [SetUp]
        public void Setup()
        {
            var solutionLoader = MockRepository.GenerateStub<ISolution>();
            project = Helper.GetMockProjectLoader(projectIdentifier, projectReference);

            solutionLoader.Stub(s => s.SolutionName).Return("Solution");
            solutionLoader.Stub(s => s.Projects).Return(new []{project});

            solution = new Solution(solutionLoader);
        }

        private Solution solution;
        private IProject project;
        private readonly Guid projectIdentifier = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        private readonly Guid projectReference = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);
    }
}