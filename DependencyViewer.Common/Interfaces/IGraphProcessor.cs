using QuickGraph.Graphviz.Dot;

namespace DependencyViewer.Common.Interfaces
{
	public interface IGraphProcessor
	{
		void PreProcessGraph(GraphvizGraph formatter, Solution solution);

		void ProcessEdge(GraphvizEdge formatter, IProject sourceProject, IProject targetProject);
		void ProcessVertex(GraphvizVertex formatter, IProject project);
	}
}