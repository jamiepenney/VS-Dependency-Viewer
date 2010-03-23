using System;
using System.Collections.Generic;
using System.Linq;
using DependencyViewer.Common.Loaders;

namespace DependencyViewer.Common.Model
{
    public class Solution
    {
        private readonly Dictionary<Guid, Project> projects = new Dictionary<Guid, Project>();

        public Solution(SolutionLoader loader)
        {
            SolutionName = loader.SolutionName;

            foreach(var project in loader.Projects)
            {
                projects.Add(project.ProjectIdentifier, new Project(project));
            }
        }

        public string SolutionName { get; private set; }

        public IList<Project> Projects { get { return projects.Values.ToList(); } }

        public Project GetProject(Guid guid)
        {
            return projects[guid];
        }

        public void RemoveProject(Guid projectGuid)
        {
            foreach(var project in projects.Values)
            {
                if(project.HasReferencedProject(projectGuid))
                {
                    project.RemoveReferencedProject(projectGuid);
                }
            }

            projects.Remove(projectGuid);
        }
    }
}