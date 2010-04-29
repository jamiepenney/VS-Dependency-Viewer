using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using DependencyViewer.Common.Loaders;
using DependencyViewer.Common.Model;
using DependencyViewer.Tests.Model;
using NUnit.Framework;
using Rhino.Mocks;

namespace Specs_For_Project
{
    [TestFixture]
    public class When_The_Project_Is_Initialised_From_A_ProjectLoader
    {
        [Test]
        public void The_Name_Is_Set()
        {
            Assert.That(project.Name, Is.EqualTo("Name"));
        }

        [Test]
        public void The_ProjectIdentifier_Is_Set()
        {
            Assert.That(project.ProjectIdentifier, Is.EqualTo(projectIdentifier));
        }

        [Test]
        public void The_ProjectReferences_Are_Set()
        {
            CollectionAssert.Contains(project.ProjectReferences, projectReference);
        }

        [Test]
        public void The_ReferencedDlls_Are_Set()
        {
            Assert.That(project.ReferencedDlls.Any(a => a.FullName == "mscorlib"));
        }

        [Test]
        public void HasReferencedProject_Returns_True_For_Existing_Reference()
        {
            Assert.That(project.HasReferencedProject(projectReference), Is.True);
        }

        [Test]
        public void HasReferencedProject_Returns_False_For_Existing_Reference()
        {
            Assert.That(project.HasReferencedProject(Guid.Empty), Is.False);
        }

        [Test]
        public void RemoveReferencedProject()
        {
            Assert.That(project.HasReferencedProject(Guid.Empty), Is.False);
        }
        
        [SetUp]
        public void Setup()
        {
            var projectLoader = Helper.GetMockProjectLoader(projectIdentifier, projectReference);

            project = new Project(projectLoader);
        }

        private readonly Guid projectIdentifier = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        private readonly Guid projectReference = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2);
        private Project project;
    }


    [TestFixture]
    public class When_The_Selected_Property_Of_A_Project_Changes
    {
        [Test]
        public void The_PropertyChanged_Event_Is_Raised()
        {
            Assert.That(eventArgs.PropertyName, Is.EqualTo("IsSelected"));
        }

        [SetUp]
        public void Setup()
        {
            eventArgs = null;

            project = new Project("project", Guid.Empty);
            project.PropertyChanged += project_PropertyChanged;
            project.IsSelected = !project.IsSelected;
        }

        void project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            eventArgs = e;   
        }

        private PropertyChangedEventArgs eventArgs;
        private Project project;
    }
}