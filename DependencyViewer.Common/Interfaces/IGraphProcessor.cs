using QuickGraph.Graphviz.Dot;

namespace DependencyViewer.Common.Interfaces
{
	public interface IGraphProcessor
	{
		void PreProcessGraph(GraphvizGraph formatter, Solution solution);

		void PreProcessEdge(GraphvizEdge formatter, IProject sourceProject, IProject targetProject);
		void PostProcessEdge(GraphvizEdge formatter, IProject sourceProject, IProject targetProject);
		void PreProcessVertex(GraphvizVertex formatter, IProject project);
		void PostProcessVertex(GraphvizVertex formatter, IProject project);
	}
}