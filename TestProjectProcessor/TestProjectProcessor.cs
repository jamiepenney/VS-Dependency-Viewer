using System.ComponentModel.Composition;
using System.Drawing;
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
			formatter.Ratio = GraphvizRatioMode.Fill;
		}

		public void PreProcessEdge(GraphvizEdge formatter, IProject sourceProject, IProject targetProject)
		{
		}

		public void PostProcessEdge(GraphvizEdge formatter, IProject sourceProject, IProject targetProject)
		{
			formatter.StrokeColor = Color.ForestGreen;
		}

		public void PreProcessVertex(GraphvizVertex formatter, IProject project)
		{
		}

		public void PostProcessVertex(GraphvizVertex formatter, IProject project)
		{
		}
	}
}
