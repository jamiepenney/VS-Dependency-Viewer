using System;
using System.Linq;
using System.Xml;
using DependencyViewer.Common.Loaders;
using DependencyViewer.Tests;
using NUnit.Framework;

namespace Specs_For_ProjectLoader
{
    [TestFixture]
    public class When_Given_A_Project_With_One_DLL_Reference : TestBase
    {
        [Test]
        public void Get_DLL_References_Returns_A_Single_Reference()
        {
            Assert.That(project.ReferencedDlls, Has.Count.EqualTo(1));
        }

        [Test]
        public void Get_ProjectReferences_Contains_The_Referenced_Project()
        {
            Assert.That(project.ReferencedDlls.Any(asm => asm.Name.Contains("Microsoft.Xna.Framework")));
        }

        [SetUp]
        public void Setup()
        {
            project = new ProjectLoader(GetCSProjFile("testproject2.csproj.txt"));
        }

        private ProjectLoader project;
    }

    [TestFixture]
    public class When_Given_A_Project_With_One_Referenced_Project : TestBase
    {
        [Test]
        public void Get_ProjectReferences_Returns_A_Single_Reference()
        {
            Assert.That(project.ProjectReferences, Has.Count.EqualTo(1));
        }

        [Test]
        public void Get_ProjectReferences_Contains_The_Right_Project()
        {
            Assert.That(project.ProjectReferences.Any(pr => pr == new Guid("DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF")));
        }

        [Test]
        public void HasReferencedProject_Returns_True()
        {
            Assert.That(project.HasReferencedProject(new Guid("DA8BD6BF-1FC6-4905-9426-AED7F6FDB6AF")));
        }

        [SetUp]
        public void Setup()
        {
            project = new ProjectLoader(GetCSProjFile("testproject2.csproj.txt"));
        }

        private ProjectLoader project;
    }

    [TestFixture]
    public class When_Given_A_Valid_Project : TestBase
    {
        [Test]
        public void Name_Returns_The_Name_From_The_CSProj()
        {
            Assert.That(project.Name, Is.EqualTo("WindowsGame2"));
        }

        [SetUp]
        public void Setup()
        {
            project = new ProjectLoader(GetCSProjFile("testproject2.csproj.txt"));
        }

        private ProjectLoader project;
    }

    [TestFixture]
    public class When_Given_Project_File_Text_That_Does_Not_Parse : TestBase
    {
        [Test]
        [ExpectedException(typeof(LoaderException))]
        public void The_Constructor_Throws_An_Invalid_Project_Exception()
        {
            new ProjectLoader("<bad>this is not xml");
        }

        [Test]
        public void The_Exception_Has_The_Xml_Exception_As_Its_Inner_Exception()
        {
            try
            {
                new ProjectLoader("<bad>this is not xml");
                Assert.Fail();
            }
            catch(LoaderException e)
            {
                Assert.That(e.InnerException, Is.InstanceOf(typeof(XmlException)));
            }
        }
    }
}