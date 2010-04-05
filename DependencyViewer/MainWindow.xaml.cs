using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DependencyViewer.Common;
using DependencyViewer.Common.Loaders;
using DependencyViewer.Common.Model;
using DependencyViewer.Properties;

namespace DependencyViewer
{
	public partial class MainWindow
	{
	    private Solution currentSolution;

		public MainWindow()
		{
			InitializeComponent();

			SetupGraphVizPath();
		}

		private void SetupGraphVizPath()
		{
			if (string.IsNullOrEmpty(Settings.Default.GraphVizPath) == false && File.Exists(Settings.Default.GraphVizPath)) 
				return;

			MessageBox.Show("Please set the location of GraphViz");
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.Multiselect = false;
			ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				Settings.Default.GraphVizPath = ofd.FileName;
				Settings.Default.Save();
			}
			else
			{
				MessageBox.Show("Cannot continue without Graphviz, exiting...");
				Application.Current.Shutdown(0);
			}
		}

		private void FilenameBrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.CheckFileExists = true;
			ofd.Multiselect = false;
			ofd.InitialDirectory = Directory.GetCurrentDirectory();

			if(ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbFilename.Text = ofd.FileName;
			}
		}

		private void OutputBrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var ofd = new System.Windows.Forms.OpenFileDialog();
			ofd.Multiselect = false;
			ofd.InitialDirectory = Directory.GetCurrentDirectory();

			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				tbOutputFilename.Text = ofd.FileName;
			}
		}

		private void tbFilename_TextChanged(object sender, TextChangedEventArgs e)
		{
			bool fileExists = FilenameIsValid();
			((TextBox)sender).Background = new SolidColorBrush(fileExists ? Colors.White : Colors.Red);
			btnRender.IsEnabled = fileExists;
            
            LoadSolution();
            
		}

	    private bool FilenameIsValid()
	    {
	        return File.Exists(tbFilename.Text);
	    }

	    private void LoadSolution()
	    {
            lbProjects.Items.Clear();

            if (FilenameIsValid() == false) return;

	        SolutionLoader loader = GetLoader();
	        currentSolution = new Solution(loader);

            foreach (var project in currentSolution.Projects)
            {
                lbProjects.Items.Add(project);
            }
	    }

	    private SolutionLoader GetLoader()
	    {
	        var solutionLoader = new SolutionLoader(File.ReadAllText(tbFilename.Text), tbFilename.Text);
            solutionLoader.LoadProjects();

	        return solutionLoader;
	    }

	    private void btnRender_Click(object sender, RoutedEventArgs e)
		{
            QuickGraphProcessor processor = new QuickGraphProcessor(currentSolution);
			Compose(processor);
			string filename = processor.ProcessSolution();

			GraphVizService graphViz = new GraphVizService();
			graphViz.ExecGraphViz(filename, tbOutputFilename.Text);

			Process.Start(tbOutputFilename.Text);
		}

		private void Compose(QuickGraphProcessor processor)
		{
			var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var container = new CompositionContainer(new DirectoryCatalog(directory));
			container.ComposeParts(processor);
		}
	}
}
