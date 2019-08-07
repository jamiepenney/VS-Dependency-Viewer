using System.Collections.Generic;
using System.Drawing;
using DependencyViewer.Common.Interfaces;
using DependencyViewer.Common.Model;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using System.ComponentModel.Composition;

namespace DependencyViewer.Common.Graphing
{
	public class QuickGraphProcessor
	{
		private readonly AdjacencyGraph<int, IEdge<int>> _graph = new AdjacencyGraph<int, IEdge<int>>();
		private readonly Dictionary<Project, int> _projects = new Dictionary<Project, int>();
	    private readonly Solution _solution;

	    public QuickGraphProcessor(Solution solution)
	    {
	        _solution = solution;
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
			_graph.Clear();
			_projects.Clear();

			for (int i = 0; i < _solution.Projects.Count; i++)
			{
			    var project = _solution.Projects[i];
			    _projects[project] = i;
				
                if(project.IsSelected)
                    _graph.AddVertex(i);
			}

			foreach(var project in _solution.Projects)
			{
                if (project.IsSelected == false) continue;
			    
				int currentProject = _projects[project];				
				
				foreach(var projectRef in project.ProjectReferences)
				{
                    if(_solution.GetProject(projectRef).IsSelected == false) continue;

					int toVertex = _projects[_solution.GetProject(projectRef)];
					_graph.AddEdge(new Edge<int>(currentProject, toVertex));
				}
			}
		}

		private string GenerateDot()
		{
			var graphviz = new GraphvizAlgorithm<int, IEdge<int>>(_graph);
			graphviz.GraphFormat.Ratio = GraphvizRatioMode.Auto;
			graphviz.FormatVertex += graphviz_FormatVertex;
			graphviz.FormatEdge += graphviz_FormatEdge;

			foreach (var proc in GraphProcessors)
			{
                proc.PreProcessGraph(graphviz.GraphFormat, _solution);
			}

			string text = graphviz.Generate(new FileDotEngine(), "graph");

            return text;
		}

		private void graphviz_FormatEdge(object sender, FormatEdgeEventArgs<int, IEdge<int>> e)
		{
			foreach(var proc in GraphProcessors)
			{
                proc.ProcessEdge(e.EdgeFormatter, _solution.Projects[e.Edge.Source], _solution.Projects[e.Edge.Target]);
			}
		}

		public string OutputFilename
		{
			get; set;
		}

		private void graphviz_FormatVertex(object sender, FormatVertexEventArgs<int> e)
		{
            Project project = _solution.Projects[e.Vertex];
			
			e.VertexFormatter.Label = project.Name;
			e.VertexFormatter.Shape = GraphvizVertexShape.Ellipse;

			foreach (var proc in GraphProcessors)
			{
				proc.ProcessVertex(e.VertexFormatter, project);
			}
		}
	}
}