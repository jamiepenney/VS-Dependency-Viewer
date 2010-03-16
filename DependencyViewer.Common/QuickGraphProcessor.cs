using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;
using System.Drawing;
using DependencyViewer.Common.Interfaces;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace DependencyViewer.Common
{
	public class QuickGraphProcessor
	{
		private readonly AdjacencyGraph<int, IEdge<int>> graph = new AdjacencyGraph<int, IEdge<int>>();
		private readonly Dictionary<Project, int> projects = new Dictionary<Project, int>();

		private Solution sol;

		[ImportMany]
		public IEnumerable<IGraphProcessor> GraphProcessors { get; set; }

		public string ProcessSolution(Solution solution)
		{
			CreateGraph(solution);

			return GenerateDot();
		}

		private void CreateGraph(Solution solution)
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

			foreach (var proc in GraphProcessors)
			{
				proc.PreProcessGraph(graphviz.GraphFormat, sol);
			}

			string text = graphviz.Generate(new FileDotEngine(), "graph");

			return text;
		}

		void graphviz_FormatEdge(object sender, FormatEdgeEventArgs<int, IEdge<int>> e)
		{
			foreach(var proc in GraphProcessors)
			{
				proc.PreProcessEdge(e.EdgeFormatter, sol.Projects[e.Edge.Source], sol.Projects[e.Edge.Target]);
			}

			foreach (var proc in GraphProcessors)
			{
				proc.PostProcessEdge(e.EdgeFormatter, sol.Projects[e.Edge.Source], sol.Projects[e.Edge.Target]);
			}
		}

		public string OutputFilename
		{
			get; set;
		}

		private void graphviz_FormatVertex(object sender, FormatVertexEventArgs<int> e)
		{
			Project project = sol.Projects[e.Vertex];

			foreach (var proc in GraphProcessors)
			{
				proc.PreProcessVertex(e.VertexFormatter, project);
			}
			
			e.VertexFormatter.Label = project.Name();
			e.VertexFormatter.Shape = GraphvizVertexShape.Ellipse;

			foreach (var proc in GraphProcessors)
			{
				proc.PostProcessVertex(e.VertexFormatter, project);
			}
		}
	}
}