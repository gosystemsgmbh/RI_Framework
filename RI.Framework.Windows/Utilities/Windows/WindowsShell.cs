using System;
using System.Diagnostics;
using System.Text;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Utilities.Windows
{
	public static class WindowsShell
	{
		#region Constants

		private static readonly FilePath ExplorerExecutable = new FilePath("explorer.exe");

		private static readonly FilePath SystemInfoExecutable = new FilePath("msinfo32.exe");

		private static readonly FilePath TaskManagerExecutable = new FilePath("taskmgr.exe");

		#endregion




		#region Static Methods

		public static bool ExecuteCommand (string command, string arguments, string verb, DirectoryPath workingDirectory)
		{
			return WindowsShell.ExecuteCommand(command, arguments, verb, workingDirectory, ProcessWindowStyle.Normal);
		}

		public static bool ExecuteCommand (string command, string arguments, string verb, DirectoryPath workingDirectory, ProcessWindowStyle windowStyle)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			arguments = arguments ?? string.Empty;
			verb = verb ?? string.Empty;
			workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

			command = Environment.ExpandEnvironmentVariables(command);
			arguments = Environment.ExpandEnvironmentVariables(arguments);
			verb = Environment.ExpandEnvironmentVariables(verb);

			ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = true;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.Verb = verb;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool ExecuteConsoleCommandSynchronous (string command, string arguments, DirectoryPath workingDirectory, out string output)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			arguments = arguments ?? string.Empty;
			workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

			command = Environment.ExpandEnvironmentVariables(command);
			arguments = Environment.ExpandEnvironmentVariables(arguments);

			ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
			startInfo.CreateNoWindow = true;
			startInfo.ErrorDialog = true;
			startInfo.UseShellExecute = false;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.RedirectStandardOutput = true;
			startInfo.StandardOutputEncoding = Encoding.UTF8;

			try
			{
				Process process = Process.Start(startInfo);
				output = process?.StandardOutput?.ReadToEnd();
				process?.WaitForExit();
			}
			catch
			{
				output = null;
			}

			return output != null;
		}

		public static bool OpenExplorer ()
		{
			return WindowsShell.OpenExplorer(ProcessWindowStyle.Normal);
		}

		public static bool OpenExplorer (ProcessWindowStyle windowStyle)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.ExplorerExecutable);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool OpenFile (FilePath filePath)
		{
			return WindowsShell.OpenFile(filePath, ProcessWindowStyle.Normal);
		}

		public static bool OpenFile (FilePath filePath, ProcessWindowStyle windowStyle)
		{
			if (filePath == null)
			{
				throw new ArgumentNullException(nameof(filePath));
			}

			if (filePath.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(filePath));
			}

			ProcessStartInfo startInfo = new ProcessStartInfo(filePath);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = filePath.Directory;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool OpenFolder (DirectoryPath folderPath)
		{
			return WindowsShell.OpenFolder(folderPath, ProcessWindowStyle.Normal);
		}

		public static bool OpenFolder (DirectoryPath folderPath, ProcessWindowStyle windowStyle)
		{
			if (folderPath == null)
			{
				throw new ArgumentNullException(nameof(folderPath));
			}

			if (folderPath.HasWildcards)
			{
				throw new InvalidPathArgumentException(nameof(folderPath));
			}

			ProcessStartInfo startInfo = new ProcessStartInfo(folderPath);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = folderPath;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool OpenSystemInfo ()
		{
			return WindowsShell.OpenSystemInfo(ProcessWindowStyle.Normal);
		}

		public static bool OpenSystemInfo (ProcessWindowStyle windowStyle)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.SystemInfoExecutable);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool OpenTaskManager ()
		{
			return WindowsShell.OpenTaskManager(ProcessWindowStyle.Normal);
		}

		public static bool OpenTaskManager (ProcessWindowStyle windowStyle)
		{
			ProcessStartInfo startInfo = new ProcessStartInfo(WindowsShell.TaskManagerExecutable);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		public static bool OpenUrl (string url)
		{
			return WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal);
		}

		public static bool OpenUrl (string url, ProcessWindowStyle windowStyle)
		{
			if (url == null)
			{
				throw new ArgumentNullException(nameof(url));
			}

			return WindowsShell.OpenUrl(new Uri(url), windowStyle);
		}

		public static bool OpenUrl (Uri url)
		{
			return WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal);
		}

		public static bool OpenUrl (Uri url, ProcessWindowStyle windowStyle)
		{
			if (url == null)
			{
				throw new ArgumentNullException(nameof(url));
			}

			ProcessStartInfo startInfo = new ProcessStartInfo(url.ToString());

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = Environment.CurrentDirectory;

			try
			{
				Process.Start(startInfo);
			}
			catch
			{
				return false;
			}

			return true;
		}

		#endregion
	}
}
