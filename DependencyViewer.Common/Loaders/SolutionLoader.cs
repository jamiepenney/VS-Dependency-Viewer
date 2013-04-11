using System;
using System.Collections.Generic;
using System.IO;

namespace DependencyViewer.Common.Loaders
{
    public interface ISolution
    {
        string SolutionName { get; }
        IEnumerable<IProject> Projects { get; }
        void LoadProjects();
        IProject GetProject(Guid guid);
    }

    public class SolutionLoader : ISolution
    {
        private readonly string _solutionText;
        private readonly string _solutionFilename;
        private readonly Dictionary<Guid, ProjectLoader> _projects;
		
        public string SolutionName
        {
            get { return Path.GetFileName(_solutionFilename); }
        }

        public IEnumerable<IProject> Projects { get { return _projects.Values; } }

        public SolutionLoader(string solutionText, string solutionFilename)
        {
            _solutionText = solutionText;
            _solutionFilename = solutionFilename;
            _projects = new Dictionary<Guid, ProjectLoader>();
        }

        public void LoadProjects()
        {
            string filebase = Path.GetDirectoryName(_solutionFilename);

            var reader = new StringReader(_solutionText);
            string line = reader.ReadLine();

            while(line != null)
            {
                if(string.IsNullOrEmpty(line.Trim()))
                {
                    line = reader.ReadLine(); 
                    continue;
                }
                if (line.StartsWith("Microsoft Visual Studio Solution File") == false)
                {
                    throw new LoaderException(
                        "Solution file should start with \"Microsoft Visual Studio Solution File\"");
                }

                line = reader.ReadLine();
                break;
            }

            while(line != null)    
            {
                if (line.StartsWith("Project") && (line.Contains(".csproj") || line.Contains(".vcxproj")))
                {
                    var projectLoader = CreateProjectLoaderFromProjectLine(line, filebase);
                    _projects[projectLoader.ProjectIdentifier] = projectLoader;

                    line = reader.ReadLine();
                }
                else
                {
                    line = reader.ReadLine();
                    continue;
                }
            }
        }

        private static ProjectLoader CreateProjectLoaderFromProjectLine(string line, string filebase)
        {
            var chunks = line.Split('"');

            // chunk 6 holds the project filename
            string projectFilename = chunks[5];
            // chunk 8 holds the project guid
            //string projectGuid = chunks[7];

            var projectFullPath = Path.Combine(filebase, projectFilename);

            if (File.Exists(projectFullPath) == false)
                throw new LoaderException("Could not find referenced project at " + projectFullPath);
            
            return new ProjectLoader(File.ReadAllText(projectFullPath), projectFullPath);
        }

        public IProject GetProject(Guid guid)
        {
            return _projects[guid];
        }
    }
}