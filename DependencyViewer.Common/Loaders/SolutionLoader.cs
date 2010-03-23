using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DependencyViewer.Common.Loaders
{
    public class SolutionLoader
    {
        private readonly TextReader reader;
        private readonly string solutionFilename;
        private readonly Dictionary<Guid, ProjectLoader> projects;
		
        public string SolutionName
        {
            get { return Path.GetFileName(solutionFilename); }
        }

        public List<ProjectLoader> Projects { get { return projects.Values.ToList(); } }

        public SolutionLoader(string solutionText, string solutionFilename)
        {
            reader = new StringReader(solutionText);
            this.solutionFilename = solutionFilename;
            projects = new Dictionary<Guid, ProjectLoader>();
        }

        public void LoadProjects()
        {
            string filebase = Path.GetDirectoryName(solutionFilename);

            string line = reader.ReadLine();

            while(line != null)
            {
                if (line.StartsWith("Project") && line.Contains(".csproj"))
                {
                    ProjectLoader projectLoader = CreateProjectLoaderFromProjectLine(line, filebase);
                    projects[projectLoader.ProjectIdentifier] = projectLoader;

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
            string[] chunks = line.Split('"');

            // chunk 6 holds the project filename
            string projectFilename = chunks[5];
            // chunk 8 holds the project guid
            //string projectGuid = chunks[7];

            return new ProjectLoader(File.ReadAllText(Path.Combine(filebase, projectFilename)));
        }

        public ProjectLoader GetProject(Guid guid)
        {
            return projects[guid];
        }
    }
}