using System;
using System.Collections.Generic;
using System.Xml;

namespace DependencyViewer.Common
{
	public interface IProject
	{
		XmlDocument ProjectFile { get; }
		string Name();
		Guid ProjectIdentifier();
		IList<Guid> ProjectReferences();
	}

	public class Project : IProject
	{
		private readonly XmlDocument projectFile;
		private readonly XmlNamespaceManager nsManager;
		private Guid? projectIdentifier;
		private List<Guid> projectReferences;
		private string name;

		public Project(string projectFileXml)
		{
			projectFile = new XmlDocument();
			projectFile.LoadXml(projectFileXml);

			nsManager = new XmlNamespaceManager(projectFile.NameTable);
			nsManager.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
		}

		public XmlDocument ProjectFile { get { return projectFile; } }

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