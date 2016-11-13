using System;
using System.Diagnostics;
using System.Text;

using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Utilities.Windows
{
	/// <summary>
	///     Provides utilities for working with the Windows shell environment.
	/// </summary>
	/// TODO: Return Process objects for ExecuteXXXCommand
	/// TODO: Elevated option for OpenExplorer, OpenFile, OpenFolder
	/// TODO: OpenCommandPrompt
	public static class WindowsShell
	{
		#region Constants

		private static readonly FilePath ExplorerExecutable = new FilePath("explorer.exe");

		private static readonly FilePath SystemInfoExecutable = new FilePath("msinfo32.exe");

		private static readonly FilePath TaskManagerExecutable = new FilePath("taskmgr.exe");

		#endregion




		#region Static Methods

		/// <summary>
		///     Executes a console command.
		/// </summary>
		/// <param name="command"> The command to execute. </param>
		/// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
		/// <param name="output"> Returns the string which was the output of the console command. </param>
		/// <returns>
		///     true if the command could be executed successfully, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method does not return until the command finished executing.
		///     </para>
		///     <para>
		///         Environment variables will be resolved for <paramref name="command" /> and <paramref name="arguments" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		public static bool ExecuteConsoleCommand (string command, string arguments, out string output)
		{
			return WindowsShell.ExecuteConsoleCommand(command, arguments, null, out output);
		}

		/// <summary>
		///     Executes a console command.
		/// </summary>
		/// <param name="command"> The command to execute. </param>
		/// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
		/// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
		/// <param name="output"> Returns the string which was the output of the console command (null if the command failed to execute, an empty string if there was no output). </param>
		/// <returns>
		///     true if the command could be executed successfully, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method does not return until the command finished executing.
		///     </para>
		///     <para>
		///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />, and <paramref name="workingDirectory" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		public static bool ExecuteConsoleCommand (string command, string arguments, DirectoryPath workingDirectory, out string output)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			if (command.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(command));
			}

			arguments = arguments ?? string.Empty;
			workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

			command = Environment.ExpandEnvironmentVariables(command);
			arguments = Environment.ExpandEnvironmentVariables(arguments);

			string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

			ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
			startInfo.CreateNoWindow = true;
			startInfo.ErrorDialog = true;
			startInfo.UseShellExecute = false;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.WorkingDirectory = directory;
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

		/// <summary>
		///     Executes a shell command.
		/// </summary>
		/// <param name="command"> The command, program, file, or folder to execute. </param>
		/// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
		/// <param name="verb"> The verb. Can be null or an empty string if not used. </param>
		/// <returns>
		///     true if the command could be executed successfully, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method returns immediately after the command started executing.
		///         It is not waited until the command finished executing.
		///     </para>
		///     <para>
		///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />, and <paramref name="verb" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		public static bool ExecuteShellCommand (string command, string arguments, string verb)
		{
			return WindowsShell.ExecuteShellCommand(command, arguments, verb, null, ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Executes a shell command.
		/// </summary>
		/// <param name="command"> The command, program, file, or folder to execute. </param>
		/// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
		/// <param name="verb"> The verb. Can be null or an empty string if not used. </param>
		/// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
		/// <returns>
		///     true if the command could be executed successfully, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method returns immediately after the command started executing.
		///         It is not waited until the command finished executing.
		///     </para>
		///     <para>
		///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />, <paramref name="verb" />, and <paramref name="workingDirectory" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		public static bool ExecuteShellCommand (string command, string arguments, string verb, DirectoryPath workingDirectory)
		{
			return WindowsShell.ExecuteShellCommand(command, arguments, verb, workingDirectory, ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Executes a shell command.
		/// </summary>
		/// <param name="command"> The command, program, file, or folder to execute. </param>
		/// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
		/// <param name="verb"> The verb. Can be null or an empty string if not used. </param>
		/// <param name="windowStyle"> The window style of the opened program window (if any). </param>
		/// <returns>
		///     true if the command could be executed successfully, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method returns immediately after the command started executing.
		///         It is not waited until the command finished executing.
		///     </para>
		///     <para>
		///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />, and <paramref name="verb" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		public static bool ExecuteShellCommand (string command, string arguments, string verb, ProcessWindowStyle windowStyle)
		{
			return WindowsShell.ExecuteShellCommand(command, arguments, verb, null, windowStyle);
		}

		/// <summary>
		///     Executes a shell command.
		/// </summary>
		/// <param name="command"> The command, program, file, or folder to execute. </param>
		/// <param name="arguments"> The arguments. Can be null or an empty string if not used. </param>
		/// <param name="verb"> The verb. Can be null or an empty string if not used. </param>
		/// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
		/// <param name="windowStyle"> The window style of the opened program window (if any). </param>
		/// <returns>
		///     true if the command could be executed successfully, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         This method returns immediately after the command started executing.
		///         It is not waited until the command finished executing.
		///     </para>
		///     <para>
		///         Environment variables will be resolved for <paramref name="command" />, <paramref name="arguments" />, <paramref name="verb" />, and <paramref name="workingDirectory" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="command" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="command" /> is an empty string. </exception>
		public static bool ExecuteShellCommand (string command, string arguments, string verb, DirectoryPath workingDirectory, ProcessWindowStyle windowStyle)
		{
			if (command == null)
			{
				throw new ArgumentNullException(nameof(command));
			}

			if (command.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(command));
			}

			arguments = arguments ?? string.Empty;
			verb = verb ?? string.Empty;
			workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();

			command = Environment.ExpandEnvironmentVariables(command);
			arguments = Environment.ExpandEnvironmentVariables(arguments);
			verb = Environment.ExpandEnvironmentVariables(verb);

			string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

			ProcessStartInfo startInfo = new ProcessStartInfo(command, arguments);
			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = true;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = directory;
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

		/// <summary>
		///     Opens the Windows Explorer.
		/// </summary>
		/// <returns>
		///     true if the Windows Explorer could be opened, false otherwise.
		/// </returns>
		public static bool OpenExplorer ()
		{
			return WindowsShell.OpenExplorer(ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens the Windows Explorer.
		/// </summary>
		/// <param name="windowStyle"> The window style of the opened Windows Explorer window. </param>
		/// <returns>
		///     true if the Windows Explorer could be opened, false otherwise.
		/// </returns>
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

		/// <summary>
		///     Opens a file with its associated program.
		/// </summary>
		/// <param name="filePath"> The file path to open. </param>
		/// <returns>
		///     true if the file could be opened, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Environment variables will be resolved for <paramref name="filePath" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> has wildcards. </exception>
		public static bool OpenFile (FilePath filePath)
		{
			return WindowsShell.OpenFile(filePath, ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens a file with its associated program.
		/// </summary>
		/// <param name="filePath"> The file path to open. </param>
		/// <param name="windowStyle"> The window style of the opened program window. </param>
		/// <returns>
		///     true if the file could be opened, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Environment variables will be resolved for <paramref name="filePath" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="filePath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="filePath" /> has wildcards. </exception>
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

			string file = Environment.ExpandEnvironmentVariables(filePath);
			string directory = Environment.ExpandEnvironmentVariables(filePath.Directory);

			ProcessStartInfo startInfo = new ProcessStartInfo(file);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = directory;

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

		/// <summary>
		///     Opens a folder in Windows Explorer.
		/// </summary>
		/// <param name="folderPath"> The folder path to open. </param>
		/// <returns>
		///     true if the folder could be opened, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Environment variables will be resolved for <paramref name="folderPath" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="folderPath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="folderPath" /> has wildcards. </exception>
		public static bool OpenFolder (DirectoryPath folderPath)
		{
			return WindowsShell.OpenFolder(folderPath, ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens a folder in Windows Explorer.
		/// </summary>
		/// <param name="folderPath"> The folder path to open. </param>
		/// <param name="windowStyle"> The window style of the opened Windows Explorer window. </param>
		/// <returns>
		///     true if the folder could be opened, false otherwise.
		/// </returns>
		/// <remarks>
		///     <para>
		///         Environment variables will be resolved for <paramref name="folderPath" />.
		///     </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="folderPath" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="folderPath" /> has wildcards. </exception>
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

			string directory = Environment.ExpandEnvironmentVariables(folderPath);

			ProcessStartInfo startInfo = new ProcessStartInfo(directory);

			startInfo.CreateNoWindow = false;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = true;
			startInfo.WindowStyle = windowStyle;
			startInfo.WorkingDirectory = directory;

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

		/// <summary>
		///     Opens the System Info.
		/// </summary>
		/// <returns>
		///     true if the System Info could be opened, false otherwise.
		/// </returns>
		public static bool OpenSystemInfo ()
		{
			return WindowsShell.OpenSystemInfo(ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens the System Info.
		/// </summary>
		/// <param name="windowStyle"> The window style of the opened System Info window. </param>
		/// <returns>
		///     true if the System Info could be opened, false otherwise.
		/// </returns>
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

		/// <summary>
		///     Opens the Task Manager.
		/// </summary>
		/// <returns>
		///     true if the Task Manager could be opened, false otherwise.
		/// </returns>
		public static bool OpenTaskManager ()
		{
			return WindowsShell.OpenTaskManager(ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens the Task Manager.
		/// </summary>
		/// <param name="windowStyle"> The window style of the opened Task Manager window. </param>
		/// <returns>
		///     true if the Task Manager could be opened, false otherwise.
		/// </returns>
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

		/// <summary>
		///     Opens an URL.
		/// </summary>
		/// <param name="url"> The URL to open. </param>
		/// <returns>
		///     true if the URL could be opened, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="url" /> is an empty string. </exception>
		/// <exception cref="UriFormatException"> <paramref name="url" /> is not a valid URI. </exception>
		public static bool OpenUrl (string url)
		{
			return WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens an URL.
		/// </summary>
		/// <param name="url"> The URL to open. </param>
		/// <param name="windowStyle"> The window style of the window started by opening the URL. </param>
		/// <returns>
		///     true if the URL could be opened, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="url" /> is an empty string. </exception>
		/// <exception cref="UriFormatException"> <paramref name="url" /> is not a valid URI. </exception>
		public static bool OpenUrl (string url, ProcessWindowStyle windowStyle)
		{
			if (url == null)
			{
				throw new ArgumentNullException(nameof(url));
			}

			if (url.IsEmpty())
			{
				throw new EmptyStringArgumentException(nameof(url));
			}

			Uri uri = null;
			try
			{
				uri = new Uri(url);
			}
			catch (Exception exception)
			{
				throw new UriFormatException(exception.Message, exception);
			}

			return WindowsShell.OpenUrl(uri, windowStyle);
		}

		/// <summary>
		///     Opens an URL.
		/// </summary>
		/// <param name="url"> The URL to open. </param>
		/// <returns>
		///     true if the URL could be opened, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
		public static bool OpenUrl (Uri url)
		{
			return WindowsShell.OpenUrl(url, ProcessWindowStyle.Normal);
		}

		/// <summary>
		///     Opens an URL.
		/// </summary>
		/// <param name="url"> The URL to open. </param>
		/// <param name="windowStyle"> The window style of the window started by opening the URL. </param>
		/// <returns>
		///     true if the URL could be opened, false otherwise.
		/// </returns>
		/// <exception cref="ArgumentNullException"> <paramref name="url" /> is null. </exception>
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
