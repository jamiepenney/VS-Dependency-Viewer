using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DependencyViewer
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
					ProjectLoader project = ProjectProjectLine(line, filebase);
					projects[project.ProjectIdentifier()] = project;

					line = reader.ReadLine();
				}
				else
				{
					line = reader.ReadLine();
					continue;
				}
			}
		}

		private static ProjectLoader ProjectProjectLine(string line, string filebase)
		{
			string[] chunks = line.Split('"');

			// chunk 6 holds the project filename
			string projectFilename = chunks[5];
			// chunk 8 holds the project guid
			//string projectGuid = chunks[7];

			return new ProjectLoader(File.ReadAllText(Path.Combine(filebase, projectFilename)));
		}

		// Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "ArchAngel.Designer", "ArchAngel.Designer\ArchAngel.Designer.csproj", "{47711AEC-E127-4141-9EAA-BE2BAD8F0597}"
		public ProjectLoader GetProject(Guid guid)
		{
			return projects[guid];
		}
	}

	public class ProjectLoader
	{
		private readonly XmlDocument projectFile;
		private readonly XmlNamespaceManager nsManager;
		private Guid? projectIdentifier;
		private List<Guid> projectReferences;
		private string name;

		public ProjectLoader(string projectFileXml)
		{
			projectFile = new XmlDocument();
			projectFile.LoadXml(projectFileXml);

			nsManager = new XmlNamespaceManager(projectFile.NameTable);
			nsManager.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
		}

		public string Name()
		{
			if(name == null)
			{
				name = projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:AssemblyName", nsManager).InnerText;
			}

			return name;
		}

		public Guid ProjectIdentifier()
		{
			if (projectIdentifier == null)
			{
				string guidValue =
					projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:ProjectGuid", nsManager).InnerText;
				projectIdentifier = new Guid(guidValue);
			}

			return projectIdentifier.Value;
		}

		public IList<Guid> ProjectReferences()
		{
			if (projectReferences == null)
			{
				projectReferences = new List<Guid>();

				var nodes = projectFile.SelectNodes("/msb:Project/msb:ItemGroup/msb:ProjectReference", nsManager);

				if (nodes == null) return projectReferences;

				foreach (XmlNode node in nodes)
				{
					string referenceGuid = node.SelectSingleNode("msb:Project", nsManager).InnerText;

					projectReferences.Add(new Guid(referenceGuid));
				}
			}

			return projectReferences;
		}
	}
}
