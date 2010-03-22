using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DependencyViewer.Common
{
	public class Solution
	{
		private readonly TextReader reader;
		private readonly string solutionFilename;
		private readonly Dictionary<Guid, Project> projects;
		
		public string SolutionName
		{
			get { return Path.GetFileName(solutionFilename); }
		}

		public List<Project> Projects { get { return projects.Values.ToList(); } }

		public Solution(string solutionText, string solutionFilename)
		{
			reader = new StringReader(solutionText);
			this.solutionFilename = solutionFilename;
			projects = new Dictionary<Guid, Project>();
		}

		public void LoadProjects()
		{
			string filebase = Path.GetDirectoryName(solutionFilename);

			string line = reader.ReadLine();

			while(line != null)
			{
				if (line.StartsWith("Project") && line.Contains(".csproj"))
				{
					Project project = CreateProjectLoaderFromProjectLine(line, filebase);
					projects[project.ProjectIdentifier] = project;

					line = reader.ReadLine();
				}
				else
				{
					line = reader.ReadLine();
					continue;
				}
			}
		}

		private static Project CreateProjectLoaderFromProjectLine(string line, string filebase)
		{
			string[] chunks = line.Split('"');

			// chunk 6 holds the project filename
			string projectFilename = chunks[5];
			// chunk 8 holds the project guid
			//string projectGuid = chunks[7];

			return new Project(File.ReadAllText(Path.Combine(filebase, projectFilename)));
		}

		public Project GetProject(Guid guid)
		{
			return projects[guid];
		}
	}
}