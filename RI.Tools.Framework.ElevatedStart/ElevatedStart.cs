using System;
using System.Diagnostics;

namespace RI.Tools.Framework.ElevatedStart
{
	public static class ElevatedStart
	{
		public static int Main (string[] args)
		{
			const string executable = "elevstart.exe";
			string command = Environment.GetCommandLineArgs()[1];

			string executableCommandLine = Environment.CommandLine;
			int executableIndex = executableCommandLine.LastIndexOf(executable, StringComparison.InvariantCultureIgnoreCase);
			bool executableHasQuotes = executableCommandLine[0] == '\"';

			string realCommandLine = executableCommandLine.Substring(executableIndex + executable.Length + (executableHasQuotes ? 1 : 0)).TrimStart();
			int realIndex = realCommandLine.LastIndexOf(command, StringComparison.InvariantCultureIgnoreCase);
			bool realHasQuotes = realCommandLine[0] == '\"';

			string arguments = realCommandLine.Substring(realIndex + command.Length + (realHasQuotes ? 1 : 0)).TrimStart();

			ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
			startInfo.Verb = "runas";
			startInfo.UseShellExecute = true;
			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			Process process = Process.Start(startInfo);
			if (process != null)
			{
				process.WaitForExit();
			}

			Environment.ExitCode = (process?.ExitCode).GetValueOrDefault(0);
			return Environment.ExitCode;
		}
	}
}
