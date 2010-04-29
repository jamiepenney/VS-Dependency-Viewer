using DependencyViewer.Common.Model;
using QuickGraph.Graphviz.Dot;

namespace DependencyViewer.Common.Interfaces
{
	public interface IGraphProcessor
	{
		void PreProcessGraph(GraphvizGraph formatter, Solution solutionLoader);

		void ProcessEdge(GraphvizEdge formatter, Project sourceProject, Project targetProject);
		void ProcessVertex(GraphvizVertex formatter, Project project);
	}
}