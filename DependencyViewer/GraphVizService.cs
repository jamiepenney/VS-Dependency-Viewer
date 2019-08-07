using System;
using System.Diagnostics;
using System.IO;
using DependencyViewer.Properties;

namespace DependencyViewer
{
	public class GraphVizService
	{
		public void ExecGraphViz(string dotFile, string outputFilename)
		{
			FileInfo fi = new FileInfo(dotFile);
			if(fi.Exists == false) throw new ArgumentException("dot file does not exist - " + dotFile);

			ProcessStartInfo psi = new ProcessStartInfo(Settings.Default.GraphVizPath);
			psi.RedirectStandardError = true;
			psi.Arguments += "-Tpng";
			psi.Arguments += " -o"+outputFilename;
			psi.Arguments += " \"" + dotFile + "\"";
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			psi.UseShellExecute = false;
			psi.CreateNoWindow = true;

			Process proc = Process.Start(psi);

			string procError = proc.StandardError.ReadToEnd();
			if (proc == null)
				throw new Exception("Could not run GraphViz");
		
            proc.WaitForExit();
			if (procError != "")
				throw new Exception(procError);
		}
	}
}
