using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace DependencyViewer.Common.Loaders
{
    public interface IProject
    {
        string Name { get; }
        Guid ProjectIdentifier { get; }
        IList<Guid> ProjectReferences { get; }
        IEnumerable<AssemblyName> ReferencedDlls { get; }
        bool HasReferencedProject(Guid projectGuid);
    }

    public class ProjectLoader : IProject
    {
		private readonly string _filename;
        private readonly XmlNamespaceManager _nsManager;
        private readonly XmlDocument _projectFile;
        private string _name;
        private Guid? _projectIdentifier;
        private List<Guid> _projectReferences;
        private List<AssemblyName> _referencedDlls;

        public ProjectLoader(string projectFileXml, string filename)
        {
			_filename = filename;
            _projectFile = new XmlDocument();
            try
            {
                _projectFile.LoadXml(projectFileXml);
            }
            catch(XmlException e)
            {
                throw new LoaderException("Failed to parse the project", e);
            }

            _nsManager = new XmlNamespaceManager(_projectFile.NameTable);
            _nsManager.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");
        }

        public IEnumerable<AssemblyName> ReferencedDlls
        {
            get
            {
                if (_referencedDlls == null)
                {
                    _referencedDlls = new List<AssemblyName>();

                    var assemblyNames =
                        _projectFile.SelectNodes("/msb:Project/msb:ItemGroup/msb:Reference", _nsManager) ??
                        new EmptyXmlNodeList();

                    foreach (XmlNode xmlNode in assemblyNames)
                    {
                        _referencedDlls.Add(new AssemblyName(xmlNode.Attributes["Include"].Value));
                    }
                }

                return _referencedDlls;
            }
        }

        public bool HasReferencedProject(Guid projectGuid)
        {
            return ProjectReferences.Any(p => p == projectGuid);
        }

        public string Name
        {
            get
            {
                return _name ?? ( _name = GetName() );
            }
        }

        public Guid ProjectIdentifier
        {
            get
            {
                if (_projectIdentifier == null)
                {
                    string guidValue =
                        _projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:ProjectGuid", _nsManager).
                            InnerText;
                    _projectIdentifier = new Guid(guidValue);
                }

                return _projectIdentifier.Value;
            }
        }

        public IList<Guid> ProjectReferences
        {
            get
            {
                if (_projectReferences == null)
                {
                    _projectReferences = new List<Guid>();

                    var nodes =
                        _projectFile.SelectNodes("/msb:Project/msb:ItemGroup/msb:ProjectReference", _nsManager) ??
                        new EmptyXmlNodeList();

                    foreach (XmlNode node in nodes)
                    {
                        string referenceGuid = node.SelectSingleNode("msb:Project", _nsManager).InnerText;

                        _projectReferences.Add(new Guid(referenceGuid));
                    }
                }

                return _projectReferences;
            }
        }


		private string GetName()
		{
			var assemblyName = _projectFile.SelectSingleNode("/msb:Project/msb:PropertyGroup/msb:AssemblyName",
																			_nsManager);
			if (assemblyName == null)
				return Path.GetFileNameWithoutExtension(_filename);
			else
				return assemblyName.InnerText;
		}
    }
}