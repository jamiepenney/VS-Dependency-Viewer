using System.Collections.Generic;
using System.Drawing;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace DependencyViewer
{
	public class QuickGraphProcessor
	{
		private readonly AdjacencyGraph<int, IEdge<int>> graph = new AdjacencyGraph<int, IEdge<int>>();
		private readonly Dictionary<ProjectLoader, int> projects = new Dictionary<ProjectLoader, int>();

		private SolutionLoader sol;

		public string ProcessSolution(SolutionLoader solution)
		{
			CreateGraph(solution);

			return GenerateDot();
		}

		private void CreateGraph(SolutionLoader solution)
		{
			graph.Clear();
			projects.Clear();

			sol = solution;

			for (int i = 0; i < solution.Projects.Count; i++)
			{
				projects[solution.Projects[i]] = i;
				graph.AddVertex(i);
			}

			foreach(var project in solution.Projects)
			{
				int currentProject = projects[project];				
				
				foreach(var projectRef in project.ProjectReferences())
				{
					int toVertex = projects[solution.GetProject(projectRef)];
					graph.AddEdge(new Edge<int>(currentProject, toVertex));
				}
			}
		}

		private string GenerateDot()
		{
			var graphviz = new GraphvizAlgorithm<int, IEdge<int>>(graph);
			graphviz.GraphFormat.Size = new Size(100, 100);
			graphviz.GraphFormat.Ratio = GraphvizRatioMode.Auto;
			graphviz.FormatVertex += graphviz_FormatVertex;
			graphviz.FormatEdge += graphviz_FormatEdge;

			string text = graphviz.Generate(new FileDotEngine(), "graph");

			return text;
		}

		void graphviz_FormatEdge(object sender, FormatEdgeEventArgs<int, IEdge<int>> e)
		{
			//if(e.Edge.Source == 0)
			//{
			//    e.EdgeFormatter.Style = GraphvizEdgeStyle.Dashed;
			//}
		}

		public string OutputFilename
		{
			get; set;
		}

		private void graphviz_FormatVertex(object sender, FormatVertexEventArgs<int> e)
		{
			//if(e.Vertex == 0)
			//{
			//    e.VertexFormatter.Label = "Solution";
			//    e.VertexFormatter.Shape = GraphvizVertexShape.Rectangle;
			//}
			
			{
				ProjectLoader project = sol.Projects[e.Vertex ];
				e.VertexFormatter.Label = project.Name();
				e.VertexFormatter.Shape = GraphvizVertexShape.Ellipse;
			}
		}
	}
}
