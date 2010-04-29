using System;
using System.Collections.Generic;
using System.Linq;
using DependencyViewer.Common.Loaders;

namespace DependencyViewer.Common.Model
{
    public class Solution
    {
        private readonly Dictionary<Guid, Project> _projects = new Dictionary<Guid, Project>();

        public Solution(ISolution loader)
        {
            SolutionName = loader.SolutionName;

            foreach(var project in loader.Projects)
            {
                _projects.Add(project.ProjectIdentifier, new Project(project));
            }
        }

        public Solution(string solutionName, IEnumerable<Project> projects)
        {
            SolutionName = solutionName;

            foreach (var project in projects)
            {
                _projects.Add(project.ProjectIdentifier, project);
            }
        }

        public string SolutionName { get; private set; }

        public IList<Project> Projects { get { return _projects.Values.ToList(); } }

        public Project GetProject(Guid guid)
        {
            if (!_projects.ContainsKey(guid)) return null;

            return _projects[guid];
        }

        public bool HasProject(Guid guid)
        {
            return _projects.ContainsKey(guid);
        }
    }
}