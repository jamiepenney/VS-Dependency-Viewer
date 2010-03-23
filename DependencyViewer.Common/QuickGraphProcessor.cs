using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using DependencyViewer.Common.Interfaces;
using DependencyViewer.Common.Model;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace DependencyViewer.Common
{
	public class QuickGraphProcessor
	{
		private readonly AdjacencyGraph<int, IEdge<int>> graph = new AdjacencyGraph<int, IEdge<int>>();
		private readonly Dictionary<Project, int> projects = new Dictionary<Project, int>();

	    private readonly Solution solution;

	    public QuickGraphProcessor(Solution solution)
	    {
	        this.solution = solution;
	    }

	    [ImportMany]
		public IEnumerable<IGraphProcessor> GraphProcessors { get; set; }

		public string ProcessSolution()
		{
			CreateGraph();

			return GenerateDot();
		}

		private void CreateGraph()
		{
			graph.Clear();
			projects.Clear();

			for (int i = 0; i < solution.Projects.Count; i++)
			{
			    var project = solution.Projects[i];
			    projects[project] = i;
				
                if(project.IsSelected)
                    graph.AddVertex(i);
			}

			foreach(var project in solution.Projects)
			{
                if (project.IsSelected == false) continue;
			    
				int currentProject = projects[project];				
				
				foreach(var projectRef in project.ProjectReferences)
				{
                    if(solution.GetProject(projectRef).IsSelected == false) continue;

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
                proc.PreProcessGraph(graphviz.GraphFormat, solution);
			}

			string text = graphviz.Generate(new FileDotEngine(), "graph");

			return text;
		}

		void graphviz_FormatEdge(object sender, FormatEdgeEventArgs<int, IEdge<int>> e)
		{
			foreach(var proc in GraphProcessors)
			{
                proc.ProcessEdge(e.EdgeFormatter, solution.Projects[e.Edge.Source], solution.Projects[e.Edge.Target]);
			}
		}

		public string OutputFilename
		{
			get; set;
		}

		private void graphviz_FormatVertex(object sender, FormatVertexEventArgs<int> e)
		{
            Project project = solution.Projects[e.Vertex];
			
			e.VertexFormatter.Label = project.Name;
			e.VertexFormatter.Shape = GraphvizVertexShape.Ellipse;

			foreach (var proc in GraphProcessors)
			{
				proc.ProcessVertex(e.VertexFormatter, project);
			}
		}
	}
}