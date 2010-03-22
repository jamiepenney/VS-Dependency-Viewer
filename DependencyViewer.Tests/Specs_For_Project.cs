using DependencyViewer.Common;
using DependencyViewer.Tests;
using NUnit.Framework;

namespace Specs_For_Project
{
    [TestFixture]
    public class When_Given_A_Project_With_One_DLL_Reference : TestBase
    {
        [Test]
        public void Get_DLL_References_Returns_A_Single_Reference()
        {
            var project = new Project(GetCSProjFile("testproject2.csproj.txt"));
            
            Assert.That(project.ReferencedDLLs, Has.Count.EqualTo(1));
        }
    }
}