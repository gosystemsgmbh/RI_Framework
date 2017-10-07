using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using RI.Framework.IO.Files;
using RI.Framework.IO.Paths;
using RI.Framework.Utilities.Exceptions;




namespace RI.Framework.Utilities.Linux
{
	/// <summary>
	///     Provides utilities for working with the Linux shell environment.
	/// </summary>
	public static class LinuxShell
	{
		#region Constants

		private const string BashInterpreterDirective = "#!/bin/bash";

		private const string BashInterpreterProgram = "/bin/bash";

		private static readonly string BashFileExtension = ".sh";

		#endregion




		#region Static Methods

		/// <summary>
		///     Executes bash commands.
		/// </summary>
		/// <param name="commands"> The bash commands. </param>
		/// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
		/// <returns>
		///     The <see cref="Process" /> if the commands could be started successfully, null otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         The standard output is redirected.
		///         Therefore, you must read the <see cref="Process.StandardOutput" /> reader.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="commands" /> is null. </exception>
		/// <exception cref="EmptyStringArgumentException"> <paramref name="commands" /> is an empty string. </exception>
		public static Process ExecuteBashCommands (string commands, DirectoryPath workingDirectory)
		{
			if (commands == null)
			{
				throw new ArgumentNullException(nameof(commands));
			}

			if (commands.IsEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(commands));
			}

			workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();
			string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

			if (!commands.StartsWith(LinuxShell.BashInterpreterDirective, StringComparison.Ordinal))
			{
				commands = LinuxShell.BashInterpreterDirective + Environment.NewLine + commands;
			}

			TemporaryFile tempFile = new TemporaryFile(LinuxShell.BashFileExtension);
			tempFile.File.WriteText(commands, Encoding.Default);

			ProcessStartInfo startInfo = new ProcessStartInfo(LinuxShell.BashInterpreterProgram, tempFile.File);
			startInfo.CreateNoWindow = true;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = false;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.WorkingDirectory = directory;
			startInfo.RedirectStandardOutput = true;
			startInfo.StandardOutputEncoding = Encoding.UTF8;

			Process process;
			try
			{
				process = Process.Start(startInfo);
			}
			catch
			{
				return null;
			}

			if (process == null)
			{
				return null;
			}

			process.EnableRaisingEvents = true;

			EventHandler exitHandler = null;
			exitHandler = (sender, args) =>
			{
				process.Exited -= exitHandler;
				tempFile.Delete();
			};

			process.Exited += exitHandler;
			if (process.HasExited)
			{
				process.Exited -= exitHandler;
				tempFile.Delete();
			}

			return process;
		}

		/// <summary>
		///     Executes a bash script file.
		/// </summary>
		/// <param name="scriptFile"> The bash script file. </param>
		/// <param name="workingDirectory"> The used working directory. Can be null to use the current directory. </param>
		/// <returns>
		///     The <see cref="Process" /> if the script file could be started successfully, null otherwise.
		/// </returns>
		/// <remarks>
		///     <note type="important">
		///         The standard output is redirected.
		///         Therefore, you must read the <see cref="Process.StandardOutput" /> reader.
		///     </note>
		/// </remarks>
		/// <exception cref="ArgumentNullException"> <paramref name="scriptFile" /> is null. </exception>
		/// <exception cref="InvalidPathArgumentException"> <paramref name="scriptFile" /> is not a valid path. </exception>
		/// <exception cref="FileNotFoundException"> <paramref name="scriptFile" /> does not exist. </exception>
		public static Process ExecuteBashScript (FilePath scriptFile, DirectoryPath workingDirectory)
		{
			if (scriptFile == null)
			{
				throw new ArgumentNullException(nameof(scriptFile));
			}

			if (!scriptFile.IsRealFile)
			{
				throw new InvalidPathArgumentException(nameof(scriptFile));
			}

			if (!scriptFile.Exists)
			{
				throw new FileNotFoundException("Script file not found.", scriptFile);
			}

			workingDirectory = workingDirectory ?? DirectoryPath.GetCurrentDirectory();
			string directory = Environment.ExpandEnvironmentVariables(workingDirectory);

			ProcessStartInfo startInfo = new ProcessStartInfo(LinuxShell.BashInterpreterProgram, scriptFile);
			startInfo.CreateNoWindow = true;
			startInfo.ErrorDialog = false;
			startInfo.UseShellExecute = false;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.WorkingDirectory = directory;
			startInfo.RedirectStandardOutput = true;
			startInfo.StandardOutputEncoding = Encoding.UTF8;

			try
			{
				return Process.Start(startInfo);
			}
			catch
			{
				return null;
			}
		}

		#endregion
	}
}
