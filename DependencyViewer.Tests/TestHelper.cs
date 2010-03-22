using System.IO;

namespace DependencyViewer.Tests
{
    public class TestBase
    {
        protected string GetSolutionFilename(string fileName)
        {
            var path = Path.Combine("Fixtures", Path.Combine("SolutionFiles", fileName));
            return path;
        }

        protected string GetCSProjFile(string fileName)
        {
            var path = Path.Combine("Fixtures", Path.Combine("CSProjFiles", fileName));
            return File.ReadAllText(path);
        }
    }
}