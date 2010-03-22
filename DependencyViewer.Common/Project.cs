using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace DependencyViewer.Common
{
	public interface IProject
	{
		XmlDocument ProjectFile { get; }
        string Name { get; }
        Guid ProjectIdentifier { get; }
		IList<Guid> ProjectReferences { get; }
        IEnumerable<AssemblyName> ReferencedDLLs { get; }
	}

	public class Project : IProject
	{
		private readonly XmlDocument projectFile;
		private readonly XmlNamespaceManager nsManager;
		private Guid? projectIdentifier;
		private List<Guid> projectReferences;
	    private List<AssemblyName> referencedDLLs;
		private string name;

		public Project(string projectFileXml)
		{
			projectFile = new XmlDocument();
			projectFile.LoadXml(projectFileXml);

			nsManager = new XmlNamespaceManager(projectFile.NameTable);
			nsManager.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
		}

		public XmlDocument ProjectFile { get { return projectFile; } }

	    public IEnumerable<AssemblyName> ReferencedDLLs
	    {
            get
            {
                if (referencedDLLs == null)
                {
                    referencedDLLs = new List<AssemblyName>();

                    var assemblyNames =
                        projectFile.SelectNodes("/msb:Project/msb:ItemGroup/msb:Reference", nsManager) ?? new EmptyXmlNodeList();

                    foreach(XmlNode xmlNode in assemblyNames)
                    {
                         referencedDLLs.Add(new AssemblyName(xmlNode.Attributes["Include"].Value));
                    }
                }

                return referencedDLLs;
            }
	    }

	    public string Name
		{
            get
            {
                if (name == null)
                {
                    name =
                        projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:AssemblyName", nsManager).
                            InnerText;
                }

                return name;
            }
		}

		public Guid ProjectIdentifier
		{
            get
            {
                if (projectIdentifier == null)
                {
                    string guidValue =
                        projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:ProjectGuid", nsManager).
                            InnerText;
                    projectIdentifier = new Guid(guidValue);
                }

                return projectIdentifier.Value;
            }
		}

		public IList<Guid> ProjectReferences
		{
            get
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
}