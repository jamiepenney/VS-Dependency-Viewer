using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using DependencyViewer.Common;
using DependencyViewer.Common.Interfaces;
using QuickGraph.Graphviz.Dot;

namespace TestProjectProcessor
{
	[Export(typeof(IGraphProcessor))]
	public class TestProjectProcessor : IGraphProcessor
	{
		public void PreProcessGraph(GraphvizGraph formatter, Solution solution)
		{
			//formatter.Ratio = GraphvizRatioMode.Fill;
		    formatter.PageSize = new Size(300, 300);
		}

		public void ProcessEdge(GraphvizEdge formatter, IProject sourceProject, IProject targetProject)
		{
            if(IsModelProject(targetProject))
            {
                formatter.Style = GraphvizEdgeStyle.Bold;
                formatter.StrokeColor = Color.DeepSkyBlue;
            }
		}

		public void ProcessVertex(GraphvizVertex formatter, IProject project)
		{
		    if (IsModelProject(project))
            {
                formatter.StrokeColor = Color.DeepSkyBlue;
            }
		}

	    private bool IsModelProject(IProject project)
	    {
	        var containsNHibernateAssemblies = project.ReferencedDLLs.Any(rf => rf.Name.Contains("NHibernate"));
	        var containsTestAssemblies = project.ReferencedDLLs.Any(rf => rf.Name.Contains("nunit"));
	        return containsNHibernateAssemblies && !containsTestAssemblies;
	    }
	}
}
