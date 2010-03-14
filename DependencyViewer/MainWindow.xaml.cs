using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DependencyViewer.Properties;

namespace DependencyViewer
{
	public partial class MainWindow
	{
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
			bool fileExists = File.Exists(tbFilename.Text);
			((TextBox)sender).Background = new SolidColorBrush(fileExists ? Colors.White : Colors.Red);
			btnRender.IsEnabled = fileExists;
		}

		private void btnRender_Click(object sender, RoutedEventArgs e)
		{
			var loader = new SolutionLoader(File.ReadAllText(tbFilename.Text), tbFilename.Text);
			loader.LoadProjects();

			QuickGraphProcessor processor = new QuickGraphProcessor();
			string filename = processor.ProcessSolution(loader);

			GraphVizService graphViz = new GraphVizService();
			graphViz.ExecGraphViz(filename, tbOutputFilename.Text);

			Process.Start(tbOutputFilename.Text);
		}
	}
}
