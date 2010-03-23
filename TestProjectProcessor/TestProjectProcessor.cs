using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using DependencyViewer.Common.Interfaces;
using DependencyViewer.Common.Model;
using QuickGraph.Graphviz.Dot;

namespace TestProjectProcessor
{
	[Export(typeof(IGraphProcessor))]
	public class TestProjectProcessor : IGraphProcessor
	{
		public void PreProcessGraph(GraphvizGraph formatter, Solution solutionLoader)
		{
			//formatter.Ratio = GraphvizRatioMode.Fill;
		    formatter.PageSize = new Size(300, 300);
		}

		public void ProcessEdge(GraphvizEdge formatter, Project sourceProject, Project targetProject)
		{
            if(IsModelProject(targetProject))
            {
                formatter.Style = GraphvizEdgeStyle.Bold;
                formatter.StrokeColor = Color.DeepSkyBlue;
            }
		}

		public void ProcessVertex(GraphvizVertex formatter, Project project)
		{
		    if (IsModelProject(project))
            {
                formatter.StrokeColor = Color.DeepSkyBlue;
            }
		}

	    private bool IsModelProject(Project project)
	    {
	        var containsNHibernateAssemblies = project.ReferencedDLLs.Any(rf => rf.Name.Contains("NHibernate"));
	        var containsTestAssemblies = project.ReferencedDLLs.Any(rf => rf.Name.Contains("nunit"));
	        return containsNHibernateAssemblies && !containsTestAssemblies;
	    }
	}
}
